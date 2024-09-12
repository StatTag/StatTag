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
            result.WarningResult = "  ";
            Assert.IsTrue(result.IsEmpty());

            result.ValueResult = "";
            result.FigureResult = "ok";
            result.VerbatimResult = "";
            result.WarningResult = "";
            Assert.IsFalse(result.IsEmpty());

            result.ValueResult = "ok";
            result.FigureResult = "";
            result.VerbatimResult = "";
            result.WarningResult = "";
            Assert.IsFalse(result.IsEmpty());

            result.ValueResult = "";
            result.FigureResult = "";
            result.TableResult = new Table();
            result.VerbatimResult = "";
            result.WarningResult = "";
            Assert.IsTrue(result.IsEmpty());

            result.ValueResult = "";
            result.FigureResult = "";
            result.TableResult = new Table(1, 1, new string[,] { {"0.0"} });
            result.VerbatimResult = "";
            result.WarningResult = "";
            Assert.IsFalse(result.IsEmpty());

            result.ValueResult = "";
            result.FigureResult = "";
            result.VerbatimResult = "ok";
            result.WarningResult = "";
            Assert.IsFalse(result.IsEmpty());

            result.ValueResult = "";
            result.FigureResult = "";
            result.VerbatimResult = "";
            result.WarningResult = "ok";
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
            result.VerbatimResult = "";
            result.TableResult = new Table(1, 1, new string[,] { { "0.0" } });
            result.TableResultPromise = "";
            Assert.AreEqual("StatTag.Core.Models.Table", result.ToString());

            result.ValueResult = "";
            result.FigureResult = "";
            result.VerbatimResult = "verbatim ok";
            result.TableResult = null;
            result.TableResultPromise = "";
            Assert.AreEqual("verbatim ok", result.ToString());

            result.ValueResult = "";
            result.FigureResult = "";
            result.VerbatimResult = "";
            result.TableResult = null;
            result.TableResultPromise = "promise ok";
            result.WarningResult = "";
            Assert.AreEqual("Table promise: promise ok", result.ToString());

            result.ValueResult = "";
            result.FigureResult = "";
            result.VerbatimResult = "";
            result.TableResult = null;
            result.TableResultPromise = "";
            result.WarningResult = "warning ok";
            Assert.AreEqual("warning ok", result.ToString());
        }

        [TestMethod]
        public void ToString_OrderOfValues()
        {
            var result = new CommandResult {
                ValueResult = "value ok",
                FigureResult = "figure ok",
                TableResult = new Table(1, 1, new string[,] { { "0.0" } }),
                TableResultPromise = "promise ok",
                VerbatimResult = "verbatim ok",
                WarningResult = "warning ok"
            };
            Assert.AreEqual("value ok", result.ToString());

            result.ValueResult = null;
            Assert.AreEqual("figure ok", result.ToString());

            result.FigureResult = null;
            Assert.AreEqual("verbatim ok", result.ToString());

            result.VerbatimResult = null;
            Assert.AreEqual("StatTag.Core.Models.Table", result.ToString());

            result.TableResult = null;
            Assert.AreEqual("Table promise: promise ok", result.ToString());

            result.TableResultPromise = null;
            Assert.AreEqual("warning ok", result.ToString());
        }
    }
}
