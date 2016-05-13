using System;
using StatTag.Core.Generator;
using StatTag.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Generator
{
    [TestClass]
    public class TableGeneratorTests
    {
        [TestMethod]
        public void CreateParameters_NoLabel()
        {
            var generator = new TableGenerator();
            Assert.AreEqual("", generator.CreateParameters(new Tag()));
        }

        [TestMethod]
        public void CreateParameters_Label()
        {
            var generator = new TableGenerator();
            Assert.AreEqual("Label=\"Test\"", generator.CreateParameters(new Tag() { OutputLabel = "Test" }));
        }
    }
}
