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
        public void CopyCtor_Normal()
        {
            var annotation1 = new Annotation()
            {
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Value,
                LineStart = 1,
                LineEnd = 2
            };
            var annotation2 = new Annotation(annotation1);
            Assert.IsTrue(annotation1.Equals(annotation2));
        }

        [TestMethod]
        public void CopyCtor_Null()
        {
            var annotation = new Annotation(null);
            Assert.IsNull(annotation.CodeFile);
            Assert.IsNull(annotation.CachedResult);
            Assert.IsNull(annotation.FigureFormat);
            Assert.IsNull(annotation.LineEnd);
            Assert.IsNull(annotation.LineStart);
            Assert.IsNull(annotation.OutputLabel);
            Assert.IsNull(annotation.RunFrequency);
            Assert.IsNull(annotation.TableFormat);
            Assert.IsNull(annotation.Type);
            Assert.IsNull(annotation.ValueFormat);
        }

        [TestMethod]
        public void Equals_Match()
        {
            var file1 = new CodeFile() { FilePath = "File1.txt" };
            var annotation1 = new Annotation()
            {
                OutputLabel= "Test",
                Type = Constants.AnnotationType.Value,
                LineStart = 1,
                LineEnd = 2,
                CodeFile = file1
            };

            var annotation2 = new Annotation()
            {
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Value,
                LineStart = 3,
                LineEnd = 4,
                CodeFile = file1
            };

            Assert.IsTrue(annotation1.Equals(annotation2));
            Assert.IsTrue(annotation2.Equals(annotation1));

            // Even if the file object changes, if the file is the same (based on the path) the
            // annotations should remain as equal
            var file2 = new CodeFile() { FilePath = "File1.txt" };
            annotation2.CodeFile = file2;
            Assert.IsTrue(annotation1.Equals(annotation2));
            Assert.IsTrue(annotation2.Equals(annotation1));
        }

        [TestMethod]
        public void Equals_NoMatch()
        {
            var file1 = new CodeFile() { FilePath = "File1.txt" };
            var file2 = new CodeFile() { FilePath = "File2.txt" };

            var annotation1 = new Annotation()
            {
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Value,
                LineStart = 1,
                LineEnd = 2,
                CodeFile = file1
            };

            // Same file as annotation 1, but in a different file
            var annotation2 = new Annotation()
            {
                OutputLabel = "Test2",
                Type = Constants.AnnotationType.Value,
                LineStart = 1,
                LineEnd = 2,
                CodeFile = file1
            };

            // Same label as annotation1, but in a different file.
            var annotation3 = new Annotation()
            {
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Value,
                LineStart = 3,
                LineEnd = 4,
                CodeFile = file2
            };

            Assert.IsFalse(annotation1.Equals(annotation2));
            Assert.IsFalse(annotation2.Equals(annotation1));
            Assert.IsFalse(annotation1.Equals(annotation3));
            Assert.IsFalse(annotation3.Equals(annotation1));
            Assert.IsFalse(annotation2.Equals(annotation3));
            Assert.IsFalse(annotation3.Equals(annotation2));
        }

        [TestMethod]
        public void EqualsWithPosition()
        {
            var file1 = new CodeFile() { FilePath = "File1.txt" };
            var annotation1 = new Annotation()
            {
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Value,
                LineStart = 1,
                LineEnd = 2,
                CodeFile = file1
            };

            var annotation2 = new Annotation()
            {
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Value,
                LineStart = 3,
                LineEnd = 4,
                CodeFile = file1
            };

            Assert.IsFalse(annotation1.EqualsWithPosition(annotation2));
            Assert.IsFalse(annotation2.EqualsWithPosition(annotation1));

            annotation2.LineStart = annotation1.LineStart;
            annotation2.LineEnd = annotation1.LineEnd;
            Assert.IsTrue(annotation1.EqualsWithPosition(annotation2));
            Assert.IsTrue(annotation2.EqualsWithPosition(annotation1));
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

        [TestMethod]
        public void HasTableData()
        {
            // Null result cache
            var annotation = new Annotation()
            {
                Type = Constants.AnnotationType.Table,
                CachedResult = null
            };
            Assert.IsFalse(annotation.HasTableData());

            // Non-null, but empty, result cache
            annotation.CachedResult = new List<CommandResult>();
            Assert.IsFalse(annotation.HasTableData());

            // Non-table data
            annotation = new Annotation()
            {
                Type = Constants.AnnotationType.Value,
                CachedResult = new List<CommandResult>(new[] {new CommandResult() {ValueResult = "Test 1"}})
            };
            Assert.IsFalse(annotation.HasTableData());

            // Actual data
            var format = new TableFormat() { IncludeColumnNames = false, IncludeRowNames = false };
            var table = new Table(new[] { "Row1", "Row2" }, new[] { "Col1", "Col2" }, 2, 2,
                new double?[] { 0.0, 1.0, 2.0, 3.0 });
            annotation = new Annotation()
            {
                Type = Constants.AnnotationType.Table,
                TableFormat = format,
                CachedResult = new List<CommandResult>(new[] {new CommandResult() {TableResult = table}})
            };
            Assert.IsTrue(annotation.HasTableData());
        }

        [TestMethod]
        public void GetTableDisplayDimensions()
        {
            // Non-table annotation
            var annotation = new Annotation()
            {
                Type = Constants.AnnotationType.Value,
                CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "Test 1" } })
            };
            Assert.IsNull(annotation.GetTableDisplayDimensions());

            // No table format information
            annotation = new Annotation()
            {
                Type = Constants.AnnotationType.Table,
                TableFormat = null,
                CachedResult = new List<CommandResult>(new[] { new CommandResult() { TableResult = null } })
            };
            Assert.IsNull(annotation.GetTableDisplayDimensions());

            // No table data
            var format = new TableFormat() { IncludeColumnNames = false, IncludeRowNames = false };
            annotation = new Annotation()
            {
                Type = Constants.AnnotationType.Table,
                TableFormat = format,
                CachedResult = new List<CommandResult>(new[] { new CommandResult() { TableResult = null } })
            };
            Assert.IsNull(annotation.GetTableDisplayDimensions());

            // Actual data
            var table = new Table(new[] { "Row1", "Row2" }, new[] { "Col1", "Col2", "Col3" }, 2, 3,
                new double?[] { 0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0 });
            annotation = new Annotation()
            {
                Type = Constants.AnnotationType.Table,
                TableFormat = format,
                CachedResult = new List<CommandResult>(new[] { new CommandResult() { TableResult = table } })
            };
            var dimensions = annotation.GetTableDisplayDimensions();
            Assert.AreEqual(2, dimensions[Constants.DimensionIndex.Rows]);
            Assert.AreEqual(3, dimensions[Constants.DimensionIndex.Columns]);

            annotation.TableFormat.IncludeColumnNames = true;
            annotation.TableFormat.IncludeRowNames = false;
            dimensions = annotation.GetTableDisplayDimensions();
            Assert.AreEqual(3, dimensions[Constants.DimensionIndex.Rows]);
            Assert.AreEqual(3, dimensions[Constants.DimensionIndex.Columns]);

            annotation.TableFormat.IncludeColumnNames = true;
            annotation.TableFormat.IncludeRowNames = true;
            dimensions = annotation.GetTableDisplayDimensions();
            Assert.AreEqual(3, dimensions[Constants.DimensionIndex.Rows]);
            Assert.AreEqual(4, dimensions[Constants.DimensionIndex.Columns]);

            annotation.TableFormat.IncludeColumnNames = false;
            annotation.TableFormat.IncludeRowNames = true;
            dimensions = annotation.GetTableDisplayDimensions();
            Assert.AreEqual(2, dimensions[Constants.DimensionIndex.Rows]);
            Assert.AreEqual(4, dimensions[Constants.DimensionIndex.Columns]);
        }

        [TestMethod]
        public void FormatLineNumberRange()
        {
            var annotation = new Annotation();
            Assert.AreEqual(string.Empty, annotation.FormatLineNumberRange());

            annotation.LineStart = 1;
            annotation.LineEnd = 1;
            Assert.AreEqual("1", annotation.FormatLineNumberRange());

            annotation.LineEnd = 5;
            Assert.AreEqual("1 - 5", annotation.FormatLineNumberRange());
        }
    }
}
