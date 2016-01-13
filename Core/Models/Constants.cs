namespace AnalysisManager.Core.Models
{
    public class Constants
    {
        public static class StatisticalPackages
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

        public static class RunFrequency
        {
            public const string Default = "Default";
            public const string OnDemand = "On Demand";

            public static string[] GetList()
            {
                return new[]
                {
                    Default, OnDemand
                };
            }
        }

        public static class AnnotationType
        {
            public const string Value = "Value";
            public const string Figure = "Figure";
            public const string Table = "Table";
        }

        public static class ValueFormatType
        {
            public const string Default = "Default";
            public const string Numeric = "Numeric";
            public const string DateTime = "DateTime";
            public const string Percentage = "Percentage";
        }

        public static class DialogLabels
        {
            public const string Elipsis = "...";
            public const string Details = "Detail";
            public const string Edit = "Edit";
        }

        public static class FileFilters
        {
            public const string StataLabel = "Stata Do Files";
            public const string StataFilter = "*.do;*.ado";
            public const string RLabel = "R";
            public const string RFilter = "*.r";
            public const string SASLabel = "SAS";
            public const string SASFilter = "*.sas";
            public const string AllLabel = "All files";
            public const string AllFilter = "*.*";

            public static string FormatForOpenFileDialog()
            {
                return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}",
                    StataLabel, StataFilter, RLabel, RFilter, SASLabel, SASFilter, AllLabel, AllFilter);
            }
        }

        public static class FileExtensions
        {
            public const string Backup = "am-bak";
        }

        /// <summary>
        /// A list of parameter names that are available across all types of
        /// annotations.
        /// </summary>
        public static class AnnotationParameters
        {
            public const string Label = "Label";
            public const string Frequency = "Frequency";
        }

        public static class ValueParameters
        {
            public const string Type = "Type";
            public const string Decimals = "Decimals";
            public const string UseThousands = "Thousands";
            public const string DateFormat = "DateFormat";
            public const string TimeFormat = "TimeFormat";
        }

        public static class CodeFileComment
        {
            public const string Stata = "*";
            //public const string R = "*";
            //public const string SAS = "";
        }

        public static class AnnotationTags
        {
            public const string StartAnnotation = ">>>";
            public const string EndAnnotation = "<<<";
            public const string AnnotationPrefix = "AM:";
            public const string ParamStart = "(";
            public const string ParamEnd = ")";
        }

        public static class ParserFilterMode
        {
            public const int IncludeAll = 0;
            public const int ExcludeOnDemand = 1;
            public const int AnnotationList = 2;
        }

        public static class ExecutionStepType
        {
            public const int CodeBlock = 0;
            public const int Annotation = 1;
        }

        public static class DateFormats
        {
            public const string MMDDYYYY = "MM/dd/yyyy";
            public const string MonthDDYYYY = "MMMM dd, yyyy";
        }

        public static class TimeFormats
        {
            public const string HHMM = "HH:mm";
            public const string HHMMSS = "HH:mm:ss";
        }
    }
}
