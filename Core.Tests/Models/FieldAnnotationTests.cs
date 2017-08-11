using System;
using System.Collections.Generic;
using StatTag.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Models
{
    [TestClass]
    public class FieldTagTests
    {
        [TestMethod]
        public void Constructor_Empty()
        {
            var fieldTag = new FieldTag();
            Assert.IsNull(fieldTag.TableCellIndex);
        }

        [TestMethod]
        public void Constructor_Tag()
        {
            var tag = new Tag()
            {
                Name = "Test",
                Type = Constants.TagType.Value
            };

            var fieldTag = new FieldTag(tag);
            Assert.AreEqual(tag.Name, fieldTag.Name);
            Assert.AreEqual(tag.Type, fieldTag.Type);
            Assert.IsNull(fieldTag.TableCellIndex);
        }

        [TestMethod]
        public void Constructor_TagWithIndex()
        {
            var tag = new Tag()
            {
                Name = "Test",
                Type = Constants.TagType.Table
            };

            var fieldTag = new FieldTag(tag, 10);
            Assert.AreEqual(tag.Name, fieldTag.Name);
            Assert.AreEqual(tag.Type, fieldTag.Type);
            Assert.AreEqual(10, fieldTag.TableCellIndex);

            fieldTag = new FieldTag(tag);
            Assert.IsNull(fieldTag.TableCellIndex);
        }

        [TestMethod]
        public void Constructor_TagWithFieldTag()
        {
            var codeFile = new CodeFile() { FilePath = "Test.do" };
            var tag = new Tag()
            {
                Name = "Test",
                Type = Constants.TagType.Table,
                CodeFile = codeFile
            };

            var fieldTag = new FieldTag() { TableCellIndex = 10 };
            var newFieldTag = new FieldTag(tag, fieldTag);
            Assert.AreEqual(tag.Name, newFieldTag.Name);
            Assert.AreEqual(tag.Type, newFieldTag.Type);
            Assert.AreEqual(10, newFieldTag.TableCellIndex);
            Assert.AreEqual("Test.do", newFieldTag.CodeFilePath);
        }

        [TestMethod]
        public void Constructor_NullTagWithFieldTag()
        {
            var fieldTag = new FieldTag()
            {
                Name = "Test",
                Type = Constants.TagType.Table,
                CodeFilePath = "Test.do",
                TableCellIndex = 10
            };

            var newFieldTag = new FieldTag(null, fieldTag);
            Assert.AreEqual(fieldTag.Name, newFieldTag.Name);
            Assert.AreEqual(fieldTag.Type, newFieldTag.Type);
            Assert.AreEqual(fieldTag.TableCellIndex, newFieldTag.TableCellIndex);
            Assert.AreEqual(fieldTag.CodeFilePath, newFieldTag.CodeFilePath);
        }

        [TestMethod]
        public void Constructor_TagWithIndex_TableCell()
        {
            var tag = new Tag()
            {
                Name = "Test",
                Type = Constants.TagType.Table,
                TableFormat = new TableFormat()
                {
                    ColumnFilter = new FilterFormat(Constants.FilterPrefix.Column) { Enabled = false },
                    RowFilter = new FilterFormat(Constants.FilterPrefix.Row) { Enabled = false }
                },
                CachedResult = new List<CommandResult>()
                {
                    new CommandResult()
                    {
                        TableResult = new Table()
                        {
                            ColumnSize = 3,
                            RowSize = 3,
                            Data = new string[,]{ {"", "c1", "c2"}, {"r1", "1.0", "2.0"}, {"r2", "3.0", "4.0"} },
                            FormattedCells = new [,]{ {"", "c1", "c2"}, {"r1", "1.0", "2.0"}, {"r2", "3.0", "4.0"} }
                        }
                    }
                }
            };

            var fieldTag = new FieldTag(tag, 0);
            Assert.AreEqual(0, fieldTag.TableCellIndex);
            Assert.AreEqual(string.Empty, fieldTag.FormattedResult);

            fieldTag = new FieldTag(tag, 3);
            Assert.AreEqual(3, fieldTag.TableCellIndex);
            Assert.AreEqual("r1", fieldTag.FormattedResult);

            fieldTag = new FieldTag(tag, 7);
            Assert.AreEqual(7, fieldTag.TableCellIndex);
            Assert.AreEqual("3.0", fieldTag.FormattedResult);
        }

        [TestMethod]
        public void Constructor_Copy()
        {
            var tag = new FieldTag()
            {
                Name = "Test",
                Type = Constants.TagType.Table,
                TableCellIndex = 15
            };

            var fieldTag = new FieldTag(tag);
            Assert.AreEqual(tag.Name, fieldTag.Name);
            Assert.AreEqual(tag.Type, fieldTag.Type);
            Assert.AreEqual(tag.TableCellIndex, fieldTag.TableCellIndex);
        }

        [TestMethod]
        public void Serialize_NormalizesObjectName()
        {
            // The Serailize function will modify the Name property to a normalized value.
            // We expect this and explicitly confirm it's intended behavior.
            var tag = new FieldTag();
            Assert.IsNull(tag.Name);
            var serialized = tag.Serialize();
            Assert.IsNotNull(tag.Name);
        }

        //TODO: Test serialize/deserialize with a TableResult in the CachedResult collection

        [TestMethod]
        public void Serialize_Deserialize()
        {
            var codeFile = new CodeFile() {FilePath = "Test.do"};
            var tag = new FieldTag() { Type = Constants.TagType.Table, CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "Test 1" } }), TableCellIndex = 10, CodeFile = codeFile};
            var serialized = tag.Serialize();
            var recreatedTag = FieldTag.Deserialize(serialized);
            Assert.AreEqual(tag.FigureFormat, recreatedTag.FigureFormat);
            Assert.AreEqual(tag.FormattedResult, recreatedTag.FormattedResult);
            Assert.AreEqual(tag.LineEnd, recreatedTag.LineEnd);
            Assert.AreEqual(tag.LineStart, recreatedTag.LineStart);
            Assert.AreEqual(tag.Name, recreatedTag.Name);
            Assert.AreEqual(tag.RunFrequency, recreatedTag.RunFrequency);
            Assert.AreEqual(tag.Type, recreatedTag.Type);
            Assert.AreEqual(tag.ValueFormat, recreatedTag.ValueFormat);
            Assert.AreEqual(tag.FigureFormat, recreatedTag.FigureFormat);
            Assert.AreEqual(tag.TableFormat, recreatedTag.TableFormat);
            Assert.AreEqual(tag.TableCellIndex, recreatedTag.TableCellIndex);
            // The recreated tag doesn't truly recreate the code file object.  We attempt to restore it the best we can with the file path.
            Assert.AreEqual(tag.CodeFilePath, recreatedTag.CodeFile.FilePath);
            Assert.AreEqual(tag.CodeFilePath, recreatedTag.CodeFilePath);
        }

        [TestMethod]
        public void LinkToCodeFile_Found()
        {
            var tag = new FieldTag() { Type = Constants.TagType.Table, CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "Test 1" } }), TableCellIndex = 10, CodeFile = new CodeFile() { FilePath = "Test.do" } };

            var files = new[]
            {
                new CodeFile() {FilePath = "Test.do"},
                new CodeFile() {FilePath = "Test2.do"},
            };
            FieldTag.LinkToCodeFile(tag, files);
            Assert.AreEqual(files[0], tag.CodeFile);

            // Should match with case differences
            files[0].FilePath = files[0].FilePath.ToUpper();
            FieldTag.LinkToCodeFile(tag, files);
            Assert.AreEqual(files[0], tag.CodeFile);
        }

        [TestMethod]
        public void LinkToCodeFile_NotFound()
        {
            // Shouldn't crash, but shouldn't do anything that we can test.
            FieldTag.LinkToCodeFile(null, null);

            var tag = new FieldTag() { Type = Constants.TagType.Table, CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "Test 1" } }), TableCellIndex = 10, CodeFile = null };
            var files = new[]
            {
                new CodeFile() {FilePath = "Test.do"},
                new CodeFile() {FilePath = "Test2.do"},
            };

            // Check against a null list.
            FieldTag.LinkToCodeFile(tag, null);
            Assert.IsNull(tag.CodeFile);

            // Check against the real list.
            FieldTag.LinkToCodeFile(tag, files);
            Assert.IsNull(tag.CodeFile);

            // Should match with case differences
            tag.CodeFile = new CodeFile() { FilePath = "Test3.do" };
            FieldTag.LinkToCodeFile(tag, files);
            Assert.AreNotEqual(files[0], tag.CodeFile);
            Assert.AreNotEqual(files[1], tag.CodeFile);
        }
    }
}
