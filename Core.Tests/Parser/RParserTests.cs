using System;
using System.Collections.Generic;
using System.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTag.Core.Models;
using StatTag.Core.Parser;

namespace Core.Tests.Parser
{
    [TestClass]
    public class RParserTests
    {
        [TestMethod]
        public void IsValueDisplay()
        {
            // We consider anything to be a candidate value, given how R works, so this is just to
            // assert it should always return true.
            var parser = new RParser();
            Assert.IsTrue(parser.IsValueDisplay("Anything can go here"));
        }

        [TestMethod]
        public void GetValueName()
        {
            // We are unrestrictive on what can be considered a value, so we don't ever return a
            // value name.  This just asserts it's always an empty string returned.
            var parser = new RParser();
            Assert.AreEqual(string.Empty, parser.GetValueName("Anything can go here"));
        }

        [TestMethod]
        public void IsImageExport()
        {
            var parser = new RParser();
            Assert.IsTrue(parser.IsImageExport("png('test.png')"));
            Assert.IsTrue(parser.IsImageExport("png(\"test.png\")"));
            Assert.IsFalse(parser.IsImageExport("png(\"test.png\""));
            Assert.IsTrue(parser.IsImageExport(" png ( \"test.png\" ) "));
            Assert.IsFalse(parser.IsImageExport("spng(\"test.png\""));
            Assert.IsFalse(parser.IsImageExport("pngs('test.png')"));
            Assert.IsFalse(parser.IsImageExport("pn('test.png')"));
            Assert.IsTrue(parser.IsImageExport("png\r\n(\r\n\"test.png\"\r\n)\r\n"));
            Assert.IsTrue(parser.IsImageExport("png('test, file.png')"));

            Assert.IsTrue(parser.IsImageExport("pdf('test.pdf')"));
            Assert.IsTrue(parser.IsImageExport("win.metafile('test.wmf')"));
            Assert.IsTrue(parser.IsImageExport("jpeg('test.jpg')"));
            Assert.IsTrue(parser.IsImageExport("png('test.png')"));
            Assert.IsTrue(parser.IsImageExport("bmp('test.bmp')"));
            Assert.IsTrue(parser.IsImageExport("postscript('test.ps')"));
        }

        [TestMethod]
        public void GetImageSaveLocation()
        {
            var parser = new RParser();
            Assert.AreEqual("\"test.pdf\"", parser.GetImageSaveLocation("pdf(\"test.pdf\")"));
            Assert.AreEqual("\"test.pdf\"", parser.GetImageSaveLocation("pdf(\"test.pdf\",100,100)"));
            Assert.AreEqual("\"test.png\"", parser.GetImageSaveLocation("png(width = 100, height=100, \r\n\tfile=\r\n\t\t\"test.png\")"));
            Assert.AreEqual("\"test.png\"", parser.GetImageSaveLocation("png(\"test.png\", width=100,height=100)"));
            Assert.AreEqual("\"test.png\"", parser.GetImageSaveLocation("png(width=100, \"test.png\", height=100)")); // First unnamed parameter is file
            Assert.AreEqual("\"test.png\"", parser.GetImageSaveLocation("png (width=100,height=100,fi=\r\n\t\"test.png\")"));
            Assert.AreEqual("\"test.png\"", parser.GetImageSaveLocation("png (width=100,height=100,FILE=\r\n\t\"test.png\")")); // Check capitalization is ignored
            Assert.AreEqual("\"test.png\"", parser.GetImageSaveLocation("png(width=100,f=\"test.png\",height=100)"));
            Assert.AreEqual("'test.png'", parser.GetImageSaveLocation("png(width=100,file='test.png',height=100)"));
            Assert.AreEqual("\"C:\\\\Test\\\\Path with spaces\\\\test.pdf\"", parser.GetImageSaveLocation("pdf(\"C:\\\\Test\\\\Path with spaces\\\\test.pdf\")"));
            Assert.AreEqual("\"C:\\Test\\Path's\\test.pdf\"", parser.GetImageSaveLocation("pdf(\"C:\\Test\\Path's\\test.pdf\")"));
            Assert.AreEqual(string.Empty, parser.GetImageSaveLocation("png(width=100, height=100)")); // Here there is no unnamed parameter or file parameter (this would be an error in R)
            Assert.AreEqual(string.Empty, parser.GetImageSaveLocation("spng(width=100,'test.png',height=100)"));
            Assert.AreEqual("\"test, file.png\"", parser.GetImageSaveLocation("png(width = 100, height=100, filename=\"test, file.png\")"));

            // Allow paste command to be used for file name parameter.  Make sure nested functions are processed correctly too.
            Assert.AreEqual("paste(\"test\", \".pdf\")", parser.GetImageSaveLocation("pdf(file=paste(\"test\", \".pdf\"))"));
            Assert.AreEqual("paste(\"test\", paste(\".\", \"pdf\"))", parser.GetImageSaveLocation("pdf(file=paste(\"test\", paste(\".\", \"pdf\")))"));
            Assert.AreEqual("paste(\"test\", \".pdf\")", parser.GetImageSaveLocation("pdf(paste(\"test\", \".pdf\"))"));
            Assert.AreEqual("paste(\"test\", paste(\".\", \"pdf\"))", parser.GetImageSaveLocation("pdf(paste(\"test\", paste(\".\", \"pdf\")))"));
            // This checks to make sure we ignore named parameters within a function call.  When we are looking for named parameters of the pdf call, the named parameters
            // in the inner path call should be ignored as named parameters.
            Assert.AreEqual("paste(Path,\"RExampleFigure.pdf\",sep=\"\")", parser.GetImageSaveLocation("pdf(paste(Path,\"RExampleFigure.pdf\",sep=\"\"))"));

            // Variable names should be allowed for file name parameter too
            Assert.AreEqual("file_path", parser.GetImageSaveLocation("pdf(file=file_path)"));

            // Some duplication, but verifies each file type works
            Assert.AreEqual("\"test.pdf\"", parser.GetImageSaveLocation("pdf(\"test.pdf\")"));
            Assert.AreEqual("\"test.wmf\"", parser.GetImageSaveLocation("win.metafile(\"test.wmf\")"));
            Assert.AreEqual("\"test.jpeg\"", parser.GetImageSaveLocation("jpeg(\"test.jpeg\")"));
            Assert.AreEqual("\"test.png\"", parser.GetImageSaveLocation("png(\"test.png\")"));
            Assert.AreEqual("\"test.bmp\"", parser.GetImageSaveLocation("bmp(\"test.bmp\")"));
            Assert.AreEqual("\"test.ps\"", parser.GetImageSaveLocation("postscript(\"test.ps\")"));

            // If we have two image commands in the same text block, we will get the first one
            Assert.AreEqual("\"test.pdf\"", parser.GetImageSaveLocation("pdf(\"test.pdf\");png(\"test.png\")"));

            // Closing parenthesis in string should be treated as part of the parameter string
            Assert.AreEqual("\"test).pdf\"", parser.GetImageSaveLocation("pdf(\"test).pdf\")"));
            Assert.AreEqual("'test).pdf'", parser.GetImageSaveLocation("pdf('test).pdf')"));

            // If we have a larger block of commands, make sure we are only pulling the correct boundaries of the relevant function
            Assert.AreEqual("paste(\"test\", \".pdf\")", parser.GetImageSaveLocation("pdf(file=paste(\"test\", \".pdf\"),\r\nwidth = getw(),\r\nheight = geth())\r\n\r\ntest()"));
            Assert.AreEqual("\"test.jpg\"", parser.GetImageSaveLocation("jpeg(\"test.jpg\")\r\np <- ggplot(data, aes(x=value)) + geom_histogram()\r\np\r\ndev.off()"));
        }

        [TestMethod]
        public void IsTableResult()
        {
            // Right now we assume anything could be a table, so IsTableResult will always return true
            var parser = new RParser();
            Assert.IsTrue(parser.IsTableResult("doesn't matter what i put here"));
        }

        [TestMethod]
        public void GetTableName()
        {
            // Regardless of what's sent in, GetTableName always returns null
            var parser = new RParser();
            Assert.IsNull(parser.GetTableName("write.table(x, file=\"test.txt\", append=FALSE, sep = \",\")"));
        }

        [TestMethod]
        public void GetTableDataPath()
        {
            // Although we will always allow something to be considered a table, this will test if we can
            // pull a file name out of call to write output.
            var parser = new RParser();
            Assert.AreEqual("\"test.txt\"", parser.GetTableDataPath("write.table(x, file=\"test.txt\", append=FALSE, sep = \",\")"));
            Assert.AreEqual("\"test.csv\"", parser.GetTableDataPath("write.csv(x, file=\"test.csv\", append=FALSE, sep = \",\")"));
            Assert.AreEqual("\"test2.csv\"", parser.GetTableDataPath("write.csv2(x, file=\"test2.csv\", append=FALSE, sep = \",\")"));

            // Detect positional placement of file parameter
            Assert.AreEqual("\"test.txt\"", parser.GetTableDataPath("write.table(x, \"test.txt\")"));

            // Mix order of parameters
            Assert.AreEqual("\"test2.csv\"", parser.GetTableDataPath("write.csv2(x, append=FALSE, file=\"test2.csv\", sep = \",\")"));

            // Don't return anything for assignment/other operations that end up creating a table-compatible object
            Assert.AreEqual("", parser.GetTableDataPath("x <- c(1,2,3,4,5)"));

            // Handle variables used as file paths
            Assert.AreEqual("out_file", parser.GetTableDataPath("write.table(x, out_file)"));
            Assert.AreEqual("out_file", parser.GetTableDataPath("write.table(x, file=out_file)"));

            // Handle functions used as file paths
            Assert.AreEqual("paste(getwd(), \"test.txt\", sep = \"\")", parser.GetTableDataPath("write.table(x, file = paste(getwd(), \"test.txt\", sep = \"\"))"));

            // Handle single quotes in file paths
            Assert.AreEqual("\"C:/Test/Stats/Test's/test.csv\"", parser.GetTableDataPath("write.csv(df, \"C:/Test/Stats/Test's/test.csv\")"));
        }

        [TestMethod]
        public void GetTableDataPath_MultiLine()
        {
            // Although we will always allow something to be considered a table, this will test if we can
            // pull a file name out of call to write output.
            var parser = new RParser();
            Assert.AreEqual("\"test.txt\"", parser.GetTableDataPath("y <- 5\r\nwrite.table(x, file=\"test.txt\", append=FALSE, sep = \",\")"));

            // Detect positional placement of file parameter
            Assert.AreEqual("\"test.txt\"", parser.GetTableDataPath("y <- 5\r\nprint(y)\r\nwrite.table(x, \"test.txt\")"));

            // Mix order of parameters
            Assert.AreEqual("\"test2.csv\"", parser.GetTableDataPath("y <- 5\r\nwrite.csv2(x, append=FALSE, file=\"test2.csv\", sep = \",\")\r\nprint(y)"));

            // Don't return anything for assignment/other operations that end up creating a table-compatible object
            Assert.AreEqual("", parser.GetTableDataPath("x <- c(1,2,3,4,5)"));

            // Handle variables used as file paths
            Assert.AreEqual("out_file", parser.GetTableDataPath("out_file <- \"test.csv\"\r\nwrite.table(x, out_file)"));
            Assert.AreEqual("out_file", parser.GetTableDataPath("out_file <- \"test.csv\"\r\nwrite.table(x, file=out_file)"));

            // Handle functions used as file paths
            Assert.AreEqual("paste(getwd(), \"test.txt\", sep = \"\")", parser.GetTableDataPath("y <- 5\r\nwrite.table(x, file = paste(getwd(), \"test.txt\", sep = \"\"))"));

            // Handle single quotes in file paths
            Assert.AreEqual("\"C:/Test/Stats/Test's/test.csv\"", parser.GetTableDataPath("y <- 5\r\nwrite.csv(df, \"C:/Test/Stats/Test's/test.csv\")"));
        }

        [TestMethod]
        public void CollapseMultiLineCommands()
        {
            var parser = new RParser();

            // No commands to collapse
            var text = new string[]
            {
                "line 1",
                "line 2",
                "line 3"
            };

            var modifiedText = parser.CollapseMultiLineCommands(text);
            Assert.AreEqual(3, modifiedText.Length);


            // Simple multiline and single line combined
            text = new string[]
            {
                "cmd(",
                "  param",
                ")",
                "cmd2()"
            };
            modifiedText = parser.CollapseMultiLineCommands(text);
            Assert.AreEqual(2, modifiedText.Length);
            Assert.AreEqual("cmd(    param  )", modifiedText[0]);
            Assert.AreEqual("cmd2()", modifiedText[1]);

            // Nested multiline
            text = new string[]
            {
                "cmd(",
                "\tparam(tmp(), (",
                ")))"
            };
            modifiedText = parser.CollapseMultiLineCommands(text);
            Assert.AreEqual(1, modifiedText.Length);
            Assert.AreEqual("cmd(  \tparam(tmp(), (  )))", modifiedText[0]);

            // Nested and unbalanced (not enough closing parens)
            text = new string[]
            {
                "cmd(",
                "\tparam(tmp(), (",
                "))"
            };
            modifiedText = parser.CollapseMultiLineCommands(text);
            Assert.AreEqual(3, modifiedText.Length);

            // Parens at the bounds
            text = new string[]
            {
                "()"
            };
            modifiedText = parser.CollapseMultiLineCommands(text);
            Assert.AreEqual(1, modifiedText.Length);
            Assert.AreEqual(text[0], modifiedText[0]);

            // Unbalanced (not enough opening parens)
            text = new string[]
            {
                "cmd(",
                "))"
            };
            modifiedText = parser.CollapseMultiLineCommands(text);
            Assert.AreEqual(1, modifiedText.Length);
            Assert.AreEqual("cmd(  ))", modifiedText[0]);
        }

        [TestMethod]
        public void CollapseMultiLineCommands_PlotCommands()
        {
            var parser = new RParser();

            // Multiple ggplot commands strung together across multiple lines, with varying
            // whitespace between the segments
            var text = new string[]
            {
                "Plot_DistSpeed <- ggplot(data=cars, aes(x=dist, y=speed)) +",
                " \t geom_boxplot() +",
                "geom_point()+",
                "ggtitle(\"Dist x Speed\") + \t ",
                "theme(plot.title = element_text(hjust = 0.5))"
            };

            var modifiedText = parser.CollapseMultiLineCommands(text);
            Assert.AreEqual(1, modifiedText.Length);
            Assert.AreEqual("Plot_DistSpeed <- ggplot(data=cars, aes(x=dist, y=speed)) + geom_boxplot() + geom_point() + ggtitle(\"Dist x Speed\") + theme(plot.title = element_text(hjust = 0.5))",
                modifiedText[0]);

            // This mixes multi-line commands as well as strung together commands
            text = new string[]
            {
                "Plot_DistSpeed <- ggplot(",
                "  data=cars, aes(x=dist, y=speed)) +",
                " \t geom_boxplot() +",
                "geom_point()+",
                "ggtitle(\"Dist x Speed\") + \t ",
                "theme(plot.title = element_text(hjust = 0.5))"
            };

            modifiedText = parser.CollapseMultiLineCommands(text);
            Assert.AreEqual(1, modifiedText.Length);
            Assert.AreEqual("Plot_DistSpeed <- ggplot(    data=cars, aes(x=dist, y=speed)) + geom_boxplot() + geom_point() + ggtitle(\"Dist x Speed\") + theme(plot.title = element_text(hjust = 0.5))",
                modifiedText[0]);

            // This isn't valid R, but makes sure we're not collapsing general addition
            text = new string[]
            {
                "1 + ",
                "2 + ",
                "3+4+5+",
                "\t6"
            };

            modifiedText = parser.CollapseMultiLineCommands(text);
            Assert.AreEqual(4, modifiedText.Length);
            Assert.AreEqual("1 + \r\n2 + \r\n3+4+5+\r\n\t6",
                string.Join("\r\n", modifiedText));
        }

        [TestMethod]
        public void CollapseMultiLineCommands_PipeCommands()
        {
            var parser = new RParser();

            // Multiple piped commands strung together across multiple lines, with varying
            // whitespace between the segments
            var text = new string[]
            {
                "iris %>% ",
                " \t group_by(Species) %>%   \t",
                "summarize_if(is.numeric, mean) %>%",
                " ungroup()"
            };

            var modifiedText = parser.CollapseMultiLineCommands(text);
            Assert.AreEqual(1, modifiedText.Length);
            Assert.AreEqual(
                "iris %>% group_by(Species) %>% summarize_if(is.numeric, mean) %>% ungroup()",
                modifiedText[0]);

            // Handle a mix of pipe types.  We don't care if this is valid R code or not
            text = new string[]
            {
                "iris %<>%",
                "subset(Sepal.Length > mean(Sepal.Length)) %$%",
                "",
                " \t cor(Sepal.Length, Sepal.Width) %>% ",
                "  matrix(ncol = 2)   %T>% \t  ",
                "\tarrange(value) %>%",
                " arrange(value)"
            };

            modifiedText = parser.CollapseMultiLineCommands(text);
            Assert.AreEqual(1, modifiedText.Length);
            Assert.AreEqual(
                "iris %<>% subset(Sepal.Length > mean(Sepal.Length)) %$% cor(Sepal.Length, Sepal.Width) %>% matrix(ncol = 2)   %T>% arrange(value) %>% arrange(value)",
                modifiedText[0]);
        }

        [TestMethod]
        public void PreProcessContent_Null_Empty()
        {
            var parser = new RParser();
            Assert.AreEqual(0, parser.PreProcessContent(null).Count);
            var testList = new List<string>();
            Assert.AreEqual(0, parser.PreProcessContent(testList).Count);
        }

        [TestMethod]
        public void PreProcessContent_CommentsPreserved()
        {
            var parser = new RParser();
            var text = new List<string>()
            {
                "line 1 # comment",
                "line 2  ",
                "line 3  "
            };

            // The purpose of this test is to document that in September 2019 we moved the processing of
            // comments from PreProcessContent to PreProcessExecutionStepCode.  Our expected behavior
            // then is to see comments preserved, and this test ensures that holds true.
            var modifiedText = parser.PreProcessContent(text);
            Assert.AreEqual(3, modifiedText.Count);
            Assert.AreEqual("line 1 # comment", modifiedText[0]);
            Assert.AreEqual("line 2", modifiedText[1]);
            Assert.AreEqual("line 3", modifiedText[2]);
        }

        [TestMethod]
        public void PreProcessExecutionStepCode_Null()
        {
            var parser = new RParser();
            Assert.IsNull(parser.PreProcessExecutionStepCode(null));
            var executionStep = new ExecutionStep() {Code = null};
            Assert.IsNull(parser.PreProcessExecutionStepCode(executionStep));
        }

        [TestMethod]
        public void PreProcessExecutionStepCode_NoTag()
        {
            // Note that for all of these tests we are testing not only the removal of comments, but
            // also because there is no tag we will collapse multiple code lines into a single line.
            // We re-split some of the results into multiple lines just because it makes our assertions
            // easier to write, read, and maintain.
            var parser = new RParser();

            var executionStep = new ExecutionStep()
            {
                Code = new List<string>()
                {
                    "line 1",
                    "line 2",
                    "line 3"
                },
                Tag = null
            };
            // No comments to remove
            var modifiedText = parser.PreProcessExecutionStepCode(executionStep);
            Assert.AreEqual(1, modifiedText.Length);
            Assert.AreEqual("line 1\r\nline 2\r\nline 3", modifiedText[0]);


            executionStep = new ExecutionStep()
            {
                Code = new List<string>()
                {
                    "line 1 # comment",
                    "line 2",
                    "line 3"
                },
                Tag = null
            };
            modifiedText = parser.PreProcessExecutionStepCode(executionStep);
            Assert.AreEqual(1, modifiedText.Length);
            Assert.AreEqual("line 1\r\nline 2\r\nline 3", modifiedText[0]);


            // This may seem odd, so let me explain - we don't have a tag here.  Because the PreProcess code only applies
            // the "keep StatTag comments" logic if a tag is set, we EXPECT it to strip out our special comments.  In
            // general practice we don't anticipate this happening, but need to explicitly test that this is the expected
            // behavior.
            executionStep = new ExecutionStep()
            {
                Code = new List<string>()
                {
                    "##>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                    "line 1 # comment",
                    "line 2",
                    "line 3",
                    "##<<<"
                },
                Tag = null  // NOTE: the tag is null, so ALL comments should be stripped (even our tags)
            };
            modifiedText = parser.PreProcessExecutionStepCode(executionStep);
            Assert.AreEqual(1, modifiedText.Length);
            Assert.AreEqual("\r\nline 1\r\nline 2\r\nline 3\r\n", modifiedText[0]);


            executionStep = new ExecutionStep()
            {
                Code = new List<string>()
                {
                    "line 1 # comment",
                    "hours <- read.csv(file = \"//path/to/data.csv\",header=TRUE, na=\"\") # comment 2",
                    "line 3 # comment 3"
                },
                Tag = null
            };
            modifiedText = parser.PreProcessExecutionStepCode(executionStep);
            Assert.AreEqual(1, modifiedText.Length);
            var splitText = modifiedText[0].Split(new string[] { "\r\n" }, StringSplitOptions.None);
            Assert.AreEqual(3, splitText.Length);
            Assert.AreEqual("line 1", splitText[0]);
            Assert.AreEqual("hours <- read.csv(file = \"//path/to/data.csv\",header=TRUE, na=\"\")", splitText[1]);
            Assert.AreEqual("line 3", splitText[2]);

            // Account for comments on individual lines
            executionStep = new ExecutionStep()
            {
                Code = new List<string>()
                {
                    "cmd(",
                    " # Test",
                    "  param",
                    " # Test ",
                    ")"
                },
                Tag = null
            };
            modifiedText = parser.PreProcessExecutionStepCode(executionStep);
            Assert.AreEqual(1, modifiedText.Length);
            splitText = modifiedText[0].Split(new string[] { "\r\n" }, StringSplitOptions.None);
            Assert.AreEqual(5, splitText.Length);
            Assert.AreEqual("cmd(", splitText[0]);
            Assert.AreEqual("", splitText[1]);
            Assert.AreEqual("param", splitText[2]);
            Assert.AreEqual("", splitText[3]);
            Assert.AreEqual(")", splitText[4]);

            // Ensure the comment char is included when it's included in a string (it's not then a comment),
            // and also ensure we handle escaped characters appropriately.
            executionStep = new ExecutionStep()
            {
                Code = new List<string>()
                {
                    "cmd(",
                    " \"#1\",",
                    " param,",
                    " '#2' , ",
                    "#3 'is a comment # '",
                    " 'param 3', # \\\"Escapes are fun\"\\",
                    " \"More fun with \\\"ESCAPED\\\" characters in 'strings'\",",
                    ")"
                },
                Tag = null
            };
            modifiedText = parser.PreProcessExecutionStepCode(executionStep);
            Assert.AreEqual(1, modifiedText.Length);
            splitText = modifiedText[0].Split(new string[] { "\r\n" }, StringSplitOptions.None);
            Assert.AreEqual(8, splitText.Length);
            Assert.AreEqual("cmd(", splitText[0]);
            Assert.AreEqual("\"#1\",", splitText[1]);
            Assert.AreEqual("param,", splitText[2]);
            Assert.AreEqual("'#2' ,", splitText[3]);
            Assert.AreEqual("", splitText[4]);
            Assert.AreEqual("'param 3',", splitText[5]);
            Assert.AreEqual("\"More fun with \\\"ESCAPED\\\" characters in 'strings'\",", splitText[6]);
            Assert.AreEqual(")", splitText[7]);
        }

        [TestMethod]
        public void PreProcessExecutionStepCode_Tag()
        {
            var parser = new RParser();

            var executionStep = new ExecutionStep()
            {
                Code = new List<string>()
                {
                    "##>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                    "line 1 # comment",
                    "line 2",
                    "line 3",
                    "##<<<"
                },
                Tag = new Tag()
            };
            var modifiedText = parser.PreProcessExecutionStepCode(executionStep);
            Assert.AreEqual(5, modifiedText.Length);
            Assert.AreEqual("##>>>ST:Value(Label=\"Test\", Type=\"Default\")", modifiedText[0]);
            Assert.AreEqual("line 1", modifiedText[1]);
            Assert.AreEqual("line 2", modifiedText[2]);
            Assert.AreEqual("line 3", modifiedText[3]);
            Assert.AreEqual("##<<<", modifiedText[4]);

            executionStep = new ExecutionStep()
            {
                Code = new List<string>()
                {
                    "##>>>ST:Value(Label=\"Test\", Type=\"Default\")",
                    "line 1 # ##>>>ST:Value(Label=\"Test\", Type=\"Default\")",   // This should be stripped as a comment, but not considered a valid starting tag
                    "line 2",
                    "line 3 # ##<<<",  // Similarly, this should be stripped but not considered a valid ending tag
                    "##<<<"
                },
                Tag = new Tag()
            };
            modifiedText = parser.PreProcessExecutionStepCode(executionStep);
            Assert.AreEqual(5, modifiedText.Length);
            Assert.AreEqual("##>>>ST:Value(Label=\"Test\", Type=\"Default\")", modifiedText[0]);
            Assert.AreEqual("line 1", modifiedText[1]);
            Assert.AreEqual("line 2", modifiedText[2]);
            Assert.AreEqual("line 3", modifiedText[3]);
            Assert.AreEqual("##<<<", modifiedText[4]);
        }


        [TestMethod]
        public void CommandContainsPrint_EmptyNull()
        {
            var parser = new RParser();
            Assert.IsFalse(parser.CommandContainsPrint(null));
            Assert.IsFalse(parser.CommandContainsPrint(""));
            Assert.IsFalse(parser.CommandContainsPrint("     "));
        }

        [TestMethod]
        public void CommandContainsPrint_ExistsSingleLine()
        {
            var parser = new RParser();
            Assert.IsTrue(parser.CommandContainsPrint("print(x)"));
            Assert.IsTrue(parser.CommandContainsPrint("  print(  x  )  "));
            Assert.IsTrue(parser.CommandContainsPrint("print()"));
            Assert.IsTrue(parser.CommandContainsPrint(" print ( ) "));
        }

        [TestMethod]
        public void CommandContainsPrint_FalsePositiveKeyword()
        {
            var parser = new RParser();
            Assert.IsFalse(parser.CommandContainsPrint("isprint(x)"));
            Assert.IsFalse(parser.CommandContainsPrint("prints()"));
            Assert.IsFalse(parser.CommandContainsPrint("pr int()"));
        }

        [TestMethod]
        public void CommandContainsPrint_NotBeginningOfCommand()
        {
            // It doesn't matter if these are valid R commands or not, just testing difference scenarios
            // that the method should handle.
            var parser = new RParser();
            Assert.IsFalse(parser.CommandContainsPrint("x <- print(y)"));
            Assert.IsFalse(parser.CommandContainsPrint("myfn(print(x))"));
        }

        [TestMethod]
        public void CommandContainsPrint_MultipleLines()
        {
            var parser = new RParser();
            Assert.IsTrue(parser.CommandContainsPrint("\r\nprint(x)\r\n"));
            Assert.IsTrue(parser.CommandContainsPrint("x <- 5\r\nprint(\r\n  x\r\n)\r\nx"));
            Assert.IsTrue(parser.CommandContainsPrint("x <- 5\r\nprint  (\r\n  x\r\n)\r\ny <- 12\r\nprint ( y )\r\n y + x"));
            Assert.IsTrue(parser.CommandContainsPrint("x <- 5\r\n  print(x)"));
        }
    }
}
