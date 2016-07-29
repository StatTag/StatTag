﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.ValueFormatter;

namespace StatTag.Core.Models
{
    public class TableFormat
    {
        public bool IncludeColumnNames { get; set; }
        public bool IncludeRowNames { get; set; }


        // This is going to start out assuming left to right filling.  In the future
        // this will have different fill options.
        public string[] Format(Table tableData, IValueFormatter valueFormatter = null)
        {
            valueFormatter = valueFormatter ?? new BaseValueFormatter();

            var formattedResults = new List<string>();
            
            if (tableData == null || tableData.Data == null)
            {
                return formattedResults.ToArray();
            }

            bool canIncludeColumnNames = (IncludeColumnNames && tableData.ColumnNames != null && tableData.ColumnNames.Count > 0);
            if (canIncludeColumnNames)
            {
                formattedResults.AddRange(tableData.ColumnNames);
            }

            bool canIncludeRowNames = (IncludeRowNames && tableData.RowNames != null && tableData.RowNames.Count > 0);
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

            formattedResults = formattedResults.Select(x => valueFormatter.Finalize(x)).ToList();

            // If we have rows and columns, we want to include a blank first value so
            // it fits nicely into an N x M table.
            // Note that we do NOT use the valueFormatter here.  We absolutely want this to
            // be blank, so we don't touch it.
            if (canIncludeColumnNames && canIncludeRowNames && formattedResults.Count > 0)
            {
                formattedResults.Insert(0, string.Empty);
            }

            return formattedResults.ToArray();
        }
    }
}
