using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Parser
{
    /// <summary>
    /// Reads through a file containing stata commands and identifies the blocks of
    /// code that use the Analysis Manager annotation syntax
    /// </summary>
    public sealed class Stata : BaseParser
    {
        private static readonly char[] MacroDelimiters = {'`', '\''};
        private static readonly char[] CalculationOperators = { '*', '/', '-', '+' };
        private static string ValueCommand = "di(?:splay)?";
        private static Regex ValueKeywordRegex = new Regex(string.Format("^\\s*{0}\\b", ValueCommand));
        //private static Regex ValueRegex = new Regex(string.Format("^\\s*{0}\\s+(.*)", ValueCommand));
        private static Regex ValueRegex = new Regex(string.Format("^\\s*{0}((\\s*\\()|(\\s+))(.*)(?(2)\\))", ValueCommand));
        private static string GraphCommand = "gr(?:aph)? export";
        private static Regex GraphKeywordRegex = new Regex(string.Format("^\\s*{0}\\b", GraphCommand.Replace(" ", "\\s+")));
        private static Regex GraphRegex = new Regex(string.Format("^\\s*{0}\\s+\\\"?([^\\\",]*)[\\\",]?", GraphCommand.Replace(" ", "\\s+")));
        private static string TableCommand = "mat(?:rix)? l(?:ist)?";
        private static Regex TableKeywordRegex = new Regex(string.Format("^\\s*{0}\\b", TableCommand.Replace(" ", "\\s+")));
        private static Regex TableRegex = new Regex(string.Format("^\\s*{0}\\s+([^,\\s]*)\\b", TableCommand.Replace(" ", "\\s+")));

        /// <summary>
        /// This is used to test/extract a macro display value.
        /// <remarks>It assumes the rest of the display command has been extracted, 
        /// and only the value name remains.</remarks>
        /// </summary>
        private static Regex MacroValueRegex = new Regex("^\\s*`(.+)'");

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
    }
}
