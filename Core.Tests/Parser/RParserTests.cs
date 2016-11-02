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

            // Now test the print command with parameters
            Assert.IsTrue(parser.IsValueDisplay("print(x, digits = 16, quote = TRUE)"));
            Assert.IsTrue(parser.IsValueDisplay("print(x,digits=16,quote=TRUE)"));
            Assert.IsTrue(parser.IsValueDisplay(" print ( x , digits = 16 , quote = TRUE ) "));

            // And test the print command with its other aliases
            Assert.IsTrue(parser.IsValueDisplay("print.default(x)"));
            Assert.IsTrue(parser.IsValueDisplay("print.default(x,digits=16,quote=TRUE)"));
            Assert.IsTrue(parser.IsValueDisplay(" print.default ( x , digits = 16 , quote = TRUE ) "));
            Assert.IsTrue(parser.IsValueDisplay("print.noquote(x)"));
            Assert.IsFalse(parser.IsValueDisplay("print.noquotes(x)"));
            Assert.IsTrue(parser.IsValueDisplay("noquote(x)"));
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

            // Now test the print command with parameters
            Assert.AreEqual("x", parser.GetValueName("print(x, digits = 16, quote = TRUE)"));
            Assert.AreEqual("x", parser.GetValueName("print(x,digits=16,quote=TRUE)"));
            Assert.AreEqual("x", parser.GetValueName(" print ( x , digits = 16 , quote = TRUE ) "));

            // And test the print command with its other aliases
            Assert.AreEqual("x", parser.GetValueName("print.default(x)"));
            Assert.AreEqual("x", parser.GetValueName("print.default(x,digits=16,quote=TRUE)"));
            Assert.AreEqual("x", parser.GetValueName(" print.default ( x , digits = 16 , quote = TRUE ) "));
            Assert.AreEqual("x_noquote", parser.GetValueName("print.noquote(x_noquote)"));
            Assert.AreEqual("x_noquote", parser.GetValueName("noquote(x_noquote)"));
        }
    }
}
