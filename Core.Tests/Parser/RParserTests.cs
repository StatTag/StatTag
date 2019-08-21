using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void PreProcessContent_Null_Empty()
        {
            var parser = new RParser();
            Assert.AreEqual(0, parser.PreProcessContent(null).Count);
            var testList = new List<string>();
            Assert.AreEqual(0, parser.PreProcessContent(testList).Count);
        }

        [TestMethod]
        public void PreProcessContent()
        {
            var parser = new RParser();

            // No comments to remove
            var text = new List<string>()
            {
                "line 1",
                "line 2",
                "line 3"
            };
            var modifiedText = parser.PreProcessContent(text);
            Assert.AreEqual(text.Count, modifiedText.Count);
            for (int index = 0; index < text.Count; index++)
            {
                Assert.AreEqual(text[index], modifiedText[index]);   
            }


            text = new List<string>()
            {
                "line 1 // comment",
                "line 2  ",
                "line 3  "
            };
            modifiedText = parser.PreProcessContent(text);
            Assert.AreEqual(text.Count, modifiedText.Count);
            Assert.AreEqual("line 1", modifiedText[0]);
            Assert.AreEqual("line 2", modifiedText[1]);
            Assert.AreEqual("line 3", modifiedText[2]);

            text = new List<string>()
            {
                "line 1 // comment",
                "hours <- read.csv(file = \"//path/to/data.csv\",header=TRUE, na=\"\") // comment 2",
                "line 3 // comment 3"
            };
            modifiedText = parser.PreProcessContent(text);
            Assert.AreEqual(text.Count, modifiedText.Count);
            Assert.AreEqual("line 1", modifiedText[0]);
            Assert.AreEqual("hours <- read.csv(file = \"//path/to/data.csv\",header=TRUE, na=\"\")", modifiedText[1]);
            Assert.AreEqual("line 3", modifiedText[2]);
        }
    }
}
