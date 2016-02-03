﻿using System;
using AnalysisManager.Core.Models;
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

        [TestMethod]
        public void Format_DataOnly()
        {
            var format = new TableFormat() { IncludeColumnNames = false, IncludeRowNames = false };
            var table = new Table(new[] {"Row1", "Row2"}, new[] {"Col1", "Col2"}, 2, 2,
                new double[4] {0.0, 1.0, 2.0, 3.0});
            Assert.AreEqual(4, format.Format(table).Length);
            Assert.AreEqual("0, 1, 2, 3", string.Join(", ", format.Format(table)));

            table = new Table(null, null, 2, 2, new double[4] { 0.0, 1.0, 2.0, 3.0 });
            Assert.AreEqual(4, format.Format(table).Length);
            Assert.AreEqual("0, 1, 2, 3", string.Join(", ", format.Format(table)));
        }

        [TestMethod]
        public void Format_DataAndColumns()
        {
            var format = new TableFormat() { IncludeColumnNames = true, IncludeRowNames = false };
            var table = new Table(new[] { "Row1", "Row2" }, new[] { "Col1", "Col2" }, 2, 2,
                new double[4] { 0.0, 1.0, 2.0, 3.0 });
            Assert.AreEqual(6, format.Format(table).Length);
            Assert.AreEqual("Col1, Col2, 0, 1, 2, 3", string.Join(", ", format.Format(table)));

            table = new Table(null, null, 2, 2, new double[4] { 0.0, 1.0, 2.0, 3.0 });
            Assert.AreEqual(4, format.Format(table).Length);
            Assert.AreEqual("0, 1, 2, 3", string.Join(", ", format.Format(table)));
        }

        [TestMethod]
        public void Format_DataAndRows()
        {
            var format = new TableFormat() { IncludeColumnNames = false, IncludeRowNames = true };
            var table = new Table(new[] { "Row1", "Row2" }, new[] { "Col1", "Col2" }, 2, 2,
                new double[4] { 0.0, 1.0, 2.0, 3.0 });
            Assert.AreEqual(6, format.Format(table).Length);
            Assert.AreEqual("Row1, 0, 1, Row2, 2, 3", string.Join(", ", format.Format(table)));

            table = new Table(null, null, 2, 2, new double[4] { 0.0, 1.0, 2.0, 3.0 });
            Assert.AreEqual(4, format.Format(table).Length);
            Assert.AreEqual("0, 1, 2, 3", string.Join(", ", format.Format(table)));
        }

        [TestMethod]
        public void Format_DataColumnsAndRows()
        {
            var format = new TableFormat() { IncludeColumnNames = true, IncludeRowNames = true };
            var table = new Table(new[] { "Row1", "Row2" }, new[] { "Col1", "Col2" }, 2, 2,
                new double[4] { 0.0, 1.0, 2.0, 3.0 });
            Assert.AreEqual(9, format.Format(table).Length);
            Assert.AreEqual(", Col1, Col2, Row1, 0, 1, Row2, 2, 3", string.Join(", ", format.Format(table)));

            table = new Table(null, null, 2, 2, new double[4] { 0.0, 1.0, 2.0, 3.0 });
            Assert.AreEqual(4, format.Format(table).Length);
            Assert.AreEqual("0, 1, 2, 3", string.Join(", ", format.Format(table)));
        }
    }
}
