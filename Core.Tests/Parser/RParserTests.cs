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
            // We consider anything to be a candidate value, given how R works, so this is just to
            // assert it should always return true.
            var parser = new RParser();
            Assert.IsTrue(parser.IsValueDisplay("Anything can go here"));
        }

        [TestMethod]
        public void GetValueName()
        {
            // We are unrestrictive on what can be considered a value, so we don't ever return a
            // value name.  This just asserts it's always an empty string returned.
            var parser = new RParser();
            Assert.AreEqual(string.Empty, parser.GetValueName("Anything can go here"));
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

        [TestMethod]
        public void IsTableResult()
        {
            // Right now we assume anything could be a table, so IsTableResult will always return true
            var parser = new RParser();
            Assert.IsTrue(parser.IsTableResult("doesn't matter what i put here"));
        }

        [TestMethod]
        public void GetTableName()
        {
            // Same as with IsTableResult we are ignoring finding table names, so this always returns an empty string.
            var parser = new RParser();
            Assert.AreEqual(string.Empty, parser.GetTableName("doesn't matter what i put here"));
        }
    }
}
