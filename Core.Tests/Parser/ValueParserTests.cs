using System;
using AnalysisManager.Core.Models;
using AnalysisManager.Core.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Parser
{
    [TestClass]
    public class ValueParserTests
    {
        [TestMethod]
        public void Parse_EmptyParams_Defaults()
        {
            var annotation = new Annotation();
            ValueParser.Parse("Value", annotation);
            Assert.AreEqual(Constants.ValueFormatType.Default, annotation.ValueFormat.FormatType);
        }

        [TestMethod]
        public void Parse_EmptyParams_Label()
        {
            const string label = "Test";
            var annotation = new Annotation();
            ValueParser.Parse("Value(Label=\"" + label + "\")", annotation);
            Assert.AreEqual(label, annotation.OutputLabel);
        }

        [TestMethod]
        public void Parse_AllParams()
        {
            var annotation = new Annotation();
            ValueParser.Parse("Value(Label=\"Test\", Type=\"Numeric\", Decimals=5, Thousands=true, DateFormat=\"MM-DD-YYYY\", TimeFormat=\"HH:MM:SS\")", annotation);
            Assert.AreEqual("Test", annotation.OutputLabel);
            Assert.AreEqual("Numeric", annotation.ValueFormat.FormatType);
            Assert.AreEqual(5, annotation.ValueFormat.DecimalPlaces);
            Assert.IsTrue(annotation.ValueFormat.UseThousands);
            Assert.AreEqual("MM-DD-YYYY", annotation.ValueFormat.DateFormat);
            Assert.AreEqual("HH:MM:SS", annotation.ValueFormat.TimeFormat);
        }
    }
}
