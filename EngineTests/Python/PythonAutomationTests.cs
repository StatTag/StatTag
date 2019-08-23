using System;
using Jupyter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Exceptions;
using StatTag.Core.Models;

namespace EngineTests.Python
{
    [TestClass]
    public class PythonAutomationTests
    {
        private void AssertEmptyTable(Table table)
        {
            Assert.AreEqual(0, table.ColumnSize);
            Assert.AreEqual(0, table.RowSize);
            Assert.IsNull(table.Data);
        }

        [TestMethod]
        public void ParseTableResult_EmptyNullInput()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult(string.Empty);
            AssertEmptyTable(table);
            table = automation.ParseTableResult("   ");
            AssertEmptyTable(table);
            table = automation.ParseTableResult(null);
            AssertEmptyTable(table);
        }

        [TestMethod]
        public void ParseTableResult_NotATable()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult(" This is not a table we can handle ");
            AssertEmptyTable(table);
        }

        [TestMethod]
        public void ParseTableResult_EmptyTable()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult("[]");
            AssertEmptyTable(table);
        }

        [TestMethod]
        public void ParseTableResult_EmptyTable_WithSpaces()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseTableResult(" [  ]  ");
            AssertEmptyTable(table);
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

        [TestMethod]
        public void ParseHtmlTableResult_EmptyNullInput()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseHtmlTableResult(string.Empty);
            AssertEmptyTable(table);
            table = automation.ParseHtmlTableResult("   ");
            AssertEmptyTable(table);
            table = automation.ParseHtmlTableResult(null);
            AssertEmptyTable(table);
        }

        [TestMethod]
        public void ParseHtmlTableResult_NotATable()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseHtmlTableResult(" This is not a table we can handle ");
            AssertEmptyTable(table);
        }

        [TestMethod]
        public void ParseHtmlTableResult_EmptyTable()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseHtmlTableResult("<table/>");
            AssertEmptyTable(table);
            table = automation.ParseHtmlTableResult("<table>  </table>");
            AssertEmptyTable(table);
        }

        [TestMethod]
        public void ParseHtmlTableResult_5x6_Thead_Tbody_THRowLabels()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseHtmlTableResult("<table border='1' class='dataframe'> <thead> <tr style='text-align: right;'> <th></th> <th>sepal_length</th> <th>sepal_width</th> <th>petal_length</th> <th>petal_width</th> <th>species</th> </tr> </thead> <tbody> <tr> <th>0</th> <td>5.1</td> <td>3.5</td> <td>1.4</td> <td>0.2</td> <td>setosa</td> </tr> <tr> <th>1</th> <td>4.9</td> <td>3.0</td> <td>1.4</td> <td>0.2</td> <td>setosa</td> </tr> <tr> <th>2</th> <td>4.7</td> <td>3.2</td> <td>1.3</td> <td>0.2</td> <td>setosa</td> </tr> <tr> <th>3</th> <td>4.6</td> <td>3.1</td> <td>1.5</td> <td>0.2</td> <td>setosa</td> </tr> </tbody> </table>");
            Assert.AreEqual(6, table.ColumnSize);
            Assert.AreEqual(5, table.RowSize);
            Assert.IsNotNull(table.Data);
            Assert.AreEqual("", table.Data[0, 0]);
            Assert.AreEqual("sepal_length", table.Data[0, 1]);
            Assert.AreEqual("0.2", table.Data[4, 4]);
            Assert.AreEqual("setosa", table.Data[4, 5]);
        }

        [TestMethod]
        public void ParseHtmlTableResult_3x2_PlainTable()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseHtmlTableResult("<table><tr><td>1</td><td>2</td></tr><tr><td>3</td><td>4</td></tr><tr><td>5</td><td>6</td></tr></table>");
            Assert.AreEqual(2, table.ColumnSize);
            Assert.AreEqual(3, table.RowSize);
            Assert.IsNotNull(table.Data);
            Assert.AreEqual("1", table.Data[0, 0]);
            Assert.AreEqual("2", table.Data[0, 1]);
            Assert.AreEqual("3", table.Data[1, 0]);
            Assert.AreEqual("4", table.Data[1, 1]);
            Assert.AreEqual("5", table.Data[2, 0]);
            Assert.AreEqual("6", table.Data[2, 1]);
        }

        [TestMethod]
        public void ParseHtmlTableResult_3x4_Unbalanced()
        {
            var automation = new PythonAutomation();
            var table = automation.ParseHtmlTableResult("<table><tr><td>1</td><td>2</td></tr><tr><td>3</td><td>4</td><td>5</td><td>6</td></tr><tr><td>7</td></tr></table>");
            Assert.AreEqual(4, table.ColumnSize);
            Assert.AreEqual(3, table.RowSize);
            Assert.IsNotNull(table.Data);
            Assert.AreEqual("1", table.Data[0, 0]);
            Assert.AreEqual("2", table.Data[0, 1]);
            Assert.IsNull(table.Data[0, 2]);
            Assert.IsNull(table.Data[0, 3]);
            Assert.AreEqual("3", table.Data[1, 0]);
            Assert.AreEqual("4", table.Data[1, 1]);
            Assert.AreEqual("5", table.Data[1, 2]);
            Assert.AreEqual("6", table.Data[1, 3]);
            Assert.AreEqual("7", table.Data[2, 0]);
            Assert.IsNull(table.Data[2, 1]);
            Assert.IsNull(table.Data[2, 2]);
            Assert.IsNull(table.Data[2, 3]);
        }
    }
}
