using System.Collections;
using System.Collections.Generic;
using System.IO;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Core.Tests.Models
{
    [TestClass]
    public class CodeFileTests
    {
        [TestMethod]
        public void Default_ToString()
        {
            Assert.AreEqual(string.Empty, new CodeFile().ToString());

            var file = new CodeFile
            {
                FilePath = "C:\\Test.txt",
                StatisticalPackage = Constants.StatisticalPackages.Stata
            };
            Assert.AreEqual("C:\\Test.txt", file.ToString());

            file.FilePath = "C:\\Test2.txt";
            Assert.AreEqual("C:\\Test2.txt", file.ToString());
        }

        [TestMethod]
        public void LoadTagsFromContent_Empty()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new string[0]);

            var codeFile = new CodeFile(mock.Object);
            codeFile.LoadTagsFromContent();
            Assert.AreEqual(0, codeFile.Tags.Count);
        }

        [TestMethod]
        public void LoadTagsFromContent_InvalidFilePath()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(false);  // Mock this to always say the file path is invalid

            var codeFile = new CodeFile(mock.Object);
            codeFile.LoadTagsFromContent();
            Assert.AreEqual(0, codeFile.Tags.Count);
        }

        [TestMethod]
        public void LoadTagsFromContent_UnknownType()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
                {
                    "**>>>ST:Test(Type=\"Default\")",
                    "some code here",
                    "**<<<"
                });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata, FilePath = "Test.do" };
            codeFile.LoadTagsFromContent();
            Assert.AreEqual(0, codeFile.Tags.Count);
        }

        [TestMethod]
        public void LoadTagsFromContent_Normal()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
                {
                    "**>>>ST:Value(Type=\"Default\")",
                    "some code here",
                    "**<<<"
                });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata, FilePath="Test.do" };
            codeFile.LoadTagsFromContent();
            Assert.AreEqual(1, codeFile.Tags.Count);
            Assert.AreEqual(Constants.TagType.Value, codeFile.Tags[0].Type);
            Assert.AreSame(codeFile, codeFile.Tags[0].CodeFile);
        }

        [TestMethod]
        public void LoadTagsFromContent_RestoreCache()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
                {
                    "**>>>ST:Value(Type=\"Default\")",
                    "some code here",
                    "**<<<"
                });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata, FilePath = "Test.do" };
            codeFile.LoadTagsFromContent();
            codeFile.Tags[0].CachedResult = new List<CommandResult>(new[]
            {
                new CommandResult() { ValueResult = "Test result 1" },
            });

            // Now restore and preserve the cahced value result
            codeFile.LoadTagsFromContent();
            Assert.AreEqual(1, codeFile.Tags.Count);
            Assert.AreEqual(Constants.TagType.Value, codeFile.Tags[0].Type);
            var cachedResult = codeFile.Tags[0].CachedResult[0];
            Assert.AreEqual("Test result 1", cachedResult.ValueResult);

            // Restore again but do not preserve the cahced value result
            codeFile.LoadTagsFromContent(false);
            Assert.AreEqual(1, codeFile.Tags.Count);
            Assert.AreEqual(Constants.TagType.Value, codeFile.Tags[0].Type);
            Assert.IsNull(codeFile.Tags[0].CachedResult);
        }

        [TestMethod]
        public void SaveBackup_AlreadyExists()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);
            var codeFile = new CodeFile(mock.Object);
            codeFile.SaveBackup();
            mock.Verify(file => file.Copy(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void SaveBackup_New()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(false);
            var codeFile = new CodeFile(mock.Object);
            codeFile.SaveBackup();
            mock.Verify(file => file.Copy(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void GuessStatisticalPackage_Empty()
        {
            Assert.AreEqual(string.Empty, CodeFile.GuessStatisticalPackage(""));
            Assert.AreEqual(string.Empty, CodeFile.GuessStatisticalPackage("  "));
            Assert.AreEqual(string.Empty, CodeFile.GuessStatisticalPackage(null));
        }

        [TestMethod]
        public void GuessStatisticalPackage_Valid()
        {
            Assert.AreEqual(Constants.StatisticalPackages.Stata, CodeFile.GuessStatisticalPackage("C:\\test.do"));
            Assert.AreEqual(Constants.StatisticalPackages.Stata, CodeFile.GuessStatisticalPackage("C:\\test.DO"));
            Assert.AreEqual(Constants.StatisticalPackages.Stata, CodeFile.GuessStatisticalPackage("  C:\\test.do  "));
            Assert.AreEqual(Constants.StatisticalPackages.R, CodeFile.GuessStatisticalPackage("C:\\test.r"));
            Assert.AreEqual(Constants.StatisticalPackages.R, CodeFile.GuessStatisticalPackage("C:\\test.R"));
            Assert.AreEqual(Constants.StatisticalPackages.R, CodeFile.GuessStatisticalPackage("  C:\\test.r  "));
            Assert.AreEqual(Constants.StatisticalPackages.SAS, CodeFile.GuessStatisticalPackage("C:\\test.sas"));
            Assert.AreEqual(Constants.StatisticalPackages.SAS, CodeFile.GuessStatisticalPackage("C:\\test.SAS"));
            Assert.AreEqual(Constants.StatisticalPackages.SAS, CodeFile.GuessStatisticalPackage("  C:\\test.sas  "));
        }

        [TestMethod]
        public void GuessStatisticalPackage_Unknown()
        {
            Assert.AreEqual(string.Empty, CodeFile.GuessStatisticalPackage("C:\\"));
            Assert.AreEqual(string.Empty, CodeFile.GuessStatisticalPackage("C:\\test.txt"));
            Assert.AreEqual(string.Empty, CodeFile.GuessStatisticalPackage("C:\\test.dor"));
            Assert.AreEqual(string.Empty, CodeFile.GuessStatisticalPackage("C:\\test.r t"));
            Assert.AreEqual(string.Empty, CodeFile.GuessStatisticalPackage("C:\\test.sa"));
            Assert.AreEqual(string.Empty, CodeFile.GuessStatisticalPackage("C:\\testsas"));
            Assert.AreEqual(string.Empty, CodeFile.GuessStatisticalPackage("C:\\testr"));
            Assert.AreEqual(string.Empty, CodeFile.GuessStatisticalPackage("C:\\testdo"));
        }

        [TestMethod]
        public void Save_NullContent()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(false);  // Mock this to always say the file path is invalid

            var codeFile = new CodeFile(mock.Object) {FilePath = "Test.do"};
            codeFile.LoadTagsFromContent();
            codeFile.Save();
            mock.Verify(file => file.WriteAllLines(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Never);
        }

        [TestMethod]
        public void Save_ValidContents()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
                {
                    "**>>>ST:Test(Type=\"Default\")",
                    "some code here",
                    "**<<<"
                });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);  // Mock this to always say the file path is valid

            var codeFile = new CodeFile(mock.Object) { FilePath = "Test.do" };
            codeFile.LoadTagsFromContent();
            codeFile.Save();
            mock.Verify(file => file.WriteAllLines(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Once);
        }

        [TestMethod]
        public void ContentCache_Load()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
                {
                    "**>>>ST:Test(Type=\"Default\")",
                    "some code here",
                    "**<<<"
                });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata };
            var content = codeFile.Content;
            Assert.AreEqual(3, content.Count);
            mock.Verify(file => file.ReadAllLines(It.IsAny<string>()), Times.Once);

            // When called a second time, we should not increment the number of times the file
            // is reloaded.
            content = codeFile.Content;
            Assert.AreEqual(3, content.Count);
            mock.Verify(file => file.ReadAllLines(It.IsAny<string>()), Times.Once);

            // Forcing the cache to null will cause a reload
            codeFile.Content = null;
            content = codeFile.Content;
            Assert.IsNotNull(content);
            Assert.AreEqual(3, content.Count);
            mock.Verify(file => file.ReadAllLines(It.IsAny<string>()), Times.Exactly(2));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void AddTag_FlippedIndex()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[] { "first line" });

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata };
            var tag = new Tag()
            {
                LineStart = 2,
                LineEnd = 1
            };
            codeFile.AddTag(tag);
        }

        [TestMethod]
        public void AddTag_Null()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[] { "first line" });

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata };
            Assert.IsNull(codeFile.AddTag(null));
            Assert.IsNull(codeFile.AddTag(new Tag()));
            Assert.IsNull(codeFile.AddTag(new Tag() { LineStart = 1 }));
        }

        [TestMethod]
        public void AddTag_New()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
                {
                    "first line",
                    "second line",
                    "third line",
                    "fourth line",
                    "fifth line",
                });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata };
            var tag = new Tag()
            {
                LineStart = 1,
                LineEnd = 2,
                Name = "Test",
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            var updatedTag = codeFile.AddTag(tag);
            Assert.AreEqual(1, codeFile.Tags.Count);
            Assert.AreEqual(1, updatedTag.LineStart);
            Assert.AreEqual(4, updatedTag.LineEnd);
            Assert.AreEqual(7, codeFile.Content.Count);  // Two tag lines should be added
            Assert.AreEqual("**>>>ST:Value(Label=\"Test\", Type=\"Default\")", codeFile.Content[updatedTag.LineStart.Value]);
            Assert.AreEqual("**<<<", codeFile.Content[updatedTag.LineEnd.Value]);
            // Existing tag should not be modified (we don't check start because that happens to always be the same)
            Assert.AreNotEqual(updatedTag.LineEnd, tag.LineEnd);


            // Insert after an existing tag
            tag = new Tag()
            {
                LineStart = 5,
                LineEnd = 6,
                Name = "Test2",
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            updatedTag = codeFile.AddTag(tag);
            Assert.AreEqual(2, codeFile.Tags.Count);
            Assert.AreEqual(5, updatedTag.LineStart);
            Assert.AreEqual(8, updatedTag.LineEnd);
            Assert.AreEqual(9, codeFile.Content.Count);  // Two tag lines should be added
            Assert.AreEqual("**>>>ST:Value(Label=\"Test2\", Type=\"Default\")", codeFile.Content[updatedTag.LineStart.Value]);
            Assert.AreEqual("**<<<", codeFile.Content[updatedTag.LineEnd.Value]);

            // Insert before existing tags
            tag = new Tag()
            {
                LineStart = 0,
                LineEnd = 0,
                Name = "Test3",
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            updatedTag = codeFile.AddTag(tag);
            Assert.AreEqual(3, codeFile.Tags.Count);
            Assert.AreEqual(0, updatedTag.LineStart);
            Assert.AreEqual(2, updatedTag.LineEnd);
            Assert.AreEqual(11, codeFile.Content.Count);  // Two tag lines should be added
            Assert.AreEqual("**>>>ST:Value(Label=\"Test3\", Type=\"Default\")", codeFile.Content[updatedTag.LineStart.Value]);
            Assert.AreEqual("**<<<", codeFile.Content[updatedTag.LineEnd.Value]);

            // Final check that the file content is exactly as we expect:
            Assert.AreEqual("**>>>ST:Value(Label=\"Test3\", Type=\"Default\"), first line, **<<<, **>>>ST:Value(Label=\"Test\", Type=\"Default\"), second line, third line, **<<<, **>>>ST:Value(Label=\"Test2\", Type=\"Default\"), fourth line, fifth line, **<<<", string.Join(", ", codeFile.Content));
        }

        [TestMethod]
        public void AddTag_NoChange()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "**>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                "first line",
                "second line",
                "**<<<",
                "third line",
                "fourth line",
                "fifth line",
            });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object) {StatisticalPackage = Constants.StatisticalPackages.Stata};
            codeFile.LoadTagsFromContent();

            // Overlap the new selection with the existing opening tag tag
            var oldTag = codeFile.Tags[0];
            var newTag = new Tag()
            {
                LineStart = 0,
                LineEnd = 3,
                Name = "Test",
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            var updatedTag = codeFile.AddTag(newTag, oldTag);
            Assert.AreEqual(1, codeFile.Tags.Count);
            Assert.AreEqual(0, updatedTag.LineStart);
            Assert.AreEqual(3, updatedTag.LineEnd);
            Assert.AreEqual(7, codeFile.Content.Count);
            Assert.AreEqual("**>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                codeFile.Content[updatedTag.LineStart.Value]);
            Assert.AreEqual("**<<<", codeFile.Content[updatedTag.LineEnd.Value]);
        }

        [TestMethod]
        public void AddTag_Update()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "first line",
                "**>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                "second line",
                "third line",
                "**<<<",
                "fourth line",
                "fifth line",
            });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object) {StatisticalPackage = Constants.StatisticalPackages.Stata};
            codeFile.LoadTagsFromContent();

            // Overlap the new selection with the existing opening tag tag
            var oldTag = codeFile.Tags[0];
            var newTag = new Tag()
            {
                LineStart = 0,
                LineEnd = 2,
                Name = "Test",
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            var updatedTag = codeFile.AddTag(newTag, oldTag);
            Assert.AreEqual(1, codeFile.Tags.Count);
            Assert.AreEqual(0, updatedTag.LineStart);
            Assert.AreEqual(3, updatedTag.LineEnd);
            Assert.AreEqual(7, codeFile.Content.Count);
            Assert.AreEqual("**>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                codeFile.Content[updatedTag.LineStart.Value]);
            Assert.AreEqual("**<<<", codeFile.Content[updatedTag.LineEnd.Value]);

            // Now select the last two lines and update that tag
            oldTag = codeFile.Tags[0];
            newTag = new Tag()
            {
                LineStart = 5,
                LineEnd = 6,
                Name = "Test",
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            updatedTag = codeFile.AddTag(newTag, oldTag);
            Assert.AreEqual(1, codeFile.Tags.Count);
            Assert.AreEqual(3, updatedTag.LineStart);
            Assert.AreEqual(6, updatedTag.LineEnd);
            Assert.AreEqual(7, codeFile.Content.Count);
            Assert.AreEqual("**>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                codeFile.Content[updatedTag.LineStart.Value]);
            Assert.AreEqual("**<<<", codeFile.Content[updatedTag.LineEnd.Value]);

            // Select an tag that's entirely before the existing tag
            oldTag = codeFile.Tags[0];
            newTag = new Tag()
            {
                LineStart = 1,
                LineEnd = 2,
                Name = "Test",
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            updatedTag = codeFile.AddTag(newTag, oldTag);
            Assert.AreEqual(1, codeFile.Tags.Count);
            Assert.AreEqual(1, updatedTag.LineStart);
            Assert.AreEqual(4, updatedTag.LineEnd);
            Assert.AreEqual(7, codeFile.Content.Count);
            Assert.AreEqual("**>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                codeFile.Content[updatedTag.LineStart.Value]);
            Assert.AreEqual("**<<<", codeFile.Content[updatedTag.LineEnd.Value]);

            // Overlap the selection with the existing closing tag tag
            oldTag = codeFile.Tags[0];
            newTag = new Tag()
            {
                LineStart = 5,
                LineEnd = 6,
                Name = "Test",
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            updatedTag = codeFile.AddTag(newTag, oldTag);
            Assert.AreEqual(1, codeFile.Tags.Count);
            Assert.AreEqual(3, updatedTag.LineStart);
            Assert.AreEqual(6, updatedTag.LineEnd);
            Assert.AreEqual(7, codeFile.Content.Count);
            Assert.AreEqual("**>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                codeFile.Content[updatedTag.LineStart.Value]);
            Assert.AreEqual("**<<<", codeFile.Content[updatedTag.LineEnd.Value]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void AddTag_ExactLineMatch_NotFound()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "first line",
                "**>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                "second line",
                "**<<<",
                "**>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                "third line",
                "**<<<",
                "fourth line"
            });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata };
            codeFile.LoadTagsFromContent();

            // Try to add a new tag that does not have matching line numbers
            var oldTag = new Tag(codeFile.Tags[0])
            {
                LineStart = 4,
                LineEnd = 5
            };
            var newTag = new Tag()
            {
                LineStart = 0,
                LineEnd = 2,
                Name = "Test",
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            codeFile.AddTag(newTag, oldTag, true);
        }

        [TestMethod]
        public void AddTag_ExactLineMatch()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "first line",
                "**>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                "second line",
                "**<<<",
                "**>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                "third line",
                "**<<<",
                "fourth line"
            });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata };
            codeFile.LoadTagsFromContent();
            Assert.AreEqual(2, codeFile.Tags.Count);

            // Match the second one - this should bypass the first one which matches on name but not on line number.
            var oldTag = codeFile.Tags[1];
            var newTag = new Tag()
            {
                LineStart = 4,
                LineEnd = 6,
                Name = "Test 2",
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            var updatedTag = codeFile.AddTag(newTag, oldTag, true);
            Assert.AreEqual(2, codeFile.Tags.Count);
            Assert.AreEqual(8, codeFile.Content.Count);
            // Make sure it didn't modify the first tag - only the second one should be a match.
            Assert.AreEqual("**>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                            codeFile.Content[1]);
            Assert.AreEqual("**>>>ST:Value(Label=\"Test 2\", Type=\"Default\")",
                codeFile.Content[4]);
        }

        [TestMethod]
        public void Save()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.WriteAllLines(It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).Verifiable();
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);
            var codeFile = new CodeFile(mock.Object) { FilePath = "Test.do" };
            codeFile.Save();
            mock.Verify();
        }

        [TestMethod]
        public void RemoveTag_Exists()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "first line",
                "**>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                "second line",
                "**<<<",
                "**>>>ST:Value(Label=\"Test 2\", Type=\"Default\")",
                "third line",
                "**<<<",
                "fourth line",
                "fifth line",
            });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object) {StatisticalPackage = Constants.StatisticalPackages.Stata};
            codeFile.LoadTagsFromContent();

            codeFile.RemoveTag(codeFile.Tags[0]);
            Assert.AreEqual(1, codeFile.Tags.Count);
            Assert.AreEqual(7, codeFile.Content.Count);
            Assert.AreEqual(2, codeFile.Tags[0].LineStart);
            Assert.AreEqual(4, codeFile.Tags[0].LineEnd);

            codeFile.RemoveTag(codeFile.Tags[0]);
            Assert.AreEqual(0, codeFile.Tags.Count);
            Assert.AreEqual(5, codeFile.Content.Count);
        }

        [TestMethod]
        public void RemoveTag_DoesNotExist()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "first line",
                "**>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                "second line",
                "**<<<",
                "**>>>ST:Value(Label=\"Test 2\", Type=\"Default\")",
                "third line",
                "**<<<",
                "fourth line",
                "fifth line",
            });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata };
            codeFile.LoadTagsFromContent();

            codeFile.RemoveTag(null);
            codeFile.RemoveTag(new Tag() { Name = "NotHere", Type = Constants.TagType.Value });
            codeFile.RemoveTag(new Tag() { Name = "test", Type = Constants.TagType.Value });
            Assert.AreEqual(2, codeFile.Tags.Count);
            Assert.AreEqual(9, codeFile.Content.Count);
        }

        [TestMethod]
        public void RemoveCollidingTags_MixedOuterInner()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "attach(mtcars)",
                "##>>>ST:Table(Label=\"Tag 1\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=1, Thousands=False)",
                "mtcars$mpg",
                "##>>>ST:Table(Label=\"Tag 2\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=0, Thousands=False)",
                "mtcars",
                "##<<<",
                "##<<<",
                "##>>>ST:Table(Label=\"Tag 3\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=1, Thousands=False)",
                "mtcars$mpg",
                "##>>>ST:Table(Label=\"Tag 4\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=0, Thousands=False)",
                "mtcars",
                "##<<<",
                "##<<<",
                "detach(mtcars)",
                "##>>>ST:Verbatim(Label=\"Tag 5\", Frequency=\"Always\")",
                "mtcars$mpg",
                "##<<<"
            });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.R };
            codeFile.LoadTagsFromContent();
            Assert.AreEqual(3, codeFile.Tags.Count);  // Only detects 3 out of the 5

            var removeTag = new Tag() {Name = "Tag 1", CodeFile = codeFile, LineStart = 1, LineEnd = 6};
            codeFile.RemoveCollidingTag(removeTag);
            // Note that we offset lines by 2 (from 9, 11) since we are removing an earlier tag first
            removeTag = new Tag() {Name = "Tag 4", CodeFile = codeFile, LineStart = 7, LineEnd = 9};
            codeFile.RemoveCollidingTag(removeTag);

            var actual = string.Join("\r\n", codeFile.Content);
            var expected = string.Join("\r\n", new[]
            {
                "attach(mtcars)",
                "mtcars$mpg",
                "##>>>ST:Table(Label=\"Tag 2\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=0, Thousands=False)",
                "mtcars",
                "##<<<",
                "##>>>ST:Table(Label=\"Tag 3\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=1, Thousands=False)",
                "mtcars$mpg",
                "mtcars",
                "##<<<",
                "detach(mtcars)",
                "##>>>ST:Verbatim(Label=\"Tag 5\", Frequency=\"Always\")",
                "mtcars$mpg",
                "##<<<"
            });
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(13, codeFile.Content.Count);
        }

        [TestMethod]
        public void RemoveCollidingTags_BothOuter()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "attach(mtcars)",
                "##>>>ST:Table(Label=\"Tag 1\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=1, Thousands=False)",
                "mtcars$mpg",
                "##>>>ST:Table(Label=\"Tag 2\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=0, Thousands=False)",
                "mtcars",
                "##<<<",
                "##<<<",
                "##>>>ST:Table(Label=\"Tag 3\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=1, Thousands=False)",
                "mtcars$mpg",
                "##>>>ST:Table(Label=\"Tag 4\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=0, Thousands=False)",
                "mtcars",
                "##<<<",
                "##<<<",
                "detach(mtcars)",
                "##>>>ST:Verbatim(Label=\"Tag 5\", Frequency=\"Always\")",
                "mtcars$mpg",
                "##<<<"
            });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.R };
            codeFile.LoadTagsFromContent();
            Assert.AreEqual(3, codeFile.Tags.Count);

            var removeTag = new Tag() { Name = "Tag 3", CodeFile = codeFile, LineStart = 7, LineEnd = 12 };
            codeFile.RemoveCollidingTag(removeTag); 
            removeTag = new Tag() { Name = "Tag 1", CodeFile = codeFile, LineStart = 1, LineEnd = 6 };
            codeFile.RemoveCollidingTag(removeTag);


            var actual = string.Join("\r\n", codeFile.Content);
            var expected = string.Join("\r\n", new[]
            {
                "attach(mtcars)",
                "mtcars$mpg",
                "##>>>ST:Table(Label=\"Tag 2\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=0, Thousands=False)",
                "mtcars",
                "##<<<",
                "mtcars$mpg",
                "##>>>ST:Table(Label=\"Tag 4\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=0, Thousands=False)",
                "mtcars",
                "##<<<",
                "detach(mtcars)",
                "##>>>ST:Verbatim(Label=\"Tag 5\", Frequency=\"Always\")",
                "mtcars$mpg",
                "##<<<"
            });
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(13, codeFile.Content.Count);
        }
        [TestMethod]
        public void RemoveCollidingTags_NestedSameNames()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "attach(mtcars)",
                "",
                "mtcars$mpg",
                "mtcars",
                "##>>>ST:Table(Label=\"Inner Tag (Group 1)\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=0, Thousands=False)",
                "mtcars",
                "##<<<",
                "",
                "##>>>ST:Table(Label=\"Inner Tag (Group 2)\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=0, Thousands=False)",
                "mtcars",
                "##>>>ST:Table(Label=\"Inner Tag (Group 2)\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=0, Thousands=False)",
                "mtcars",
                "##>>>ST:Table(Label=\"Inner Tag (Group 2)\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=0, Thousands=False)",
                "mtcars",
                "##<<<",
                "##<<<",
                "##<<<",
                "",
                "##>>>ST:Verbatim(Label=\"Test\", Frequency=\"Always\")",
                "mtcars$mpg",
                "##<<<"
            });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.R };
            codeFile.LoadTagsFromContent();
            Assert.AreEqual(3, codeFile.Tags.Count);

            var removeTag = new Tag() { Name = "Inner Tag (Group 2)", CodeFile = codeFile, LineStart = 10, LineEnd = 15 };
            codeFile.RemoveCollidingTag(removeTag);
            removeTag = new Tag() { Name = "Inner Tag (Group 2)", CodeFile = codeFile, LineStart = 8, LineEnd = 14 };

            codeFile.RemoveCollidingTag(removeTag);

            var actual = string.Join("\r\n", codeFile.Content);
            var expected = string.Join("\r\n", new[]
            {
                "attach(mtcars)",
                "",
                "mtcars$mpg",
                "mtcars",
                "##>>>ST:Table(Label=\"Inner Tag (Group 1)\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=0, Thousands=False)",
                "mtcars",
                "##<<<",
                "",
                "mtcars",
                "mtcars",
                "##>>>ST:Table(Label=\"Inner Tag (Group 2)\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=0, Thousands=False)",
                "mtcars",
                "##<<<",
                "",
                "##>>>ST:Verbatim(Label=\"Test\", Frequency=\"Always\")",
                "mtcars$mpg",
                "##<<<"
            });
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(17, codeFile.Content.Count);
        }

        [TestMethod]
        public void RemoveCollidingTags_BothInner()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "attach(mtcars)",
                "##>>>ST:Table(Label=\"Tag 1\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=1, Thousands=False)",
                "mtcars$mpg",
                "##>>>ST:Table(Label=\"Tag 2\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=0, Thousands=False)",
                "mtcars",
                "##<<<",
                "##<<<",
                "##>>>ST:Table(Label=\"Tag 3\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=1, Thousands=False)",
                "mtcars$mpg",
                "##>>>ST:Table(Label=\"Tag 4\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=0, Thousands=False)",
                "mtcars",
                "##<<<",
                "##<<<",
                "detach(mtcars)",
                "##>>>ST:Verbatim(Label=\"Tag 5\", Frequency=\"Always\")",
                "mtcars$mpg",
                "##<<<"
            });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.R };
            codeFile.LoadTagsFromContent();
            Assert.AreEqual(3, codeFile.Tags.Count);

            var removeTag = new Tag() { Name = "Tag 4", CodeFile = codeFile, LineStart = 9, LineEnd = 11 };
            codeFile.RemoveCollidingTag(removeTag);
            removeTag = new Tag() { Name = "Tag 2", CodeFile = codeFile, LineStart = 3, LineEnd = 5 };
            codeFile.RemoveCollidingTag(removeTag);


            var actual = string.Join("\r\n", codeFile.Content);
            var expected = string.Join("\r\n", new[]
            {
                "attach(mtcars)",
                "##>>>ST:Table(Label=\"Tag 1\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=1, Thousands=False)",
                "mtcars$mpg",
                "mtcars",
                "##<<<",
                "##>>>ST:Table(Label=\"Tag 3\", Frequency=\"Always\", Type=\"Numeric\", AllowInvalid=True, Decimals=1, Thousands=False)",
                "mtcars$mpg",
                "mtcars",
                "##<<<",
                "detach(mtcars)",
                "##>>>ST:Verbatim(Label=\"Tag 5\", Frequency=\"Always\")",
                "mtcars$mpg",
                "##<<<"
            });
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(13, codeFile.Content.Count);
        }

        [TestMethod]
        public void UpdateContent()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.WriteAllText(It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "**>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                "first line",
                "second line",
                "**<<<"
            });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object);
            codeFile.StatisticalPackage = Constants.StatisticalPackages.Stata;
            codeFile.UpdateContent("test content");
            mock.Verify();
            Assert.AreEqual(1, codeFile.Tags.Count);
        }

        [TestMethod]
        public void FindDuplicateTags_EmptyTags()
        {
            // This is when we have code files with null or otherwise empty collections of tags, to ensure we are
            // handling this boundary scenarios appropriately.
            var codeFile = new CodeFile() { FilePath = "Test.do", Tags = null };
            var result = codeFile.FindDuplicateTags();
            Assert.AreEqual(0, result.Count);

            codeFile = new CodeFile() { FilePath = "Test.do" };
            result = codeFile.FindDuplicateTags();
            Assert.AreEqual(0, result.Count);

            codeFile = new CodeFile() { FilePath = "Test.do", Tags = new List<Tag>() };
            result = codeFile.FindDuplicateTags();
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void FindDuplicateTags_NoDuplicates()
        {
            var codeFile = new CodeFile() { FilePath = "Test.do", Tags = new List<Tag>(new []
            {
                new Tag() { Name = "Test"}, 
                new Tag() { Name = "Test2"},
            }) };
            var result = codeFile.FindDuplicateTags();
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void FindDuplicateTags_Duplicates()
        {
            var codeFile = new CodeFile()
            {
                FilePath = "Test.do",
                Tags = new List<Tag>(new[]
            {
                new Tag() { Name = "Test"}, 
                new Tag() { Name = "Test2"},
                new Tag() { Name = "test"},
                new Tag() { Name = "test2"},
                new Tag() { Name = "Test"},
            })
            };
            var result = codeFile.FindDuplicateTags();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result[codeFile.Tags[0]].Count);
            Assert.AreEqual(1, result[codeFile.Tags[1]].Count);
        }

        [TestMethod]
        public void IsValid_Empty()
        {
            var codeFile = new CodeFile();
            Assert.IsFalse(codeFile.IsValid());
        }

        [TestMethod]
        public void IsValid_NonExistent()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(false);
            var codeFile = new CodeFile(mock.Object)
            {
                FilePath = "TestFile.r"
            };
            Assert.IsFalse(codeFile.IsValid());
        }

        [TestMethod]
        public void IsValid_ValidPath()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);
            var codeFile = new CodeFile(mock.Object)
            {
                FilePath = "TestFile.r"
            };
            Assert.IsTrue(codeFile.IsValid());
        }

        [TestMethod]
        public void CopyCtor()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "**>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                "first line",
                "second line",
                "**<<<"
            });
            mock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(mock.Object) {StatisticalPackage = Constants.StatisticalPackages.Stata};
            var copyCodeFile = new CodeFile(codeFile);
            Assert.AreEqual(codeFile.Tags.Count, copyCodeFile.Tags.Count);
            Assert.AreEqual(codeFile.StatisticalPackage, copyCodeFile.StatisticalPackage);
            Assert.AreEqual(codeFile.FilePath, copyCodeFile.FilePath);
        }
    }
}
