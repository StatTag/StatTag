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
            Assert.IsTrue(parser.IsValueDisplay("%PUT"));
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
            Assert.AreEqual("test", parser.GetValueName("%PUT test;"));
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
            Assert.IsFalse(parser.IsImageExport("ods pdf"));
            Assert.IsTrue(parser.IsImageExport("ods pdf file"));
            Assert.IsFalse(parser.IsImageExport("ods pdf;"));
            Assert.IsFalse(parser.IsImageExport("ods pdf close;"));
            Assert.IsTrue(parser.IsImageExport("ods pdf file;"));
            Assert.IsTrue(parser.IsImageExport("ODS PDF FILE"));
            Assert.IsTrue(parser.IsImageExport("Ods Pdf File"));
            Assert.IsFalse(parser.IsImageExport("ods rtf file"));
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
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("ODS Pdf File=\"test.pdf\";"));
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("ods pdf file = \"test.pdf\";"));
            Assert.AreEqual("", parser.GetImageSaveLocation("ods pdf file=test.pdf;")); // It won't match because there are no quotes
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("   ods    pdf    file   =   \"test.pdf\" ;"));
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("ods pdf\r\n   file=\"test.pdf\";"));
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("ods\r\npdf\r\nfile\r\n=\"test.pdf\"\r\n;"));
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("ods pdf file=\" test.pdf \";")); // Trims the response
            Assert.AreEqual("", parser.GetImageSaveLocation("ods pdfd file = \"test.pdf\";"));

            // Testing to ensure single-quotes work as well as double-quotes
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("ods pdf file='test.pdf';"));

            // Our regex is kind of dumb... this will pass, but it is invalid in SAS
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("ods pdf file='test.pdf\";"));
            Assert.AreEqual("test.pdf", parser.GetImageSaveLocation("ods pdf file=\"test.pdf';"));
        }

        [TestMethod]
        public void GetTableName()
        {
            var parser = new SASParser();
            Assert.AreEqual("", parser.GetTableName("ods csv file=\"\";"));
            Assert.AreEqual("", parser.GetTableName("ods csv file=\"test.csv\"")); // It won't match because there is no semicolon
            Assert.AreEqual("test.csv", parser.GetTableName("ods csv file=\"test.csv\";"));
            Assert.AreEqual("test.csv", parser.GetTableName("ODS Csv File=\"test.csv\";"));
            Assert.AreEqual("test.csv", parser.GetTableName("ods csv file = \"test.csv\";"));
            Assert.AreEqual("", parser.GetTableName("ods csv file=test.csv;")); // It won't match because there are no quotes
            Assert.AreEqual("test.csv", parser.GetTableName("   ods    csv    file   =   \"test.csv\" ;"));
            Assert.AreEqual("test.csv", parser.GetTableName("ods csv\r\n   file=\"test.csv\";"));
            Assert.AreEqual("test.csv", parser.GetTableName("ods\r\ncsv\r\nfile\r\n=\"test.csv\"\r\n;"));
            Assert.AreEqual("test.csv", parser.GetTableName("ods csv file=\" test.csv \";")); // Trims the response
            Assert.AreEqual("", parser.GetTableName("ods csvd file = \"test.csv\";"));

            // Testing to ensure single-quotes work as well as double-quotes
            Assert.AreEqual("test.csv", parser.GetTableName("ODS Csv File='test.csv';"));

            // Our regex is kind of dumb... this will pass, but it is invalid in SAS
            Assert.AreEqual("test.csv", parser.GetTableName("ODS Csv File='test.csv\";"));
            Assert.AreEqual("test.csv", parser.GetTableName("ODS Csv File=\"test.csv';"));

            // Case shouldn't matter
            Assert.AreEqual("TEST.CSV", parser.GetTableName("ODS CSV FILE=\"TEST.CSV\";"));
        }

        [TestMethod]
        public void HasMacroIndicator()
        {
            var parser = new SASParser();
            Assert.IsTrue(parser.HasMacroIndicator("&test"));
            Assert.IsFalse(parser.HasMacroIndicator("%test"));
            Assert.IsFalse(parser.HasMacroIndicator("test"));
        }

        [TestMethod]
        public void HasFunctionIndicator()
        {
            var parser = new SASParser();
            Assert.IsFalse(parser.HasFunctionIndicator("&test"));
            Assert.IsTrue(parser.HasFunctionIndicator("%test"));
            Assert.IsFalse(parser.HasFunctionIndicator("test"));
        }
    }
}
