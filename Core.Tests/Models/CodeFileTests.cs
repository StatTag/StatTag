using AnalysisManager.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
