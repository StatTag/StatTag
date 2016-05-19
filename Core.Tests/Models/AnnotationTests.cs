using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using StatTag.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Models
{
    [TestClass]
    public class TagTests
    {
        [TestMethod]
        public void CopyCtor_Normal()
        {
            var tag1 = new Tag()
            {
                Name = "Test",
                Type = Constants.TagType.Value,
                LineStart = 1,
                LineEnd = 2
            };
            var tag2 = new Tag(tag1);
            Assert.IsTrue(tag1.Equals(tag2));
        }

        [TestMethod]
        public void CopyCtor_Null()
        {
            var tag = new Tag(null);
            Assert.IsNull(tag.CodeFile);
            Assert.IsNull(tag.CachedResult);
            Assert.IsNull(tag.FigureFormat);
            Assert.IsNull(tag.LineEnd);
            Assert.IsNull(tag.LineStart);
            Assert.IsNull(tag.Name);
            Assert.IsNull(tag.RunFrequency);
            Assert.IsNull(tag.TableFormat);
            Assert.IsNull(tag.Type);
            Assert.IsNull(tag.ValueFormat);
        }

        [TestMethod]
        public void Equals_Match()
        {
            var file1 = new CodeFile() { FilePath = "File1.txt" };
            var tag1 = new Tag()
            {
                Name= "Test",
                Type = Constants.TagType.Value,
                LineStart = 1,
                LineEnd = 2,
                CodeFile = file1
            };

            var tag2 = new Tag()
            {
                Name = "Test",
                Type = Constants.TagType.Value,
                LineStart = 3,
                LineEnd = 4,
                CodeFile = file1
            };

            Assert.IsTrue(tag1.Equals(tag2));
            Assert.IsTrue(tag2.Equals(tag1));

            // Even if the file object changes, if the file is the same (based on the path) the
            // tags should remain as equal
            var file2 = new CodeFile() { FilePath = "File1.txt" };
            tag2.CodeFile = file2;
            Assert.IsTrue(tag1.Equals(tag2));
            Assert.IsTrue(tag2.Equals(tag1));
        }

        [TestMethod]
        public void Equals_NoMatch()
        {
            var file1 = new CodeFile() { FilePath = "File1.txt" };
            var file2 = new CodeFile() { FilePath = "File2.txt" };

            var tag1 = new Tag()
            {
                Name = "Test",
                Type = Constants.TagType.Value,
                LineStart = 1,
                LineEnd = 2,
                CodeFile = file1
            };

            // Same file as tag 1, but in a different file
            var tag2 = new Tag()
            {
                Name = "Test2",
                Type = Constants.TagType.Value,
                LineStart = 1,
                LineEnd = 2,
                CodeFile = file1
            };

            // Same label as tag1, but in a different file.
            var tag3 = new Tag()
            {
                Name = "Test",
                Type = Constants.TagType.Value,
                LineStart = 3,
                LineEnd = 4,
                CodeFile = file2
            };

            Assert.IsFalse(tag1.Equals(tag2));
            Assert.IsFalse(tag2.Equals(tag1));
            Assert.IsFalse(tag1.Equals(tag3));
            Assert.IsFalse(tag3.Equals(tag1));
            Assert.IsFalse(tag2.Equals(tag3));
            Assert.IsFalse(tag3.Equals(tag2));
        }

        [TestMethod]
        public void EqualsWithPosition()
        {
            var file1 = new CodeFile() { FilePath = "File1.txt" };
            var tag1 = new Tag()
            {
                Name = "Test",
                Type = Constants.TagType.Value,
                LineStart = 1,
                LineEnd = 2,
                CodeFile = file1
            };

            var tag2 = new Tag()
            {
                Name = "Test",
                Type = Constants.TagType.Value,
                LineStart = 3,
                LineEnd = 4,
                CodeFile = file1
            };

            Assert.IsFalse(tag1.EqualsWithPosition(tag2));
            Assert.IsFalse(tag2.EqualsWithPosition(tag1));

            tag2.LineStart = tag1.LineStart;
            tag2.LineEnd = tag1.LineEnd;
            Assert.IsTrue(tag1.EqualsWithPosition(tag2));
            Assert.IsTrue(tag2.EqualsWithPosition(tag1));
        }

        [TestMethod]
        public void FormattedResult_Empty()
        {
            var tag = new Tag();
            Assert.AreEqual(Constants.Placeholders.EmptyField, tag.FormattedResult);

            tag = new Tag() { CachedResult = new List<CommandResult>() };
            Assert.AreEqual(Constants.Placeholders.EmptyField, tag.FormattedResult);
        }

        [TestMethod]
        public void FormattedResult_Values()
        {
            var tag = new Tag() { CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "Test 1" } }) };
            Assert.AreEqual("Test 1", tag.FormattedResult);

            tag = new Tag() { CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "Test 1" }, new CommandResult() { ValueResult = "Test 2" } }) };
            Assert.AreEqual("Test 2", tag.FormattedResult);

            tag = new Tag() { CachedResult = new List<CommandResult>(new[] {  new CommandResult() { ValueResult = "1234" },  new CommandResult() { ValueResult = "456789" } }), Type = Constants.TagType.Value, ValueFormat = new ValueFormat() { FormatType = Constants.ValueFormatType.Numeric, UseThousands = true}};
            Assert.AreEqual("456,789", tag.FormattedResult);
        }

        [TestMethod]
        public void FormattedResult_ValuesBlank()
        {
            var tag = new Tag() { CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "" } }) };
            Assert.AreEqual(Constants.Placeholders.EmptyField, tag.FormattedResult);

            tag = new Tag() { CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "    " }, new CommandResult() { ValueResult = "         " } }) };
            Assert.AreEqual(Constants.Placeholders.EmptyField, tag.FormattedResult);
        }

        [TestMethod]
        public void ToString_Tests()
        {
            var tag = new Tag();
            Assert.AreEqual("StatTag.Core.Models.Tag", tag.ToString());
            tag.Type = Constants.TagType.Figure;
            Assert.AreEqual("Figure", tag.ToString());
            tag.Name = "Test";
            Assert.AreEqual("Test", tag.ToString());
        }

        [TestMethod]
        public void Serialize_Deserialize()
        {
            var tag = new Tag() { Type = Constants.TagType.Value, CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "Test 1" } }) };
            var serialized = tag.Serialize();
            var recreatedTag = Tag.Deserialize(serialized);
            Assert.AreEqual(tag.CodeFile, recreatedTag.CodeFile);
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
        }

        [TestMethod]
        public void NormalizeName_Blanks()
        {
            Assert.AreEqual(string.Empty, Tag.NormalizeName(null));
            Assert.AreEqual(string.Empty, Tag.NormalizeName(string.Empty));
            Assert.AreEqual(string.Empty, Tag.NormalizeName("   "));
        }

        [TestMethod]
        public void NormalizeName_Values()
        {
            Assert.AreEqual("Test", Tag.NormalizeName("Test"));
            Assert.AreEqual("Test", Tag.NormalizeName("|Test"));
            Assert.AreEqual("Test", Tag.NormalizeName("   |   Test"));
            Assert.AreEqual("Test", Tag.NormalizeName("Test|"));
            Assert.AreEqual("Test", Tag.NormalizeName("Test |   "));
            Assert.AreEqual("Test one", Tag.NormalizeName("Test|one"));
        }

        [TestMethod]
        public void IsTableTag()
        {
            Assert.IsTrue(new Tag() { Type = Constants.TagType.Table }.IsTableTag());
            Assert.IsFalse(new Tag() { Type = Constants.TagType.Value }.IsTableTag());
            Assert.IsFalse(new Tag() { Type = string.Empty }.IsTableTag());
        }

        [TestMethod]
        public void HasTableData()
        {
            // Null result cache
            var tag = new Tag()
            {
                Type = Constants.TagType.Table,
                CachedResult = null
            };
            Assert.IsFalse(tag.HasTableData());

            // Non-null, but empty, result cache
            tag.CachedResult = new List<CommandResult>();
            Assert.IsFalse(tag.HasTableData());

            // Non-table data
            tag = new Tag()
            {
                Type = Constants.TagType.Value,
                CachedResult = new List<CommandResult>(new[] {new CommandResult() {ValueResult = "Test 1"}})
            };
            Assert.IsFalse(tag.HasTableData());

            // Actual data
            var format = new TableFormat() { IncludeColumnNames = false, IncludeRowNames = false };
            var table = new Table(new[] { "Row1", "Row2" }, new[] { "Col1", "Col2" }, 2, 2,
                new double?[] { 0.0, 1.0, 2.0, 3.0 });
            tag = new Tag()
            {
                Type = Constants.TagType.Table,
                TableFormat = format,
                CachedResult = new List<CommandResult>(new[] {new CommandResult() {TableResult = table}})
            };
            Assert.IsTrue(tag.HasTableData());
        }

        [TestMethod]
        public void GetTableDisplayDimensions()
        {
            // Non-table tag
            var tag = new Tag()
            {
                Type = Constants.TagType.Value,
                CachedResult = new List<CommandResult>(new[] { new CommandResult() { ValueResult = "Test 1" } })
            };
            Assert.IsNull(tag.GetTableDisplayDimensions());

            // No table format information
            tag = new Tag()
            {
                Type = Constants.TagType.Table,
                TableFormat = null,
                CachedResult = new List<CommandResult>(new[] { new CommandResult() { TableResult = null } })
            };
            Assert.IsNull(tag.GetTableDisplayDimensions());

            // No table data
            var format = new TableFormat() { IncludeColumnNames = false, IncludeRowNames = false };
            tag = new Tag()
            {
                Type = Constants.TagType.Table,
                TableFormat = format,
                CachedResult = new List<CommandResult>(new[] { new CommandResult() { TableResult = null } })
            };
            Assert.IsNull(tag.GetTableDisplayDimensions());

            // Actual data
            var table = new Table(new[] { "Row1", "Row2" }, new[] { "Col1", "Col2", "Col3" }, 2, 3,
                new double?[] { 0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0 });
            tag = new Tag()
            {
                Type = Constants.TagType.Table,
                TableFormat = format,
                CachedResult = new List<CommandResult>(new[] { new CommandResult() { TableResult = table } })
            };
            var dimensions = tag.GetTableDisplayDimensions();
            Assert.AreEqual(2, dimensions[Constants.DimensionIndex.Rows]);
            Assert.AreEqual(3, dimensions[Constants.DimensionIndex.Columns]);

            tag.TableFormat.IncludeColumnNames = true;
            tag.TableFormat.IncludeRowNames = false;
            dimensions = tag.GetTableDisplayDimensions();
            Assert.AreEqual(3, dimensions[Constants.DimensionIndex.Rows]);
            Assert.AreEqual(3, dimensions[Constants.DimensionIndex.Columns]);

            tag.TableFormat.IncludeColumnNames = true;
            tag.TableFormat.IncludeRowNames = true;
            dimensions = tag.GetTableDisplayDimensions();
            Assert.AreEqual(3, dimensions[Constants.DimensionIndex.Rows]);
            Assert.AreEqual(4, dimensions[Constants.DimensionIndex.Columns]);

            tag.TableFormat.IncludeColumnNames = false;
            tag.TableFormat.IncludeRowNames = true;
            dimensions = tag.GetTableDisplayDimensions();
            Assert.AreEqual(2, dimensions[Constants.DimensionIndex.Rows]);
            Assert.AreEqual(4, dimensions[Constants.DimensionIndex.Columns]);
        }

        [TestMethod]
        public void FormatLineNumberRange()
        {
            var tag = new Tag();
            Assert.AreEqual(string.Empty, tag.FormatLineNumberRange());

            tag.LineStart = 1;
            tag.LineEnd = 1;
            Assert.AreEqual("1", tag.FormatLineNumberRange());

            tag.LineEnd = 5;
            Assert.AreEqual("1 - 5", tag.FormatLineNumberRange());
        }
    }
}
