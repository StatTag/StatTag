using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Models;
using System.IO;

namespace Core.Tests.Models
{
    [TestClass]
    public class DataFileToTableTests
    {
        [TestMethod]
        public void GetCSVTableDimensions_NonExistant()
        {
            var folder = TestUtil.GetTestDataFolder("CSV");
            var dimensions = DataFileToTable.GetCSVTableDimensions(Path.Combine(folder, "notarealfile.csv"));
            Assert.IsNull(dimensions);
        }

        [TestMethod]
        public void GetCSVTableDimensions_Empty()
        {
            var folder = TestUtil.GetTestDataFolder("CSV");
            var dimensions = DataFileToTable.GetCSVTableDimensions(Path.Combine(folder, "empty.csv"));
            Assert.AreEqual(0, dimensions[0]);
            Assert.AreEqual(0, dimensions[1]);
        }

        [TestMethod]
        public void GetCSVTableDimensions_Unbalanced()
        {
            var folder = TestUtil.GetTestDataFolder("CSV");
            var dimensions = DataFileToTable.GetCSVTableDimensions(Path.Combine(folder, "unbalanced.csv"));
            Assert.AreEqual(10, dimensions[0]);
            Assert.AreEqual(5, dimensions[1]);
        }

        [TestMethod]
        public void GetCSVTableDimensions_Balanced()
        {
            var folder = TestUtil.GetTestDataFolder("CSV");
            var dimensions = DataFileToTable.GetCSVTableDimensions(Path.Combine(folder, "balanced.csv"));
            Assert.AreEqual(4, dimensions[0]);
            Assert.AreEqual(3, dimensions[1]);
        }

        [TestMethod]
        public void GetTableResults_NonExistant()
        {
            var folder = TestUtil.GetTestDataFolder("CSV");
            var table = DataFileToTable.GetTableResult(Path.Combine(folder, "notarealfile.csv"));
            Assert.AreEqual(0, table.RowSize);
            Assert.AreEqual(0, table.ColumnSize);
            Assert.IsNull(table.Data);
        }

        [TestMethod]
        public void GetTableResults_CSV_Empty()
        {
            var folder = TestUtil.GetTestDataFolder("CSV");
            var table = DataFileToTable.GetTableResult(Path.Combine(folder, "empty.csv"));
            Assert.AreEqual(0, table.RowSize);
            Assert.AreEqual(0, table.ColumnSize);
            Assert.IsNull(table.Data);
        }

        [TestMethod]
        public void GetTableResults_XLSX_Empty()
        {
            var folder = TestUtil.GetTestDataFolder("XLSX");
            var table = DataFileToTable.GetTableResult(Path.Combine(folder, "empty.XLSX"));
            Assert.AreEqual(0, table.RowSize);
            Assert.AreEqual(0, table.ColumnSize);
            Assert.IsNull(table.Data);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetTableResults_UnsupportedXLS()
        {
            var folder = TestUtil.GetTestDataFolder("CSV");
            var table = DataFileToTable.GetTableResult(Path.Combine(folder, "empty.xls"));
        }

        [TestMethod]
        public void GetTableResults_CSV_Unbalanced()
        {
            var folder = TestUtil.GetTestDataFolder("CSV");
            var table = DataFileToTable.GetTableResult(Path.Combine(folder, "unbalanced.csv"));
            Assert.AreEqual(10, table.RowSize);
            Assert.AreEqual(5, table.ColumnSize);
            Assert.AreEqual((table.RowSize * table.ColumnSize), table.Data.Length);
            Assert.AreEqual(string.Empty, table.Data[0,0]);
            Assert.AreEqual("Frequency", table.Data[0,1]);
            Assert.AreEqual(string.Empty, table.Data[0,2]);
            Assert.AreEqual(string.Empty, table.Data[0,3]);
            Assert.AreEqual(string.Empty, table.Data[0,4]);
            Assert.AreEqual("Table of role_name by Status", table.Data[1,0]);
        }

        [TestMethod]
        public void GetTableResults_XLSX_Unbalanced()
        {
            var folder = TestUtil.GetTestDataFolder("XLSX");
            var table = DataFileToTable.GetTableResult(Path.Combine(folder, "unbalanced.xlSx"));
            Assert.AreEqual(11, table.RowSize);
            Assert.AreEqual(5, table.ColumnSize);
            Assert.AreEqual((table.RowSize * table.ColumnSize), table.Data.Length);
            Assert.AreEqual(string.Empty, table.Data[0, 0]);
            Assert.AreEqual("Frequency", table.Data[0, 1]);
            Assert.AreEqual(string.Empty, table.Data[0, 2]);
            Assert.AreEqual(string.Empty, table.Data[0, 3]);
            Assert.AreEqual(string.Empty, table.Data[0, 4]);
            Assert.AreEqual("Table of role_name by Status", table.Data[2, 0]);
        }

        [TestMethod]
        public void GetTableResults_CSV_Balanced()
        {
            var folder = TestUtil.GetTestDataFolder("CSV");
            var table = DataFileToTable.GetTableResult(Path.Combine(folder, "balanced.csv"));
            Assert.AreEqual(4, table.RowSize);
            Assert.AreEqual(3, table.ColumnSize);
            Assert.AreEqual((table.RowSize * table.ColumnSize), table.Data.Length);
            Assert.AreEqual("Status", table.Data[0,0]);
            Assert.AreEqual("11.54", table.Data[3,2]);
        }

        [TestMethod]
        public void GetTableResults_XLSX_Balanced()
        {
            var folder = TestUtil.GetTestDataFolder("XLSX");
            var table = DataFileToTable.GetTableResult(Path.Combine(folder, "balanced.xlsx"));
            Assert.AreEqual(4, table.RowSize);
            Assert.AreEqual(3, table.ColumnSize);
            Assert.AreEqual((table.RowSize * table.ColumnSize), table.Data.Length);
            Assert.AreEqual("Status", table.Data[0, 0]);
            Assert.AreEqual("11.54", table.Data[3, 2]);
        }
    }
}
