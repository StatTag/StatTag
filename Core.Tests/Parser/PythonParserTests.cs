using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Parser;

namespace Core.Tests.Parser
{
    [TestClass]
    public class PythonParserTests
    {
        [TestMethod]
        public void IsValueDisplay()
        {
            var parser = new PythonParser();
            // Python 2.*
            Assert.IsTrue(parser.IsValueDisplay("print x"));
            Assert.IsFalse(parser.IsValueDisplay("print"));
            Assert.IsFalse(parser.IsValueDisplay("print "));
            Assert.IsFalse(parser.IsValueDisplay("printx"));

            // Python 3.*
            Assert.IsTrue(parser.IsValueDisplay("print(x)"));
            Assert.IsTrue(parser.IsValueDisplay("print (x)"));
            Assert.IsTrue(parser.IsValueDisplay(" print  (  x  ) "));
            Assert.IsTrue(parser.IsValueDisplay("print()"));
            Assert.IsTrue(parser.IsValueDisplay("print(\nx\n)"));   // Can span multiple lines
            Assert.IsTrue(parser.IsValueDisplay("print(\r\n)"));

            Assert.IsFalse(parser.IsValueDisplay("print("));
            Assert.IsFalse(parser.IsValueDisplay("print\r\n()"));  // Open paren has to be on the same line as print
            Assert.IsFalse(parser.IsValueDisplay("print )"));
            Assert.IsFalse(parser.IsValueDisplay("print )("));
        }
    }
}
