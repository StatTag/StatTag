using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Parser;

namespace Core.Tests.Parser
{
    [TestClass]
    public class StataParserTests
    {
        [TestMethod]
        public void IsImageExport()
        {
            var parser = new StataParser();
            Assert.IsFalse(parser.IsImageExport("graph"));
            Assert.IsTrue(parser.IsImageExport("graph export"));
            Assert.IsTrue(parser.IsImageExport("  graph export  "));
            Assert.IsTrue(parser.IsImageExport("  graph    export  "));   // Stata allows whitespace between commands
            Assert.IsFalse(parser.IsImageExport("graph exported"));
            Assert.IsFalse(parser.IsImageExport("agraph export"));
            Assert.IsFalse(parser.IsImageExport("a graph export"));
            Assert.IsTrue(parser.IsImageExport("graph export file=tmp.pdf"));
        }

        [TestMethod]
        public void IsValueDisplay()
        {
            var parser = new StataParser();
            Assert.IsFalse(parser.IsValueDisplay("displa"));
            Assert.IsTrue(parser.IsValueDisplay("display"));
            Assert.IsTrue(parser.IsValueDisplay("  display  "));
            Assert.IsFalse(parser.IsValueDisplay("displayed"));
            Assert.IsFalse(parser.IsValueDisplay("adisplay"));
            Assert.IsFalse(parser.IsValueDisplay("a display"));
            Assert.IsTrue(parser.IsValueDisplay("display value"));
            Assert.IsFalse(parser.IsValueDisplay("diplay value"));  // Making sure our optional capture of "s" doesn't cause invalid commands to be accepted
            Assert.IsTrue(parser.IsValueDisplay("di value"));  // Handle abbreviated command
            Assert.IsTrue(parser.IsValueDisplay("dis value"));  // Handle abbreviated command
        }

        [TestMethod]
        public void IsMacroDisplayValue()
        {
            var parser = new StataParser();
            Assert.IsFalse(parser.IsMacroDisplayValue("displa `x'"));
            Assert.IsTrue(parser.IsMacroDisplayValue("display `x'"));
            Assert.IsTrue(parser.IsMacroDisplayValue("display ` x '"));
            Assert.IsTrue(parser.IsMacroDisplayValue("  display   `x'   "));
            Assert.IsFalse(parser.IsMacroDisplayValue("display 'x'"));
            Assert.IsFalse(parser.IsMacroDisplayValue("display `'"));

            // Global macro values
            Assert.IsTrue(parser.IsMacroDisplayValue("  display   $x   "));
            Assert.IsFalse(parser.IsMacroDisplayValue("display $ x"));
        }

        [TestMethod]
        public void IsTableResult()
        {
            var parser = new StataParser();
            Assert.IsFalse(parser.IsTableResult("matri lis"));
            Assert.IsTrue(parser.IsTableResult("matrix list"));
            Assert.IsTrue(parser.IsTableResult("  matrix   list "));
            Assert.IsFalse(parser.IsTableResult("matrix listed"));
            Assert.IsFalse(parser.IsTableResult("amatrix list"));
            Assert.IsFalse(parser.IsTableResult("a matrix list"));
            Assert.IsTrue(parser.IsTableResult("matrix list value"));
            Assert.IsTrue(parser.IsTableResult("mat l value"));  // Handle abbreviated command
            Assert.IsTrue(parser.IsTableResult("matrix list r(coefs)"));
        }


        [TestMethod]
        public void IsTableResult_DataFile()
        {
            var parser = new StataParser();
            Assert.IsTrue(parser.IsTableResult("estadd using test.csv"));  // Even though this isn't allowed for data export ("estadd"), we are just checking the presence of file paths
            Assert.IsTrue(parser.IsTableResult("estout using test.csv"));
            Assert.IsTrue(parser.IsTableResult("estout using C:\\test.csv"));
            Assert.IsTrue(parser.IsTableResult("estout using /c:/test.csv"));
            Assert.IsTrue(parser.IsTableResult("esttab using example.csv, replace wide plain"));
            Assert.IsTrue(parser.IsTableResult("  esttab  using  example.csv ,  replace  wide  plain "));
            Assert.IsFalse(parser.IsTableResult("estadd using test csv"));

            // Handle local and global macro values - they MAY contain a filename, so the heuristic check should allow them
            Assert.IsTrue(parser.IsTableResult("estout using `filename'"));
            Assert.IsTrue(parser.IsTableResult("estout using $filename"));
            Assert.IsFalse(parser.IsTableResult("estout using $ filename"));
        }

        [TestMethod]
        public void IsStartingLog()
        {
            var parser = new StataParser();
            Assert.IsFalse(parser.IsStartingLog("*log using tmp.txt"));
            Assert.IsFalse(parser.IsStartingLog("*cmdlog using tmp.txt"));
            Assert.IsFalse(parser.IsStartingLog("  *  log using tmp.txt  "));
            Assert.IsFalse(parser.IsStartingLog("  *  cmdlog using tmp.txt  "));
            Assert.IsFalse(parser.IsStartingLog("l og using tmp.txt  "));
            Assert.IsFalse(parser.IsStartingLog("logs using tmp.txt  "));
            Assert.IsFalse(parser.IsStartingLog("cmdlogs using tmp.txt  "));
            Assert.IsFalse(parser.IsStartingLog("cmd log using tmp.txt  "));
            Assert.IsTrue(parser.IsStartingLog("log using tmp.txt"));
            Assert.IsTrue(parser.IsStartingLog(" log   using   tmp.txt   "));
            Assert.IsTrue(parser.IsStartingLog("cmdlog using tmp.txt"));
            Assert.IsTrue(parser.IsStartingLog(" cmdlog   using   tmp.txt   "));
            Assert.IsTrue(parser.IsStartingLog("*comment line followed by command\r\ncmdlog   using   tmp.txt   "));
        }

        protected void ValidateFoundLogs(string[] expected, string[] received)
        {
            Assert.AreEqual(expected.Length, received.Length);
            foreach (var log in expected)
            {
                Assert.IsTrue(received.Contains(log));
            }
        }

        [TestMethod]
        public void GetLogType()
        {
            var parser = new StataParser();
            Assert.IsNull(parser.GetLogType("*log using tmp.txt"));
            Assert.IsNull(parser.GetLogType("*cmdlog using tmp.txt"));
            Assert.IsNull(parser.GetLogType("  *  log using tmp.txt  "));
            Assert.IsNull(parser.GetLogType("  *  cmdlog using tmp.txt  "));
            Assert.IsNull(parser.GetLogType("l og using tmp.txt  "));
            Assert.IsNull(parser.GetLogType("logs using tmp.txt  "));
            Assert.IsNull(parser.GetLogType("cmdlogs using tmp.txt  "));
            Assert.IsNull(parser.GetLogType("cmd log using tmp.txt  "));
            ValidateFoundLogs(new []{"log"}, parser.GetLogType("log using tmp.txt"));
            ValidateFoundLogs(new []{"log"}, parser.GetLogType(" log   using   tmp.txt   "));
            ValidateFoundLogs(new []{"cmdlog"}, parser.GetLogType("cmdlog using tmp.txt"));
            ValidateFoundLogs(new[] { "cmdlog" }, parser.GetLogType(" cmdlog   using   tmp.txt   "));
            ValidateFoundLogs(new[] { "log" }, parser.GetLogType("log   using   log using 2.txt   "));
            ValidateFoundLogs(new[] { "log", "cmdlog" }, parser.GetLogType("log using log.txt\r\ncmdlog using cmdlog.txt"));
            ValidateFoundLogs(new[] { "log", "cmdlog" }, parser.GetLogType("cmdlog using cmdlog.txt\r\nlog using log.txt"));
            ValidateFoundLogs(new[] { "log" }, parser.GetLogType("*cmdlog using cmdlog.txt\r\nlog using log.txt"));
            ValidateFoundLogs(new[] { "log", "log" }, parser.GetLogType("log using log.txt\r\nlog using log2.txt"));
        }

        [TestMethod]
        public void GetLogFile()
        {
            var parser = new StataParser();
            Assert.IsNull(parser.GetLogFile("*log using tmp.txt"));
            Assert.IsNull(parser.GetLogFile("*cmdlog using tmp.txt"));
            Assert.IsNull(parser.GetLogFile("  *  log using tmp.txt  "));
            Assert.IsNull(parser.GetLogFile("  *  cmdlog using tmp.txt  "));
            Assert.IsNull(parser.GetLogFile("l og using tmp.txt  "));
            Assert.IsNull(parser.GetLogFile("logs using tmp.txt  "));
            Assert.IsNull(parser.GetLogFile("cmdlogs using tmp.txt  "));
            Assert.IsNull(parser.GetLogFile("cmd log using tmp.txt  "));
            ValidateFoundLogs(new[] { "tmp.txt" }, parser.GetLogFile("log using tmp.txt"));
            ValidateFoundLogs(new[] { "tmp.txt" }, parser.GetLogFile(" log   using   tmp.txt   "));
            ValidateFoundLogs(new[] { "tmp.txt" }, parser.GetLogFile("cmdlog using tmp.txt"));
            ValidateFoundLogs(new[] { "tmp.txt" }, parser.GetLogFile(" cmdlog   using   tmp.txt   "));
            ValidateFoundLogs(new[] { "log using 2.txt" }, parser.GetLogFile("log   using   log using 2.txt   "));
            ValidateFoundLogs(new[] { "log.txt", "cmdlog.txt" }, parser.GetLogFile("log using log.txt\r\ncmdlog using cmdlog.txt"));
            ValidateFoundLogs(new[] { "log.txt", "cmdlog.txt" }, parser.GetLogFile("cmdlog using cmdlog.txt\r\nlog using log.txt"));
            ValidateFoundLogs(new[] { "log.txt" }, parser.GetLogFile("*cmdlog using cmdlog.txt\r\nlog using log.txt"));
            ValidateFoundLogs(new[] { "log.txt", "log2.txt" }, parser.GetLogFile("log using log.txt\r\nlog using log2.txt"));
        }

        [TestMethod]
        public void GetImageSaveLocation()
        {
            var parser = new StataParser();
            Assert.AreEqual("C:\\Development\\Stats\\bpgraph.pdf", parser.GetImageSaveLocation("graph export \"C:\\Development\\Stats\\bpgraph.pdf\", as(pdf) replace"));
            Assert.AreEqual("C:\\Development\\Stats\\bpgraph.pdf", parser.GetImageSaveLocation(" graph   export   \"C:\\Development\\Stats\\bpgraph.pdf\" ,  as(pdf)  replace"));
            Assert.AreEqual(string.Empty, parser.GetImageSaveLocation("agraph export \"C:\\Development\\Stats\\bpgraph.pdf\", as(pdf) replace"));
            Assert.AreEqual("mygraph.pdf", parser.GetImageSaveLocation("graph export mygraph.pdf"));
            Assert.AreEqual("mygraph.pdf", parser.GetImageSaveLocation("graph export mygraph.pdf, as(pdf)"));
            Assert.AreEqual("mygraph.pdf", parser.GetImageSaveLocation("gr export mygraph.pdf")); // "gr" shortcut
            Assert.AreNotEqual("mygraph.pdf", parser.GetImageSaveLocation("gra export mygraph.pdf"));
            Assert.AreEqual("C:\\Development\\Stats\\bpgraph.pdf", parser.GetImageSaveLocation("gr export \"C:\\Development\\Stats\\bpgraph.pdf\", as(pdf) replace"));
        }

        [TestMethod]
        public void GetValueName()
        {
            var parser = new StataParser();
            Assert.AreEqual("test", parser.GetValueName("display test"));
            Assert.AreEqual("`x2'", parser.GetValueName("display  `x2'"));
            Assert.AreEqual("$x2", parser.GetValueName("display  $x2"));
            Assert.AreEqual("test", parser.GetValueName(" display   test  "));
            Assert.AreEqual(string.Empty, parser.GetValueName("adisplay test"));
            Assert.AreEqual("test", parser.GetValueName("display (test)"));
            Assert.AreEqual("test", parser.GetValueName("display(test)"));
            Assert.AreEqual("r(n)", parser.GetValueName("display r(n)"));
            Assert.AreEqual("n*(3)", parser.GetValueName("display n*(3)"));
            Assert.AreEqual("r(n)", parser.GetValueName("display r(n)\r\n\r\n*Some comments following"));
            Assert.AreEqual("2", parser.GetValueName("display 2 \r\n \r\n*Some comments following"));
            Assert.AreEqual("5*2", parser.GetValueName("display (5*2)")); // Handle calculations as display parameters
            Assert.AreEqual("5*2+(7*8)", parser.GetValueName("display(5*2+(7*8))")); // Handle calculations with nested parentheses
            Assert.AreEqual("(5*2", parser.GetValueName("display (5*2")); // Mismatched parentheses.  We want to grab it, even though it'll be an error in Stata
            Assert.AreEqual("7   *    8   +   ( 5 * 7 )", parser.GetValueName("  display   (  7   *    8   +   ( 5 * 7 )  )   "));
            // Stata does not appear to support multiple commands on one line, even in a do file, so this shouldn't work.  We are just asserting that we don't
            // support this functionality.
            Assert.AreNotEqual("test", parser.GetValueName("display test; display test"));
        }

        [TestMethod]
        public void IsCalculatedDisplayValue()
        {
            var parser = new StataParser();
            Assert.IsFalse(parser.IsCalculatedDisplayValue(null));
            Assert.IsFalse(parser.IsCalculatedDisplayValue(""));
            Assert.IsFalse(parser.IsCalculatedDisplayValue("2*3"));
            Assert.IsTrue(parser.IsCalculatedDisplayValue("display (5*2)"));
            Assert.IsTrue(parser.IsCalculatedDisplayValue("display(5*2+(7*8))"));
            Assert.IsTrue(parser.IsCalculatedDisplayValue("display 5*2"));
            Assert.IsFalse(parser.IsCalculatedDisplayValue("display r[n]"));

            Assert.IsTrue(parser.IsCalculatedDisplayValue("display 5"));
            Assert.IsTrue(parser.IsCalculatedDisplayValue("display 00005"));
            Assert.IsTrue(parser.IsCalculatedDisplayValue("display 5."));
            Assert.IsTrue(parser.IsCalculatedDisplayValue("display 0.3059"));
            Assert.IsTrue(parser.IsCalculatedDisplayValue("display .3059"));
            Assert.IsTrue(parser.IsCalculatedDisplayValue("display 5e-10"));
            Assert.IsFalse(parser.IsCalculatedDisplayValue("display 5test"));
            Assert.IsFalse(parser.IsCalculatedDisplayValue("display 5,000"));
        }

        [TestMethod]
        public void GetMacroValueName()
        {
            var parser = new StataParser();
            Assert.AreEqual("x2", parser.GetMacroValueName("display  `x2'"));
            Assert.AreEqual("x2", parser.GetMacroValueName("display  $x2"));
            Assert.AreEqual("x2", parser.GetMacroValueName("display  `x2'\r\n\r\n*Some comments following"));
            Assert.AreEqual("test", parser.GetMacroValueName("display test"));   // This isn't a proper Stata macro value, but is the expected return
        }

        [TestMethod]
        public void GetTableName()
        {
            var parser = new StataParser();
            Assert.AreEqual("test_matrix", parser.GetTableName("matrix list test_matrix"));
            Assert.AreEqual("test_matrix", parser.GetTableName("   matrix   list    test_matrix  "));
            Assert.AreEqual("test  value", parser.GetTableName("   matrix   list    test  value  "));  // Not sure if this is valid for Stata, but it's what we should pull out
            Assert.AreEqual(string.Empty, parser.GetTableName("amatrix list test"));
            Assert.AreEqual("test", parser.GetTableName("mat list test"));
            Assert.AreEqual("test", parser.GetTableName("mat l test"));
            Assert.AreEqual("r(coefs)", parser.GetTableName("mat l r(coefs)"));
            Assert.AreEqual("test", parser.GetTableName("mat l test, format(%5.0g)"));
            Assert.AreEqual("r(coefs)", parser.GetTableName("matrix list r(coefs), format(%5.0g)"));
            Assert.AreEqual("r ( coefs )", parser.GetTableName("mat list r ( coefs ) "));
            Assert.AreEqual("B", parser.GetTableName("matrix list B\r\n\r\n*Some comments following"));
        }

        [TestMethod]
        public void GetTableDataPath()
        {
            var parser = new StataParser();

            // Check for file names
            Assert.AreEqual("example.csv", parser.GetTableDataPath("esttab using example.csv, replace wide plain"));
            Assert.AreEqual("example.csv", parser.GetTableDataPath("esttab using example.csv , replace wide plain"));
            Assert.AreEqual("example 2.csv", parser.GetTableDataPath("esttab using \"example 2.csv\", replace wide plain"));

            // Check for file paths
            Assert.AreEqual("C:\\example.csv", parser.GetTableDataPath("esttab using C:\\example.csv, replace wide plain"));
            Assert.AreEqual("C:\\data path\\example.csv", parser.GetTableDataPath("esttab using \"C:\\data path\\example.csv\", replace wide plain"));
            Assert.AreEqual("..\\example.csv", parser.GetTableDataPath("esttab using ..\\example.csv, replace wide plain"));
            Assert.AreEqual("C:/example.csv", parser.GetTableDataPath("esttab using C:/example.csv, replace wide plain"));

            // Commands with parentheses
            Assert.AreEqual("testing.csv", parser.GetTableDataPath("table1, vars(gender cat \\ race cat \\ ridageyr contn %4.2f \\ married cat \\ income cat \\ education cat \\ bmxht contn %4.2f \\ bmxwt conts \\ bmxbmi conts \\ bmxwaist contn %4.2f \\ lbdhdd contn %4.2f \\ lbdldl contn %4.2f \\ lbxtr conts \\ lbxglu conts \\ lbxin conts) saving(testing.csv, replace)"));
            Assert.AreEqual("testing.csv", parser.GetTableDataPath("table1, vars(gender cat \\ race cat \\ ridageyr contn %4.2f \\ married cat \\ income cat \\ education cat \\ bmxht contn %4.2f \\ bmxwt conts \\ bmxbmi conts \\ bmxwaist contn %4.2f \\ lbdhdd contn %4.2f \\ lbdldl contn %4.2f \\ lbxtr conts \\ lbxglu conts \\ lbxin conts) saving(  testing.csv , replace)"));
            Assert.AreEqual("testing 2.csv", parser.GetTableDataPath("table1, vars(gender cat \\ race cat \\ ridageyr contn %4.2f \\ married cat \\ income cat \\ education cat \\ bmxht contn %4.2f \\ bmxwt conts \\ bmxbmi conts \\ bmxwaist contn %4.2f \\ lbdhdd contn %4.2f \\ lbdldl contn %4.2f \\ lbxtr conts \\ lbxglu conts \\ lbxin conts) saving(\"testing 2.csv\", replace)"));

            // Check for macros
            Assert.AreEqual("`filename'", parser.GetTableDataPath("esttab using `filename', replace wide plain"));
            Assert.AreEqual("$filename", parser.GetTableDataPath("esttab using $filename, replace wide plain"));

            // Don't forget you can mix paths and macros
            Assert.AreEqual("C:\\`file'", parser.GetTableDataPath("esttab using C:\\`file', replace wide plain"));
            Assert.AreEqual("C:\\data path\\`file'", parser.GetTableDataPath("esttab using \"C:\\data path\\`file'\", replace wide plain"));
        }

        [TestMethod]
        public void IsTable1Command()
        {
            var parser = new StataParser();
            Assert.IsTrue(parser.IsTable1Command("table1, vars(gender cat \\ race cat \\ ridageyr contn %4.2f \\ married cat \\ income cat \\ education cat \\ bmxht contn %4.2f \\ bmxwt conts \\ bmxbmi conts \\ bmxwaist contn %4.2f \\ lbdhdd contn %4.2f \\ lbdldl contn %4.2f \\ lbxtr conts \\ lbxglu conts \\ lbxin conts) saving(table1.xls, replace)"));
            Assert.IsTrue(parser.IsTable1Command("table1 ,  vars(gender cat \\ race cat \\ ridageyr contn %4.2f \\ married cat \\ income cat \\ education cat \\ bmxht contn %4.2f \\ bmxwt conts \\ bmxbmi conts \\ bmxwaist contn %4.2f \\ lbdhdd contn %4.2f \\ lbdldl contn %4.2f \\ lbxtr conts \\ lbxglu conts \\ lbxin conts) saving(table1.xls, replace)"));
            Assert.IsFalse(parser.IsTable1Command("esttab using table1.csv, replace wide plain"));
            Assert.IsFalse(parser.IsTable1Command("esttab using $table1, replace wide plain"));
        }

        [TestMethod]
        public void PreProcessContent_Empty()
        {
            var parser = new StataParser();
            Assert.AreEqual(0, parser.PreProcessContent(null).Count);
            var emptyList = new List<string>();
            Assert.AreEqual(0, parser.PreProcessContent(emptyList).Count);
        }

        [TestMethod]
        public void PreProcessContent_TrailingComment()
        {
            var testList = new List<string>(new string[]
            {
                "First line",
                "Second line",
                "Third line"
            });

            var parser = new StataParser();
            Assert.AreEqual(3, parser.PreProcessContent(testList).Count);

            testList = new List<string>(new string[]
            {
                "First line",
                "Second line ///",
                "Third line"
            });
            Assert.AreEqual(2, parser.PreProcessContent(testList).Count);


            testList = new List<string>(new string[]
            {
                "First line ///",
                "Second line ///",
                "Third line ///"
            });
            Assert.AreEqual(1, parser.PreProcessContent(testList).Count);
        }

        [TestMethod]
        public void PreProcessContent_MultiLineComment()
        {
            var parser = new StataParser();
            var testList = new List<string>(new string[]
            {
                "First line",
                "Second line /*",
                "*/Third line"
            });
            Assert.AreEqual(2, parser.PreProcessContent(testList).Count);
            Assert.AreEqual("First line\r\nSecond line Third line", string.Join("\r\n", parser.PreProcessContent(testList)));


            testList = new List<string>(new string[]
            {
                "First line /*",
                "Second line ///",
                "Third line */"
            });
            Assert.AreEqual(1, parser.PreProcessContent(testList).Count);
            Assert.AreEqual("First line", string.Join("\r\n", parser.PreProcessContent(testList)));

            // This tests nested comments
            testList = new List<string>(new string[]
            {
                "First line /*",
                "Second line /*",
                "Third line */",
                "Fourth line */"
            });
            Assert.AreEqual(1, parser.PreProcessContent(testList).Count);
            Assert.AreEqual("First line", string.Join("\r\n", parser.PreProcessContent(testList)));

            // This was in response to an issue reported by a user.  The code file had multiple comments in it, and our regex
            // was being too greedy and pulling extra code out (until it found the last closing comment indicator)
            testList = new List<string>(new string[]
            {
                "/*First line*/",
                "Second line",
                "/*Third line*/",
                "Fourth line"
            });
            Assert.AreEqual(3, parser.PreProcessContent(testList).Count);
            Assert.AreEqual("Second line\r\n\r\nFourth line", string.Join("\r\n", parser.PreProcessContent(testList)));

            testList = new List<string>(new string[]
            {
                "/*First line*/",
                "/*Second line*/ /*More on the same line*/",
                "/*Third line*/",
                "Fourth line"
            });
            Assert.AreEqual(1, parser.PreProcessContent(testList).Count);
            Assert.AreEqual("Fourth line", string.Join("\r\n", parser.PreProcessContent(testList)));

            // This is to test unbalanced comments (missing whitespace near the end so it is treated like
            // an unending comment.
            testList = new List<string>(new string[]
            {
                "/*First line",
                "/*Second line*//*More on the same line*/",
                "/*Third line",
                "Fourth line*/*/"
            });
            Assert.AreEqual(4, parser.PreProcessContent(testList).Count);
            Assert.AreEqual("/*First line\r\n/*Second line*//*More on the same line*/\r\n/*Third line\r\nFourth line*/*/", string.Join("\r\n", parser.PreProcessContent(testList)));

            testList = new List<string>(new string[]
            {
                "/*First line",
                "/*Second line*/ /*More on the same line*/",
                "/*Third line",
                "Fourth line*/ */"
            });
            Assert.AreEqual(1, parser.PreProcessContent(testList).Count);
            Assert.AreEqual("", string.Join("\r\n", parser.PreProcessContent(testList)));

            testList = new List<string>(new string[]
            {
                "/**/First line"
            });
            Assert.AreEqual(1, parser.PreProcessContent(testList).Count);
            Assert.AreEqual("First line", string.Join("\r\n", parser.PreProcessContent(testList)));

            testList = new List<string>(new string[]
            {
                "First line/**/"
            });
            Assert.AreEqual(1, parser.PreProcessContent(testList).Count);
            Assert.AreEqual("First line", string.Join("\r\n", parser.PreProcessContent(testList)));
        }

        [TestMethod]
        public void PreProcessContent_TrailingLineComment()
        {
            var parser = new StataParser();
            var testList = new List<string>(new string[]
            {
                "First line  // comment",
                "Second line",
                "//Third line"
            });
            Assert.AreEqual(2, parser.PreProcessContent(testList).Count);
            Assert.AreEqual("First line  \r\nSecond line", string.Join("\r\n", parser.PreProcessContent(testList)));

            testList = new List<string>(new string[]
            {
                "First line //*Test*/",
                "Second line"
            });
            Assert.AreEqual(2, parser.PreProcessContent(testList).Count);
            Assert.AreEqual("First line \r\nSecond line", string.Join("\r\n", parser.PreProcessContent(testList)));

            testList = new List<string>(new string[]
            {
                "* //First line",
                "Second line"
            });
            Assert.AreEqual(2, parser.PreProcessContent(testList).Count);
            Assert.AreEqual("* \r\nSecond line", string.Join("\r\n", parser.PreProcessContent(testList)));

            // When we have trailing line comments within comment blocks, we want to make sure at the end of the day
            // that it's not causing any problems (it shouldn't be).
            testList = new List<string>(new string[]
            {
                "/*",
                "display 2 // Comment on second line",
                "*/"
            });
            Assert.AreEqual(1, parser.PreProcessContent(testList).Count);
            Assert.AreEqual("", string.Join("\r\n", parser.PreProcessContent(testList)));
        }

        [TestMethod]
        public void GetMacros()
        {
            var parser = new StataParser();
            Assert.AreEqual(0, parser.GetMacros(null).Length);
            Assert.AreEqual(0, parser.GetMacros(string.Empty).Length);
            Assert.AreEqual(0, parser.GetMacros("display x").Length);

            var result = parser.GetMacros("display `x'");
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("x", result.First());

            result = parser.GetMacros("display `x'\\`y'");
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("x", result[0]);
            Assert.AreEqual("y", result[1]);

            result = parser.GetMacros("display `x'\\$y");
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("x", result[0]);
            Assert.AreEqual("y", result[1]);
        }

        [TestMethod]
        public void IsSavedResultCommand()
        {
            var parser = new StataParser();
            Assert.IsTrue(parser.IsSavedResultCommand(" c(pwd) "));
            Assert.IsTrue(parser.IsSavedResultCommand("e(N)"));
            Assert.IsTrue(parser.IsSavedResultCommand("r(N)"));
            Assert.IsFalse(parser.IsSavedResultCommand("p(N)"));
            Assert.IsFalse(parser.IsSavedResultCommand("c ( N ) "));  // This is not valid in Stata because of the space between c and (
        }
    }
}
