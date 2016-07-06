using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Generator;
using StatTag.Core.Models;

namespace Core.Tests.Generator
{
    [TestClass]
    public class SASTests
    {
        [TestMethod]
        public void CommentCharacter()
        {
            Assert.AreEqual(Constants.CodeFileComment.SAS, new SAS().CommentCharacter);
        }

        [TestMethod]
        public void CreateOpenTag()
        {
            var generator = new StatTag.Core.Generator.SAS();
            var tag = new Tag()
            {
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            Assert.AreEqual("**>>>ST:Value(Type=\"Default\");", generator.CreateOpenTag(tag));
        }

        [TestMethod]
        public void CreateCloseTag()
        {
            var generator = new StatTag.Core.Generator.SAS();
            var tag = new Tag()
            {
                Type = Constants.TagType.Value,
                ValueFormat = new ValueFormat()
            };
            Assert.AreEqual("**<<<;", generator.CreateClosingTag());
        }
    }
}
