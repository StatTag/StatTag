using Microsoft.VisualStudio.TestTools.UnitTesting;
using R;
using System;
using System.Text;

namespace EngineTests.R
{
    [TestClass]
    public class RAutomationTests
    {
        private static OperatingSystem Windows11 = new OperatingSystem(PlatformID.Win32NT, new Version(11, 0));
        private static OperatingSystem Windows10 = new OperatingSystem(PlatformID.Win32NT, new Version(10, 0));
        private static OperatingSystem Windows7 = new OperatingSystem(PlatformID.Win32NT, new Version(6, 1));

        [TestMethod]
        public void IsAffectedByCFGIssue_InvalidVersion()
        {
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(true, null, null));
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(true, null, null, null));
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(true, null, new Version(), null));
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(true, new OperatingSystem(PlatformID.Win32NT, new Version()), null, null));
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(true, new OperatingSystem(PlatformID.Win32NT, new Version()), new Version(), null));
        }

        [TestMethod]
        public void IsAffectedByCFGIssue_RVersionAffected()
        {
            Assert.IsTrue(RAutomation.IsAffectedByCFGIssue(true, Windows10, new Version(4, 0, 3), null));
            Assert.IsTrue(RAutomation.IsAffectedByCFGIssue(true, Windows10, new Version(4, 1), null));
            Assert.IsTrue(RAutomation.IsAffectedByCFGIssue(true, Windows10, new Version(4, 1, 7), null));
        }

        [TestMethod]
        public void IsAffectedByCFGIssue_RVersionNotAffected()
        {
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(true, Windows10, new Version(4, 0, 2), null));
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(true, Windows10, new Version(4, 0), null));
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(true, Windows10, new Version(3, 5, 0), null));
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(true, Windows10, new Version(4, 2), null));
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(true, Windows10, new Version(4, 2, 1), null));
        }

        [TestMethod]
        public void IsAffectedByCFGIssue_PlatformNotAffected()
        {
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(false, Windows10, new Version(4, 1), null));
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(false, Windows10, new Version(3, 1), null));
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(false, Windows10, new Version(5, 0, 0), null));
        }

        [TestMethod]
        public void IsAffectedByCFGIssue_OSNotAffected()
        {
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(true, Windows7, new Version(4, 1), null));
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(true, Windows7, new Version(3, 1), null));
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(true, Windows7, new Version(5, 0, 0), null));
            Assert.IsFalse(RAutomation.IsAffectedByCFGIssue(true, Windows11, new Version(5, 0, 0), null));
        }

        [TestMethod]
        public void IsAffectedByCFGIssue_Logger()
        {
            var builder = new StringBuilder();
            RAutomation.IsAffectedByCFGIssue(true, null, null, builder);
            Assert.AreEqual<string>("Unable to determine if R version is affected by Control Flow Guard issue\r\n", builder.ToString());
            builder.Clear();

            RAutomation.IsAffectedByCFGIssue(false, Windows10, new Version(3, 1), builder);
            Assert.AreEqual<string>("Not running a 64-bit process - assuming 32-bit version of R\r\n", builder.ToString());
            builder.Clear();

            RAutomation.IsAffectedByCFGIssue(true, Windows10, new Version(4, 1, 2), builder);
            Assert.AreEqual<string>("There are known issues with R 4.0.3 and higher.  Current R version 4.1.2 is not supported at this time.\r\n", builder.ToString());
            builder.Clear();

            RAutomation.IsAffectedByCFGIssue(true, Windows10, new Version(3, 5, 0), builder);
            Assert.AreEqual<string>(string.Empty, builder.ToString());
        }
    }
}
