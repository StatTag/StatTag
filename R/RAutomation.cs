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

            // If we have a value command, we will pull out the last relevant line from the output.
            if (Parser.IsValueDisplay(command))
            {
                return new CommandResult() { ValueResult = GetValueResult(result) };
            }

            if (Parser.IsImageExport(command))
            {
                return new CommandResult() { FigureResult = Parser.GetImageSaveLocation(command) };
            }

            return null;
        }

        /// <summary>
        /// Given a 2D data frame, flatten it into a 1D array that is organized as data by row.
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <returns></returns>
        private string[] FlattenData(DataFrame dataFrame)
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
                        FlattenData(data), rowCount, columnCount)
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
                    null, result.GetAttributeNames(), data.Select(x => x.ToString()).ToArray(), data.Length, 1)
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
            return "Could not communicate with R.  R may not be fully installed, or be missing some of the automation pieces that StatTag requires.";

        }

        private string GetValueResult(SymbolicExpression result)
        {
            switch (result.Type)
            {
                case SymbolicExpressionType.NumericVector:
                case SymbolicExpressionType.IntegerVector:
                case SymbolicExpressionType.CharacterVector:
                case SymbolicExpressionType.LogicalVector:
                    return result.AsCharacter().First();
            }
            return null;
        }
    }
}
