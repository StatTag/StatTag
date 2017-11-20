using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;
using StatTag.Core.ValueFormatter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.ValueFormatter
{
    /// <summary>
    /// 11/17 - Changed the underlying behavior to only consider a "missing" value to be when a string is null
    /// or empty.  Previously (prior to 3.2) we would consider a whitespace string to be a missing value.
    /// </summary>
    [TestClass]
    public class BaseValueFormatterTests
    {
        private const string WhitespaceValue = "   ";
        private const string NonWhitespaceValue = "3.14159";

        [TestMethod]
        public void TestEmptyNullStrings_NoProperties()
        {
            Assert.AreEqual(BaseValueFormatter.MissingValue, new BaseValueFormatter().Finalize(null, null));
            Assert.AreEqual(BaseValueFormatter.MissingValue, new BaseValueFormatter().Finalize(string.Empty, null));
            Assert.AreEqual(WhitespaceValue, new BaseValueFormatter().Finalize(WhitespaceValue, null));
            Assert.AreEqual(NonWhitespaceValue, new BaseValueFormatter().Finalize(NonWhitespaceValue, null));
        }

        [TestMethod]
        public void TestEmptyNullStrings_BlankForMissing()
        {
            var properties = new Properties()
            {
                RepresentMissingValues = Constants.MissingValueOption.BlankString
            };
            Assert.AreEqual(string.Empty, new BaseValueFormatter().Finalize(null, properties));
            Assert.AreEqual(string.Empty, new BaseValueFormatter().Finalize(string.Empty, properties));
            Assert.AreEqual(WhitespaceValue, new BaseValueFormatter().Finalize(WhitespaceValue, properties));
            Assert.AreEqual(NonWhitespaceValue, new BaseValueFormatter().Finalize(NonWhitespaceValue, properties));
        }

        [TestMethod]
        public void TestEmptyNullStrings_DefaultMissingForFormatter()
        {
            var properties = new Properties()
            {
                RepresentMissingValues = Constants.MissingValueOption.StatPackageDefault
            };
            Assert.AreEqual(BaseValueFormatter.MissingValue, new BaseValueFormatter().Finalize(null, properties));
            Assert.AreEqual(BaseValueFormatter.MissingValue, new BaseValueFormatter().Finalize(string.Empty, properties));
            Assert.AreEqual(WhitespaceValue, new BaseValueFormatter().Finalize(WhitespaceValue, properties));
            Assert.AreEqual(NonWhitespaceValue, new BaseValueFormatter().Finalize(NonWhitespaceValue, properties));
        }

        [TestMethod]
        public void TestEmptyNullStrings_UseSpecifiedString()
        {
            var properties = new Properties()
            {
                CustomMissingValue = "TEST",
                RepresentMissingValues = Constants.MissingValueOption.CustomValue
            };
            Assert.AreEqual(properties.CustomMissingValue, new BaseValueFormatter().Finalize(null, properties));
            Assert.AreEqual(properties.CustomMissingValue, new BaseValueFormatter().Finalize(string.Empty, properties));
            Assert.AreEqual(WhitespaceValue, new BaseValueFormatter().Finalize(WhitespaceValue, properties));
            Assert.AreEqual(NonWhitespaceValue, new BaseValueFormatter().Finalize(NonWhitespaceValue, properties));
        }

        [TestMethod]
        public void TestEmptyNullStrings_InvalidMissingValueOption()
        {
            var properties = new Properties()
            {
                CustomMissingValue = "TEST",
                RepresentMissingValues = "THISISNOTVALID"
            };
            Assert.AreEqual(BaseValueFormatter.MissingValue, new BaseValueFormatter().Finalize(null, properties));
            Assert.AreEqual(BaseValueFormatter.MissingValue, new BaseValueFormatter().Finalize(string.Empty, properties));
            Assert.AreEqual(WhitespaceValue, new BaseValueFormatter().Finalize(WhitespaceValue, properties));
            Assert.AreEqual(NonWhitespaceValue, new BaseValueFormatter().Finalize(NonWhitespaceValue, properties));
        }

        [TestMethod]
        public void TestEmptyNullStrings_CustomStringValueNull()
        {
            // Since we can't really put a null in the document, our fallback behavior is to provide a blank
            // string for the custom missing value indicator if it's showing up as null.
            var properties = new Properties()
            {
                CustomMissingValue = null,
                RepresentMissingValues = Constants.MissingValueOption.CustomValue
            };
            Assert.AreEqual(string.Empty, new BaseValueFormatter().Finalize(null, properties));
            Assert.AreEqual(string.Empty, new BaseValueFormatter().Finalize(string.Empty, properties));
            Assert.AreEqual(WhitespaceValue, new BaseValueFormatter().Finalize(WhitespaceValue, properties));
            Assert.AreEqual(NonWhitespaceValue, new BaseValueFormatter().Finalize(NonWhitespaceValue, properties));
        }

        [TestMethod]
        public void TestRegularStrings()
        {
            var properties = new Properties();
            Assert.AreEqual("Test", new BaseValueFormatter().Finalize("Test", properties));
        }
    }
}
