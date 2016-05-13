using System;
using StatTag.Core.Models;
using StatTag.Core.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Parser
{
    [TestClass]
    public class TableParserTests
    {
        [TestMethod]
        public void Parse_EmptyParams_Defaults()
        {
            var tag = new Tag();
            TableParameterParser.Parse("Table", tag);
            Assert.AreEqual(Constants.RunFrequency.Default, tag.RunFrequency);
            Assert.AreEqual(Constants.TableParameterDefaults.ColumnNames, tag.TableFormat.IncludeColumnNames);
            Assert.AreEqual(Constants.TableParameterDefaults.RowNames, tag.TableFormat.IncludeRowNames);
        }

        [TestMethod]
        public void Parse_SingleParams()
        {
            // Check each parameter by itself to ensure there are no spacing/boundary errors in our regex
            var tag = new Tag();
            TableParameterParser.Parse("Table(Label=\"Test\")", tag);
            Assert.AreEqual("Test", tag.OutputLabel);
            TableParameterParser.Parse("Table(ColumnNames=True)", tag);
            Assert.IsTrue(tag.TableFormat.IncludeColumnNames);
            TableParameterParser.Parse("Table(RowNames=True)", tag);
            Assert.IsTrue(tag.TableFormat.IncludeRowNames);
        }

        [TestMethod]
        public void Parse_AllParams()
        {
            var tag = new Tag();
            TableParameterParser.Parse("Table(Label=\"Test\", ColumnNames=True, RowNames=False)", tag);
            Assert.AreEqual("Test", tag.OutputLabel);
            Assert.IsTrue(tag.TableFormat.IncludeColumnNames);
            Assert.IsFalse(tag.TableFormat.IncludeRowNames);

            // Run it again, flipping the order of parameters to test it works in any order
            TableParameterParser.Parse("Table(RowNames=True, ColumnNames=False, Label=\"Test\")", tag);
            Assert.AreEqual("Test", tag.OutputLabel);
            Assert.IsFalse(tag.TableFormat.IncludeColumnNames);
            Assert.IsTrue(tag.TableFormat.IncludeRowNames);

            // Run one more time, playing around with spacing
            TableParameterParser.Parse("Table( RowNames = True , ColumnNames = True , Label = \"Test\" ) ", tag);
            Assert.AreEqual("Test", tag.OutputLabel);
            Assert.IsTrue(tag.TableFormat.IncludeColumnNames);
            Assert.IsTrue(tag.TableFormat.IncludeRowNames);
        }
    }
}
