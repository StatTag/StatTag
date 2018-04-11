using System;
using System.Collections.Generic;
using StatTag.Core.Models;
using StatTag.Core.ValueFormatter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Models
{
    [TestClass]
    public class TableFormatTests
    {
        [TestMethod]
        public void Format_Empty()
        {
            var format = new TableFormat();
            Assert.IsNotNull(format.Format(null));
            Assert.AreEqual(0, format.Format(null).Length);
            Assert.IsNotNull(format.Format(new Table()));
            Assert.AreEqual(0, format.Format(new Table()).Length);
        }

        public string FormatArrayForChecking(string[,] data)
        {
            var flatData = new List<string>();
            int rows = data.GetLength(0);
            int columns = data.GetLength(1);
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    flatData.Add(data[row, column]);
                }
            }

            return string.Join(", ", flatData.ToArray());
        }

        [TestMethod]
        public void Format_DataColumnsAndRows()
        {
            var format = new TableFormat()
            {
                ColumnFilter = new FilterFormat(Constants.FilterPrefix.Column) { Enabled = false },
                RowFilter = new FilterFormat(Constants.FilterPrefix.Row) { Enabled = false }
            };
            var table = new Table(3, 3,
                new string[,] { { "", "Col1", "Col2" }, { "Row1", "0", "1" }, { "Row2", "2", "3" } });
            Assert.AreEqual(9, format.Format(table).Length);
            Assert.AreEqual(", Col1, Col2, Row1, 0, 1, Row2, 2, 3", FormatArrayForChecking(format.Format(table)));
        }

        public class TestValueFormatter : BaseValueFormatter
        {
            public override string GetMissingValue()
            {
                return "MISSING";
            }
        }

        [TestMethod]
        public void Format_DataColumnsAndRowsWithMissingValues()
        {
            var format = new TableFormat()
            {
                ColumnFilter = new FilterFormat(Constants.FilterPrefix.Column) { Enabled = false },
                RowFilter = new FilterFormat(Constants.FilterPrefix.Row) { Enabled = false }
            };
            var table = new Table(3, 3,
                new string[,] { { "", "Col1", "Col2" }, { "Row1", "0", "1" }, { "Row2", null, "3" } });
            Assert.AreEqual(9, format.Format(table).Length);
            // Without any properties, the default is to make missing values show up as empty strings
            Assert.AreEqual(", Col1, Col2, Row1, 0, 1, Row2, , 3", FormatArrayForChecking(format.Format(table, new TestValueFormatter())));

            // When we specify properties, the behavior should be what we specify
            var metadata = new DocumentMetadata()
            {
                RepresentMissingValues = Constants.MissingValueOption.StatPackageDefault
            };
            Assert.AreEqual(", Col1, Col2, Row1, 0, 1, Row2, MISSING, 3", FormatArrayForChecking(format.Format(table, new TestValueFormatter(), metadata)));

            metadata = new DocumentMetadata()
            {
                RepresentMissingValues = Constants.MissingValueOption.CustomValue,
                CustomMissingValue =  "Custom"
            };
            Assert.AreEqual(", Col1, Col2, Row1, 0, 1, Row2, Custom, 3", FormatArrayForChecking(format.Format(table, new TestValueFormatter(), metadata)));
        }
    }
}
