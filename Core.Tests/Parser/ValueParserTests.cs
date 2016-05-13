using System;
using StatTag.Core.Models;
using StatTag.Core.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Parser
{
    [TestClass]
    public class ValueParserTests
    {
        [TestMethod]
        public void Parse_EmptyParams_Defaults()
        {
            var tag = new Tag();
            ValueParameterParser.Parse("Value", tag);
            Assert.AreEqual(Constants.ValueFormatType.Default, tag.ValueFormat.FormatType);
            Assert.AreEqual(Constants.RunFrequency.Default, tag.RunFrequency);
        }

        [TestMethod]
        public void Parse_SingleParams()
        {
            // Check each parameter by itself to ensure there are no spacing/boundary errors in our regex
            var tag = new Tag();
            ValueParameterParser.Parse("Value(Label=\"Test\")", tag);
            Assert.AreEqual("Test", tag.OutputLabel);
            ValueParameterParser.Parse("Value(Type=\"Numeric\")", tag);
            Assert.AreEqual("Numeric", tag.ValueFormat.FormatType);
            ValueParameterParser.Parse("Value(Decimals=5)", tag);
            Assert.AreEqual(5, tag.ValueFormat.DecimalPlaces);
            ValueParameterParser.Parse("Value(Thousands=true)", tag);
            Assert.IsTrue(tag.ValueFormat.UseThousands);
            ValueParameterParser.Parse("Value(DateFormat=\"MM-DD-YYYY\")", tag);
            Assert.AreEqual("MM-DD-YYYY", tag.ValueFormat.DateFormat);
            ValueParameterParser.Parse("Value(TimeFormat=\"HH:MM:SS\")", tag);
            Assert.AreEqual("HH:MM:SS", tag.ValueFormat.TimeFormat);
        }

        [TestMethod]
        public void Parse_AllParams()
        {
            var tag = new Tag();
            ValueParameterParser.Parse("Value(Label=\"Test\", Type=\"Numeric\", Decimals=5, Thousands=true, DateFormat=\"MM-DD-YYYY\", TimeFormat=\"HH:MM:SS\")", tag);
            Assert.AreEqual("Test", tag.OutputLabel);
            Assert.AreEqual("Numeric", tag.ValueFormat.FormatType);
            Assert.AreEqual(5, tag.ValueFormat.DecimalPlaces);
            Assert.IsTrue(tag.ValueFormat.UseThousands);
            Assert.AreEqual("MM-DD-YYYY", tag.ValueFormat.DateFormat);
            Assert.AreEqual("HH:MM:SS", tag.ValueFormat.TimeFormat);

            // Run it again, flipping the order of parameters to test it works in any order
            ValueParameterParser.Parse("Value(Type=\"Numeric\", TimeFormat=\"HH:MM:SS\", Label=\"Test\", Thousands=true, Decimals=5, DateFormat=\"MM-DD-YYYY\")", tag);
            Assert.AreEqual("Test", tag.OutputLabel);
            Assert.AreEqual("Numeric", tag.ValueFormat.FormatType);
            Assert.AreEqual(5, tag.ValueFormat.DecimalPlaces);
            Assert.IsTrue(tag.ValueFormat.UseThousands);
            Assert.AreEqual("MM-DD-YYYY", tag.ValueFormat.DateFormat);
            Assert.AreEqual("HH:MM:SS", tag.ValueFormat.TimeFormat);

            // Run one more time, playing around with spacing
            ValueParameterParser.Parse("Value( Type = \"Numeric\" , TimeFormat = \"HH:MM:SS\" , Label = \"Test\" , Thousands = true , Decimals = 5 , DateFormat = \"MM-DD-YYYY\" )", tag);
            Assert.AreEqual("Test", tag.OutputLabel);
            Assert.AreEqual("Numeric", tag.ValueFormat.FormatType);
            Assert.AreEqual(5, tag.ValueFormat.DecimalPlaces);
            Assert.IsTrue(tag.ValueFormat.UseThousands);
            Assert.AreEqual("MM-DD-YYYY", tag.ValueFormat.DateFormat);
            Assert.AreEqual("HH:MM:SS", tag.ValueFormat.TimeFormat);
        }
    }
}
