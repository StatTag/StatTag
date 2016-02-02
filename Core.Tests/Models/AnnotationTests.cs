using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using AnalysisManager.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Models
{
    [TestClass]
    public class AnnotationTests
    {
        [TestMethod]
        public void Equals_Match()
        {
            var annotation1 = new Annotation()
            {
                OutputLabel= "Test",
                Type = Constants.AnnotationType.Value,
                LineStart = 1,
                LineEnd = 2
            };

            var annotation2 = new Annotation()
            {
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Value,
                LineStart = 3,
                LineEnd = 4
            };

            Assert.IsTrue(annotation1.Equals(annotation2));
            Assert.IsTrue(annotation2.Equals(annotation1));
        }

        [TestMethod]
        public void Equals_CaseDifference()
        {
            var annotation1 = new Annotation()
            {
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Value,
                LineStart = 1,
                LineEnd = 2
            };

            var annotation2 = new Annotation()
            {
                OutputLabel = "test",
                Type = Constants.AnnotationType.Value,
                LineStart = 3,
                LineEnd = 4
            };

            Assert.IsFalse(annotation1.Equals(annotation2));
            Assert.IsFalse(annotation2.Equals(annotation1));
        }

        [TestMethod]
        public void Equals_TypeDifference()
        {
            var annotation1 = new Annotation()
            {
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Value,
                LineStart = 1,
                LineEnd = 2
            };

            var annotation2 = new Annotation()
            {
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Figure,
                LineStart = 3,
                LineEnd = 4
            };

            Assert.IsFalse(annotation1.Equals(annotation2));
            Assert.IsFalse(annotation2.Equals(annotation1));
        }

        [TestMethod]
        public void FormattedResult_Empty()
        {
            var annotation = new Annotation();
            Assert.AreEqual(Constants.Placeholders.EmptyField, annotation.FormattedResult);

            annotation = new Annotation() { CachedResult = new List<CommandResult>() };
            Assert.AreEqual(Constants.Placeholders.EmptyField, annotation.FormattedResult);
        }

        [TestMethod]
        public void FormattedResult_Values()
        {
            var annotation = new Annotation() { CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "Test 1" } }) };
            Assert.AreEqual("Test 1", annotation.FormattedResult);

            annotation = new Annotation() { CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "Test 1" }, new CommandResult() { ValueResult = "Test 2" } }) };
            Assert.AreEqual("Test 2", annotation.FormattedResult);

            annotation = new Annotation() { CachedResult = new List<CommandResult>(new[] {  new CommandResult() { ValueResult = "1234" },  new CommandResult() { ValueResult = "456789" } }), Type = Constants.AnnotationType.Value, ValueFormat = new ValueFormat() { FormatType = Constants.ValueFormatType.Numeric, UseThousands = true}};
            Assert.AreEqual("456,789", annotation.FormattedResult);
        }

        [TestMethod]
        public void FormattedResult_ValuesBlank()
        {
            var annotation = new Annotation() { CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "" } }) };
            Assert.AreEqual(Constants.Placeholders.EmptyField, annotation.FormattedResult);

            annotation = new Annotation() { CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "    " }, new CommandResult() { ValueResult = "         " } }) };
            Assert.AreEqual(Constants.Placeholders.EmptyField, annotation.FormattedResult);
        }

        [TestMethod]
        public void ToString_Tests()
        {
            var annotation = new Annotation();
            Assert.AreEqual("AnalysisManager.Core.Models.Annotation", annotation.ToString());
            annotation.Type = Constants.AnnotationType.Figure;
            Assert.AreEqual("Figure", annotation.ToString());
            annotation.OutputLabel = "Test";
            Assert.AreEqual("Test", annotation.ToString());
        }

        [TestMethod]
        public void Serialize_Deserialize()
        {
            var annotation = new Annotation() { Type = Constants.AnnotationType.Value, CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "Test 1" } }) };
            var serialized = annotation.Serialize();
            var recreatedAnnotation = Annotation.Deserialize(serialized);
            Assert.AreEqual(annotation.CodeFile, recreatedAnnotation.CodeFile);
            Assert.AreEqual(annotation.FigureFormat, recreatedAnnotation.FigureFormat);
            Assert.AreEqual(annotation.FormattedResult, recreatedAnnotation.FormattedResult);
            Assert.AreEqual(annotation.LineEnd, recreatedAnnotation.LineEnd);
            Assert.AreEqual(annotation.LineStart, recreatedAnnotation.LineStart);
            Assert.AreEqual(annotation.OutputLabel, recreatedAnnotation.OutputLabel);
            Assert.AreEqual(annotation.RunFrequency, recreatedAnnotation.RunFrequency);
            Assert.AreEqual(annotation.Type, recreatedAnnotation.Type);
            Assert.AreEqual(annotation.ValueFormat, recreatedAnnotation.ValueFormat);
            Assert.AreEqual(annotation.FigureFormat, recreatedAnnotation.FigureFormat);
            Assert.AreEqual(annotation.TableFormat, recreatedAnnotation.TableFormat);
        }

        [TestMethod]
        public void NormalizeOutputLabel_Blanks()
        {
            Assert.AreEqual(string.Empty, Annotation.NormalizeOutputLabel(null));
            Assert.AreEqual(string.Empty, Annotation.NormalizeOutputLabel(string.Empty));
            Assert.AreEqual(string.Empty, Annotation.NormalizeOutputLabel("   "));
        }

        [TestMethod]
        public void NormalizeOutputLabel_Values()
        {
            Assert.AreEqual("Test", Annotation.NormalizeOutputLabel("Test"));
            Assert.AreEqual("Test", Annotation.NormalizeOutputLabel("|Test"));
            Assert.AreEqual("Test", Annotation.NormalizeOutputLabel("   |   Test"));
            Assert.AreEqual("Test", Annotation.NormalizeOutputLabel("Test|"));
            Assert.AreEqual("Test", Annotation.NormalizeOutputLabel("Test |   "));
            Assert.AreEqual("Test one", Annotation.NormalizeOutputLabel("Test|one"));
        }

        [TestMethod]
        public void IsTableAnnotation()
        {
            Assert.IsTrue(new Annotation() { Type = Constants.AnnotationType.Table }.IsTableAnnotation());
            Assert.IsFalse(new Annotation() { Type = Constants.AnnotationType.Value }.IsTableAnnotation());
            Assert.IsFalse(new Annotation() { Type = string.Empty }.IsTableAnnotation());
        }
    }
}
