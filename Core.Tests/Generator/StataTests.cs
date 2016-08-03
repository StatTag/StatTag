using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Generator;
using StatTag.Core.Models;

namespace Core.Tests.Generator
{
    [TestClass]
    public class StataTests
    {
        [TestMethod]
        public void CommentCharacter()
        {
            Assert.AreEqual(Constants.CodeFileComment.Stata, new Stata().CommentCharacter);
        }
    }
}
