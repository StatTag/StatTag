using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using StatTag.Core.Exceptions;
using StatTag.Core.Models;
using StatTag.Core.Parser;
using StatTag.Core.Utility;

namespace R
{
    public class RMarkdownAutomation : RAutomation
    {
        public RMarkdownAutomation()
        {
            Parser = new RMarkdownParser();
            State = new StatPackageState();
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
            var expression = Engine.Evaluate("require('knitr')");
            if (expression != null)
            {
                var result = expression.AsLogical();
                if (result != null && result.Length > 0)
                {
                    if (result[0])
                    {
                        return true;
                    }
                }
            }

            throw new StatTagUserException("To run R Markdown documents, StatTag requires that you have the knitr package installed.\r\n\r\nPlease see the User’s Guide for more information.");
        }
    }
}
