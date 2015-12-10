using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Generator;
using AnalysisManager.Core.Models;
using AnalysisManager.Core.Parser;

namespace AnalysisManager.Core
{
    public static class Factories
    {
        public static IParser GetParser(CodeFile file)
        {
            switch (file.StatisticalPackage)
            {
                case Constants.StatisticalPackages.Stata:
                    return new AnalysisManager.Core.Parser.Stata();
            }

            return null;
        }

        public static IGenerator GetGenerator(CodeFile file)
        {
            switch (file.StatisticalPackage)
            {
                case Constants.StatisticalPackages.Stata:
                    return new AnalysisManager.Core.Generator.Stata();
            }

            return null;
        }
    }
}
