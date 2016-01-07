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
            // Stata does not appear to support multiple commands on one line, even in a do file, so this shouldn't work.  We are just asserting that we don't
            // support this functionality.
            Assert.AreNotEqual("test", parser.GetValueName("display test; display test"));
        }

        [TestMethod]
        public void GetMacroValueName()
        {
            var parser = new Stata();
            Assert.AreEqual("x2", parser.GetMacroValueName("display  `x2'"));
            Assert.AreEqual("test", parser.GetMacroValueName("display test"));   // This isn't a proper Stata macro value, but is the expected return
        }
    }
}
