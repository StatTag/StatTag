using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using RDotNet.Internals;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using StatTag.Core.Parser;
using StatTag.Core.Utility;

namespace R
{
    public class RAutomation : IStatAutomation
    {
        private const string MATRIX_DIMENSION_NAMES_ATTRIBUTE = "dimnames";

        private REngine Engine = null;
        protected RParser Parser { get; set; }

        public RAutomation()
        {
            Parser = new RParser();
        }

        public bool Initialize()
        {
            if (Engine == null)
            {
                REngine.SetEnvironmentVariables(); // <-- May be omitted; the next line would call it.
                Engine = REngine.GetInstance();
            }

            return (Engine != null);
        }

        public void Dispose()
        {
            //if (Engine != null)
            //{
            //    Engine.Dispose();
            //    Engine = null;
            //}
        }

        public StatTag.Core.Models.CommandResult[] RunCommands(string[] commands, Tag tag = null)
        {
            var commandResults = new List<CommandResult>();
            foreach (var command in commands)
            {
                var result = RunCommand(command, tag);
                if (result != null && !result.IsEmpty())
                {
                    commandResults.Add(result);
                }
            }

            return commandResults.ToArray();
        }

        public CommandResult RunCommand(string command, Tag tag = null)
        {
            var result = Engine.Evaluate(command);
            if (result == null || result.IsInvalid)
            {
                return null;
            }

            // We take a hint from the tag type to identify tables.  Because of how open R is with its
            // return of results, a user can just specify a variable and get the result.
            if (tag != null && tag.Type == Constants.TagType.Table)
            {
                return new CommandResult() { TableResult = GetTableResult(result)};
            }

            // Image comes next, because everything else we will count as a value type.
            if (Parser.IsImageExport(command))
            {
                var imageLocation = RunCommand(Parser.GetImageSaveLocation(command), new Tag() { Type = Constants.TagType.Value });
                return new CommandResult() { FigureResult = imageLocation.ValueResult };
            }

            // If we have a value command, we will pull out the last relevant line from the output.
            // Because we treat every type of output as a possible value result, we are only going
            // to capture the result if it's flagged as a tag.
            if (tag != null && tag.Type == Constants.TagType.Value)
            {
                return new CommandResult() { ValueResult = GetValueResult(result) };
            }

            return null;
        }

        /// <summary>
        /// Given a 2D data frame, flatten it into a 1D array that is organized as data by row.
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <returns></returns>
        private string[] FlattenDataFrame(DataFrame dataFrame)
        {
            // Because we can only cast columns (not individual cells), we will go through all columns
            // first and cast them to characters so things like NA values are represented appropriately.
            // If we use the default format for these columns/cells, we end up with large negative int
            // values where NA exists.
            var castColumns = new List<CharacterVector>();
            for (int column = 0; column < dataFrame.ColumnCount; column++)
            {
                castColumns.Add(dataFrame[column].AsCharacter());
            }

            var data = new List<string>(dataFrame.RowCount * dataFrame.ColumnCount);
            for (int row = 0; row < dataFrame.RowCount; row++)
            {
                for (int column = 0; column < dataFrame.ColumnCount; column++)
                {
                    data.Add(castColumns[column][row]);
                }
            }

            return data.ToArray();
        }

        /// <summary>
        /// General function to extract dimension (row/column) names for an R matrix.  This deals with
        /// the specific of how R packages matrix results, which is different from a data frame.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="rowNames"></param>
        /// <returns></returns>
        private string[] GetMatrixDimensionNames(SymbolicExpression exp, bool rowNames)
        {
            var attributeName = exp.GetAttributeNames().FirstOrDefault(x => x.Equals(MATRIX_DIMENSION_NAMES_ATTRIBUTE, StringComparison.InvariantCultureIgnoreCase));
            if (attributeName == null)
            {
                return null;
            }

            var dimnames = exp.GetAttribute(attributeName).AsList();
            if (dimnames == null)
            {
                return null;
            }

            // Per the R specification, the dimnames will contain 0 to 2 vectors that contain
            // dimension labels.  The first entry is for rows, and the second is for columns.
            // https://stat.ethz.ch/R-manual/R-devel/library/base/html/matrix.html
            switch (dimnames.Length)
            {
                case 1:
                    // If we want columns and we only have one dimname entry, we don't have column names.
                    if (!rowNames)
                    {
                        return null;
                    }
                    return dimnames[0].AsCharacter().ToArray();
                case 2:
                    return dimnames[rowNames ? 0 : 1].AsCharacter().ToArray();
            }

            return null;
        }

        /// <summary>
        /// Return the row names (if they exist) for a matrix.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns>An array of strings containing the row names, or null if not present.</returns>
        private string[] GetMatrixRowNames(SymbolicExpression exp)
        {
            return GetMatrixDimensionNames(exp, true);
        }

        /// <summary>
        /// Return the column names (if they exist) for a matrix.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns>An array of strings containing the column names, or null if not present.</returns>
        private string[] GetMatrixColumnNames(SymbolicExpression exp)
        {
            return GetMatrixDimensionNames(exp, false);
        }

        /// <summary>
        /// Take the 2D data in a matrix and flatten it to a 1D representation (by row)
        /// which we use internally in StatTag.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private string[] FlattenMatrix(CharacterMatrix matrix)
        {
            var data = new List<string>();
            for (int row = 0; row < matrix.RowCount; row++)
            {
                for (int column = 0; column < matrix.ColumnCount; column++)
                {
                    data.Add(matrix[row, column]);
                }
            }
            return data.ToArray();
        }

        private Table GetTableResult(SymbolicExpression result)
        {
            // You'll notice that regardless of the type, we convert everything to a string.  This
            // is just a simplification that StatTag makes about the results.  Often times we are
            // doing additional formatting and will do a conversion from the string type to the right
            // numeric type and format it appropriately.
            if (result.IsDataFrame())
            {
                var data = result.AsDataFrame();
                int rowCount = data.RowCount + 1;
                int columnCount = data.ColumnCount + 1;
                return new Table()
                {
                    ColumnSize = columnCount, RowSize = rowCount,
                    Data = TableUtil.MergeTableVectorsToArray(data.RowNames, data.ColumnNames,
                        FlattenDataFrame(data), rowCount, columnCount)
                };
            }
            else if (result.IsList())
            {
                // A list can be a collection of anything, including other vectors.
                // We will do our best to expand these.  If they are deeply nested
                // though, it becomes difficult to put these in a table structure in
                // a Word document.  For now, we'll limit to 2D structures.
                var list = result.AsList();
                int maxSize = 0;
                var data = new List<List<string>>();
                foreach (var item in list)
                {
                    var characterData = item.AsCharacter();
                    if (characterData == null)
                    {
                        data.Add(new List<string>());
                    }
                    else
                    {
                        var dataAsArray = characterData.Select(x => x.ToString()).ToList();
                        data.Add(dataAsArray);
                        maxSize = Math.Max(dataAsArray.Count, maxSize);
                    }
                }

                // Build data into a vector
                var vectorData = new List<string>();
                for (int row = 0; row < maxSize; row++)
                {
                    foreach (var column in data)
                    {
                        vectorData.Add(row < column.Count ? column[row] : null);
                    }
                }

                var rowSize = (list.Names == null || list.Names.Length == 0) ? maxSize : (maxSize + 1);
                return new Table()
                {
                    ColumnSize = data.Count,
                    RowSize = rowSize,
                    Data = TableUtil.MergeTableVectorsToArray(
                        null, list.Names, vectorData.ToArray(), rowSize, data.Count)
                };
            }
            else if (result.IsMatrix())
            {
                var matrix = result.AsCharacterMatrix();
                var rowNames = GetMatrixRowNames(result);
                var columnNames = GetMatrixColumnNames(result);
                // Just to note this isn't an error checking columnNames for rowCount and vice-versa.  Remember that
                // if we have column names, that will take up a row of data.  Likewise, row names are an additional
                // column.
                int rowCount = matrix.RowCount + (columnNames == null ? 0 : 1);
                int columnCount = matrix.ColumnCount + (rowNames == null ? 0 : 1);
                return new Table()
                {
                    ColumnSize = columnCount,
                    RowSize = rowCount,
                    Data = TableUtil.MergeTableVectorsToArray(rowNames, columnNames,
                        FlattenMatrix(matrix), rowCount, columnCount)
                };
            }

            if (result.Type == SymbolicExpressionType.NumericVector
                || result.Type == SymbolicExpressionType.IntegerVector
                || result.Type == SymbolicExpressionType.CharacterVector
                || result.Type == SymbolicExpressionType.LogicalVector)
            {
                var data = result.AsCharacter();
                return new Table()
                {
                    ColumnSize = 1, RowSize = data.Length, Data = TableUtil.MergeTableVectorsToArray(
                        null, null, data.Select(x => x.ToString()).ToArray(), data.Length, 1)
                };
            }

            return null;
        }

        public bool IsReturnable(string command)
        {
            return Parser.IsValueDisplay(command) || Parser.IsImageExport(command) || Parser.IsTableResult(command);
        }

        public string GetInitializationErrorMessage()
        {
            return "Could not communicate with R.  R may not be fully installed, or might be missing some of the automation pieces that StatTag requires.";

        }

        private string GetValueResult(SymbolicExpression result)
        {
            if (result.IsDataFrame())
            {
                return result.AsDataFrame()[0].AsCharacter().FirstOrDefault();
            }
            else if (result.IsList())
            {
                return result.AsList()[0].AsCharacter().FirstOrDefault();
            }

            switch (result.Type)
            {
                case SymbolicExpressionType.NumericVector:
                case SymbolicExpressionType.IntegerVector:
                case SymbolicExpressionType.CharacterVector:
                case SymbolicExpressionType.LogicalVector:
                    return result.AsCharacter().FirstOrDefault();
            }
            return null;
        }
    }
}
