using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisManager.Core.Models
{
    public class TableFormat
    {
        public bool IncludeColumnNames { get; set; }
        public bool IncludeRowNames { get; set; }

        /// <summary>
        /// TODO: Make this more efficient, we shouldn't format the whole table every time
        /// we need a cell
        /// </summary>
        /// <param name="tableData"></param>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        public string FormatCell(Table tableData, int cellIndex)
        {
            var cells = Format(tableData);
            if (cellIndex < 0 || cellIndex >= cells.Length)
            {
                return null;
            }

            return cells[cellIndex];
        }

        // This is going to start out assuming left to right filling.  In the future
        // this will have different fill options.
        public string[] Format(Table tableData)
        {
            var formattedResults = new List<string>();
            
            if (tableData == null || tableData.Data == null)
            {
                return formattedResults.ToArray();
            }

            bool canIncludeColumnNames = (IncludeColumnNames && tableData.ColumnNames != null);
            if (canIncludeColumnNames)
            {
                formattedResults.AddRange(tableData.ColumnNames);
            }

            bool canIncludeRowNames = (IncludeRowNames && tableData.RowNames != null);
            for (int rowIndex = 0; rowIndex < tableData.RowSize; rowIndex++)
            {
                if (canIncludeRowNames)
                {
                    formattedResults.Add(tableData.RowNames[rowIndex]);
                }

                for (int columnIndex = 0; columnIndex < tableData.ColumnSize; columnIndex++)
                {
                    int index = (rowIndex * tableData.ColumnSize) + columnIndex;
                    formattedResults.Add(tableData.Data[index].ToString());
                }
            }

            // If we have rows and columns, we want to include a blank first value so
            // it fits nicely into an N x M table.
            if (canIncludeColumnNames && canIncludeRowNames && formattedResults.Count > 0)
            {
                formattedResults.Insert(0, string.Empty);
            }

            return formattedResults.ToArray();
        }
    }
}
