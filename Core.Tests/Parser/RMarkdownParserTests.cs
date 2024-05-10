using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StatTag.Core.Exceptions;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using StatTag.Core.Parser;

namespace Core.Tests.Parser
{
    [TestClass]
    public class RMarkdownParserTests
    {
        [TestMethod]
        [ExpectedException(typeof(StatTagUserException))]
        public void PreProcessFile_NullAutomation()
        {
            var fileHandlerMock = new Mock<IFileHandler>();
            fileHandlerMock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
                {
                    "---",
                });
            fileHandlerMock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var codeFile = new CodeFile(fileHandlerMock.Object) { StatisticalPackage = Constants.StatisticalPackages.RMarkdown, FilePath = "Test.Rmd" };

            var parser = new RMarkdownParser();
            parser.PreProcessFile(codeFile, null);
        }

        [TestMethod]
        [ExpectedException(typeof(StatTagUserException))]
        public void PreProcessFile_NotRMD()
        {
            var fileHandlerMock = new Mock<IFileHandler>();
            fileHandlerMock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
                {
                    "---",
                });
            fileHandlerMock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var automationMock = new Mock<IStatAutomation>();

            var codeFile = new CodeFile(fileHandlerMock.Object) { StatisticalPackage = Constants.StatisticalPackages.RMarkdown, FilePath = "Test.R" };

            var parser = new RMarkdownParser();
            parser.PreProcessFile(codeFile, automationMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(StatTagUserException))]
        public void PreProcessFile_RFileExists()
        {
            var codeFileHandlerMock = new Mock<IFileHandler>();
            codeFileHandlerMock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
                {
                    "---",
                });
            codeFileHandlerMock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var parserFileHandlerMock = new Mock<IFileHandler>();
            parserFileHandlerMock.Setup(file => file.Exists(It.IsRegex("Test\\.R"))).Returns(true);  // Trigger an error condition by making it think the R file exists

            var automationMock = new Mock<IStatAutomation>();

            var codeFile = new CodeFile(codeFileHandlerMock.Object) { StatisticalPackage = Constants.StatisticalPackages.RMarkdown, FilePath = "Test.Rmd" };

            var parser = new RMarkdownParser(parserFileHandlerMock.Object);
            parser.PreProcessFile(codeFile, automationMock.Object);
        }

        [TestMethod]
        public void PreProcessFile_Processed()
        {
            // This test is not entirely perfect.  We do a lot of mocking, but are verifying the path through
            // PreProcessFile that it will return appropriately when done.  We don't even have any assertions
            // at the end, and instead are expecting this to finish without throwing exceptions as done in the
            // earlier tests.
            var codeFileHandlerMock = new Mock<IFileHandler>();
            codeFileHandlerMock.Setup(file => file.ReadAllLines(It.IsAny<string>())).Returns(new[]
                {
                    "---",
                    "title: \"Test\"",
                    "author: \"Test\"",
                    "date: \"November 28, 2018\"",
                    "output: html_document",
                    "---",
                    "",
                    "```{r cars}",
                    "##>>>ST:Table(Label=\"Summary\", Frequency=\"On Demand\", Type=\"Default\")",
                    "summary(cars)",
                    "##<<<",
                    "```"
                });
            codeFileHandlerMock.Setup(file => file.Exists(It.IsAny<string>())).Returns(true);

            var parserFileHandlerMock = new Mock<IFileHandler>();
            parserFileHandlerMock.Setup(file => file.Exists(It.IsRegex("Test\\.R"))).Returns(false);  // We don't want the R file to exist

            var automationMock = new Mock<IStatAutomation>();
            automationMock.Setup(aut => aut.RunCommands(It.IsAny<string[]>(), It.IsAny<Tag>())).Returns(new CommandResult[] { });

            var codeFile = new CodeFile(codeFileHandlerMock.Object) { StatisticalPackage = Constants.StatisticalPackages.RMarkdown, FilePath = "Test.Rmd" };

            var parser = new RMarkdownParser(parserFileHandlerMock.Object);
            parser.PreProcessFile(codeFile, automationMock.Object);

            // Why no assertions?  We could go through all of the hassle of mocking up the results, but since it's all
            // mocked up data, what's the point?
        }

        [TestMethod]
        public void ReplaceKnitrCommands_NullEmpty()
        {
            var parser = new RMarkdownParser();
            Assert.IsNull(parser.ReplaceKnitrCommands(null));
            Assert.AreEqual(0, parser.ReplaceKnitrCommands(new List<string>()).Count);
        }

        [TestMethod]
        public void ReplaceKnitrCommands_NoKnitrCommands()
        {
            var parser = new RMarkdownParser();
            Assert.IsNull(parser.ReplaceKnitrCommands(null));
            Assert.AreEqual("x <- 15\r\nprint(x)", string.Join("\r\n", parser.ReplaceKnitrCommands(new List<string>(){ "x <- 15", "print(x)" })));
            Assert.AreEqual("x <- 15\r\ntable(x)", string.Join("\r\n", parser.ReplaceKnitrCommands(new List<string>() { "x <- 15", "table(x)" })));
        }

        [TestMethod]
        public void ReplaceKnitrCommands_KnitrCommands()
        {
            var parser = new RMarkdownParser();
            Assert.IsNull(parser.ReplaceKnitrCommands(null));
            Assert.AreEqual("PrettyTable <- print(TableOne, printToggle = FALSE, noSpaces = TRUE)\r\n(PrettyTable)", string.Join("\r\n", parser.ReplaceKnitrCommands(new List<string>() { "PrettyTable <- print(TableOne, printToggle = FALSE, noSpaces = TRUE)", "knitr::kable(PrettyTable)" })));
            Assert.AreEqual("PrettyTable <- print(TableOne, printToggle = FALSE, noSpaces = TRUE)\r\n(PrettyTable)", string.Join("\r\n", parser.ReplaceKnitrCommands(new List<string>() { "PrettyTable <- print(TableOne, printToggle = FALSE, noSpaces = TRUE)", "kable(PrettyTable)" })));
        }
    }
}
