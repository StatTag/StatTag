using System.Collections;
using System.Collections.Generic;
using System.IO;
using AnalysisManager.Core.Interfaces;
using AnalysisManager.Core.Models;
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
        public void LoadAnnotationsFromContent_Empty()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new string[0]);

            var codeFile = new CodeFile(mock.Object);
            codeFile.LoadAnnotationsFromContent();
            Assert.AreEqual(0, codeFile.Annotations.Count);
        }

        [TestMethod]
        public void LoadAnnotationsFromContent_UnknownType()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
                {
                    "**>>>AM:Test(Type=\"Default\")",
                    "some code here",
                    "**<<<"
                });

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata, };
            codeFile.LoadAnnotationsFromContent();
            Assert.AreEqual(0, codeFile.Annotations.Count);
        }

        [TestMethod]
        public void LoadAnnotationsFromContent_Normal()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
                {
                    "**>>>AM:Value(Type=\"Default\")",
                    "some code here",
                    "**<<<"
                });

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata };
            codeFile.LoadAnnotationsFromContent();
            Assert.AreEqual(1, codeFile.Annotations.Count);
            Assert.AreEqual(Constants.AnnotationType.Value, codeFile.Annotations[0].Type);
            Assert.AreSame(codeFile, codeFile.Annotations[0].CodeFile);
        }

        [TestMethod]
        public void LoadAnnotationsFromContent_RestoreCache()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
                {
                    "**>>>AM:Value(Type=\"Default\")",
                    "some code here",
                    "**<<<"
                });

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata };
            codeFile.LoadAnnotationsFromContent();
            codeFile.Annotations[0].CachedResult = new List<CommandResult>(new[]
            {
                new CommandResult() { ValueResult = "Test result 1" },
            });

            // Now restore and preserve the cahced value result
            codeFile.LoadAnnotationsFromContent();
            Assert.AreEqual(1, codeFile.Annotations.Count);
            Assert.AreEqual(Constants.AnnotationType.Value, codeFile.Annotations[0].Type);
            var cachedResult = codeFile.Annotations[0].CachedResult[0];
            Assert.AreEqual("Test result 1", cachedResult.ValueResult);

            // Restore again but do not preserve the cahced value result
            codeFile.LoadAnnotationsFromContent(false);
            Assert.AreEqual(1, codeFile.Annotations.Count);
            Assert.AreEqual(Constants.AnnotationType.Value, codeFile.Annotations[0].Type);
            Assert.IsNull(codeFile.Annotations[0].CachedResult);
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
            Assert.AreEqual(Constants.StatisticalPackages.Stata, CodeFile.GuessStatisticalPackage("  C:\\test.do  "));
            Assert.AreEqual(Constants.StatisticalPackages.R, CodeFile.GuessStatisticalPackage("C:\\test.r"));
            Assert.AreEqual(Constants.StatisticalPackages.R, CodeFile.GuessStatisticalPackage("  C:\\test.r  "));
            Assert.AreEqual(Constants.StatisticalPackages.SAS, CodeFile.GuessStatisticalPackage("C:\\test.sas"));
            Assert.AreEqual(Constants.StatisticalPackages.SAS, CodeFile.GuessStatisticalPackage("  C:\\test.sas  "));
        }

        [TestMethod]
        public void GuessStatisticalPackage_Unknown()
        {
            Assert.AreEqual(string.Empty, CodeFile.GuessStatisticalPackage("C:\\"));
            Assert.AreEqual(string.Empty, CodeFile.GuessStatisticalPackage("C:\\test.txt"));
            Assert.AreEqual(string.Empty, CodeFile.GuessStatisticalPackage("C:\\test.dor"));
            Assert.AreEqual(string.Empty, CodeFile.GuessStatisticalPackage("C:\\test.r t"));
        }

        [TestMethod]
        public void ContentCache_Load()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
                {
                    "**>>>AM:Test(Type=\"Default\")",
                    "some code here",
                    "**<<<"
                });

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
        public void AddAnnotation_FlippedIndex()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[] { "first line" });

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata };
            var annotation = new Annotation()
            {
                LineStart = 2,
                LineEnd = 1
            };
            codeFile.AddAnnotation(annotation);
        }

        [TestMethod]
        public void AddAnnotation_Null()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[] { "first line" });

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata };
            Assert.IsNull(codeFile.AddAnnotation(null));
            Assert.IsNull(codeFile.AddAnnotation(new Annotation()));
            Assert.IsNull(codeFile.AddAnnotation(new Annotation() { LineStart = 1 }));
        }

        [TestMethod]
        public void AddAnnotation_New()
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

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata };
            var annotation = new Annotation()
            {
                LineStart = 1,
                LineEnd = 2,
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Value,
                ValueFormat = new ValueFormat()
            };
            var updatedAnnotation = codeFile.AddAnnotation(annotation);
            Assert.AreEqual(1, codeFile.Annotations.Count);
            Assert.AreEqual(1, updatedAnnotation.LineStart);
            Assert.AreEqual(4, updatedAnnotation.LineEnd);
            Assert.AreEqual(7, codeFile.Content.Count);  // Two annotation lines should be added
            Assert.AreEqual("**>>>AM:Value(Label=\"Test\", Type=\"Default\")", codeFile.Content[updatedAnnotation.LineStart.Value]);
            Assert.AreEqual("**<<<", codeFile.Content[updatedAnnotation.LineEnd.Value]);
            // Existing annotation should not be modified (we don't check start because that happens to always be the same)
            Assert.AreNotEqual(updatedAnnotation.LineEnd, annotation.LineEnd);


            // Insert after an existing annotation
            annotation = new Annotation()
            {
                LineStart = 5,
                LineEnd = 6,
                OutputLabel = "Test2",
                Type = Constants.AnnotationType.Value,
                ValueFormat = new ValueFormat()
            };
            updatedAnnotation = codeFile.AddAnnotation(annotation);
            Assert.AreEqual(2, codeFile.Annotations.Count);
            Assert.AreEqual(5, updatedAnnotation.LineStart);
            Assert.AreEqual(8, updatedAnnotation.LineEnd);
            Assert.AreEqual(9, codeFile.Content.Count);  // Two annotation lines should be added
            Assert.AreEqual("**>>>AM:Value(Label=\"Test2\", Type=\"Default\")", codeFile.Content[updatedAnnotation.LineStart.Value]);
            Assert.AreEqual("**<<<", codeFile.Content[updatedAnnotation.LineEnd.Value]);

            // Insert before existing annotations
            annotation = new Annotation()
            {
                LineStart = 0,
                LineEnd = 0,
                OutputLabel = "Test3",
                Type = Constants.AnnotationType.Value,
                ValueFormat = new ValueFormat()
            };
            updatedAnnotation = codeFile.AddAnnotation(annotation);
            Assert.AreEqual(3, codeFile.Annotations.Count);
            Assert.AreEqual(0, updatedAnnotation.LineStart);
            Assert.AreEqual(2, updatedAnnotation.LineEnd);
            Assert.AreEqual(11, codeFile.Content.Count);  // Two annotation lines should be added
            Assert.AreEqual("**>>>AM:Value(Label=\"Test3\", Type=\"Default\")", codeFile.Content[updatedAnnotation.LineStart.Value]);
            Assert.AreEqual("**<<<", codeFile.Content[updatedAnnotation.LineEnd.Value]);

            // Final check that the file content is exactly as we expect:
            Assert.AreEqual("**>>>AM:Value(Label=\"Test3\", Type=\"Default\"), first line, **<<<, **>>>AM:Value(Label=\"Test\", Type=\"Default\"), second line, third line, **<<<, **>>>AM:Value(Label=\"Test2\", Type=\"Default\"), fourth line, fifth line, **<<<", string.Join(", ", codeFile.Content));
        }

        [TestMethod]
        public void AddAnnotation_NoChange()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "**>>>AM:Value(Label=\"Test\", Type=\"Default\")",
                "first line",
                "second line",
                "**<<<",
                "third line",
                "fourth line",
                "fifth line",
            });

            var codeFile = new CodeFile(mock.Object) {StatisticalPackage = Constants.StatisticalPackages.Stata};
            codeFile.LoadAnnotationsFromContent();

            // Overlap the new selection with the existing opening annotation tag
            var oldAnnotation = codeFile.Annotations[0];
            var newAnnotation = new Annotation()
            {
                LineStart = 0,
                LineEnd = 3,
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Value,
                ValueFormat = new ValueFormat()
            };
            var updatedAnnotation = codeFile.AddAnnotation(newAnnotation, oldAnnotation);
            Assert.AreEqual(1, codeFile.Annotations.Count);
            Assert.AreEqual(0, updatedAnnotation.LineStart);
            Assert.AreEqual(3, updatedAnnotation.LineEnd);
            Assert.AreEqual(7, codeFile.Content.Count);
            Assert.AreEqual("**>>>AM:Value(Label=\"Test\", Type=\"Default\")",
                codeFile.Content[updatedAnnotation.LineStart.Value]);
            Assert.AreEqual("**<<<", codeFile.Content[updatedAnnotation.LineEnd.Value]);
        }

        [TestMethod]
        public void AddAnnotation_Update()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "first line",
                "**>>>AM:Value(Label=\"Test\", Type=\"Default\")",
                "second line",
                "third line",
                "**<<<",
                "fourth line",
                "fifth line",
            });

            var codeFile = new CodeFile(mock.Object) {StatisticalPackage = Constants.StatisticalPackages.Stata};
            codeFile.LoadAnnotationsFromContent();

            // Overlap the new selection with the existing opening annotation tag
            var oldAnnotation = codeFile.Annotations[0];
            var newAnnotation = new Annotation()
            {
                LineStart = 0,
                LineEnd = 2,
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Value,
                ValueFormat = new ValueFormat()
            };
            var updatedAnnotation = codeFile.AddAnnotation(newAnnotation, oldAnnotation);
            Assert.AreEqual(1, codeFile.Annotations.Count);
            Assert.AreEqual(0, updatedAnnotation.LineStart);
            Assert.AreEqual(3, updatedAnnotation.LineEnd);
            Assert.AreEqual(7, codeFile.Content.Count);
            Assert.AreEqual("**>>>AM:Value(Label=\"Test\", Type=\"Default\")",
                codeFile.Content[updatedAnnotation.LineStart.Value]);
            Assert.AreEqual("**<<<", codeFile.Content[updatedAnnotation.LineEnd.Value]);

            // Now select the last two lines and update that annotation
            oldAnnotation = codeFile.Annotations[0];
            newAnnotation = new Annotation()
            {
                LineStart = 5,
                LineEnd = 6,
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Value,
                ValueFormat = new ValueFormat()
            };
            updatedAnnotation = codeFile.AddAnnotation(newAnnotation, oldAnnotation);
            Assert.AreEqual(1, codeFile.Annotations.Count);
            Assert.AreEqual(3, updatedAnnotation.LineStart);
            Assert.AreEqual(6, updatedAnnotation.LineEnd);
            Assert.AreEqual(7, codeFile.Content.Count);
            Assert.AreEqual("**>>>AM:Value(Label=\"Test\", Type=\"Default\")",
                codeFile.Content[updatedAnnotation.LineStart.Value]);
            Assert.AreEqual("**<<<", codeFile.Content[updatedAnnotation.LineEnd.Value]);

            // Select an annotation that's entirely before the existing annotation
            oldAnnotation = codeFile.Annotations[0];
            newAnnotation = new Annotation()
            {
                LineStart = 1,
                LineEnd = 2,
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Value,
                ValueFormat = new ValueFormat()
            };
            updatedAnnotation = codeFile.AddAnnotation(newAnnotation, oldAnnotation);
            Assert.AreEqual(1, codeFile.Annotations.Count);
            Assert.AreEqual(1, updatedAnnotation.LineStart);
            Assert.AreEqual(4, updatedAnnotation.LineEnd);
            Assert.AreEqual(7, codeFile.Content.Count);
            Assert.AreEqual("**>>>AM:Value(Label=\"Test\", Type=\"Default\")",
                codeFile.Content[updatedAnnotation.LineStart.Value]);
            Assert.AreEqual("**<<<", codeFile.Content[updatedAnnotation.LineEnd.Value]);

            // Overlap the selection with the existing closing annotation tag
            oldAnnotation = codeFile.Annotations[0];
            newAnnotation = new Annotation()
            {
                LineStart = 5,
                LineEnd = 6,
                OutputLabel = "Test",
                Type = Constants.AnnotationType.Value,
                ValueFormat = new ValueFormat()
            };
            updatedAnnotation = codeFile.AddAnnotation(newAnnotation, oldAnnotation);
            Assert.AreEqual(1, codeFile.Annotations.Count);
            Assert.AreEqual(3, updatedAnnotation.LineStart);
            Assert.AreEqual(6, updatedAnnotation.LineEnd);
            Assert.AreEqual(7, codeFile.Content.Count);
            Assert.AreEqual("**>>>AM:Value(Label=\"Test\", Type=\"Default\")",
                codeFile.Content[updatedAnnotation.LineStart.Value]);
            Assert.AreEqual("**<<<", codeFile.Content[updatedAnnotation.LineEnd.Value]);
        }

        [TestMethod]
        public void Save()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.WriteAllLines(It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).Verifiable();
            var codeFile = new CodeFile(mock.Object);
            codeFile.Save();
            mock.Verify();
        }

        [TestMethod]
        public void RemoveAnnotation_Exists()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "first line",
                "**>>>AM:Value(Label=\"Test\", Type=\"Default\")",
                "second line",
                "**<<<",
                "**>>>AM:Value(Label=\"Test 2\", Type=\"Default\")",
                "third line",
                "**<<<",
                "fourth line",
                "fifth line",
            });

            var codeFile = new CodeFile(mock.Object) {StatisticalPackage = Constants.StatisticalPackages.Stata};
            codeFile.LoadAnnotationsFromContent();

            codeFile.RemoveAnnotation(codeFile.Annotations[0]);
            Assert.AreEqual(1, codeFile.Annotations.Count);
            Assert.AreEqual(7, codeFile.Content.Count);
            Assert.AreEqual(2, codeFile.Annotations[0].LineStart);
            Assert.AreEqual(4, codeFile.Annotations[0].LineEnd);

            codeFile.RemoveAnnotation(codeFile.Annotations[0]);
            Assert.AreEqual(0, codeFile.Annotations.Count);
            Assert.AreEqual(5, codeFile.Content.Count);
        }

        [TestMethod]
        public void RemoveAnnotation_DoesNotExist()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "first line",
                "**>>>AM:Value(Label=\"Test\", Type=\"Default\")",
                "second line",
                "**<<<",
                "**>>>AM:Value(Label=\"Test 2\", Type=\"Default\")",
                "third line",
                "**<<<",
                "fourth line",
                "fifth line",
            });

            var codeFile = new CodeFile(mock.Object) { StatisticalPackage = Constants.StatisticalPackages.Stata };
            codeFile.LoadAnnotationsFromContent();

            codeFile.RemoveAnnotation(null);
            codeFile.RemoveAnnotation(new Annotation() { OutputLabel = "NotHere", Type = Constants.AnnotationType.Value });
            codeFile.RemoveAnnotation(new Annotation() { OutputLabel = "test", Type = Constants.AnnotationType.Value });
            Assert.AreEqual(2, codeFile.Annotations.Count);
            Assert.AreEqual(9, codeFile.Content.Count);
        }

        [TestMethod]
        public void UpdateContent()
        {
            var mock = new Mock<IFileHandler>();
            mock.Setup(file => file.WriteAllText(It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            mock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "**>>>AM:Value(Label=\"Test\", Type=\"Default\")",
                "first line",
                "second line",
                "**<<<"
            });

            var codeFile = new CodeFile(mock.Object);
            codeFile.StatisticalPackage = Constants.StatisticalPackages.Stata;
            codeFile.UpdateContent("test content");
            mock.Verify();
            Assert.AreEqual(1, codeFile.Annotations.Count);
        }
    }
}
