using System;
using System.Collections.Generic;
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
            Assert.AreEqual(string.Empty, annotation.FormattedResult);

            annotation = new Annotation() { CachedResult = new List<string>() };
            Assert.AreEqual(string.Empty, annotation.FormattedResult);
        }

        [TestMethod]
        public void FormattedResult_Values()
        {
            var annotation = new Annotation() { CachedResult = new List<string>(new[] { "Test 1" }) };
            Assert.AreEqual("Test 1", annotation.FormattedResult);

            annotation = new Annotation() { CachedResult = new List<string>(new[] { "Test 1", "Test 2" }) };
            Assert.AreEqual("Test 1\r\nTest 2", annotation.FormattedResult);
        }
    }
}
