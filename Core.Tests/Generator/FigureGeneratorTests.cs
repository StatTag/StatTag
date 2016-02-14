using System;
using AnalysisManager.Core.Generator;
using AnalysisManager.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Generator
{
    [TestClass]
    public class FigureGeneratorTests
    {
        [TestMethod]
        public void CreateParameters_NoLabel()
        {
            var generator = new FigureGenerator();
            Assert.AreEqual("Id=\"id\"", generator.CreateParameters(new Annotation(){Id = "id"}));
        }

        [TestMethod]
        public void CreateParameters_Label()
        {
            var generator = new FigureGenerator();
            Assert.AreEqual("Id=\"id\", Label=\"Test\"", generator.CreateParameters(new Annotation() { Id = "id", OutputLabel = "Test" }));
        }
    }
}
