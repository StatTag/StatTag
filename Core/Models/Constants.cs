using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisManager.Core.Models
{
    public class Constants
    {
        public class StatisticalPackages
        {
            public const string Stata = "Stata";
            public const string R = "R";
            public const string SAS = "SAS";

            public string[] GetList()
            {
                return new[]
                {
                    Stata, R, SAS
                };
            }
        }

        public class RunFrequency
        {
            public const string Always = "Always";
            public const string OnDemand = "On Demand";

            public string[] GetList()
            {
                return new[]
                {
                    Always, OnDemand
                };
            }
        }
    }
}
