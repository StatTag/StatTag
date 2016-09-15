using System;
using StatTag.Core.Generator;
using StatTag.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Generator
{
    [TestClass]
    public class TableParameterGeneratorTests
    {
        [TestMethod]
        public void CreateParameters_NoLabel()
        {
            var generator = new TableParameterGenerator();
            Assert.AreEqual("", generator.CreateParameters(new Tag()));
        }

        [TestMethod]
        public void CreateParameters_Label()
        {
            var generator = new TableParameterGenerator();
            Assert.AreEqual("Label=\"Test\"", generator.CreateParameters(new Tag() { Name = "Test" }));
        }

        [TestMethod]
        public void CreateParameters_FiltersDisabled()
        {
            var generator = new TableParameterGenerator();

            // Null table format
            Assert.AreEqual("Label=\"Test\"", generator.CreateParameters(new Tag() { Name = "Test" }));

            // Row and column filters both disabled
            Assert.AreEqual("Label=\"Test\"",
                generator.CreateParameters(new Tag()
                {
                    Name = "Test",
                    TableFormat = new TableFormat()
                    {
                        ColumnFilter = new FilterFormat("Column") {Enabled = false},
                        RowFilter = new FilterFormat("Row") { Enabled = false }
                    }
                }));
        }

        [TestMethod]
        public void CreateParameters_RowFilter()
        {
            var generator = new TableParameterGenerator();
            Assert.AreEqual("Label=\"Test\", RowFilterEnabled=True, RowFilterType=\"Exclude\", RowFilterValue=\"1\"",
                generator.CreateParameters(new Tag()
                {
                    Name = "Test",
                    TableFormat = new TableFormat()
                    {
                        RowFilter = new FilterFormat("Row") { Enabled = true, Type = Constants.FilterType.Exclude, Value = "1" }
                    }
                }));
        }

        [TestMethod]
        public void CreateParameters_ColumnFilter()
        {
            var generator = new TableParameterGenerator();
            Assert.AreEqual("Label=\"Test\", ColumnFilterEnabled=True, ColumnFilterType=\"Exclude\", ColumnFilterValue=\"1\"",
                generator.CreateParameters(new Tag()
                {
                    Name = "Test",
                    TableFormat = new TableFormat()
                    {
                        ColumnFilter = new FilterFormat("Column") { Enabled = true, Type = Constants.FilterType.Exclude, Value = "1" }
                    }
                }));
        }
    }
}
