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
    }
}
