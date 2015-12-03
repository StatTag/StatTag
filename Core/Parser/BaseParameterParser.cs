using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnalysisManager.Core.Parser
{
    public class BaseParameterParser
    {
        public const string ParamStart = "(";
        public const string ParamEnd = ")";
        public const string StringValueMatch = ".*?";
        public const string IntValueMatch = "\\d+";
        public const string BoolValueMatch = "true|false";

        protected static Regex BuildRegex(string name, string valueMatch, bool isQuoted)
        {
            return new Regex(string.Format("\\{2}.*{0}\\s*=\\s*{1}({4}){1}.*\\{3}",
                name, (isQuoted ? "\\\"" : string.Empty), ParamStart, ParamEnd, valueMatch));
        }

        protected static string GetParameter(string name, string valueMatch, string text, string defaultValue = "", bool quoted = true)
        {
            var match = BuildRegex(name, valueMatch, quoted).Match(text);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return defaultValue;
        }


        public static string GetStringParameter(string name, string text, string defaultValue = "", bool quoted = true)
        {
            return GetParameter(name, StringValueMatch, text, defaultValue, quoted);
        }

        public static int? GetIntParameter(string name, string text, int? defaultValue = null)
        {
            var stringValue = GetParameter(name, IntValueMatch, text, null, false);
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return defaultValue;
            }

            int value = 0;
            if (int.TryParse(stringValue, out value))
            {
                return value;
            }

            return defaultValue;
        }

        public static bool? GetBoolParameter(string name, string text, bool? defaultValue = null)
        {
            var stringValue = GetParameter(name, BoolValueMatch, text, null, false);
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return defaultValue;
            }

            bool value = false;
            if (bool.TryParse(stringValue, out value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}
