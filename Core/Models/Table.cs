using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Core.Models
{
    /// <summary>
    /// A table is a generic representation of a matrix, vector, list, etc. from different
    /// statistical packages.  It provides a consistent interface across the statistical
    /// package representations, but is not necessarily an optimized view of the data.
    /// </summary>
    public class Table
    {
        public int RowSize { get; set; }
        public int ColumnSize { get; set; }
        public string[,] Data { get; set; }

        /// <summary>
        /// The formatted cells will be filled for all values in the Data array (meaning, these
        /// two collections will always be the same size).  If the user chooses to filter out
        /// rows or columns, they will still be present in this collection.
        /// </summary>
        public string[,] FormattedCells { get; set; }

        public Table()
        {
        }

        public Table(int rowSize, int columnSize, string[,] data)
        {
            if ((data == null && (rowSize * columnSize != 0))
                || (data != null 
                    && (data.GetLength(0) != rowSize
                        || data.GetLength(1) != columnSize)))
            {
                throw new Exception("The dimensions of the data do not match the row and column dimensions.");
            }

            RowSize = rowSize;
            ColumnSize = columnSize;
            Data = data;
        }

        /// <summary>
        /// Determines if this table is empty - meaning it has no data, or does not have a
        /// column or row dimension specified.
        /// </summary>
        /// <returns>true if empty, false otherwise</returns>
        public bool IsEmpty()
        {
            return Data == null || Data.Length == 0 || RowSize == 0 || ColumnSize == 0;
        }


        /// <summary>
        /// Our approach has been to sequentially number table cells, so this is used to pull
        /// out data at the appropriate 2D location.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetDataAtIndex(string[,] data, int index)
        {
            if (data == null || index >= data.Length)
            {
                return string.Empty;
            }

            int columns = data.GetLength(1);
            return data[(index / columns), (index % columns)];
        }
    }
}
