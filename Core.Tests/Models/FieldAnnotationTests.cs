using System;
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

            fieldAnnotation = new FieldAnnotation(annotation);
            Assert.IsNull(fieldAnnotation.TableCellIndex);
        }

        [TestMethod]
        public void Constructor_AnnotationWithFieldAnnotation()
        {
            var annotation = new Annotation()
            {
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Table
            };

            var fieldAnnotation = new FieldAnnotation() { CodeFilePath = "Test.do", TableCellIndex = 10};
            var newFieldAnnotation = new FieldAnnotation(annotation, fieldAnnotation);
            Assert.AreEqual(annotation.OutputLabel, newFieldAnnotation.OutputLabel);
            Assert.AreEqual(annotation.Type, newFieldAnnotation.Type);
            Assert.AreEqual(10, newFieldAnnotation.TableCellIndex);
            Assert.AreEqual("Test.do", newFieldAnnotation.CodeFilePath);
        }

        [TestMethod]
        public void Constructor_AnnotationWithIndex_TableCell()
        {
            var annotation = new Annotation()
            {
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Table,
                CachedResult = new List<CommandResult>()
                {
                    new CommandResult()
                    {
                        TableResult = new Table()
                        {
                            ColumnNames = new List<string>() { "c1", "c2" },
                            RowNames = new List<string>() { "r1", "r2" },
                            ColumnSize = 2,
                            RowSize = 2,
                            Data = new double?[]{ 1.0, 2.0, 3.0, 4.0 },
                            FormattedCells = new []{ "1.0", "2.0", "3.0", "4.0" }
                        }
                    }
                }
            };

            var fieldAnnotation = new FieldAnnotation(annotation, 0);
            Assert.AreEqual(0, fieldAnnotation.TableCellIndex);
            Assert.AreEqual("1.0", fieldAnnotation.FormattedResult);

            fieldAnnotation = new FieldAnnotation(annotation, 2);
            Assert.AreEqual(2, fieldAnnotation.TableCellIndex);
            Assert.AreEqual("3.0", fieldAnnotation.FormattedResult);
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
            var codeFile = new CodeFile() {FilePath = "Test.do"};
            var annotation = new FieldAnnotation() { Type = Constants.AnnotationType.Table, CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "Test 1" } }), TableCellIndex = 10, CodeFile = codeFile};
            var serialized = annotation.Serialize();
            var recreatedAnnotation = FieldAnnotation.Deserialize(serialized);
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
            // The recreated annotation doesn't truly recreate the code file object.  We attempt to restore it the best we can with the file path.
            Assert.AreEqual(annotation.CodeFilePath, recreatedAnnotation.CodeFile.FilePath);
            Assert.AreEqual(annotation.CodeFilePath, recreatedAnnotation.CodeFilePath);
        }

        [TestMethod]
        public void LinkToCodeFile_Found()
        {
            var annotation = new FieldAnnotation() { Type = Constants.AnnotationType.Table, CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "Test 1" } }), TableCellIndex = 10, CodeFile = new CodeFile() { FilePath = "Test.do" } };

            var files = new[]
            {
                new CodeFile() {FilePath = "Test.do"},
                new CodeFile() {FilePath = "Test2.do"},
            };
            FieldAnnotation.LinkToCodeFile(annotation, files);
            Assert.AreEqual(files[0], annotation.CodeFile);

            // Should match with case differences
            files[0].FilePath = files[0].FilePath.ToUpper();
            FieldAnnotation.LinkToCodeFile(annotation, files);
            Assert.AreEqual(files[0], annotation.CodeFile);
        }

        [TestMethod]
        public void LinkToCodeFile_NotFound()
        {
            // Shouldn't crash, but shouldn't do anything that we can test.
            FieldAnnotation.LinkToCodeFile(null, null);

            var annotation = new FieldAnnotation() { Type = Constants.AnnotationType.Table, CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "Test 1" } }), TableCellIndex = 10, CodeFile = null };
            var files = new[]
            {
                new CodeFile() {FilePath = "Test.do"},
                new CodeFile() {FilePath = "Test2.do"},
            };

            // Check against a null list.
            FieldAnnotation.LinkToCodeFile(annotation, null);
            Assert.IsNull(annotation.CodeFile);

            // Check against the real list.
            FieldAnnotation.LinkToCodeFile(annotation, files);
            Assert.IsNull(annotation.CodeFile);

            // Should match with case differences
            annotation.CodeFile = new CodeFile() { FilePath = "Test3.do" };
            FieldAnnotation.LinkToCodeFile(annotation, files);
            Assert.AreNotEqual(files[0], annotation.CodeFile);
            Assert.AreNotEqual(files[1], annotation.CodeFile);
        }
    }
}
