using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    return new Stata();
            }

            return null;
        }
    }
}
