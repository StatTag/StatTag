using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StatTag.Core.Exceptions;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using StatTag.Core.Utility;

namespace StatTag.Core.Parser
{
    public class RMarkdownParser : RParser
    {
        public const string TempFileSuffix = ".st-tmp";

        public readonly Regex KableCommand = new Regex("(knitr::)?kable\\s*\\(");

        protected IFileHandler FileHandler { get; set; }

        public RMarkdownParser()
        {
            FileHandler = new FileHandler();
        }

        /// <summary>
        /// This constructor is primarily planned for use with unit tests
        /// </summary>
        /// <param name="handler"></param>
        public RMarkdownParser(IFileHandler handler)
        {
            FileHandler = handler;
        }

        /// <summary>
        /// Convert some knitr-specific commands that may exist to ones that will work better
        /// with StatTag.  Note that we are doing this in-memory, not modifying the code file
        /// directly.
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        public List<string> ReplaceKnitrCommands(List<string> commands)
        {
            if (commands == null)
            {
                return null;
            }

            // Why replace with "("?  Because the kabel command could be nested, we don't
            // want to have to worry about matching up the closing parenthesis.  Instead
            // we can just put whatever is left in parentheses and R will handle it as
            // if we had just entered the command.
            return commands.Select(x => KableCommand.Replace(x, "(")).ToList();
        } 

        /// <summary>
        /// Used to convert the R Markdown document into just an R code file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="automation"></param>
        /// <returns></returns>
        public override List<string> PreProcessFile(CodeFile file, IStatAutomation automation = null)
        {
            if (automation == null)
            {
                throw new StatTagUserException("The R environment is needed to process R Markdown files.  Unfortunately, no R environment was provided.");
            }

            if (!file.FilePath.EndsWith("Rmd", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new StatTagUserException("StatTag is only able to process R Markdown files with the .Rmd extension");
            }

            // Trim off the "md" to get our .R file name
            var generatedCodeFilePath = file.FilePath.Substring(0, file.FilePath.Length - 2);

            // As a guard, check to see if the .R version of the file already exists.  We will clean the code file up when we're done, so we
            // know we didn't create it.  Unfortunately we will need the user to intervene.
            if (FileHandler.Exists(generatedCodeFilePath))
            {
                throw new StatTagUserException(string.Format("StatTag tries to generate an R file from your R Markdown document, and a file already exists at {0}.\r\n\r\nTo avoid deleting code that you wish to keep, StatTag cannot continue.  If you don't need {0}, please delete that file and try running again.", generatedCodeFilePath));
            }

            var filePath = file.FilePath.Replace("\\", "\\\\").Replace("'", "\\'");
            var purlCommands = new string[] {"library(knitr)", string.Format("purl(\"{0}\")", filePath)};
            automation.RunCommands(purlCommands);

            // At this point, we need the generated R file to be there.  If not, something happened during the purl() process.
            // Move will throw an error if the file doesn't exist, so that will be our implied error check.
            // Let's give the generated file a new name, just to help us identify that it was created by StatTag
            FileHandler.Move(generatedCodeFilePath, generatedCodeFilePath + TempFileSuffix);
            generatedCodeFilePath += TempFileSuffix;

            var tempCodeFile = new CodeFile()
            {
                FilePath = generatedCodeFilePath,
                StatisticalPackage = Constants.StatisticalPackages.R
            };
            var processedFileResults = base.PreProcessFile(tempCodeFile, automation);

            // Clean up the generated R file
            FileHandler.Delete(generatedCodeFilePath);

            return ReplaceKnitrCommands(processedFileResults);
        }
    }
}
