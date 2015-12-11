using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AnalysisManager.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AnalysisManager.Core.Parser;

namespace Core.Tests.Parser
{
    [TestClass]
    public class BaseParserTests
    {
        public sealed class StubParser : BaseParser
        {
            public override string CommentCharacter
            {
                get { return "*"; }
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
                "**>>>AM:Test",
                "declare value",
                "**<<<AM:Test"
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
                "**>>>AM:Test",
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
                "**>>>AM:Test",
                "declare value",
                "**>>>AM:Test",
                "declare value",
                "**<<<AM:Test"
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

        [TestMethod]
        public void Filter()
        {
            var parser = new StubParser();
            var lines = new List<string>(new string[]
            {
                "declare value",
                "**>>>AM:Value(Frequency=\"On Demand\")",
                "declare value",
                "**<<<",
                "**>>>AM:Value",
                "declare value2",
                "**<<<"
            });
            var results = parser.Filter(lines, Constants.ParserFilterMode.ExcludeOnDemand);
            Assert.AreEqual(4, results.Length);
            Assert.AreEqual("declare value, **>>>AM:Value, declare value2, **<<<", string.Join(", ", results));

            results = parser.Filter(lines);
            Assert.AreEqual(7, results.Length);
        }

        [TestMethod]
        public void GetExecutionSteps()
        {
            var parser = new StubParser();
            var lines = new List<string>(new string[]
            {
                "declare value1",
                "**>>>AM:Value",
                "declare value2",
                "**<<<",
                "declare value3",
            });
            var results = parser.GetExecutionSteps(lines, Constants.ParserFilterMode.ExcludeOnDemand);
            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(Constants.ExecutionStepType.CodeBlock, results[0].Type);
            Assert.IsNull(results[0].Annotation);
            Assert.AreEqual(Constants.ExecutionStepType.Annotation, results[1].Type);
            Assert.IsNotNull(results[1].Annotation);
            Assert.AreEqual(Constants.ExecutionStepType.CodeBlock, results[2].Type);
            Assert.IsNull(results[2].Annotation);

            // Annotation at the beginning
            lines = new List<string>(new string[]
            {
                "**>>>AM:Value",
                "declare value2",
                "**<<<",
                "declare value3",
            });
            results = parser.GetExecutionSteps(lines, Constants.ParserFilterMode.ExcludeOnDemand);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(Constants.ExecutionStepType.Annotation, results[0].Type);
            Assert.IsNotNull(results[0].Annotation);
            Assert.AreEqual(Constants.ExecutionStepType.CodeBlock, results[1].Type);
            Assert.IsNull(results[1].Annotation);

            // Annotation at the end
            lines = new List<string>(new string[]
            {
                
                "declare value2",
                "**>>>AM:Value",
                "declare value3",
                "**<<<",
            });
            results = parser.GetExecutionSteps(lines, Constants.ParserFilterMode.ExcludeOnDemand);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(Constants.ExecutionStepType.CodeBlock, results[0].Type);
            Assert.IsNull(results[0].Annotation);
            Assert.AreEqual(Constants.ExecutionStepType.Annotation, results[1].Type);
            Assert.IsNotNull(results[1].Annotation);

            // Back to back annotations
            lines = new List<string>(new string[]
            {
                "**>>>AM:Value",
                "declare value1",
                "**<<<",
                "**>>>AM:Value",
                "declare value2",
                "**<<<",
                "**>>>AM:Value",
                "declare value3",
                "**<<<",
            });
            results = parser.GetExecutionSteps(lines, Constants.ParserFilterMode.ExcludeOnDemand);
            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(Constants.ExecutionStepType.Annotation, results[0].Type);
            Assert.IsNotNull(results[0].Annotation);
            Assert.AreEqual(Constants.ExecutionStepType.Annotation, results[1].Type);
            Assert.IsNotNull(results[1].Annotation);
            Assert.AreEqual(Constants.ExecutionStepType.Annotation, results[2].Type);
            Assert.IsNotNull(results[2].Annotation);
        }

        [TestMethod]
        public void GetExecutionSteps_OnDemandFilter()
        {
            var parser = new StubParser();
            var lines = new List<string>(new string[]
            {
                "declare value",
                "**>>>AM:Value(Frequency=\"On Demand\")",
                "declare value",
                "**<<<",
                "**>>>AM:Value",
                "declare value2",
                "**<<<"
            });
            var results = parser.GetExecutionSteps(lines, Constants.ParserFilterMode.ExcludeOnDemand);
            Assert.AreEqual(2, results.Count);

            results = parser.GetExecutionSteps(lines);
            Assert.AreEqual(3, results.Count);
        }
    }
}
