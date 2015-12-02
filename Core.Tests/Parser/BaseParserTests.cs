using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AnalysisManager.Core.Parser;

namespace Core.Tests.Parser
{
    [TestClass]
    public class BaseParserTests
    {
        public sealed class StubParser : BaseParser
        {
            public override char CommentCharacter
            {
                get { return '*'; }
            }

            public Match DetectStartAnnotation(string line)
            {
                return base.DetectAnnotation(StartAnnotationRegEx, line);
            }

            public Match DetectEndAnnotation(string line)
            {
                return base.DetectAnnotation(EndAnnotationRegEx, line);
            }
        }

        [TestMethod]
        public void Parse_Null_Empty()
        {
            var parser = new StubParser();
            var result = parser.Parse(null);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);

            result = parser.Parse(new string[]{});
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void Parse_Simple()
        {
            var parser = new StubParser();
            var lines = new List<string>(new string[]
            {
                "**>>>AM:Value",
                "declare value",
                "**<<<AM:Value"
            });
            var result = parser.Parse(lines.ToArray());
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(0, result[0].LineStart);
            Assert.AreEqual(2, result[0].LineEnd);
        }

        [TestMethod]
        public void Parse_StartNoEnd()
        {
            var parser = new StubParser();
            var lines = new List<string>(new string[]
            {
                "**>>>AM:Value",
                "declare value"
            });
            var result = parser.Parse(lines.ToArray());
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void Parse_TwoStartsOneEnd()
        {
            var parser = new StubParser();
            var lines = new List<string>(new string[]
            {
                "**>>>AM:Value",
                "declare value",
                "**>>>AM:Value",
                "declare value",
                "**<<<AM:Value"
            });
            var result = parser.Parse(lines.ToArray());
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(2, result[0].LineStart);
            Assert.AreEqual(4, result[0].LineEnd);
        }

        [TestMethod]
        public void DetectStartAnnotation_Null_Empty()
        {
            var parser = new StubParser();
            Assert.IsFalse(parser.DetectStartAnnotation(null).Success);
            Assert.IsFalse(parser.DetectStartAnnotation(string.Empty).Success);
        }

        [TestMethod]
        public void DetectStartAnnotation_Simple()
        {
            var parser = new StubParser();
            var match = parser.DetectStartAnnotation("**>>>AM:Test");
            Assert.IsTrue(match.Success);
        }
    }
}
