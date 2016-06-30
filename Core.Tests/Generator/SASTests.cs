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
    }
}
