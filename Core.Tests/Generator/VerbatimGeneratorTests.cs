using System;
using StatTag.Core.Generator;
using StatTag.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Generator
{
    [TestClass]
    public class VerbatimGeneratorTests
    {
        [TestMethod]
        public void CreateParameters_NoLabel()
        {
            var generator = new VerbatimGenerator();
            Assert.AreEqual("", generator.CreateParameters(new Tag()));
        }

        [TestMethod]
        public void CreateParameters_Label()
        {
            var generator = new VerbatimGenerator();
            Assert.AreEqual("Label=\"Test\"", generator.CreateParameters(new Tag() { Name = "Test" }));
        }
    }
}
