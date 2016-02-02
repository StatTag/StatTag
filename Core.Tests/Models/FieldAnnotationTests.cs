﻿using System;
using System.Collections.Generic;
using AnalysisManager.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Models
{
    [TestClass]
    public class FieldAnnotationTests
    {
        [TestMethod]
        public void Constructor_Empty()
        {
            var fieldAnnotation = new FieldAnnotation();
            Assert.IsNull(fieldAnnotation.TableCellIndex);
        }

        [TestMethod]
        public void Constructor_Annotation()
        {
            var annotation = new Annotation()
            {
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Value
            };

            var fieldAnnotation = new FieldAnnotation(annotation);
            Assert.AreEqual(annotation.OutputLabel, fieldAnnotation.OutputLabel);
            Assert.AreEqual(annotation.Type, fieldAnnotation.Type);
            Assert.IsNull(fieldAnnotation.TableCellIndex);
        }

        [TestMethod]
        public void Constructor_AnnotationWithIndex()
        {
            var annotation = new Annotation()
            {
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Table
            };

            var fieldAnnotation = new FieldAnnotation(annotation, 10);
            Assert.AreEqual(annotation.OutputLabel, fieldAnnotation.OutputLabel);
            Assert.AreEqual(annotation.Type, fieldAnnotation.Type);
            Assert.AreEqual(10, fieldAnnotation.TableCellIndex);

            fieldAnnotation = new FieldAnnotation(annotation, null);
            Assert.IsNull(fieldAnnotation.TableCellIndex);
        }

        [TestMethod]
        public void Constructor_Copy()
        {
            var annotation = new FieldAnnotation()
            {
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Table,
                TableCellIndex = 15
            };

            var fieldAnnotation = new FieldAnnotation(annotation);
            Assert.AreEqual(annotation.OutputLabel, fieldAnnotation.OutputLabel);
            Assert.AreEqual(annotation.Type, fieldAnnotation.Type);
            Assert.AreEqual(annotation.TableCellIndex, fieldAnnotation.TableCellIndex);
        }

        [TestMethod]
        public void Serialize_Deserialize()
        {
            var annotation = new FieldAnnotation() { Type = Constants.AnnotationType.Table, CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "Test 1" } }), TableCellIndex = 10};
            var serialized = annotation.Serialize();
            var recreatedAnnotation = FieldAnnotation.Deserialize(serialized);
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
            Assert.AreEqual(annotation.TableCellIndex, recreatedAnnotation.TableCellIndex);
        }
    }
}
