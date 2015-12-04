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

            var file = new CodeFile()
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
    }
}
