using System;
using StatTag.Core.Generator;
using StatTag.Core.Models;
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
            Assert.AreEqual("", generator.CreateParameters(new Annotation()));
        }

        [TestMethod]
        public void CreateParameters_Label()
        {
            var generator = new FigureGenerator();
            Assert.AreEqual("Label=\"Test\"", generator.CreateParameters(new Annotation() { OutputLabel = "Test" }));
        }
    }
}
