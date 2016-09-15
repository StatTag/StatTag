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

            if (tableData == null || tableData.Data == null)
            {
                return new string[,]{};
            }

            var formattedResults = new string[tableData.RowSize, tableData.ColumnSize];
            for (int row = 0; row < tableData.RowSize; row++)
            {
                for (int column = 0; column < tableData.ColumnSize; column++)
                {
                    // If we are not filtering, and the first cell is blank, don't finalize it.  We purposely want to
                    // allow that cell to have an empty string (not an empty placeholder value) to account for the
                    // intersection of row and column names.
                    if (row == 0 && column == 0 && !RowFilter.Enabled && !ColumnFilter.Enabled)
                    {
                        formattedResults[row, column] = tableData.Data[row, column];
                        continue;
                    }

                    formattedResults[row, column] = valueFormatter.Finalize(tableData.Data[row, column]);
                }
            }
            return formattedResults;
        }
    }
}
