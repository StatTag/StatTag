using System.Collections.Generic;
using System.Text.RegularExpressions;
using StatTag.Core.Models;

namespace StatTag.Core.Parser
{
    public class SASParser : BaseParser
    {
        private static string ValueCommand = "%put";
        private static readonly Regex ValueKeywordRegex = new Regex(string.Format("^\\s*{0}\\b", ValueCommand), RegexOptions.IgnoreCase);
        private static readonly Regex ValueRegex = new Regex(string.Format("^\\s*{0}\\s+([^;]*);", ValueCommand), RegexOptions.IgnoreCase);
        private static string FigureCommand = "ods pdf";
        private static readonly Regex FigureKeywordRegex = new Regex(string.Format("^\\s*{0}\\b[\\S\\s]*file", FigureCommand.Replace(" ", "\\s+")), RegexOptions.IgnoreCase);
        private static readonly Regex FigureRegex = new Regex(string.Format("^\\s*{0}\\b[\\S\\s]*file\\s*=\\s*\"(.*)\"[\\S\\s]*;", FigureCommand.Replace(" ", "\\s+")), RegexOptions.IgnoreCase);
        private static string TableCommand = "ods csv";
        private static readonly Regex TableKeywordRegex = new Regex(string.Format("^\\s*{0}\\b[\\S\\s]*file", TableCommand.Replace(" ", "\\s+")), RegexOptions.IgnoreCase);
        private static readonly Regex TableRegex = new Regex(string.Format("^\\s*{0}\\b[\\S\\s]*file\\s*=\\s*\"(.*)\"[\\S\\s]*;", TableCommand.Replace(" ", "\\s+")), RegexOptions.IgnoreCase);
        public const string MacroIndicator = "&";

        public override string CommentCharacter
        {
            get { return Constants.CodeFileComment.SAS; }
        }

        /// <summary>
        /// Determine if a command is for exporting an image
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override bool IsImageExport(string command)
        {
            return FigureKeywordRegex.IsMatch(command);
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
            return MatchRegexReturnGroup(command, FigureRegex, 1);
        }

        /// <summary>
        /// Determine if a command has character(s) that indicate a macro value may
        /// be included within.  This is primarily intended for use when a filename
        /// is used and we need to resolve the path to a literal value.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool HasMacroIndicator(string command)
        {
            return command.Contains(MacroIndicator);
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
        /// Returns the name of the variable/scalar to display.
        /// </summary>
        /// <remarks>Assumes that you have verified this is a display command using
        /// IsValueDisplay first.</remarks>
        /// <param name="command"></param>
        /// <returns></returns>
        public override string GetValueName(string command)
        {
            return MatchRegexReturnGroup(command, ValueRegex, 1);
        }

        public override bool IsTableResult(string command)
        {
            return TableKeywordRegex.IsMatch(command);
        }

        public override string GetTableName(string command)
        {
            return MatchRegexReturnGroup(command, TableRegex, 1);
        }

        public override List<string> PreProcessContent(List<string> originalContent)
        {
            return originalContent;
        }
    }
}