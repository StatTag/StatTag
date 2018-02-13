using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Parser;

namespace Core.Tests.Parser
{
    [TestClass]
    public class SASParserTests
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
            // Regardless of what's sent in, GetTableName always returns null
            var parser = new RParser();
            Assert.IsNull(parser.GetTableName("ods csv file=\"test.csv\";"));
        }

        [TestMethod]
        public void GetTableDataPath()
        {
            var parser = new SASParser();
            Assert.AreEqual("", parser.GetTableDataPath("ods csv file=\"\";"));
            Assert.AreEqual("", parser.GetTableDataPath("ods csv file=\"test.csv\"")); // It won't match because there is no semicolon
            Assert.AreEqual("test.csv", parser.GetTableDataPath("ods csv file=\"test.csv\";"));
            Assert.AreEqual("test.csv", parser.GetTableDataPath("ODS Csv File=\"test.csv\";"));
            Assert.AreEqual("test.csv", parser.GetTableDataPath("ods csv file = \"test.csv\";"));
            Assert.AreEqual("", parser.GetTableDataPath("ods csv file=test.csv;")); // It won't match because there are no quotes
            Assert.AreEqual("test.csv", parser.GetTableDataPath("   ods    csv    file   =   \"test.csv\" ;"));
            Assert.AreEqual("test.csv", parser.GetTableDataPath("ods csv\r\n   file=\"test.csv\";"));
            Assert.AreEqual("test.csv", parser.GetTableDataPath("ods\r\ncsv\r\nfile\r\n=\"test.csv\"\r\n;"));
            Assert.AreEqual("test.csv", parser.GetTableDataPath("ods csv file=\" test.csv \";")); // Trims the response
            Assert.AreEqual("", parser.GetTableDataPath("ods csvd file = \"test.csv\";"));

            // Testing to ensure single-quotes work as well as double-quotes
            Assert.AreEqual("test.csv", parser.GetTableDataPath("ODS Csv File='test.csv';"));

            // Our regex is kind of dumb... this will pass, but it is invalid in SAS
            Assert.AreEqual("test.csv", parser.GetTableDataPath("ODS Csv File='test.csv\";"));
            Assert.AreEqual("test.csv", parser.GetTableDataPath("ODS Csv File=\"test.csv';"));

            // Case shouldn't matter
            Assert.AreEqual("TEST.CSV", parser.GetTableDataPath("ODS CSV FILE=\"TEST.CSV\";"));

            // Test with path set - variable and constant
            Assert.AreEqual("C:\\Stats\\Test.csv", parser.GetTableDataPath("ODS CSV FILE=\"Test.csv\" path=\"C:\\Stats\";"));
            Assert.AreEqual("&outpath.\\Test.csv", parser.GetTableDataPath("ODS CSV path=&outpath file=\"Test.csv\";"));
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

        [TestMethod]
        public void GetPathParameter()
        {
            var parser = new SASParser();
            Assert.AreEqual("C:\\test\\", parser.GetPathParameter("ods csv path=\"C:\\test\\\" file=\"tmp.txt\" ;"));
            Assert.AreEqual("&outpath", parser.GetPathParameter("ods csv path = &outpath file=\"test.csv\" ;"));
            Assert.AreEqual("C:\\test\\", parser.GetPathParameter("ods csv path = \"C:\\test\\\" file=\"tmp.csv\" ;"));
            Assert.AreEqual("&outpath", parser.GetPathParameter("ods csv file=\"test.csv\" path=&outpath;"));
            Assert.AreEqual("C:\\test\\", parser.GetPathParameter("ods csv file=\"tmp.csv\" path=\"C:\\test\\\";"));
            Assert.IsNull(parser.GetPathParameter("ods csv file=\"tmp.csv\";"));
            Assert.AreEqual("", parser.GetPathParameter("ods csv file=\"tmp.csv\" path=\"\";"));
        }

        [TestMethod]
        public void PreProcessContent_MultiLineCommands()
        {
            var parser = new SASParser();
            var testList = new List<string>(new string[]
            {
                "proc export",
                "  data=Subset",
                "  dbms=xlsx",
                "  outfile=\"&Pathway.\\test.xlsx\"",
                "replace;",
                "run;"
            });
            Assert.AreEqual(2, parser.PreProcessContent(testList).Count);
            Assert.AreEqual("proc export\r\n  data=Subset\r\n  dbms=xlsx\r\n  outfile=\"&Pathway.\\test.xlsx\"\r\nreplace;\r\nrun;", string.Join("\r\n", parser.PreProcessContent(testList)));

            // Not that this would actually happen in real code, but we want to make sure that a lack
            // of any semicolon strings everything into one line, and that there is no semicolon at
            // the end
            testList = new List<string>(new string[]
            {
                "proc export",
                "  data=Subset",
                "  dbms=xlsx",
                "  outfile=\"&Pathway.\\test.xlsx\"",
                "replace",
                "run"
            });
            Assert.AreEqual(1, parser.PreProcessContent(testList).Count);
            Assert.AreEqual("proc export\r\n  data=Subset\r\n  dbms=xlsx\r\n  outfile=\"&Pathway.\\test.xlsx\"\r\nreplace\r\nrun", string.Join("\r\n", parser.PreProcessContent(testList)));

            // For this, make sure that if the last statement did not end in a semicolon that
            // we're not adding one.  It seems unlikely this would happen in practice, but
            // we want to make sure our code isn't inappropriately adding in semicolons.
            testList = new List<string>(new string[]
            {
                "proc export",
                "  data=Subset",
                "  dbms=xlsx",
                "  outfile=\"&Pathway.\\test.xlsx\"",
                "replace;",
                "run"
            });
            Assert.AreEqual(2, parser.PreProcessContent(testList).Count);
            Assert.AreEqual("proc export\r\n  data=Subset\r\n  dbms=xlsx\r\n  outfile=\"&Pathway.\\test.xlsx\"\r\nreplace;\r\nrun", string.Join("\r\n", parser.PreProcessContent(testList)));

            // Check an empty string
            testList = new List<string>(new string[]
            {
                ""
            });
            Assert.AreEqual(1, parser.PreProcessContent(testList).Count);
            Assert.AreEqual(string.Empty, string.Join("\r\n", parser.PreProcessContent(testList)));

            // Check an empty collection
            testList = new List<string>();
            Assert.AreEqual(1, parser.PreProcessContent(testList).Count);
            Assert.AreEqual(string.Empty, string.Join("\r\n", parser.PreProcessContent(testList)));

        }
    }
}
