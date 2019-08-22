using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Parser;

namespace Core.Tests.Parser
{
    [TestClass]
    public class PythonParserTests
    {
        [TestMethod]
        public void IsValueDisplay()
        {
            // We will take anything
            var parser = new PythonParser();
            Assert.IsTrue(parser.IsValueDisplay("print x"));
            Assert.IsTrue(parser.IsValueDisplay("print"));
            Assert.IsTrue(parser.IsValueDisplay("'test'"));
        }

        [TestMethod]
        public void IsImageExport()
        {
            var parser = new PythonParser();
            Assert.IsFalse(parser.IsImageExport(null));
            Assert.IsFalse(parser.IsImageExport(""));
            Assert.IsFalse(parser.IsImageExport("    "));
            Assert.IsFalse(parser.IsImageExport("iris"));
            Assert.IsFalse(parser.IsImageExport("plot("));
            Assert.IsFalse(parser.IsImageExport("()"));
            Assert.IsFalse(parser.IsImageExport("cf.savefig()"));

            Assert.IsTrue(parser.IsImageExport("print(x)"));   // Yup, not valid IRL, but valid for our regex.
            Assert.IsTrue(parser.IsImageExport("plot(\r\n  'test.png'\r\n  )"));
            Assert.IsTrue(parser.IsImageExport("cf.savefig('test.png')"));
            Assert.IsTrue(parser.IsImageExport("cf.savefig('C:/dev/' + 'test.png')"));
            Assert.IsTrue(parser.IsImageExport("print(\"C:\\{}\\{}\".format(path, file))"));
        }

        [TestMethod]
        public void GetImageSaveLocation()
        {
            var parser = new PythonParser();
            Assert.AreEqual(string.Empty, parser.GetImageSaveLocation(null));
            Assert.AreEqual(string.Empty, parser.GetImageSaveLocation(""));
            Assert.AreEqual(string.Empty, parser.GetImageSaveLocation("    "));
            Assert.AreEqual(string.Empty, parser.GetImageSaveLocation("iris"));
            Assert.AreEqual(string.Empty, parser.GetImageSaveLocation("plot("));
            Assert.AreEqual(string.Empty, parser.GetImageSaveLocation("()"));
            Assert.AreEqual(string.Empty, parser.GetImageSaveLocation("cf.savefig()"));

            Assert.AreEqual("x", parser.GetImageSaveLocation("print(x)"));   // Yup, not valid IRL, but valid for our regex.
            Assert.AreEqual("'test.png'", parser.GetImageSaveLocation("plot(\r\n  'test.png'\r\n  )"));
            Assert.AreEqual("'test.png'", parser.GetImageSaveLocation("cf.savefig('test.png')"));
            Assert.AreEqual("'C:/dev/' + 'test.png'", parser.GetImageSaveLocation("cf.savefig('C:/dev/' + 'test.png')"));
            Assert.AreEqual("\"C:\\{}\\{}\".format(path, file)", parser.GetImageSaveLocation("print(\"C:\\{}\\{}\".format(path, file))"));
        }
    }
}
