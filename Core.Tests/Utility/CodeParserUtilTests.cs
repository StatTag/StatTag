using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Parser;
using StatTag.Core.Utility;

namespace Core.Tests.Utility
{
    [TestClass]
    public class CodeParserUtilTests
    {
        [TestMethod]
        public void StripTrailingComments()
        {
            var test = "First line\r\nSecond line\r\nThird line";
            var result = CodeParserUtil.StripTrailingComments(test);
            Assert.AreEqual(test, result);

            test = "First line\r\nSecond line // comment \r\nThird line";
            result = CodeParserUtil.StripTrailingComments(test);
            Assert.AreEqual("First line\r\nSecond line \r\nThird line", result);

            test = "First line //    blah\r\nSecond line // blah \r\nThird line //blah";
            result = CodeParserUtil.StripTrailingComments(test);
            Assert.AreEqual("First line \r\nSecond line \r\nThird line ", result);
        }
    }
}
