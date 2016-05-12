using System;
using StatTag.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Models
{
    [TestClass]
    public class CommandResultTests
    {
        [TestMethod]
        public void IsEmpty()
        {
            var result = new CommandResult();
            Assert.IsTrue(result.IsEmpty());
            
            result.ValueResult = " ";
            result.FigureResult = "  ";
            Assert.IsTrue(result.IsEmpty());

            result.ValueResult = "";
            result.FigureResult = "ok";
            Assert.IsFalse(result.IsEmpty());

            result.ValueResult = "ok";
            result.FigureResult = "";
            Assert.IsFalse(result.IsEmpty());

            result.ValueResult = "";
            result.FigureResult = "";
            result.TableResult = new Table();
            Assert.IsTrue(result.IsEmpty());

            result.ValueResult = "";
            result.FigureResult = "";
            result.TableResult = new Table(new [] {"Test"}, new [] {"Test"}, 1, 1, new double?[] { 0.0 });
            Assert.IsFalse(result.IsEmpty());
        }

        [TestMethod]
        public void ToString_Formatted()
        {
            var result = new CommandResult {ValueResult = "", FigureResult = "figure ok"};
            Assert.AreEqual("figure ok", result.ToString());

            result.ValueResult = "value ok";
            result.FigureResult = "";
            Assert.AreEqual("value ok", result.ToString());

            result.ValueResult = "";
            result.FigureResult = "";
            result.TableResult = new Table(new[] { "Test" }, new[] { "Test" }, 1, 1, new double?[] { 0.0 });
            Assert.AreEqual("StatTag.Core.Models.Table", result.ToString());
        }
    }
}
