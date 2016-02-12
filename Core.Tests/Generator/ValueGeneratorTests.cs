using System;
using AnalysisManager.Core.Generator;
using AnalysisManager.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Generator
{
    [TestClass]
    public class ValueGeneratorTests
    {
        [TestMethod]
        public void CreateDefaultParameters()
        {
            var generator = new ValueGenerator();
            Assert.AreEqual("Type=\"Default\", ", generator.CreateDefaultParameters());
            // It will only add the AllowInvalidTypes attribute when it is not the default value
            Assert.AreEqual("Type=\"Default\", AllowInvalid=True, ", generator.CreateDefaultParameters(Constants.ValueFormatType.Default, true));
        }

        [TestMethod]
        public void CreatePercentageParameters_Default()
        {
            var generator = new ValueGenerator();
            Assert.AreEqual("Decimals=0", generator.CreatePercentageParameters(new ValueFormat()));
        }

        [TestMethod]
        public void CreatePercentageParameters_Value()
        {
            var generator = new ValueGenerator();
            Assert.AreEqual("Decimals=2", generator.CreatePercentageParameters(new ValueFormat() { DecimalPlaces = 2 }));
        }

        [TestMethod]
        public void CreateNumericParameters_Default()
        {
            var generator = new ValueGenerator();
            Assert.AreEqual("Decimals=0, Thousands=False", generator.CreateNumericParameters(new ValueFormat()));
        }

        [TestMethod]
        public void CreateNumericParameters_Values()
        {
            var generator = new ValueGenerator();
            Assert.AreEqual("Decimals=1, Thousands=False", generator.CreateNumericParameters(new ValueFormat() { DecimalPlaces = 1}));
            Assert.AreEqual("Decimals=0, Thousands=True", generator.CreateNumericParameters(new ValueFormat() { UseThousands = true}));
            Assert.AreEqual("Decimals=2, Thousands=True", generator.CreateNumericParameters(new ValueFormat() { DecimalPlaces = 2, UseThousands = true}));
        }

        [TestMethod]
        public void CreateDateTimeParameters_Default()
        {
            var generator = new ValueGenerator();
            Assert.AreEqual("", generator.CreateDateTimeParameters(new ValueFormat()));
        }

        [TestMethod]
        public void CreateDateTimeParameters_Values()
        {
            var generator = new ValueGenerator();
            Assert.AreEqual("DateFormat=\"MM-DD-YYYY\"", generator.CreateDateTimeParameters(new ValueFormat() { DateFormat = "MM-DD-YYYY"}));
            Assert.AreEqual("TimeFormat=\"HH:MM:SS\"", generator.CreateDateTimeParameters(new ValueFormat() { TimeFormat = "HH:MM:SS" }));
            Assert.AreEqual("DateFormat=\"MM-DD-YYYY\", TimeFormat=\"HH:MM:SS\"", generator.CreateDateTimeParameters(new ValueFormat() { DateFormat = "MM-DD-YYYY", TimeFormat = "HH:MM:SS" }));
        }

        [TestMethod]
        public void CreateParameters_Default()
        {
            var generator = new ValueGenerator();
            Assert.AreEqual("Id=\"id\", Type=\"Default\"", generator.CreateParameters(new Annotation() { Id = "id" }));
            Assert.AreEqual("Id=\"id\", Label=\"Test\", Type=\"Default\"", generator.CreateParameters(new Annotation() {Id = "id", OutputLabel = "Test" }));
            Assert.AreEqual("Id=\"id\", Type=\"Default\"", generator.CreateParameters(new Annotation() { Id = "id", ValueFormat = new ValueFormat() { FormatType = "Unknown" } }));
        }

        [TestMethod]
        public void CreateParameters_RunFrequency()
        {
            var generator = new ValueGenerator();
            Assert.AreEqual("Id=\"id\", Frequency=\"On Demand\", Type=\"Default\"", generator.CreateParameters(new Annotation() { Id = "id", RunFrequency = Constants.RunFrequency.OnDemand }));
            Assert.AreEqual("Id=\"id\", Label=\"Test\", Frequency=\"On Demand\", Type=\"Default\"", generator.CreateParameters(new Annotation() { Id = "id", OutputLabel = "Test", RunFrequency = Constants.RunFrequency.OnDemand }));
            Assert.AreEqual("Id=\"id\", Type=\"Default\"", generator.CreateParameters(new Annotation() { Id = "id", ValueFormat = new ValueFormat() { FormatType = "Unknown" } }));
        }

        [TestMethod]
        public void CreateParameters_EachType()
        {
            var generator = new ValueGenerator();
            var annotation = new Annotation() {Id = "id", ValueFormat = new ValueFormat()};
            annotation.ValueFormat.FormatType = Constants.ValueFormatType.Numeric;
            Assert.IsTrue(generator.CreateParameters(annotation).StartsWith("Id=\"id\", Type=\"Numeric\""));
            Assert.AreNotEqual("Id=\"id\", Type=\"Numeric\"", generator.CreateParameters(annotation)); // Should be more than the type parameter
            annotation.ValueFormat.FormatType = Constants.ValueFormatType.DateTime;
            Assert.IsTrue(generator.CreateParameters(annotation).StartsWith("Id=\"id\", Type=\"DateTime\""));
            Assert.AreEqual("Id=\"id\", Type=\"DateTime\"", generator.CreateParameters(annotation)); // Should be just the type parameter by default
            annotation.ValueFormat.DateFormat = "MM-DD-YYYY";
            Assert.AreNotEqual("Id=\"id\", Type=\"DateTime\"", generator.CreateParameters(annotation)); // Should be more than the type parameter when we have a format
            annotation.ValueFormat.FormatType = Constants.ValueFormatType.Percentage;
            Assert.IsTrue(generator.CreateParameters(annotation).StartsWith("Id=\"id\", Type=\"Percentage\""));
            Assert.AreNotEqual("Id=\"id\", Type=\"Percentage\"", generator.CreateParameters(annotation)); // Should be more than the type parameter
        }
    }
}
