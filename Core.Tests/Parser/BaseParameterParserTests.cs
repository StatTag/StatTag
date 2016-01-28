using System;
using AnalysisManager.Core.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Parser
{
    [TestClass]
    public class BaseParameterParserTests
    {
        private const string DefaultStringValue = "DEFAULT";
        private const int DefaultIntValue = 9999;
        private const bool DefaultBoolValue = true;

        [TestMethod]
        public void GetStringParameter_Normal()
        {
            Assert.AreEqual("OK", BaseParameterParser.GetStringParameter("Test", "(Test=\"OK\")", DefaultStringValue));
        }

        [TestMethod]
        public void GetStringParameter_MissingValue()
        {
            Assert.AreEqual(DefaultStringValue, BaseParameterParser.GetStringParameter("Test", "(Test=)", DefaultStringValue));
        }

        [TestMethod]
        public void GetStringParameter_EmptyValue()
        {
            Assert.AreEqual("", BaseParameterParser.GetStringParameter("Test", "(Test=\"\")", DefaultStringValue));
        }

        [TestMethod]
        public void GetIntParameter_Normal()
        {
            Assert.AreEqual(50, BaseParameterParser.GetIntParameter("Test", "(Test=50)", DefaultIntValue));
        }

        [TestMethod]
        public void GetIntParameter_MissingValue()
        {
            Assert.AreEqual(DefaultIntValue, BaseParameterParser.GetIntParameter("Test", "(Test=)", DefaultIntValue));
        }

        [TestMethod]
        public void GetIntParameter_Quoted()
        {
            Assert.AreEqual(DefaultIntValue, BaseParameterParser.GetIntParameter("Test", "(Test=\"50\")", DefaultIntValue));
        }

        [TestMethod]
        public void GetBoolParameter_Normal()
        {
            Assert.AreEqual(false, BaseParameterParser.GetBoolParameter("Test", "(Test=false)", DefaultBoolValue));
            Assert.AreEqual(false, BaseParameterParser.GetBoolParameter("Test", "(Test=False)", DefaultBoolValue));
        }

        [TestMethod]
        public void GetBoolParameter_MissingValue()
        {
            Assert.AreEqual(DefaultBoolValue, BaseParameterParser.GetBoolParameter("Test", "(Test=)", DefaultBoolValue));
        }

        [TestMethod]
        public void GetBoolParameter_Quoted()
        {
            Assert.AreEqual(DefaultBoolValue, BaseParameterParser.GetBoolParameter("Test", "(Test=\"false\")", DefaultBoolValue));
        }
    }
}
