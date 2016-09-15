using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Models;

namespace Core.Tests.Models
{
    [TestClass]
    public class TableTests
    {
        [TestMethod]
        public void EmptyConstructor()
        {
            var table = new Table();
            Assert.AreEqual(0, table.RowSize);
            Assert.AreEqual(0, table.ColumnSize);
            Assert.IsNull(table.Data);
            Assert.IsNull(table.FormattedCells);
        }

        [TestMethod]
        public void DataConstructor()
        {
            var table = new Table(2, 3, new [,] { {"1", "2", "3"}, {"4", "5", "6"} });
            Assert.AreEqual(2, table.RowSize);
            Assert.AreEqual(3, table.ColumnSize);
            Assert.IsNotNull(table.Data);
            Assert.AreEqual(6, table.Data.Length);
        }

        [TestMethod]
        public void DataConstructor_Empty()
        {
            var table = new Table(0, 3, null);
            Assert.AreEqual(0, table.RowSize);
            Assert.AreEqual(3, table.ColumnSize);
            Assert.IsNull(table.Data);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DataConstructor_Invalid()
        {
            var table = new Table(2, 2, new[,] { {"1", "2", "3"}, {"4", "5", "6"} });
        }

        [TestMethod]
        public void IsEmpty()
        {
            var table = new Table(2, 3, new[,] { { "1", "2", "3" }, { "4", "5", "6" } });
            Assert.IsFalse(table.IsEmpty());

            table = new Table(0, 3, null);
            Assert.IsTrue(table.IsEmpty());

            table = new Table(3, 0, null);
            Assert.IsTrue(table.IsEmpty());

            table = new Table();
            Assert.IsTrue(table.IsEmpty());
        }
    }
}
