﻿using System;
using StatTag.Core.Generator;
using StatTag.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Generator
{
    [TestClass]
    public class ValueParameterGeneratorTests
    {
        [TestMethod]
        public void CreateDefaultParameters()
        {
            var generator = new ValueParameterGenerator();
            Assert.AreEqual("Type=\"Default\", ", generator.CreateDefaultParameters());
            // It will only add the AllowInvalidTypes attribute when it is not the default value
            Assert.AreEqual("Type=\"Default\", AllowInvalid=True, ", generator.CreateDefaultParameters(Constants.ValueFormatType.Default, true));
        }

        [TestMethod]
        public void CreatePercentageParameters_Default()
        {
            var generator = new ValueParameterGenerator();
            Assert.AreEqual("Decimals=0", generator.CreatePercentageParameters(new ValueFormat()));
        }

        [TestMethod]
        public void CreatePercentageParameters_Value()
        {
            var generator = new ValueParameterGenerator();
            Assert.AreEqual("Decimals=2", generator.CreatePercentageParameters(new ValueFormat() { DecimalPlaces = 2 }));
        }

        [TestMethod]
        public void CreateNumericParameters_Default()
        {
            var generator = new ValueParameterGenerator();
            Assert.AreEqual("Decimals=0, Thousands=False", generator.CreateNumericParameters(new ValueFormat()));
        }

        [TestMethod]
        public void CreateNumericParameters_Values()
        {
            var generator = new ValueParameterGenerator();
            Assert.AreEqual("Decimals=1, Thousands=False", generator.CreateNumericParameters(new ValueFormat() { DecimalPlaces = 1}));
            Assert.AreEqual("Decimals=0, Thousands=True", generator.CreateNumericParameters(new ValueFormat() { UseThousands = true}));
            Assert.AreEqual("Decimals=2, Thousands=True", generator.CreateNumericParameters(new ValueFormat() { DecimalPlaces = 2, UseThousands = true}));
        }

        [TestMethod]
        public void CreateDateTimeParameters_Default()
        {
            var generator = new ValueParameterGenerator();
            Assert.AreEqual("", generator.CreateDateTimeParameters(new ValueFormat()));
        }

        [TestMethod]
        public void CreateDateTimeParameters_Values()
        {
            var generator = new ValueParameterGenerator();
            Assert.AreEqual("DateFormat=\"MM-DD-YYYY\"", generator.CreateDateTimeParameters(new ValueFormat() { DateFormat = "MM-DD-YYYY"}));
            Assert.AreEqual("TimeFormat=\"HH:MM:SS\"", generator.CreateDateTimeParameters(new ValueFormat() { TimeFormat = "HH:MM:SS" }));
            Assert.AreEqual("DateFormat=\"MM-DD-YYYY\", TimeFormat=\"HH:MM:SS\"", generator.CreateDateTimeParameters(new ValueFormat() { DateFormat = "MM-DD-YYYY", TimeFormat = "HH:MM:SS" }));
        }

        [TestMethod]
        public void CreateParameters_Default()
        {
            var generator = new ValueParameterGenerator();
            Assert.AreEqual("Type=\"Default\"", generator.CreateParameters(new Tag()));
            Assert.AreEqual("Label=\"Test\", Type=\"Default\"", generator.CreateParameters(new Tag() {Name = "Test" }));
            Assert.AreEqual("Type=\"Default\"", generator.CreateParameters(new Tag() { ValueFormat = new ValueFormat() { FormatType = "Unknown" } }));
        }

        [TestMethod]
        public void CreateParameters_RunFrequency()
        {
            var generator = new ValueParameterGenerator();
            Assert.AreEqual("Frequency=\"On Demand\", Type=\"Default\"", generator.CreateParameters(new Tag() { RunFrequency = Constants.RunFrequency.OnDemand }));
            Assert.AreEqual("Label=\"Test\", Frequency=\"On Demand\", Type=\"Default\"", generator.CreateParameters(new Tag() { Name = "Test", RunFrequency = Constants.RunFrequency.OnDemand }));
            Assert.AreEqual("Type=\"Default\"", generator.CreateParameters(new Tag() { ValueFormat = new ValueFormat() { FormatType = "Unknown" } }));
        }

        [TestMethod]
        public void CreateParameters_EachType()
        {
            var generator = new ValueParameterGenerator();
            var tag = new Tag() {ValueFormat = new ValueFormat()};
            tag.ValueFormat.FormatType = Constants.ValueFormatType.Numeric;
            Assert.IsTrue(generator.CreateParameters(tag).StartsWith("Type=\"Numeric\""));
            Assert.AreNotEqual("Type=\"Numeric\"", generator.CreateParameters(tag)); // Should be more than the type parameter
            tag.ValueFormat.FormatType = Constants.ValueFormatType.DateTime;
            Assert.IsTrue(generator.CreateParameters(tag).StartsWith("Type=\"DateTime\""));
            Assert.AreEqual("Type=\"DateTime\"", generator.CreateParameters(tag)); // Should be just the type parameter by default
            tag.ValueFormat.DateFormat = "MM-DD-YYYY";
            Assert.AreNotEqual("Type=\"DateTime\"", generator.CreateParameters(tag)); // Should be more than the type parameter when we have a format
            tag.ValueFormat.FormatType = Constants.ValueFormatType.Percentage;
            Assert.IsTrue(generator.CreateParameters(tag).StartsWith("Type=\"Percentage\""));
            Assert.AreNotEqual("Type=\"Percentage\"", generator.CreateParameters(tag)); // Should be more than the type parameter
        }
    }
}
