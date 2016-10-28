using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Parser;

namespace Core.Tests.Parser
{
    [TestClass]
    public class RParserTests
    {
        [TestMethod]
        public void IsValueDisplay()
        {
            var parser = new RParser();
            Assert.IsFalse(parser.IsValueDisplay("PRINT(x)"));
            Assert.IsTrue(parser.IsValueDisplay("print(x)"));
            Assert.IsFalse(parser.IsValueDisplay("print()"));
            Assert.IsFalse(parser.IsValueDisplay("print(   )"));
            Assert.IsTrue(parser.IsValueDisplay("print(\r\nx\r\n)"));
            Assert.IsTrue(parser.IsValueDisplay("print(x);"));
            Assert.IsFalse(parser.IsValueDisplay("print(x x);"));
            Assert.IsTrue(parser.IsValueDisplay("print (x)"));
            Assert.IsTrue(parser.IsValueDisplay("  print (  x  ) "));
            Assert.IsFalse(parser.IsValueDisplay("printt (x)"));
            Assert.IsFalse(parser.IsValueDisplay("p print(x)"));
            Assert.IsFalse(parser.IsValueDisplay("print x"));
        }

        [TestMethod]
        public void GetValueName()
        {
            var parser = new RParser();
            Assert.AreEqual("test", parser.GetValueName("print(test)"));
            Assert.AreEqual("test", parser.GetValueName("print(test);"));
            Assert.AreEqual("test", parser.GetValueName("print (  test  ) "));
            Assert.AreEqual("test", parser.GetValueName("print (\r\ntest\r\n  ) "));
            Assert.AreEqual(string.Empty, parser.GetValueName("PRINT(test)"));
            Assert.AreEqual(string.Empty, parser.GetValueName("aprint(test);"));
            Assert.AreEqual("test", parser.GetValueName("print(test)#Test"));
        }
    }
}
