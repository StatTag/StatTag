using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Parser
{
    public class BaseParameterParser
    {
        public const string StringValueMatch = ".*?";
        public const string IntValueMatch = "\\d+";
        public const string BoolValueMatch = "true|false";
        protected static Dictionary<string, Regex> RegexCache = new Dictionary<string, Regex>();

        /// <summary>
        /// Build the regex to identify and extract a parameter from an annotation string.
        /// Internally this uses a cache to save created regexes.  These are keyed by the
        /// parameters, as that will uniquely create the regex string.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="valueMatch"></param>
        /// <param name="isQuoted"></param>
        /// <returns></returns>
        protected static Regex BuildRegex(string name, string valueMatch, bool isQuoted)
        {
            string key = string.Format("{0}-{1}-{2}", name, valueMatch, isQuoted);
            if (!RegexCache.ContainsKey(key))
            {
                RegexCache.Add(key, new Regex(string.Format("\\{2}.*{0}\\s*=\\s*{1}({4}){1}.*\\{3}",
                    name, (isQuoted ? "\\\"" : string.Empty), Constants.AnnotationTags.ParamStart,
                    Constants.AnnotationTags.ParamEnd, valueMatch)));
            }

            return RegexCache[key];
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
