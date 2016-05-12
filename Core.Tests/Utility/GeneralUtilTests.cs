using System;
using System.Collections.Generic;
using System.Linq;
using StatTag.Core;
using StatTag.Core.Models;
using StatTag.Core.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
    [TestClass]
    public class GeneralUtilTests
    {
        [TestMethod]
        public void StringArrayToObjectArray_Null()
        {
            var result = GeneralUtil.StringArrayToObjectArray(null);
            Assert.IsNull(result);
        }
        
        [TestMethod]
        public void StringArrayToObjectArray()
        {
            var result = GeneralUtil.StringArrayToObjectArray(new string[] {"Test1", "Test2"});
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("Test1", result[0].ToString());
            Assert.AreEqual("Test2", result[1].ToString());
        }
    }
}
