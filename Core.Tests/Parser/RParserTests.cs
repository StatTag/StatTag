using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Parser;

namespace Core.Tests.Parser
{
    [TestClass]
    public class RParserTests
    {
        [TestMethod]
        public void IsValueDisplay()
        {
            var parser = new RParser();
            Assert.IsFalse(parser.IsValueDisplay("PRINT(x)"));
            Assert.IsTrue(parser.IsValueDisplay("print(x)"));
            Assert.IsFalse(parser.IsValueDisplay("print()"));
            Assert.IsFalse(parser.IsValueDisplay("print(   )"));
            Assert.IsTrue(parser.IsValueDisplay("print(\r\nx\r\n)"));
            Assert.IsTrue(parser.IsValueDisplay("print(x);"));
            Assert.IsFalse(parser.IsValueDisplay("print(x x);"));
            Assert.IsTrue(parser.IsValueDisplay("print (x)"));
            Assert.IsTrue(parser.IsValueDisplay("  print (  x  ) "));
            Assert.IsFalse(parser.IsValueDisplay("printt (x)"));
            Assert.IsFalse(parser.IsValueDisplay("p print(x)"));
            Assert.IsFalse(parser.IsValueDisplay("print x"));

            // Now test the print command with parameters
            Assert.IsTrue(parser.IsValueDisplay("print(x, digits = 16, quote = TRUE)"));
            Assert.IsTrue(parser.IsValueDisplay("print(x,digits=16,quote=TRUE)"));
            Assert.IsTrue(parser.IsValueDisplay(" print ( x , digits = 16 , quote = TRUE ) "));

            // And test the print command with its other aliases
            Assert.IsTrue(parser.IsValueDisplay("print.default(x)"));
            Assert.IsFalse(parser.IsValueDisplay("printsdefault(x)"));
            Assert.IsTrue(parser.IsValueDisplay("print.default(x,digits=16,quote=TRUE)"));
            Assert.IsTrue(parser.IsValueDisplay(" print.default ( x , digits = 16 , quote = TRUE ) "));
            Assert.IsTrue(parser.IsValueDisplay("print.noquote(x)"));
            Assert.IsFalse(parser.IsValueDisplay("print.noquotes(x)"));
            Assert.IsTrue(parser.IsValueDisplay("noquote(x)"));
        }

        [TestMethod]
        public void GetValueName()
        {
            var parser = new RParser();
            Assert.AreEqual("test", parser.GetValueName("print(test)"));
            Assert.AreEqual("test", parser.GetValueName("print(test);"));
            Assert.AreEqual("test", parser.GetValueName("print (  test  ) "));
            Assert.AreEqual("test", parser.GetValueName("print (\r\ntest\r\n  ) "));
            Assert.AreEqual(string.Empty, parser.GetValueName("PRINT(test)"));
            Assert.AreEqual(string.Empty, parser.GetValueName("aprint(test);"));
            Assert.AreEqual("test", parser.GetValueName("print(test)#Test"));

            // Now test the print command with parameters
            Assert.AreEqual("x", parser.GetValueName("print(x, digits = 16, quote = TRUE)"));
            Assert.AreEqual("x", parser.GetValueName("print(x,digits=16,quote=TRUE)"));
            Assert.AreEqual("x", parser.GetValueName(" print ( x , digits = 16 , quote = TRUE ) "));

            // And test the print command with its other aliases
            Assert.AreEqual("x", parser.GetValueName("print.default(x)"));
            Assert.AreEqual(string.Empty, parser.GetValueName("printsdefault(x)"));
            Assert.AreEqual("x", parser.GetValueName("print.default(x,digits=16,quote=TRUE)"));
            Assert.AreEqual("x", parser.GetValueName(" print.default ( x , digits = 16 , quote = TRUE ) "));
            Assert.AreEqual("x_noquote", parser.GetValueName("print.noquote(x_noquote)"));
            Assert.AreEqual("x_noquote", parser.GetValueName("noquote(x_noquote)"));
        }

        [TestMethod]
        public void IsImageExport()
        {
            var parser = new RParser();
            Assert.IsTrue(parser.IsImageExport("png('test.png')"));
            Assert.IsTrue(parser.IsImageExport("png(\"test.png\")"));
            Assert.IsFalse(parser.IsImageExport("png(\"test.png\""));
            Assert.IsTrue(parser.IsImageExport(" png ( \"test.png\" ) "));
            Assert.IsFalse(parser.IsImageExport("spng(\"test.png\""));
            Assert.IsFalse(parser.IsImageExport("pngs('test.png')"));
            Assert.IsFalse(parser.IsImageExport("pn('test.png')"));
            Assert.IsTrue(parser.IsImageExport("png\r\n(\r\n\"test.png\"\r\n)\r\n"));

            Assert.IsTrue(parser.IsImageExport("pdf('test.pdf')"));
            Assert.IsTrue(parser.IsImageExport("win.metafile('test.wmf')"));
            Assert.IsTrue(parser.IsImageExport("jpeg('test.jpg')"));
            Assert.IsTrue(parser.IsImageExport("png('test.png')"));
            Assert.IsTrue(parser.IsImageExport("bmp('test.bmp')"));
            Assert.IsTrue(parser.IsImageExport("postscript('test.ps')"));
        }

        [TestMethod]
        public void GetImageSaveLocation()
        {
            var parser = new RParser();
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("pdf(\"test.pdf\")"));
            Assert.AreEqual("test.png", parser.GetImageSaveLocation("png(width = 100, height=100, \r\n\tfile=\r\n\t\t\"test.png\")"));
            Assert.AreEqual("test.png", parser.GetImageSaveLocation("png(\"test.png\", width=100,height=100)"));
            Assert.AreEqual("test.png", parser.GetImageSaveLocation("png (width=100,height=100,\r\n\t\"test.png\")"));
            Assert.AreEqual("test.png", parser.GetImageSaveLocation("png(width=100,\"test.png\",height=100)"));
            Assert.AreEqual("test.png", parser.GetImageSaveLocation("png(width=100,'test.png',height=100)"));
            Assert.AreEqual("C:\\\\Test\\\\Path with spaces\\\\test.pdf", parser.GetImageSaveLocation("pdf(\"C:\\\\Test\\\\Path with spaces\\\\test.pdf\")"));
            Assert.AreEqual(string.Empty, parser.GetImageSaveLocation("spng(width=100,'test.png',height=100)"));

            // Some duplication, but verifies each file type works
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("pdf(\"test.pdf\")"));
            Assert.AreEqual("test.wmf", parser.GetImageSaveLocation("win.metafile(\"test.wmf\")"));
            Assert.AreEqual("test.jpeg", parser.GetImageSaveLocation("jpeg(\"test.jpeg\")"));
            Assert.AreEqual("test.png", parser.GetImageSaveLocation("png(\"test.png\")"));
            Assert.AreEqual("test.bmp", parser.GetImageSaveLocation("bmp(\"test.bmp\")"));
            Assert.AreEqual("test.ps", parser.GetImageSaveLocation("postscript(\"test.ps\")"));

            // If we have two image commands in the same text block, we should only extract the first VALID one (non-greedy matching)
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("pdf(\"test.pdf\");png(\"test.png\")"));
        }
    }
}
