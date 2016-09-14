using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.ValueFormatter;

namespace StatTag.Core.Models
{
    public class TableFormat
    {
        /// <summary>
        /// OBSOLETE: Please use column filters instead
        /// </summary>
        [Obsolete]
        public bool IncludeColumnNames { get; set; }
        /// <summary>
        /// OBSOLETE: Please use row filters instead
        /// </summary>
        [Obsolete]
        public bool IncludeRowNames { get; set; }

        public FilterFormat RowFilter { get; set; }
        public FilterFormat ColumnFilter { get; set; }

        public TableFormat()
        {
            RowFilter = new FilterFormat(Constants.FilterPrefix.Row);
            ColumnFilter = new FilterFormat(Constants.FilterPrefix.Column);
        }


        // This is going to start out assuming left to right filling.  In the future
        // this will have different fill options.
        public string[,] Format(Table tableData, IValueFormatter valueFormatter = null)
        {
            valueFormatter = valueFormatter ?? new BaseValueFormatter();

            //var formattedResults = new List<string>();
            if (tableData == null || tableData.Data == null)
            {
                return null;
            }

            var applyRowFilter = (RowFilter != null && RowFilter.Enabled &&
                      RowFilter.Type == Constants.FilterType.Exclude);
            var applyColumnFilter = (ColumnFilter != null && ColumnFilter.Enabled &&
                                  ColumnFilter.Type == Constants.FilterType.Exclude);

            var rowFilterIndices = applyRowFilter ? RowFilter.ExpandValue() : new int[0];
            var columnFilterIndices = applyColumnFilter ? ColumnFilter.ExpandValue() : new int[0];
            var formattedResults = new string[tableData.RowSize - rowFilterIndices.Length, tableData.ColumnSize - columnFilterIndices.Length];
            for (int row = 0; row < tableData.RowSize; row++)
            {
                for (int column = 0; column < tableData.ColumnSize; column++)
                {
                    formattedResults[row, column] = valueFormatter.Finalize(tableData.Data[row, column]);
                }
            }
            //formattedResults = tableData.Data.Select(x => valueFormatter.Finalize(x)).ToList();
            return formattedResults;
        }
    }
}
