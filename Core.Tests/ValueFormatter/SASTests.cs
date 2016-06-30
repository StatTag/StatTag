using System;
using System.Text;
using System.Collections.Generic;
using StatTag.Core.ValueFormatter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.ValueFormatter
{
    /// <summary>
    /// Summary description for StataTests
    /// </summary>
    [TestClass]
    public class SASTests
    {
        [TestMethod]
        public void CheckMissingValue()
        {
            Assert.AreEqual(SAS.MissingValue, new SAS().GetMissingValue());
        }
    }
}
