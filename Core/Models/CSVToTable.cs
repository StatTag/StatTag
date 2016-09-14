using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace StatTag.Core.Models
{
    public static class CSVToTable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableFilePath"></param>
        /// <returns>An array containing the dimensions (R x C), or NULL if it could not determine
        /// the table size.</returns>
        public static int[] GetTableDimensions(string tableFilePath)
        {
            if (!File.Exists(tableFilePath))
            {
                return null;
            }

            int rows = 0;
            int columns = 0;

            using (TextFieldParser parser = new TextFieldParser(tableFilePath))
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

        /// <summary>
        /// Combines the different components of a matrix command into a single structure.
        /// </summary>
        /// <param name="tableFilePath"></param>
        /// <returns></returns>
        public static Table GetTableResult(string tableFilePath)
        {
            var table = new Table();
            if (!File.Exists(tableFilePath))
            {
                return table;
            }

            var dimensions = GetTableDimensions(tableFilePath);
            if (dimensions == null || dimensions.Contains(0))
            {
                return table;
            }

            using (TextFieldParser parser = new TextFieldParser(tableFilePath, System.Text.Encoding.Default))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;

                int row = 0;
                var data = new string[dimensions[0], dimensions[1]];
                //var data = new List<string>();
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
