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
            // We will take anything
            var parser = new PythonParser();
            Assert.IsTrue(parser.IsValueDisplay("print x"));
            Assert.IsTrue(parser.IsValueDisplay("print"));
            Assert.IsTrue(parser.IsValueDisplay("'test'"));
        }
    }
}
