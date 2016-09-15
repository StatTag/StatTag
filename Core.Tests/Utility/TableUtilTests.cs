using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Models;
using StatTag.Core.Utility;

namespace Core.Tests.Utility
{
    [TestClass]
    public class TableUtilTests
    {
        [TestMethod]
        public void MergeTableVectorsToArray_NoData()
        {
            Assert.IsNull(TableUtil.MergeTableVectorsToArray(null, null, null, 0, 0));
            Assert.IsNull(TableUtil.MergeTableVectorsToArray(new[] { "Row1" }, new[] { "Col1" }, null, 0, 0));

            var result = TableUtil.MergeTableVectorsToArray(new[] { "Row1" }, new[] { "Col1" }, new string[] { }, 0, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void MergeTableVectorsToArray_ColumnAndRowNames()
        {
            var rowNames = new[] { "Row1", "Row2", "Row3" };
            var colNames = new[] { "Col1", "Col2" };
            var data = new[] { "0.0", "1.0", "2.0", "3.0", "4.0", "5.0" };
            var result = TableUtil.MergeTableVectorsToArray(rowNames, colNames, data, 4, 3);
            Assert.AreEqual(4, result.GetLength(0));
            Assert.AreEqual(3, result.GetLength(1));
            Assert.AreEqual("", result[0, 0]);
            Assert.AreEqual("Col1", result[0, 1]);
            Assert.AreEqual("Col2", result[0, 2]);
            Assert.AreEqual("Row1", result[1, 0]);
            Assert.AreEqual("0.0", result[1, 1]);
            Assert.AreEqual("1.0", result[1, 2]);
            Assert.AreEqual("Row2", result[2, 0]);
            Assert.AreEqual("2.0", result[2, 1]);
            Assert.AreEqual("3.0", result[2, 2]);
            Assert.AreEqual("Row3", result[3, 0]);
            Assert.AreEqual("4.0", result[3, 1]);
            Assert.AreEqual("5.0", result[3, 2]);
        }

        [TestMethod]
        public void MergeTableVectorsToArray_ColumnNamesOnly()
        {
            var rowNames = new string[] { };
            var colNames = new[] { "Col1", "Col2" };
            var data = new[] { "0.0", "1.0", "2.0", "3.0", "4.0", "5.0" };
            var result = TableUtil.MergeTableVectorsToArray(rowNames, colNames, data, 4, 2);
            Assert.AreEqual(4, result.GetLength(0));
            Assert.AreEqual(2, result.GetLength(1));
            Assert.AreEqual("Col1", result[0, 0]);
            Assert.AreEqual("Col2", result[0, 1]);
            Assert.AreEqual("0.0", result[1, 0]);
            Assert.AreEqual("1.0", result[1, 1]);
            Assert.AreEqual("2.0", result[2, 0]);
            Assert.AreEqual("3.0", result[2, 1]);
            Assert.AreEqual("4.0", result[3, 0]);
            Assert.AreEqual("5.0", result[3, 1]);
        }

        [TestMethod]
        public void MergeTableVectorsToArray_RowNamesOnly()
        {
            var rowNames = new[] { "Row1", "Row2", "Row3" };
            var colNames = new string[] { };
            var data = new[] { "0.0", "1.0", "2.0", "3.0", "4.0", "5.0" };
            var result = TableUtil.MergeTableVectorsToArray(rowNames, colNames, data, 3, 3);
            Assert.AreEqual(3, result.GetLength(0));
            Assert.AreEqual(3, result.GetLength(1));
            Assert.AreEqual("Row1", result[0, 0]);
            Assert.AreEqual("0.0", result[0, 1]);
            Assert.AreEqual("1.0", result[0, 2]);
            Assert.AreEqual("Row2", result[1, 0]);
            Assert.AreEqual("2.0", result[1, 1]);
            Assert.AreEqual("3.0", result[1, 2]);
            Assert.AreEqual("Row3", result[2, 0]);
            Assert.AreEqual("4.0", result[2, 1]);
            Assert.AreEqual("5.0", result[2, 2]);
        }

        [TestMethod]
        public void MergeTableVectorsToArray_DataOnly()
        {
            var rowNames = new string[] { };
            var colNames = new string[] { };
            var data = new[] { "0.0", "1.0", "2.0", "3.0", "4.0", "5.0" };
            var result = TableUtil.MergeTableVectorsToArray(rowNames, colNames, data, 2, 3);
            Assert.AreEqual(2, result.GetLength(0));
            Assert.AreEqual(3, result.GetLength(1));
            Assert.AreEqual("0.0", result[0, 0]);
            Assert.AreEqual("1.0", result[0, 1]);
            Assert.AreEqual("2.0", result[0, 2]);
            Assert.AreEqual("3.0", result[1, 0]);
            Assert.AreEqual("4.0", result[1, 1]);
            Assert.AreEqual("5.0", result[1, 2]);

            result = TableUtil.MergeTableVectorsToArray(rowNames, colNames, data, 3, 2);
            Assert.AreEqual(3, result.GetLength(0));
            Assert.AreEqual(2, result.GetLength(1));
            Assert.AreEqual("0.0", result[0, 0]);
            Assert.AreEqual("1.0", result[0, 1]);
            Assert.AreEqual("2.0", result[1, 0]);
            Assert.AreEqual("3.0", result[1, 1]);
            Assert.AreEqual("4.0", result[2, 0]);
            Assert.AreEqual("5.0", result[2, 1]);
        }

        [TestMethod]
        public void GetDisplayableVector_Empty()
        {
            var data = new[,] {{"", "Col1", "Col2"}, {"Row1", "0.0", "1.0"}};
        }

        [TestMethod]
        public void Format_DataOnly()
        {
            var format = new TableFormat()
            {
                ColumnFilter = new FilterFormat(Constants.FilterPrefix.Column)
                {
                    Enabled = true, Type = Constants.FilterType.Exclude, Value = "1"
                },
                RowFilter = new FilterFormat(Constants.FilterPrefix.Row)
                {
                    Enabled = true,
                    Type = Constants.FilterType.Exclude,
                    Value = "1"
                }
            };
            var table = new Table(4, 3,
                new string[,] { { "", "Col1", "Col2" }, { "Row1", "0", "1" }, { "Row2", "2", "3" }, { "Row3", "4", "5" } });
            var result = TableUtil.GetDisplayableVector(table.Data, format);
            Assert.AreEqual(6, result.Length);
            Assert.AreEqual("0, 1, 2, 3, 4, 5", string.Join(", ", result));
        }

        [TestMethod]
        public void Format_DataAndRowNames()
        {
            var format = new TableFormat()
            {
                ColumnFilter = new FilterFormat(Constants.FilterPrefix.Column) { Enabled = false },
                RowFilter = new FilterFormat(Constants.FilterPrefix.Row)
                {
                    Enabled = true,
                    Type = Constants.FilterType.Exclude,
                    Value = "1"
                }
            };
            var table = new Table(4, 3,
                new string[,] { { "", "Col1", "Col2" }, { "Row1", "0", "1" }, { "Row2", "2", "3" }, { "Row3", "4", "5" } });
            var result = TableUtil.GetDisplayableVector(table.Data, format);
            Assert.AreEqual(9, result.Length);
            Assert.AreEqual("Row1, 0, 1, Row2, 2, 3, Row3, 4, 5", string.Join(", ", result));
        }

        [TestMethod]
        public void Format_DataAndColumnNames()
        {
            var format = new TableFormat()
            {
                RowFilter = new FilterFormat(Constants.FilterPrefix.Column) { Enabled = false },
                ColumnFilter = new FilterFormat(Constants.FilterPrefix.Row)
                {
                    Enabled = true,
                    Type = Constants.FilterType.Exclude,
                    Value = "1"
                }
            };
            var table = new Table(4, 3,
                new string[,] { { "", "Col1", "Col2" }, { "Row1", "0", "1" }, { "Row2", "2", "3" }, { "Row3", "4", "5" } });
            var result = TableUtil.GetDisplayableVector(table.Data, format);
            Assert.AreEqual(8, result.Length);
            Assert.AreEqual("Col1, Col2, 0, 1, 2, 3, 4, 5", string.Join(", ", result));
        }

        [TestMethod]
        public void Format_DataIncludeColumnAndRowNames()
        {
            var format = new TableFormat()
            {
                ColumnFilter = new FilterFormat(Constants.FilterPrefix.Column) { Enabled = false },
                RowFilter = new FilterFormat(Constants.FilterPrefix.Row) { Enabled = false }
            };
            var table = new Table(4, 3,
                new string[,] { { "", "Col1", "Col2" }, { "Row1", "0", "1" }, { "Row2", "2", "3" }, { "Row3", "4", "5" } });
            var result = TableUtil.GetDisplayableVector(table.Data, format);
            Assert.AreEqual(12, result.Length);
            Assert.AreEqual(", Col1, Col2, Row1, 0, 1, Row2, 2, 3, Row3, 4, 5", string.Join(", ", result));
        }

        [TestMethod]
        public void Format_EnabledFilterWithNoValue()
        {
            // Use this to detect for an error situation - the user has somehow specified that a filter should
            // be enabled, but the filter value is left empty.  We will try to guard against this in most
            // circumstances, but technically it could show up in execution.
            var format = new TableFormat()
            {
                ColumnFilter = new FilterFormat(Constants.FilterPrefix.Column) { Enabled = true, Type = "Exclude", Value = "" },
                RowFilter = new FilterFormat(Constants.FilterPrefix.Row) { Enabled = true, Type = "Exclude", Value = null }
            };
            var table = new Table(4, 3,
                new string[,] { { "", "Col1", "Col2" }, { "Row1", "0", "1" }, { "Row2", "2", "3" }, { "Row3", "4", "5" } });
            var result = TableUtil.GetDisplayableVector(table.Data, format);
            Assert.AreEqual(12, result.Length);
            Assert.AreEqual(", Col1, Col2, Row1, 0, 1, Row2, 2, 3, Row3, 4, 5", string.Join(", ", result));
        }
    }
}
