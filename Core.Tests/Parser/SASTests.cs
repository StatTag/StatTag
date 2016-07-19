using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Parser;

namespace Core.Tests.Parser
{
    [TestClass]
    public class SASTests
    {
        [TestMethod]
        public void IsValueDisplay()
        {
            var parser = new SASParser();
            Assert.IsFalse(parser.IsValueDisplay("put"));
            Assert.IsTrue(parser.IsValueDisplay("%put"));
            Assert.IsTrue(parser.IsValueDisplay("  %put  "));
            Assert.IsFalse(parser.IsValueDisplay("%putt"));
            Assert.IsFalse(parser.IsValueDisplay("a%put"));
            Assert.IsFalse(parser.IsValueDisplay("% put"));
            Assert.IsTrue(parser.IsValueDisplay("%put value"));
        }

        [TestMethod]
        public void GetValueName()
        {
            var parser = new SASParser();
            Assert.AreEqual("test", parser.GetValueName("%put test;"));
            Assert.AreEqual("&test", parser.GetValueName("%put &test;"));
            Assert.AreEqual("&test", parser.GetValueName(" %put   &test;  "));
            Assert.AreEqual(string.Empty, parser.GetValueName("a%put test;"));
            Assert.AreEqual("(test)", parser.GetValueName("%put (test);"));
            Assert.AreEqual("&test", parser.GetValueName("%put &test;\r\n\r\n*Some comments following"));
            Assert.AreEqual("&test", parser.GetValueName("%put\r\n&test;"));
            Assert.AreEqual("&test", parser.GetValueName(" %put   &test  ;  "));
        }

        [TestMethod]
        public void IsImageExport()
        {
            var parser = new SASParser();
            Assert.IsTrue(parser.IsImageExport("ods pdf"));
            Assert.IsFalse(parser.IsImageExport("ods rtf"));
            Assert.IsTrue(parser.IsImageExport("ods pdf file=\"\""));
            Assert.IsTrue(parser.IsImageExport("   ods    pdf    file   =   \"test.pdf\""   ));
            Assert.IsFalse(parser.IsImageExport("ods pdfd file=\"test.pdf\""));
            Assert.IsFalse(parser.IsImageExport("aods pdf file=\"test.pdf\""));
            Assert.IsFalse(parser.IsImageExport("a ods pdf file=\"test.pdf\""));
        }

        [TestMethod]
        public void GetImageSaveLocation()
        {
            var parser = new SASParser();
            Assert.AreEqual("", parser.GetImageSaveLocation("ods pdf file=\"\";"));
            Assert.AreEqual("", parser.GetImageSaveLocation("ods pdf file=\"test.pdf\"")); // It won't match because there is no semicolon
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("ods pdf file=\"test.pdf\";"));
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("ods pdf file = \"test.pdf\";"));
            Assert.AreEqual("", parser.GetImageSaveLocation("ods pdf file=test.pdf;")); // It won't match because there are no quotes
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("   ods    pdf    file   =   \"test.pdf\" ;"));
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("ods pdf\r\n   file=\"test.pdf\";"));
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("ods pdf file=\" test.pdf \";")); // Trims the response
        }
    }
}
