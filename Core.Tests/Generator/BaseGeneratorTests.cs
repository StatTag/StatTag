using System;
using System.Security.Cryptography.X509Certificates;
using AnalysisManager.Core.Generator;
using AnalysisManager.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Generator
{
    [TestClass]
    public class BaseGeneratorTests
    {
        class StubGenerator : BaseGenerator
        {
            public override string CommentCharacter
            {
                get { return "*"; }
            }
        }

        [TestMethod]
        public void CreateOpenTagBase()
        {
            var generator = new StubGenerator();
            Assert.AreEqual("**>>>AM:", generator.CreateOpenTag(null));
        }

        [TestMethod]
        public void CreateClosingTag()
        {
            var generator = new StubGenerator();
            Assert.AreEqual("**<<<", generator.CreateClosingTag());
        }

        [TestMethod]
        public void CreateOpenTag_Value()
        {
            var generator = new StubGenerator();
            var annotation = new Annotation()
            {
                Type = Constants.AnnotationType.Value,
                ValueFormat = new ValueFormat()
            };
            Assert.AreEqual("**>>>AM:Value(Type=\"Default\")", generator.CreateOpenTag(annotation));
        }
    }
}
