using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                }
            }

            return new BaseValueFormatter();
        }
    }
}
