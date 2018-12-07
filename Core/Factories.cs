using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Exceptions;
using StatTag.Core.Interfaces;
using StatTag.Core.Generator;
using StatTag.Core.Models;
using StatTag.Core.Parser;
using StatTag.Core.ValueFormatter;

namespace StatTag.Core
{
    public static class Factories
    {
        public static ICodeFileParser GetParser(CodeFile file)
        {
            if (file != null)
            {
                switch (file.StatisticalPackage)
                {
                    case Constants.StatisticalPackages.Stata:
                        return new StataParser();
                    case Constants.StatisticalPackages.SAS:
                        return new SASParser();
                    case Constants.StatisticalPackages.R:
                        return new RParser();
                    case Constants.StatisticalPackages.RMarkdown:
                        return new RMarkdownParser();
                }
            }

            return null;
        }

        public static IGenerator GetGenerator(CodeFile file)
        {
            if (file != null)
            {
                switch (file.StatisticalPackage)
                {
                    case Constants.StatisticalPackages.Stata:
                        return new StatTag.Core.Generator.Stata();
                    case Constants.StatisticalPackages.SAS:
                        return new StatTag.Core.Generator.SAS();
                    case Constants.StatisticalPackages.R:
                        return new StatTag.Core.Generator.R();
                    case Constants.StatisticalPackages.RMarkdown:
                        return new StatTag.Core.Generator.RMarkdown();
                }
            }

            return null;
        }

        public static IValueFormatter GetValueFormatter(CodeFile file)
        {
            if (file != null)
            {
                switch (file.StatisticalPackage)
                {
                    case Constants.StatisticalPackages.Stata:
                        return new StatTag.Core.ValueFormatter.Stata();
                    case Constants.StatisticalPackages.SAS:
                        return new StatTag.Core.ValueFormatter.SAS();
                    case Constants.StatisticalPackages.R:
                        return new StatTag.Core.ValueFormatter.R();
                    case Constants.StatisticalPackages.RMarkdown:
                        return new StatTag.Core.ValueFormatter.RMarkdown();
                }
            }

            return new BaseValueFormatter();
        }
    }
}
