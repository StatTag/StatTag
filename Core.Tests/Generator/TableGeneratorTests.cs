using System;
using AnalysisManager.Core.Generator;
using AnalysisManager.Core.Models;
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
            Assert.AreEqual("", generator.CreateParameters(new Annotation()));
        }

        [TestMethod]
        public void CreateParameters_Label()
        {
            var generator = new TableGenerator();
            Assert.AreEqual("Label=\"Test\"", generator.CreateParameters(new Annotation() { OutputLabel = "Test" }));
        }
    }
}
