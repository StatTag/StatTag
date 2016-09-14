using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Utility
{
    public static class TableUtil
    {
        /// <summary>
        /// Some statistical packages (Stata for sure, but there may be others) give us row names, column names and data as separate
        /// flat vectors.  This handles merging them into a 2D array with the data in the appropriate cells.
        /// </summary>
        /// <param name="rowNames">An array of row names.  If this is null or empty, no row names are added.</param>
        /// <param name="columnNames">An array of the column names.  If this is null or empty, no column names are added.</param>
        /// <param name="data">A vector containing the data for the array.  It is assumed this is laid out by row.</param>
        /// <param name="totalRows">The total number of rows - this should be a combination of column names and data rows</param>
        /// <param name="totalColumns">The total number of columns - this should be a combination of row names and data rows</param>
        /// <returns></returns>
        public static string[,] MergeTableVectorsToArray(string[] rowNames, string[] columnNames, string[] data, int totalRows, int totalColumns)
        {
            if (data == null)
            {
                return null;
            }

            if (data.Length == 0)
            {
                return new string[,] { };
            }

            int currentRow = 0;
            var arrayData = new string[totalRows, totalColumns];
            var hasRowNames = (rowNames != null && rowNames.Length > 0);
            var hasColumnNames = (columnNames != null && columnNames.Length > 0);

            // If we have column names present, our first order of business is to fill in the
            // first row with all of those names.  We insert a blank cell if we have both row
            // names and column names.
            if (hasColumnNames)
            {
                int currentColumn = 0;
                if (hasRowNames)
                {
                    arrayData[currentRow, currentColumn] = string.Empty;
                    currentColumn++;
                }

                for (int index = 0; index < columnNames.Length; index++)
                {
                    arrayData[currentRow, currentColumn + index] = columnNames[index];
                }
                currentRow++;
            }

            // Go through all of the data next.  Insert row names when/if needed.
            int column = 0;
            int rowNameIndex = 0;
            for (int index = 0; index < data.Length; index++)
            {
                if (index != 0 && (column % totalColumns) == 0)
                {
                    column = 0;
                    currentRow++;
                    rowNameIndex++;
                }

                // If we have a collection of row names to add, and we are at the data position
                // where we are starting a new row, insert the row name now.
                if (hasRowNames && (column % totalColumns) == 0)
                {
                    arrayData[currentRow, column] = rowNames[rowNameIndex];
                    column++;
                }

                arrayData[currentRow, column] = data[index];
                column++;
            }

            return arrayData;
        }

        /// <summary>
        /// Our data collections will include all cells - even if the user said they should be excluded.
        /// The purpose of this utility function is to take the 2D data array, remove the columns and
        /// rows that need to be excluded, and flatten it into a vector.
        /// The reason we flatten it is Word uses a vector of cells that we will fill, so this eases the
        /// mapping into that structure.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string[] GetDisplayableVector(string[,] data, TableFormat format)
        {
            var excludeColumnIndices = format.ColumnFilter.ExpandValue();
            var excludeRowIndices = format.RowFilter.ExpandValue();

            var dataVector = new List<string>();
            for (int row = 0; row < data.GetLength(0); row++)
            {
                if (format.RowFilter.Enabled && excludeRowIndices.Contains(row))
                {
                    continue;
                }

                for (int column = 0; column < data.GetLength(1); column++)
                {
                    if (format.ColumnFilter.Enabled && excludeColumnIndices.Contains(column))
                    {
                        continue;
                    }

                    dataVector.Add(data[row,column]);
                }
            }

            return dataVector.ToArray();
        }
    }
}
