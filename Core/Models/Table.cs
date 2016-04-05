using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisManager.Core.Models
{
    /// <summary>
    /// A table is a generic representation of a matrix, vector, list, etc. from different
    /// statistical packages.  It provides a consistent interface across the statistical
    /// package representations, but is not necessarily an optimized view of the data.
    /// </summary>
    public class Table
    {
        public List<string> RowNames { get; set; }
        public List<string> ColumnNames { get; set; }
        public int RowSize { get; set; }
        public int ColumnSize { get; set; }
        public double?[] Data { get; set; }
        public string[] FormattedCells { get; set; }

        public Table()
        {
            RowNames = new List<string>();
            ColumnNames = new List<string>();
        }

        public Table(string[] rowNames, string[] columnNames, int rowSize, int columnSize, double?[] data)
        {
            RowNames = (rowNames == null) ? null : new List<string>(rowNames);
            ColumnNames = (columnNames == null) ? null : new List<string>(columnNames);
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
    }
}
