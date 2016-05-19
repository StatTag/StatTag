﻿using System;
using System.Collections.Generic;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Parser;
using Moq;
using Match = System.Text.RegularExpressions.Match;

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

            public Match DetectStartTag(string line)
            {
                return base.DetectTag(StartTagRegEx, line);
            }

            public Match DetectEndTag(string line)
            {
                return base.DetectTag(EndTagRegEx, line);
            }

            public override bool IsImageExport(string command)
            {
                throw new NotImplementedException();
            }

            public override bool IsValueDisplay(string command)
            {
                throw new NotImplementedException();
            }

            public override string GetImageSaveLocation(string command)
            {
                throw new NotImplementedException();
            }

            public override string GetValueName(string command)
            {
                throw new NotImplementedException();
            }

            public override bool IsTableResult(string command)
            {
                throw new NotImplementedException();
            }

            public override string GetTableName(string command)
            {
                throw new NotImplementedException();
            }

            public override List<string> PreProcessContent(List<string> originalContent)
            {
                return originalContent;
            }
        }

        [TestMethod]
        public void Parse_Null_Empty()
        {
            var parser = new StubParser();
            var result = parser.Parse(null);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);

            var mock = new Mock<CodeFile>();
            mock.Setup(file => file.LoadFileContent()).Returns(new List<string>(new string[]{}));
            result = parser.Parse(mock.Object);
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
            var mock = new Mock<CodeFile>();
            mock.Setup(file => file.LoadFileContent()).Returns(new List<string>(lines));
            var result = parser.Parse(mock.Object);
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
            var mock = new Mock<CodeFile>();
            mock.Setup(file => file.LoadFileContent()).Returns(new List<string>(lines));
            var result = parser.Parse(mock.Object);
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
            var mock = new Mock<CodeFile>();
            mock.Setup(file => file.LoadFileContent()).Returns(new List<string>(lines));
            var result = parser.Parse(mock.Object);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(2, result[0].LineStart);
            Assert.AreEqual(4, result[0].LineEnd);
        }

        [TestMethod]
        public void Parse_OnDemandFilter()
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
            var mock = new Mock<CodeFile>();
            mock.Setup(file => file.LoadFileContent()).Returns(new List<string>(lines));
            var results = parser.Parse(mock.Object, Constants.ParserFilterMode.ExcludeOnDemand);
            Assert.AreEqual(1, results.Length);
            Assert.AreEqual(Constants.RunFrequency.Always, results[0].RunFrequency);

            results = parser.Parse(mock.Object);
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual(Constants.RunFrequency.OnDemand, results[0].RunFrequency);
            Assert.AreEqual(Constants.RunFrequency.Always, results[1].RunFrequency);
        }

        [TestMethod]
        public void Parse_TagList()
        {
            var parser = new StubParser();
            var lines = new List<string>(new string[]
            {
                "declare value",
                "**>>>AM:Value(Label=\"Test1\", Frequency=\"On Demand\")",
                "declare value",
                "**<<<",
                "**>>>AM:Value(Label=\"Test2\")",
                "declare value2",
                "**<<<"
            });
            var mock = new Mock<CodeFile>();
            mock.Setup(file => file.LoadFileContent()).Returns(new List<string>(lines));
            mock.Object.FilePath = "Test.do";
            mock.Setup(file => file.Equals(It.IsAny<CodeFile>())).Returns(true);
            var results = parser.Parse(mock.Object, Constants.ParserFilterMode.TagList);
            Assert.AreEqual(2, results.Length);

            results = parser.Parse(mock.Object, Constants.ParserFilterMode.TagList, new List<Tag>()
            {
                new Tag() { Name = "Test2", Type = Constants.TagType.Value, CodeFile = mock.Object }
            });
            Assert.AreEqual(1, results.Length);
            Assert.AreEqual("Test2", results[0].Name);
        }

        [TestMethod]
        public void DetectStartTag_Null_Empty()
        {
            var parser = new StubParser();
            Assert.IsFalse(parser.DetectStartTag(null).Success);
            Assert.IsFalse(parser.DetectStartTag(string.Empty).Success);
        }

        [TestMethod]
        public void DetectStartTag_Simple()
        {
            var parser = new StubParser();
            var match = parser.DetectStartTag("**>>>AM:Test");
            Assert.IsTrue(match.Success);
        }

        [TestMethod]
        public void GetExecutionSteps()
        {
            var parser = new StubParser();
            var mock = new Mock<CodeFile>();
            mock.Setup(file => file.LoadFileContent()).Returns(new List<string>(new []
            {
                "declare value1",
                "**>>>AM:Value",
                "declare value2",
                "**<<<",
                "declare value3",
            }));
            var results = parser.GetExecutionSteps(mock.Object, Constants.ParserFilterMode.ExcludeOnDemand);
            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(Constants.ExecutionStepType.CodeBlock, results[0].Type);
            Assert.IsNull(results[0].Tag);
            Assert.AreEqual(Constants.ExecutionStepType.Tag, results[1].Type);
            Assert.IsNotNull(results[1].Tag);
            Assert.AreEqual(Constants.ExecutionStepType.CodeBlock, results[2].Type);
            Assert.IsNull(results[2].Tag);

            // Tag at the beginning
            mock.Setup(file => file.LoadFileContent()).Returns(new List<string>(new[]
            {
                "**>>>AM:Value",
                "declare value2",
                "**<<<",
                "declare value3",
            }));
            results = parser.GetExecutionSteps(mock.Object, Constants.ParserFilterMode.ExcludeOnDemand);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(Constants.ExecutionStepType.Tag, results[0].Type);
            Assert.IsNotNull(results[0].Tag);
            Assert.AreEqual(Constants.ExecutionStepType.CodeBlock, results[1].Type);
            Assert.IsNull(results[1].Tag);

            // Tag at the end
            mock.Setup(file => file.LoadFileContent()).Returns(new List<string>(new[]
            {
                "declare value2",
                "**>>>AM:Value",
                "declare value3",
                "**<<<",
            }));
            results = parser.GetExecutionSteps(mock.Object, Constants.ParserFilterMode.ExcludeOnDemand);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(Constants.ExecutionStepType.CodeBlock, results[0].Type);
            Assert.IsNull(results[0].Tag);
            Assert.AreEqual(Constants.ExecutionStepType.Tag, results[1].Type);
            Assert.IsNotNull(results[1].Tag);

            // Back to back tags
            mock.Setup(file => file.LoadFileContent()).Returns(new List<string>(new[]
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
            }));
            results = parser.GetExecutionSteps(mock.Object, Constants.ParserFilterMode.ExcludeOnDemand);
            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(Constants.ExecutionStepType.Tag, results[0].Type);
            Assert.IsNotNull(results[0].Tag);
            Assert.AreEqual(Constants.ExecutionStepType.Tag, results[1].Type);
            Assert.IsNotNull(results[1].Tag);
            Assert.AreEqual(Constants.ExecutionStepType.Tag, results[2].Type);
            Assert.IsNotNull(results[2].Tag);
        }

        [TestMethod]
        public void GetExecutionSteps_OnDemandFilter()
        {
            var parser = new StubParser();
            var mock = new Mock<CodeFile>();
            mock.Setup(file => file.LoadFileContent()).Returns(new List<string>(new[]
            {
                "declare value",
                "**>>>AM:Value(Frequency=\"On Demand\")",
                "declare value",
                "**<<<",
                "**>>>AM:Value",
                "declare value2",
                "**<<<"
            }));
            var results = parser.GetExecutionSteps(mock.Object, Constants.ParserFilterMode.ExcludeOnDemand);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(Constants.ExecutionStepType.CodeBlock, results[0].Type);
            Assert.IsNull(results[0].Tag);
            Assert.AreEqual(Constants.ExecutionStepType.Tag, results[1].Type);
            Assert.IsNotNull(results[1].Tag);

            results = parser.GetExecutionSteps(mock.Object);
            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(Constants.ExecutionStepType.CodeBlock, results[0].Type);
            Assert.IsNull(results[0].Tag);
            Assert.AreEqual(Constants.ExecutionStepType.Tag, results[1].Type);
            Assert.IsNotNull(results[1].Tag);
            Assert.AreEqual(Constants.ExecutionStepType.Tag, results[2].Type);
            Assert.IsNotNull(results[2].Tag);
        }

        [TestMethod]
        public void GetExecutionSteps_TagList()
        {
            var parser = new StubParser();
            var mock = new Mock<CodeFile>();
            mock.Setup(file => file.LoadFileContent()).Returns(new List<string>(new[]
            {
                "declare value",
                "**>>>AM:Value(Label=\"Test1\", Frequency=\"On Demand\")",
                "declare value",
                "**<<<",
                "**>>>AM:Value(Label=\"Test2\")",
                "declare value2",
                "**<<<"
            }));
            mock.Object.FilePath = "Test.do";
            mock.Setup(file => file.Equals(It.IsAny<CodeFile>())).Returns(true);
            var results = parser.GetExecutionSteps(mock.Object, Constants.ParserFilterMode.TagList);
            Assert.AreEqual(3, results.Count);

            results = parser.GetExecutionSteps(mock.Object, Constants.ParserFilterMode.TagList, new List<Tag>()
            {
                new Tag() { Name = "Test1", Type = Constants.TagType.Value, CodeFile = mock.Object }
            });
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(Constants.ExecutionStepType.CodeBlock, results[0].Type);
            Assert.IsNull(results[0].Tag);
            Assert.AreEqual(Constants.ExecutionStepType.Tag, results[1].Type);
            Assert.IsNotNull(results[1].Tag);
            Assert.AreEqual("Test1", results[1].Tag.Name);
        }
    }
}
