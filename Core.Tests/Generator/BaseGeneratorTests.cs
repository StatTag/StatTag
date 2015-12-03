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

            public override string CreateOpenTag(Annotation annotation)
            {
                return CreateOpenTagBase();
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
    }
}
