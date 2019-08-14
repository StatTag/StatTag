using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Generator;
using StatTag.Core.Models;

namespace Core.Tests.Generator
{
    [TestClass]
    public class RTests
    {
        [TestMethod]
        public void CommentCharacter()
        {
            Assert.AreEqual(Constants.CodeFileComment.R, new R().CommentCharacter);
            Assert.AreEqual(Constants.CodeFileComment.R, new RMarkdown().CommentCharacter);
        }

        [TestMethod]
        public void CreateOpenTag()
        {
            var generator = new StatTag.Core.Generator.R();
            var tag = new Tag()
            {
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            Assert.AreEqual("##>>>ST:Value(Type=\"Default\")", generator.CreateOpenTag(tag));
        }

        [TestMethod]
        public void CreateOpenTagMarkdown()
        {
            var generator = new StatTag.Core.Generator.RMarkdown();
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
            var generator = new StatTag.Core.Generator.R();
            var tag = new Tag()
            {
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            Assert.AreEqual("##<<<", generator.CreateClosingTag());
        }

        [TestMethod]
        public void CreateCloseTagMarkdown()
        {
            var generator = new StatTag.Core.Generator.RMarkdown();
            var tag = new Tag()
            {
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            Assert.AreEqual("##<<<", generator.CreateClosingTag());
        }
    }
}
