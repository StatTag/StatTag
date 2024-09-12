using Jupyter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R;
using System;

namespace EngineTests.Jupyter
{
    [TestClass]
    public class JupyterAutomationTests
    {

        [TestMethod]
        public void FormatStringFromHtml_NullEmpty()
        {
            Assert.IsNull(JupyterAutomation.FormatStringFromHtml(null));
            Assert.AreEqual("", JupyterAutomation.FormatStringFromHtml(""));
            Assert.AreEqual("  ", JupyterAutomation.FormatStringFromHtml("  "));  // Preserve the whitespace!
        }

        [TestMethod]
        public void FormatStringFromHtml_HtmlSymbols()
        {
            Assert.AreEqual(">5", JupyterAutomation.FormatStringFromHtml("&gt;5"));
            Assert.AreEqual("Yes!  It works!", JupyterAutomation.FormatStringFromHtml("Yes!&nbsp;&nbsp;It works!"));
            Assert.AreEqual("&gt;5", JupyterAutomation.FormatStringFromHtml("&amp;gt;5")); // We don't double-decode
            Assert.AreEqual("©®", JupyterAutomation.FormatStringFromHtml("&#169;&#174;"));
        }

        [TestMethod]
        public void FormatStringFromHtml_StripSingleQuotes()
        {
            Assert.AreEqual("", JupyterAutomation.FormatStringFromHtml("''"));
            Assert.AreEqual(" 'Nope'", JupyterAutomation.FormatStringFromHtml(" 'Nope'"));
            Assert.AreEqual("'Nope' ", JupyterAutomation.FormatStringFromHtml("'Nope' "));
            Assert.AreEqual("Yes, this'll work just fine.", JupyterAutomation.FormatStringFromHtml("'Yes, this'll work just fine.'"));
            Assert.AreEqual("\r\nYes\r\n'ok'\r\n", JupyterAutomation.FormatStringFromHtml("'\r\nYes\r\n'ok'\r\n'"));
        }

        [TestMethod]
        public void FormatStringFromHtml_QuotesAndHTML()
        {
            Assert.AreEqual(">5", JupyterAutomation.FormatStringFromHtml("'&gt;5'"));
            Assert.AreEqual("Yes!'  'It works!", JupyterAutomation.FormatStringFromHtml("Yes!'&nbsp;&nbsp;'It works!"));

            // The order of operations is strip the quotes, then HTML decode.  This puts in single quotes after the
            // HTML decode, so they should remain.
            Assert.AreEqual("'Leave it!'", JupyterAutomation.FormatStringFromHtml("&apos;Leave it!&apos;"));
        }
    }
}
