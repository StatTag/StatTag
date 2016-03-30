using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AnalysisManager.Core.Parser;

namespace Core.Tests.Parser
{
    [TestClass]
    public class StataTests
    {
        [TestMethod]
        public void IsImageExport()
        {
            var parser = new Stata();
            Assert.IsFalse(parser.IsImageExport("graph"));
            Assert.IsTrue(parser.IsImageExport("graph export"));
            Assert.IsTrue(parser.IsImageExport("  graph export  "));
            Assert.IsTrue(parser.IsImageExport("  graph    export  "));   // Stata allows whitespace between commands
            Assert.IsFalse(parser.IsImageExport("graph exported"));
            Assert.IsFalse(parser.IsImageExport("agraph export"));
            Assert.IsFalse(parser.IsImageExport("a graph export"));
            Assert.IsTrue(parser.IsImageExport("graph export file=tmp.pdf"));
        }

        [TestMethod]
        public void IsValueDisplay()
        {
            var parser = new Stata();
            Assert.IsFalse(parser.IsValueDisplay("displa"));
            Assert.IsTrue(parser.IsValueDisplay("display"));
            Assert.IsTrue(parser.IsValueDisplay("  display  "));
            Assert.IsFalse(parser.IsValueDisplay("displayed"));
            Assert.IsFalse(parser.IsValueDisplay("adisplay"));
            Assert.IsFalse(parser.IsValueDisplay("a display"));
            Assert.IsTrue(parser.IsValueDisplay("display value"));
            Assert.IsTrue(parser.IsValueDisplay("di value"));  // Handle abbreviated command
            Assert.IsFalse(parser.IsValueDisplay("dis value"));  // Handle abbreviated command
        }

        [TestMethod]
        public void IsMacroDisplayValue()
        {
            var parser = new Stata();
            Assert.IsFalse(parser.IsMacroDisplayValue("displa `x'"));
            Assert.IsTrue(parser.IsMacroDisplayValue("display `x'"));
            Assert.IsTrue(parser.IsMacroDisplayValue("display ` x '"));
            Assert.IsTrue(parser.IsMacroDisplayValue("  display   `x'   "));
            Assert.IsFalse(parser.IsMacroDisplayValue("display 'x'"));
            Assert.IsFalse(parser.IsMacroDisplayValue("display `'"));
        }

        [TestMethod]
        public void IsTableResult()
        {
            var parser = new Stata();
            Assert.IsFalse(parser.IsTableResult("matri lis"));
            Assert.IsTrue(parser.IsTableResult("matrix list"));
            Assert.IsTrue(parser.IsTableResult("  matrix   list "));
            Assert.IsFalse(parser.IsTableResult("matrix listed"));
            Assert.IsFalse(parser.IsTableResult("amatrix list"));
            Assert.IsFalse(parser.IsTableResult("a matrix list"));
            Assert.IsTrue(parser.IsTableResult("matrix list value"));
            Assert.IsTrue(parser.IsTableResult("mat l value"));  // Handle abbreviated command
        }

        [TestMethod]
        public void GetImageSaveLocation()
        {
            var parser = new Stata();
            Assert.AreEqual("C:\\Development\\Stats\\bpgraph.pdf", parser.GetImageSaveLocation("graph export \"C:\\Development\\Stats\\bpgraph.pdf\", as(pdf) replace"));
            Assert.AreEqual("C:\\Development\\Stats\\bpgraph.pdf", parser.GetImageSaveLocation(" graph   export   \"C:\\Development\\Stats\\bpgraph.pdf\" ,  as(pdf)  replace"));
            Assert.AreEqual(string.Empty, parser.GetImageSaveLocation("agraph export \"C:\\Development\\Stats\\bpgraph.pdf\", as(pdf) replace"));
            Assert.AreEqual("mygraph.pdf", parser.GetImageSaveLocation("graph export mygraph.pdf"));
            Assert.AreEqual("mygraph.pdf", parser.GetImageSaveLocation("graph export mygraph.pdf, as(pdf)"));
            Assert.AreEqual("mygraph.pdf", parser.GetImageSaveLocation("gr export mygraph.pdf")); // "gr" shortcut
            Assert.AreNotEqual("mygraph.pdf", parser.GetImageSaveLocation("gra export mygraph.pdf"));
            Assert.AreEqual("C:\\Development\\Stats\\bpgraph.pdf", parser.GetImageSaveLocation("gr export \"C:\\Development\\Stats\\bpgraph.pdf\", as(pdf) replace"));
        }

        [TestMethod]
        public void GetValueName()
        {
            var parser = new Stata();
            Assert.AreEqual("test", parser.GetValueName("display test"));
            Assert.AreEqual("`x2'", parser.GetValueName("display  `x2'"));
            Assert.AreEqual("test", parser.GetValueName(" display   test  "));
            Assert.AreEqual(string.Empty, parser.GetValueName("adisplay test"));
            Assert.AreEqual("test", parser.GetValueName("display (test)"));
            Assert.AreEqual("test", parser.GetValueName("display(test)"));
            Assert.AreEqual("r(n)", parser.GetValueName("display r(n)"));
            Assert.AreEqual("5*2", parser.GetValueName("display (5*2)")); // Handle calculations as display parameters
            Assert.AreEqual("5*2+(7*8)", parser.GetValueName("display(5*2+(7*8))")); // Handle calculations with nested parentheses
            Assert.AreEqual("(5*2", parser.GetValueName("display (5*2")); // Mismatched parentheses.  We want to grab it, even though it'll be an error in Stata
            Assert.AreEqual("7   *    8   +   ( 5 * 7 )", parser.GetValueName("  display   (  7   *    8   +   ( 5 * 7 )  )   "));
            // Stata does not appear to support multiple commands on one line, even in a do file, so this shouldn't work.  We are just asserting that we don't
            // support this functionality.
            Assert.AreNotEqual("test", parser.GetValueName("display test; display test"));
        }

        [TestMethod]
        public void IsCalculatedDisplayValue()
        {
            var parser = new Stata();
            Assert.IsFalse(parser.IsCalculatedDisplayValue(""));
            Assert.IsFalse(parser.IsCalculatedDisplayValue("2*3"));
            Assert.IsTrue(parser.IsCalculatedDisplayValue("display (5*2)"));
            Assert.IsTrue(parser.IsCalculatedDisplayValue("display(5*2+(7*8))"));
            Assert.IsTrue(parser.IsCalculatedDisplayValue("display 5*2"));
            Assert.IsFalse(parser.IsCalculatedDisplayValue("display r[n]"));
        }

        [TestMethod]
        public void GetMacroValueName()
        {
            var parser = new Stata();
            Assert.AreEqual("x2", parser.GetMacroValueName("display  `x2'"));
            Assert.AreEqual("test", parser.GetMacroValueName("display test"));   // This isn't a proper Stata macro value, but is the expected return
        }

        [TestMethod]
        public void GetTableName()
        {
            var parser = new Stata();
            Assert.AreEqual("test_matrix", parser.GetTableName("matrix list test_matrix"));
            Assert.AreEqual("test_matrix", parser.GetTableName("   matrix   list    test_matrix  "));
            Assert.AreEqual("test", parser.GetTableName("   matrix   list    test  value  "));
            Assert.AreEqual(string.Empty, parser.GetTableName("amatrix list test"));
            Assert.AreEqual("test", parser.GetTableName("mat list test"));
            Assert.AreEqual("test", parser.GetTableName("mat l test"));
        }
    }
}
