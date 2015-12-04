using AnalysisManager.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Models
{
    [TestClass]
    public class CodeFileTests
    {
        public class TestCodeFile : CodeFile
        {
            public string[] Content { get; set; }

            public override string[] GetContent()
            {
                return Content;
            }
        }

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
            var codeFile = new TestCodeFile();
            codeFile.LoadAnnotationsFromContent();
            Assert.AreEqual(0, codeFile.Annotations.Count);
        }

        [TestMethod]
        public void LoadAnnotationsFromContent_UnknownType()
        {
            var codeFile = new TestCodeFile
            {
                StatisticalPackage = Constants.StatisticalPackages.Stata,
                Content = new[]
                {
                    "**>>>AM:Test(Type=\"Default\")",
                    "some code here",
                    "**<<<"
                }
            };
            codeFile.LoadAnnotationsFromContent();
            Assert.AreEqual(0, codeFile.Annotations.Count);
        }

        [TestMethod]
        public void LoadAnnotationsFromContent_Normal()
        {
            var codeFile = new TestCodeFile
            {
                StatisticalPackage = Constants.StatisticalPackages.Stata,
                Content = new[]
                {
                    "**>>>AM:Value(Type=\"Default\")",
                    "some code here",
                    "**<<<"
                }
            };
            codeFile.LoadAnnotationsFromContent();
            Assert.AreEqual(1, codeFile.Annotations.Count);
            Assert.AreEqual(Constants.AnnotationType.Value, codeFile.Annotations[0].Type);
            Assert.AreSame(codeFile, codeFile.Annotations[0].CodeFile);
        }
    }
}
