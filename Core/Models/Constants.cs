﻿using System;
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

            public static string[] GetList()
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

            public static string[] GetList()
            {
                return new[]
                {
                    Always, OnDemand
                };
            }
        }

        public class AnnotationType
        {
            public const string Value = "Value";
            public const string Figure = "Figure";
            public const string Table = "Table";
        }

        public class ValueFormatType
        {
            public const string Default = "Default";
            public const string Numeric = "Numeric";
            public const string DateTime = "DateTime";
            public const string Percentage = "Percentage";
        }

        public class DialogLabels
        {
            public const string Elipsis = "...";
            public const string Details = "Detail";
            public const string Edit = "Edit";
        }
    }
}
