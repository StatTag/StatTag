using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using OfficeOpenXml;

namespace StatTag.Core.Models
{
    public static class DataFileToTable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableFilePath"></param>
        /// <returns>An array containing the dimensions (R x C), or NULL if it could not determine
        /// the table size.</returns>
        public static int[] GetCSVTableDimensions(string tableFilePath)
        {
            if (!File.Exists(tableFilePath))
            {
                return null;
            }

            int rows = 0;
            int columns = 0;

            using (var parser = new TextFieldParser(tableFilePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;

                var data = new List<string>();
                while (!parser.EndOfData)
                {
                    rows++;
                    string[] fields = parser.ReadFields();
                    if (fields != null)
                    {
                        columns = Math.Max(columns, fields.Length);
                    }
                }
            }

            return new int[] {rows, columns};
        }

        public static Table GetTableResult(string tableFilePath)
        {
            tableFilePath = tableFilePath.Trim().ToLower();
            if (tableFilePath.EndsWith(".xlsx"))
            {
                return GetXLSXTableResult(tableFilePath);
            }
            else if (tableFilePath.EndsWith(".xls"))
            {
                throw new Exception("StatTag is unable to import data from older Excel files (those ending in .XLS).  If possible, please use the newer .XLSX format, or use a CSV.");
            }
            else
            {
                return GetCSVTableResult(tableFilePath);
            }
        }

        private static Table GetXLSXTableResult(string tableFilePath)
        {
            var table = new Table();
            if (!File.Exists(tableFilePath))
            {
                return table;
            }

            var excelFile = new FileInfo(tableFilePath);
            using (var package = new ExcelPackage(excelFile))
            {
                var worksheets = package.Workbook.Worksheets;
                if (worksheets == null || worksheets.Count == 0)
                {
                    return table;
                }

                // Right now, we will only use the first sheet in a workbook
                var sheet = package.Workbook.Worksheets.First();

                // If any of the necessary objects (sheet, cells, values) are null, we assume it means that the sheet
                // is empty, and we can just return a blank table structure.
                if (sheet == null || sheet.Cells == null || sheet.Cells.Value == null)
                {
                    return table;
                }

                var sheetData = (Object[,])sheet.Cells.Value;
                var dimensions = new int[] {sheetData.GetLength(0), sheetData.GetLength(1)};
                var data = new string[dimensions[0], dimensions[1]];

                // The EPPlus library uses 1-based indexing for Excel cells (which is consistent with the usual Office
                // object interface, but not consistent with how C# works).  This is why we are doing 1-based indexing
                // in the loop, and in the data array we are subtracting 1 to get it back to 0-based.
                for (int row = 1; row <= dimensions[0]; row++)
                {
                    for (int column = 1; column <= dimensions[1]; column++)
                    {
                        data[row-1, column-1] = sheet.Cells[row, column].GetValue<string>();
                        data[row-1, column-1] = data[row-1, column-1] ?? string.Empty;
                    }
                }
                
                table.RowSize = dimensions[0];
                table.ColumnSize = dimensions[1];
                table.Data = data;
            }
            return table;
        }

        /// <summary>
        /// Combines the different components of a matrix command into a single structure.
        /// </summary>
        /// <param name="tableFilePath"></param>
        /// <returns></returns>
        private static Table GetCSVTableResult(string tableFilePath)
        {
            var table = new Table();
            if (!File.Exists(tableFilePath))
            {
                return table;
            }

            var dimensions = GetCSVTableDimensions(tableFilePath);
            if (dimensions == null || dimensions.Contains(0))
            {
                return table;
            }

            using (var parser = new TextFieldParser(tableFilePath, System.Text.Encoding.Default))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;

                int row = 0;
                var data = new string[dimensions[0], dimensions[1]];
                while (!parser.EndOfData)
                {
                    int column = 0;
                    string[] fields = parser.ReadFields();
                    if (fields != null)
                    {
                        for (int index = 0; index < fields.Length; index++)
                        {
                            data[row, index] = fields[index];
                        }
                    }

                    int fieldsLength = (fields == null ? 0 : fields.Length);
                    // If this is an unbalanced row, balance it with empty strings
                    if (fieldsLength < dimensions[1])
                    {
                        for (int index = fieldsLength; index < dimensions[1]; index++)
                        {
                            data[row, index] = string.Empty;
                        }
                    }

                    row++;
                }

                table.RowSize = dimensions[0];
                table.ColumnSize = dimensions[1];
                table.Data = data;
            }

            return table;
        }
    }
}
