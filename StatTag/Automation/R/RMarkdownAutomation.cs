using StatTag.Core.Exceptions;
using StatTag.Core.Models;
using StatTag.Core.Parser;
using System;

namespace R
{
    public class RMarkdownAutomation : RAutomation
    {
        public RMarkdownAutomation(Configuration config) : base(config)
        {
            Parser = new RMarkdownParser();
        }

        public override bool Initialize(CodeFile file, LogManager logger)
        {
            // Let the base initialization run so we know we have a valid R engine
            if (!base.Initialize(file, logger))
            {
                return false;
            }

            // We require that the knitr package be installed if a user wants to run R Markdown.
            // This is because we use the purl() command from knitr to extract the R code.
            // We think this is a fair assumption because the recommendation from StatTag is to
            // run your code to completion before running it in StatTag.  That means the user
            // should have knitted their R Markdown document.
            var result = base.RunCommand("print(require('knitr', quietly=TRUE, warn.conflicts = FALSE))",
                new Tag() { Name = "_tmp_stattag_knitr_exists", Type = Constants.TagType.Value } );
            if (result != null && result.ValueResult.Equals("TRUE"))
            {
                return true;
            }

            throw new StatTagUserException("To run R Markdown documents, StatTag requires that you have the knitr package installed.\r\n\r\nPlease see the User’s Guide for more information.");
        }
    }
}
