using System;
using Jupyter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Exceptions;

namespace EngineTests.Python
{
    [TestClass]
    public class PythonAutomationTests
    {
        [TestMethod]
        public void ParseTableResult_EmptyTable()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult("[]");
            Assert.AreEqual(0, table.ColumnSize);
            Assert.AreEqual(0, table.RowSize);
            Assert.IsNull(table.Data);
        }

        [TestMethod]
        public void ParseTableResult_EmptyTable_WithSpaces()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult(" [  ]  ");
            Assert.AreEqual(0, table.ColumnSize);
            Assert.AreEqual(0, table.RowSize);
            Assert.IsNull(table.Data);
        }

        [TestMethod]
        public void ParseTableResult_Numeric_1x3()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult("[1, 22, 333]");
            Assert.AreEqual(3, table.ColumnSize);
            Assert.AreEqual(1, table.RowSize);
            Assert.AreEqual("1", table.Data[0, 0]);
            Assert.AreEqual("22", table.Data[0, 1]);
            Assert.AreEqual("333", table.Data[0, 2]);
        }

        [TestMethod]
        public void ParseTableResult_Numeric_1x3_WithSpaces()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult(" [ 1 ,  22 ,  333  ] ");
            Assert.AreEqual(3, table.ColumnSize);
            Assert.AreEqual(1, table.RowSize);
            Assert.AreEqual("1", table.Data[0, 0]);
            Assert.AreEqual("22", table.Data[0, 1]);
            Assert.AreEqual("333", table.Data[0, 2]);
        }

        [TestMethod]
        public void ParseTableResult_Numeric_2x3_Unbalanced()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult("[[1, 22, 333], [4444, 55555]]");
            Assert.AreEqual(3, table.ColumnSize);
            Assert.AreEqual(2, table.RowSize);
            Assert.AreEqual("1", table.Data[0, 0]);
            Assert.AreEqual("22", table.Data[0, 1]);
            Assert.AreEqual("333", table.Data[0, 2]);
            Assert.AreEqual("4444", table.Data[1, 0]);
            Assert.AreEqual("55555", table.Data[1, 1]);
            Assert.IsNull(table.Data[1, 2]);
        }

        [TestMethod]
        public void ParseTableResult_Numeric_4x3_WithSpaces()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult(" [ [ 1 , 22 , 333 ] ,  [ 4444  , 55555 , 666666 ]  , [ 1 , 22 , 333  ]   ,  [ 4444  ,  55555 , 666666 ]   ] ");
            Assert.AreEqual(3, table.ColumnSize);
            Assert.AreEqual(4, table.RowSize);
            Assert.AreEqual("1", table.Data[0, 0]);
            Assert.AreEqual("22", table.Data[0, 1]);
            Assert.AreEqual("333", table.Data[0, 2]);
            Assert.AreEqual("4444", table.Data[1, 0]);
            Assert.AreEqual("55555", table.Data[1, 1]);
            Assert.AreEqual("666666", table.Data[1, 2]);
            Assert.AreEqual("1", table.Data[2, 0]);
            Assert.AreEqual("22", table.Data[2, 1]);
            Assert.AreEqual("333", table.Data[2, 2]);
            Assert.AreEqual("4444", table.Data[3, 0]);
            Assert.AreEqual("55555", table.Data[3, 1]);
            Assert.AreEqual("666666", table.Data[3, 2]);
        }

        [TestMethod]
        [ExpectedException(typeof(StatTagUserException))]
        public void ParseTableResult_Numeric_3D()
        {
            // We only allow 2D tables, so this should trigger an exception
            var automation = new PythonAutomation();
            automation.ParseTableResult(
                "[[[1,2,3],[4,5,6],[7,8,9]],[[1,2,3],[4,5,6],[7,8,9]],[[1,2,3],[4,5,6],[7,8,9]]]");
        }

        [TestMethod]
        public void ParseTableResult_Numeric_2x1_Unbalanced_WithNull()
        {
            var automation = new PythonAutomation();
            // For now we are going to throw out empty rows entirely, even if there is a subsequent non-empty row.
            var table = automation.ParseTableResult("[[], [1]]");
            Assert.AreEqual(1, table.ColumnSize);
            Assert.AreEqual(1, table.RowSize);
            Assert.AreEqual("1", table.Data[0, 0]);
        }

        [TestMethod]
        public void ParseTableResult_Numeric_2x1_AllNull()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult("[[], []]");
            Assert.AreEqual(0, table.ColumnSize);
            Assert.AreEqual(0, table.RowSize);
            Assert.IsNull(table.Data);
        }

        [TestMethod]
        public void ParseTableResult_Text_1x3_SingleQuote()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult("['1', '22', '333']");
            Assert.AreEqual(3, table.ColumnSize);
            Assert.AreEqual(1, table.RowSize);
            Assert.AreEqual("1", table.Data[0, 0]);
            Assert.AreEqual("22", table.Data[0, 1]);
            Assert.AreEqual("333", table.Data[0, 2]);
        }

        [TestMethod]
        public void ParseTableResult_Text_1x3_SingleQuote_PreserveSpaces()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult("[' 1', '22 ', ' 3 3 3 ']");
            Assert.AreEqual(3, table.ColumnSize);
            Assert.AreEqual(1, table.RowSize);
            Assert.AreEqual(" 1", table.Data[0, 0]);
            Assert.AreEqual("22 ", table.Data[0, 1]);
            Assert.AreEqual(" 3 3 3 ", table.Data[0, 2]);
        }

        [TestMethod]
        public void ParseTableResult_Text_1x3_DoubleQuote()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult("[\"1\", \"22\", \"333\"]");
            Assert.AreEqual(3, table.ColumnSize);
            Assert.AreEqual(1, table.RowSize);
            Assert.AreEqual("1", table.Data[0, 0]);
            Assert.AreEqual("22", table.Data[0, 1]);
            Assert.AreEqual("333", table.Data[0, 2]);
        }

        [TestMethod]
        public void ParseTableResult_Text_1x3_DoubleQuote_PreserveSpaces()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult(" [ \" 1\" , \"22 \" , \" 3 3 3 \" ] ");
            Assert.AreEqual(3, table.ColumnSize);
            Assert.AreEqual(1, table.RowSize);
            Assert.AreEqual(" 1", table.Data[0, 0]);
            Assert.AreEqual("22 ", table.Data[0, 1]);
            Assert.AreEqual(" 3 3 3 ", table.Data[0, 2]);
        }

        [TestMethod]
        public void ParseTableResult_Text_1x4_MixedQuotes()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult("[\"1\", '22', \"333\", '4444']");
            Assert.AreEqual(4, table.ColumnSize);
            Assert.AreEqual(1, table.RowSize);
            Assert.AreEqual("1", table.Data[0, 0]);
            Assert.AreEqual("22", table.Data[0, 1]);
            Assert.AreEqual("333", table.Data[0, 2]);
            Assert.AreEqual("4444", table.Data[0, 3]);
        }

        [TestMethod]
        public void ParseTableResult_Text_1x5_EmbeddedQuotes()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult("[\"'1'\", '\"2\"', '3', '\\'\"4\"\\'', '\\'\"5\\\\']");
            Assert.AreEqual(5, table.ColumnSize);
            Assert.AreEqual(1, table.RowSize);
            Assert.AreEqual("'1'", table.Data[0, 0]);
            Assert.AreEqual("\"2\"", table.Data[0, 1]);
            Assert.AreEqual("3", table.Data[0, 2]);
            Assert.AreEqual("'\"4\"'", table.Data[0, 3]);
            Assert.AreEqual("'\"5\\", table.Data[0, 4]);
        }

        [TestMethod]
        public void ParseTableResult_Text_Newlines()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult("['test\none', 'test\ntwo']");
            Assert.AreEqual(2, table.ColumnSize);
            Assert.AreEqual(1, table.RowSize);
            Assert.AreEqual("test\none", table.Data[0, 0]);
            Assert.AreEqual("test\ntwo", table.Data[0, 1]);
        }
    }
}
