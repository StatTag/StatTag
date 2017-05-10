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
            result.VerbatimResult = "  ";
            Assert.IsTrue(result.IsEmpty());

            result.ValueResult = "";
            result.FigureResult = "ok";
            result.VerbatimResult = "";
            Assert.IsFalse(result.IsEmpty());

            result.ValueResult = "ok";
            result.FigureResult = "";
            result.VerbatimResult = "";
            Assert.IsFalse(result.IsEmpty());

            result.ValueResult = "";
            result.FigureResult = "";
            result.TableResult = new Table();
            result.VerbatimResult = "";
            Assert.IsTrue(result.IsEmpty());

            result.ValueResult = "";
            result.FigureResult = "";
            result.TableResult = new Table(1, 1, new string[,] { {"0.0"} });
            result.VerbatimResult = "";
            Assert.IsFalse(result.IsEmpty());

            result.ValueResult = "";
            result.FigureResult = "";
            result.VerbatimResult = "ok";
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
            result.TableResult = new Table(1, 1, new string[,] { { "0.0" } });
            Assert.AreEqual("StatTag.Core.Models.Table", result.ToString());

            result.ValueResult = "";
            result.FigureResult = "";
            result.VerbatimResult = "verbatim ok";
            Assert.AreEqual("verbatim ok", result.ToString());
        }
    }
}
