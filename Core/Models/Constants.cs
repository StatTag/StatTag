﻿using System.Security.Cryptography.X509Certificates;

namespace StatTag.Core.Models
{
    public static class Constants
    {
        public const long BytesToMegabytesConversion = 1048576;

        public static class StatisticalPackages
        {
            public const string Stata = "Stata";
            public const string R = "R";
            public const string SAS = "SAS";
            public const string RMarkdown = "R Markdown";
            public const string Python = "Python";

            public static string[] GetList()
            {
                return new[]
                {
                    Stata, SAS, R, RMarkdown
                };
            }
        }

        public static class RunFrequency
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

        public static class TagType
        {
            public const string Value = "Value";
            public const string Figure = "Figure";
            public const string Table = "Table";
            public const string Verbatim = "Verbatim";
            
            public static string[] GetList()
            {
                return new[]
                {
                    Value, Figure, Table, Verbatim
                };
            }
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
            public const string SASLabel = "SAS";
            public const string SASFilter = "*.sas";
            public const string RLabel = "R";
            public const string RFilter = "*.r";
            public const string RMarkdownLabel = "R Markdown";
            public const string RMarkdownFilter = "*.Rmd";
            public const string PythonLabel = "Python";
            public const string PythonFilter = "*.py";
            public const string AllLabel = "All files";
            public const string AllFilter = "*.*";
            public const string SupportedLabel = "Supported files";

            public static string FormatForOpenFileDialog()
            {
                return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}",
                    SupportedLabel, string.Join(";", new string[] { StataFilter, SASFilter, RFilter, RMarkdownFilter, PythonFilter }),
                    StataLabel, StataFilter, SASLabel, SASFilter, RLabel, RFilter, RMarkdownLabel, RMarkdownFilter, PythonLabel, PythonFilter, AllLabel, AllFilter);
            }
        }

        public static class FileExtensions
        {
            public const string Backup = "st-bak";
        }

        public static class Placeholders
        {
            public const string EmptyField = "[NO RESULT]";
            public const string RemovedField = "[REMOVED]";
        }

        public static class ReservedCharacters
        {
            public const char TagTableCellDelimiter = '|';
            public const char ListDelimiter = ',';
            public const char RangeDelimiter = '-';
        }

        public static class FieldDetails
        {
            public const string MacroButtonName = "StatTag";
        }

        /// <summary>
        /// A list of parameter names that are available across all types of
        /// tags.
        /// </summary>
        public static class TagParameters
        {
            public const string Id = "Id";
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
            public const string AllowInvalidTypes = "AllowInvalid";
        }

        public static class TableParameters
        {
            public const string FilterEnabled = "FilterEnabled";
            public const string FilterType = "FilterType";
            public const string FilterValue = "FilterValue";
        }

        public static class TableParameterDefaults
        {
            public const bool FilterEnabled = false;
            public const string FilterType = "";
            public const string FilterValue = "";
        }

        public static class ValueParameterDefaults
        {
            public const bool AllowInvalidTypes = false;
        }

        public static class CodeFileComment
        {
            public const string Stata = "*";
            public const string SAS = "*";
            public const string R = "#";
            public const string Python = "#";
        }

        public static class CodeFileCommentSuffix
        {
            public const string Default = "";
            public const string SAS = ";";
        }

        public static class TagTags
        {
            public const string StartTag = ">>>";
            public const string EndTag = "<<<";
            public const string TagPrefix = "ST:";
            public const string ParamStart = "(";
            public const string ParamEnd = ")";
        }

        public static class ParserFilterMode
        {
            public const int IncludeAll = 0;
            public const int ExcludeOnDemand = 1;
            public const int TagList = 2;
        }

        public static class ExecutionStepType
        {
            public const int CodeBlock = 0;
            public const int Tag = 1;
        }

        public static class DateFormats
        {
            public const string MMDDYYYY = "MM/dd/yyyy";
            public const string MonthDDYYYY = "MMMM dd, yyyy";

            public static string[] GetList()
            {
                return new[]
                {
                    MMDDYYYY, MonthDDYYYY
                };
            }
        }

        public static class TimeFormats
        {
            public const string HHMM = "HH:mm";
            public const string HHMMSS = "HH:mm:ss";

            public static string[] GetList()
            {
                return new[]
                {
                    HHMM, HHMMSS
                };
            }
        }

        public static class DimensionIndex
        {
            public const int Rows = 0;
            public const int Columns = 1;
        }

        public static class CodeFileActionTask
        {
            public const int NoAction = 0;
            public const int ChangeFile = 1;
            public const int RemoveTags = 2;
            public const int ReAddFile = 3;
            public const int SelectFile = 4;
        }

        public static class FilterPrefix
        {
            public const string Row = "Row";
            public const string Column = "Col";
        }

        public static class FilterType
        {
            public const string Exclude = "Exclude";
            public const string Include = "Include";
        }

        public static class MissingValueOption
        {
            public const string StatPackageDefault = "StatPackageDefault";
            public const string CustomValue = "CustomValue";
            public const string BlankString = "BlankString";
        }

        public static class RDetectionOption
        {
            public const string System = "System";
            public const string Selected = "Selected";
        }
    }
}
