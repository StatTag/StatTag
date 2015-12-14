﻿using System;
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
        public void Parse_SingleParams()
        {
            // Check each parameter by itself to ensure there are no spacing/boundary errors in our regex
            var annotation = new Annotation();
            ValueParser.Parse("Value(Label=\"Test\")", annotation);
            Assert.AreEqual("Test", annotation.OutputLabel);
            ValueParser.Parse("Value(Type=\"Numeric\")", annotation);
            Assert.AreEqual("Numeric", annotation.ValueFormat.FormatType);
            ValueParser.Parse("Value(Decimals=5)", annotation);
            Assert.AreEqual(5, annotation.ValueFormat.DecimalPlaces);
            ValueParser.Parse("Value(Thousands=true)", annotation);
            Assert.IsTrue(annotation.ValueFormat.UseThousands);
            ValueParser.Parse("Value(DateFormat=\"MM-DD-YYYY\")", annotation);
            Assert.AreEqual("MM-DD-YYYY", annotation.ValueFormat.DateFormat);
            ValueParser.Parse("Value(TimeFormat=\"HH:MM:SS\")", annotation);
            Assert.AreEqual("HH:MM:SS", annotation.ValueFormat.TimeFormat);
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

            // Run it again, flipping the order of parameters to test it works in any order
            ValueParser.Parse("Value(Type=\"Numeric\", TimeFormat=\"HH:MM:SS\", Label=\"Test\", Thousands=true, Decimals=5, DateFormat=\"MM-DD-YYYY\")", annotation);
            Assert.AreEqual("Test", annotation.OutputLabel);
            Assert.AreEqual("Numeric", annotation.ValueFormat.FormatType);
            Assert.AreEqual(5, annotation.ValueFormat.DecimalPlaces);
            Assert.IsTrue(annotation.ValueFormat.UseThousands);
            Assert.AreEqual("MM-DD-YYYY", annotation.ValueFormat.DateFormat);
            Assert.AreEqual("HH:MM:SS", annotation.ValueFormat.TimeFormat);

            // Run one more time, playing around with spacing
            ValueParser.Parse("Value( Type = \"Numeric\" , TimeFormat = \"HH:MM:SS\" , Label = \"Test\" , Thousands = true , Decimals = 5 , DateFormat = \"MM-DD-YYYY\" )", annotation);
            Assert.AreEqual("Test", annotation.OutputLabel);
            Assert.AreEqual("Numeric", annotation.ValueFormat.FormatType);
            Assert.AreEqual(5, annotation.ValueFormat.DecimalPlaces);
            Assert.IsTrue(annotation.ValueFormat.UseThousands);
            Assert.AreEqual("MM-DD-YYYY", annotation.ValueFormat.DateFormat);
            Assert.AreEqual("HH:MM:SS", annotation.ValueFormat.TimeFormat);
        }
    }
}