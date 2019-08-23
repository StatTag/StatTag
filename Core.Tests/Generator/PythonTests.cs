using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Generator;
using StatTag.Core.Models;

namespace Core.Tests.Generator
{
    [TestClass]
    public class PythonTests
    {
        [TestMethod]
        public void CommentCharacter()
        {
            Assert.AreEqual(Constants.CodeFileComment.Python, new Python().CommentCharacter);
        }

        [TestMethod]
        public void CreateOpenTag()
        {
            var generator = new StatTag.Core.Generator.Python();
            var tag = new Tag()
            {
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            Assert.AreEqual("##>>>ST:Value(Type=\"Default\")", generator.CreateOpenTag(tag));
        }

        [TestMethod]
        public void CreateCloseTag()
        {
            var generator = new StatTag.Core.Generator.Python();
            var tag = new Tag()
            {
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            Assert.AreEqual("##<<<", generator.CreateClosingTag());
        }
    }
}
