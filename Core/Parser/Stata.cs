using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Parser
{
    /// <summary>
    /// Reads through a file containing stata commands and identifies the blocks of
    /// code that use the StatTag tag syntax
    /// </summary>
    public sealed class Stata : BaseParser
    {
        private static readonly char[] MacroDelimiters = {'`', '\''};
        private static readonly char[] CalculationOperators = { '*', '/', '-', '+' };
        private static string ValueCommand = "di(?:splay)?";
        private static readonly Regex ValueKeywordRegex = new Regex(string.Format("^\\s*{0}\\b", ValueCommand));
        //private static Regex ValueRegex = new Regex(string.Format("^\\s*{0}\\s+(.*)", ValueCommand));
        private static readonly Regex ValueRegex = new Regex(string.Format("^\\s*{0}((\\s*\\()|(\\s+))(.*)(?(2)\\))", ValueCommand));
        private static string GraphCommand = "gr(?:aph)? export";
        private static readonly Regex GraphKeywordRegex = new Regex(string.Format("^\\s*{0}\\b", GraphCommand.Replace(" ", "\\s+")));
        private static readonly Regex GraphRegex = new Regex(string.Format("^\\s*{0}\\s+\\\"?([^\\\",]*)[\\\",]?", GraphCommand.Replace(" ", "\\s+")));
        private static string TableCommand = "mat(?:rix)? l(?:ist)?";
        private static readonly Regex TableKeywordRegex = new Regex(string.Format("^\\s*{0}\\b", TableCommand.Replace(" ", "\\s+")));
        private static readonly Regex TableRegex = new Regex(string.Format("^\\s*{0}\\s+([^,]*?)(?:\\r|\\n|$)", TableCommand.Replace(" ", "\\s+")));
        private static readonly Regex LogKeywordRegex = new Regex("^\\s*((?:cmd)?log)\\s*using\\b", RegexOptions.Multiline);
        private static readonly Regex[] MultiLineIndicators = new[]
        {
            new Regex("[/]{3,}.*\\s*", RegexOptions.Multiline),
            new Regex("/\\*.*\\*/\\s?", RegexOptions.Singleline),
        };

        /// <summary>
        /// This is used to test/extract a macro display value.
        /// <remarks>It assumes the rest of the display command has been extracted, 
        /// and only the value name remains.</remarks>
        /// </summary>
        private static readonly Regex MacroValueRegex = new Regex("^\\s*`(.+)'");

        public override string CommentCharacter
        {
            get { return Constants.CodeFileComment.Stata; }
        }

        /// <summary>
        /// Determine if a command is for exporting an image
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override bool IsImageExport(string command)
        {
            return GraphKeywordRegex.IsMatch(command);
        }

        /// <summary>
        /// Determine if a command is for displaying a result
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override bool IsValueDisplay(string command)
        {
            return ValueKeywordRegex.IsMatch(command);
        }

        /// <summary>
        /// Determine if a display value is a macro type.
        /// </summary>
        /// <remarks>Assumes that you have already verified the command contains a display value.</remarks>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool IsMacroDisplayValue(string command)
        {
            string value = GetValueName(command);
            return MacroValueRegex.IsMatch(value);
        }

        /// <summary>
        /// Determine if a command is starting a log in the Stata session
        /// </summary>
        /// <param name="command"></param>
        /// <returns>true if it is starting a log, false otherwise</returns>
        public bool IsStartingLog(string command)
        {
            return LogKeywordRegex.IsMatch(command);
        }

        /// <summary>
        /// Determine if a command is for displaying a result
        /// </summary>
        /// <param name="command"></param>
        /// <returns>A string describing the type of log (log or cmdlog), or a blank string if this is not a logging command</returns>
        //public string GetLogType(string command)
        public string[] GetLogType(string command)
        {
            //return MatchRegexReturnGroup(command, LogKeywordRegex, 1);
            return GlobalMatchRegexReturnGroup(command, LogKeywordRegex, 1);
        }

        /// <summary>
        /// Returns the file path where an image exported from the statistical package
        /// is being saved.
        /// </summary>
        /// <remarks>Assumes that you have verified this is an image export command
        /// using IsImageExport first</remarks>
        /// <param name="command"></param>
        /// <returns></returns>
        public override string GetImageSaveLocation(string command)
        {
            return MatchRegexReturnGroup(command, GraphRegex, 1);
        }

        /// <summary>
        /// Returns the name of the variable/scalar to display.
        /// </summary>
        /// <remarks>Assumes that you have verified this is a display command using
        /// IsValueDisplay first.</remarks>
        /// <param name="command"></param>
        /// <returns></returns>
        public override string GetValueName(string command)
        {
            return MatchRegexReturnGroup(command, ValueRegex, 4);
        }

        /// <summary>
        /// A specialized version of GetValueName that prepares a macro display value
        /// for use.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string GetMacroValueName(string command)
        {
            return GetValueName(command).Trim(MacroDelimiters);
        }


        public override bool IsTableResult(string command)
        {
            return TableKeywordRegex.IsMatch(command);
        }

        public override string GetTableName(string command)
        {
            return MatchRegexReturnGroup(command, TableRegex, 1);
        }

        public bool IsCalculatedDisplayValue(string command)
        {
            return GetValueName(command).IndexOfAny(CalculationOperators) != -1;
        }

        private string MatchRegexReturnGroup(string text, Regex regex, int groupNum)
        {
            var match = regex.Match(text);
            if (match.Success)
            {
                return match.Groups[groupNum].Value.Trim();
            }

            return string.Empty;
        }

        private string[] GlobalMatchRegexReturnGroup(string text, Regex regex, int groupNum)
        {
            var matches = regex.Matches(text);
            if (matches.Count == 0)
            {
                return null;
            }

            var results = matches.OfType<Match>().Select(match => match.Groups[groupNum].Value.Trim()).ToList();
            return results.ToArray();
        }

        /// <summary>
        /// To prepare for use, we need to collapse down some of the text.  This includes:
        ///  - Collapsing commands that span multiple lines into a single line
        /// </summary>
        /// <param name="originalContent"></param>
        /// <returns></returns>
        public override List<string> PreProcessContent(List<string> originalContent)
        {
            if (originalContent == null || originalContent.Count == 0)
            {
                return new List<string>();
            }

            var originalText = string.Join("\r\n", originalContent);
            var modifiedText = originalText;
            foreach (var regex in MultiLineIndicators)
            {
                modifiedText = regex.Replace(modifiedText, " ");
            }
            
            return modifiedText.Split(new string[]{"\r\n"}, StringSplitOptions.None).ToList();
        }
    }
}
