using System;
using AnalysisManager.Core.Models;
using AnalysisManager.Core.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Parser
{
    [TestClass]
    public class TableParserTests
    {
        [TestMethod]
        public void Parse_EmptyParams_Defaults()
        {
            var annotation = new Annotation();
            TableParser.Parse("Table", annotation);
            Assert.AreEqual(Constants.RunFrequency.Default, annotation.RunFrequency);
            Assert.AreEqual(Constants.TableParameterDefaults.ColumnNames, annotation.TableFormat.IncludeColumnNames);
            Assert.AreEqual(Constants.TableParameterDefaults.RowNames, annotation.TableFormat.IncludeRowNames);
        }

        [TestMethod]
        public void Parse_SingleParams()
        {
            // Check each parameter by itself to ensure there are no spacing/boundary errors in our regex
            var annotation = new Annotation();
            TableParser.Parse("Table(Label=\"Test\")", annotation);
            Assert.AreEqual("Test", annotation.OutputLabel);
            TableParser.Parse("Table(ColumnNames=True)", annotation);
            Assert.IsTrue(annotation.TableFormat.IncludeColumnNames);
            TableParser.Parse("Table(RowNames=True)", annotation);
            Assert.IsTrue(annotation.TableFormat.IncludeRowNames);
        }

        [TestMethod]
        public void Parse_AllParams()
        {
            var annotation = new Annotation();
            TableParser.Parse("Table(Label=\"Test\", ColumnNames=True, RowNames=False)", annotation);
            Assert.AreEqual("Test", annotation.OutputLabel);
            Assert.IsTrue(annotation.TableFormat.IncludeColumnNames);
            Assert.IsFalse(annotation.TableFormat.IncludeRowNames);

            // Run it again, flipping the order of parameters to test it works in any order
            TableParser.Parse("Table(RowNames=True, ColumnNames=False, Label=\"Test\")", annotation);
            Assert.AreEqual("Test", annotation.OutputLabel);
            Assert.IsFalse(annotation.TableFormat.IncludeColumnNames);
            Assert.IsTrue(annotation.TableFormat.IncludeRowNames);

            // Run one more time, playing around with spacing
            TableParser.Parse("Table( RowNames = True , ColumnNames = True , Label = \"Test\" ) ", annotation);
            Assert.AreEqual("Test", annotation.OutputLabel);
            Assert.IsTrue(annotation.TableFormat.IncludeColumnNames);
            Assert.IsTrue(annotation.TableFormat.IncludeRowNames);
        }
    }
}
