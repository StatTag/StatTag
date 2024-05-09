using Microsoft.VisualStudio.TestTools.UnitTesting;
using R;
using System;
using System.Text;

namespace EngineTests.R
{
    [TestClass]
    public class RKernelAutomationTests
    {

        [TestMethod]
        public void CleanValueResult_NullEmpty()
        {
            Assert.AreEqual(null, RKernelAutomation.CleanValueResult(null));
            Assert.AreEqual("", RKernelAutomation.CleanValueResult(""));
            Assert.AreEqual("  ", RKernelAutomation.CleanValueResult("  "));
        }

        [TestMethod]
        public void CleanValueResult_Simple()
        {
            Assert.AreEqual("25", RKernelAutomation.CleanValueResult("[1] 25"));
            Assert.AreEqual("Some text", RKernelAutomation.CleanValueResult("[9] Some text"));
            Assert.AreEqual("25 [26] 27 [24.1 - 26.8]", RKernelAutomation.CleanValueResult("[25] 25 [26] 27 [24.1 - 26.8]"));
        }

        [TestMethod]
        public void CleanValueResult_NoChange()
        {
            // Ensure we handle scenarios where no change is needed to the string
            Assert.AreEqual("25", RKernelAutomation.CleanValueResult("25"));
            Assert.AreEqual("[25", RKernelAutomation.CleanValueResult("[25"));
            Assert.AreEqual("Some text 9]", RKernelAutomation.CleanValueResult("Some text 9]"));
            // We should only remove bracketed numbers at the beginning of the string
            Assert.AreEqual("Nothing to do for [25]", RKernelAutomation.CleanValueResult("Nothing to do for [25]"));
        }

        [TestMethod]
        public void CollapseTagCommandsArray_NullEmpty()
        {
            Assert.IsNull(RKernelAutomation.CollapseTagCommandsArray(null));
            var emptyArray = new string[0];
            Assert.AreEqual(emptyArray, RKernelAutomation.CollapseTagCommandsArray(emptyArray));
        }

        [TestMethod]
        public void CollapseTagCommandsArray_TooFewElements()
        {
            var oneItem = new string[] { "test line 1" };
            Assert.AreEqual(oneItem, RKernelAutomation.CollapseTagCommandsArray(oneItem));

            var twoItems = new string[] { "test line 1", "test line 2" };
            Assert.AreEqual(twoItems, RKernelAutomation.CollapseTagCommandsArray(twoItems));
        }

        [TestMethod]
        public void CollapseTagCommandsArray_ThreeElements()
        {
            var threeItems = new string[] { "test line 1", "test line 2", "test line 3" };
            CollectionAssert.AreEqual(threeItems, RKernelAutomation.CollapseTagCommandsArray(threeItems));
        }

        [TestMethod]
        public void CollapseTagCommandsArray_MultipleCodeRows()
        {
            var code = new string[] { "test line start", "code line 1", "", "code line 2", "", "code line 3", "", "test line end" };
            var collapseResult = RKernelAutomation.CollapseTagCommandsArray(code);
            Assert.AreEqual(3, collapseResult.Length);
            Assert.AreEqual("test line start", collapseResult[0]);
            Assert.AreEqual("code line 1\r\n\r\ncode line 2\r\n\r\ncode line 3\r\n", collapseResult[1]);
            Assert.AreEqual("test line end", collapseResult[2]);
        }

        [TestMethod]
        public void ProcessHtmlValue_NullEmpty()
        {
            Assert.IsNull(RKernelAutomation.ProcessHtmlValue(null));
            Assert.AreEqual("", RKernelAutomation.ProcessHtmlValue(""));
            Assert.AreEqual("  ", RKernelAutomation.ProcessHtmlValue("  "));
        }

        [TestMethod]
        public void ProcessHtmlValue_ReplaceNA()
        {
            Assert.AreEqual(string.Empty, RKernelAutomation.ProcessHtmlValue("NA"));
            Assert.AreEqual(string.Empty, RKernelAutomation.ProcessHtmlValue("&lt;NA&gt;"));
            Assert.AreEqual(string.Empty, RKernelAutomation.ProcessHtmlValue("<NA>"));
            // There can be flanking whitespace for "NA" -- note that the next set of tests
            // (ProcessHtmlValue_NoReplaceNA) makes sure flanking whitespace isn't allowed
            // for the bracketed version.
            Assert.AreEqual(string.Empty, RKernelAutomation.ProcessHtmlValue("  NA"));
            Assert.AreEqual(string.Empty, RKernelAutomation.ProcessHtmlValue("NA   "));
        }

        [TestMethod]
        public void ProcessHtmlValue_NoReplaceNA()
        {
            // It must match case
            Assert.AreEqual("<na>", RKernelAutomation.ProcessHtmlValue("<na>"));
            // There can be no flanking whitespace
            Assert.AreEqual("<NA> ", RKernelAutomation.ProcessHtmlValue("<NA> "));
            Assert.AreEqual(" <NA>", RKernelAutomation.ProcessHtmlValue(" <NA>"));
            // Must match inner string exactly
            Assert.AreEqual("<NAN>", RKernelAutomation.ProcessHtmlValue("<NAN>"));
            // Don't double-decode
            Assert.AreEqual("&lt;NA>", RKernelAutomation.ProcessHtmlValue("&amp;lt;NA&gt;"));
            // String value of 'NA' should remain
            Assert.AreEqual("NA", RKernelAutomation.ProcessHtmlValue("'NA'"));
            Assert.AreEqual("'NA'", RKernelAutomation.ProcessHtmlValue("&apos;NA&apos;"));
        }
    }
}
