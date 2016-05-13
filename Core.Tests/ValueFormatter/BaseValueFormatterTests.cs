using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.ValueFormatter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.ValueFormatter
{
    [TestClass]
    public class BaseValueFormatterTests
    {
        [TestMethod]
        public void TestEmptyNullStrings()
        {
            Assert.AreEqual(BaseValueFormatter.MissingValue, new BaseValueFormatter().Finalize(null));
            Assert.AreEqual(BaseValueFormatter.MissingValue, new BaseValueFormatter().Finalize(string.Empty));
            Assert.AreEqual(BaseValueFormatter.MissingValue, new BaseValueFormatter().Finalize("   "));
        }

        [TestMethod]
        public void TestRegularStrings()
        {
            Assert.AreEqual("Test", new BaseValueFormatter().Finalize("Test"));
        }
    }
}
