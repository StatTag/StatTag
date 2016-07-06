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
    }
}
