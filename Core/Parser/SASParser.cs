using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using StatTag.Core.Models;

namespace StatTag.Core.Parser
{
    public class SASParser : BaseParser
    {
        public static readonly string[] ValueCommands = {"%put"};
        private static readonly Regex ValueKeywordRegex = new Regex(string.Format("^\\s*{0}\\b", FormatCommandListAsNonCapturingGroup(ValueCommands)), RegexOptions.IgnoreCase);
        private static readonly Regex ValueRegex = new Regex(string.Format("^\\s*{0}\\s+([^;]*);", string.Join("|", ValueCommands)), RegexOptions.IgnoreCase);
        public static readonly string[] FigureCommands = {"ods pdf"};
        private static readonly Regex FigureKeywordRegex = new Regex(string.Format("^\\s*{0}\\b[\\S\\s]*file", FormatCommandListAsNonCapturingGroup(FigureCommands)), RegexOptions.IgnoreCase);
        private static readonly Regex FigureRegex = new Regex(string.Format("^\\s*{0}\\b[\\S\\s]*file\\s*=\\s*[\"'](.*)[\"'][\\S\\s]*;", FormatCommandListAsNonCapturingGroup(FigureCommands)), RegexOptions.IgnoreCase);
        public static readonly string[] TableCommands = {"ods csv", "ods excel", "proc export"};
        private static readonly Regex TableKeywordRegex = new Regex(string.Format("^\\s*{0}\\b[\\S\\s]*file", FormatCommandListAsNonCapturingGroup(TableCommands)), RegexOptions.IgnoreCase);
        private static readonly Regex TableRegex = new Regex(string.Format("^\\s*{0}\\b[\\S\\s]*file\\s*=\\s*[\"'](.*?)[\"'][\\S\\s]*;", FormatCommandListAsNonCapturingGroup(TableCommands)), RegexOptions.IgnoreCase);
        private static readonly Regex PathCaptureRegex = new Regex("\\bpath\\s*=\\s*(?:([&].+?\\b)|(?:[\"'](.*?)[\"']))[\\S\\s]*?;", RegexOptions.IgnoreCase);
        public const string MacroIndicator = "&";
        public const string FunctionIndicator = "%";
        public const string CommandDelimiter = ";";

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
        /// Determine if a command has character(s) that indicate a function call may
        /// be included within.  This is primarily intended for use when a filename
        /// is used and we need to resolve the path to a literal value.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool HasFunctionIndicator(string command)
        {
            return command.ToUpper().Contains(FunctionIndicator);
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

        /// <summary>
        /// Within SAS, we only return table data from files.  This will always
        /// return null to indicate that no named table objects are used.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override string GetTableName(string command)
        {
            return null;
        }

        public override string GetTableDataPath(string command)
        {
            string file = MatchRegexReturnGroup(command, TableRegex, 1);

            // Check to see if a path parameter was provided as well.  If not, we will
            // stop and return the file parameter.
            string path = GetPathParameter(command);
            if (string.IsNullOrEmpty(path))
            {
                return file;
            }

            if (path.Contains(MacroIndicator))
            {
                return string.Format("{0}.\\{1}", path, file);
            }

            return Path.Combine(path, file);
        }

        /// <summary>
        /// To ensure that we are properly processing and detecting SAS commands, we need to take commands that may
        /// span multiple lines and put them into a single line.  This is easily done in SAS, as we simply need to
        /// find the command delimiter (semicolon).
        /// </summary>
        /// <param name="originalContent">An array of command lines</param>
        /// <returns>An array of commands with multi-line commands on a single line.  The size will be &lt;= the size of originalContent</returns>
        public List<string> CollapseMultiLineCommands(List<string> originalContent)
        {
            var originalText = string.Join("\r\n", originalContent);

            // This is a fringe case - but in the event there is no command delimiter in this
            // block of code, we want to return it as-is and not add any delimiters.  If we
            // let the code below run, it will add the delimiter.
            if (!originalText.Contains(CommandDelimiter))
            {
                return new List<string>(new[] { originalText });
            }

            var modifiedText = originalText;
            var splitCommands =
                modifiedText.Split(new[] {CommandDelimiter}, StringSplitOptions.RemoveEmptyEntries)
                    .ToList()
                    .Select(x => string.Format("{0}{1}", x, CommandDelimiter).Trim())
                    .ToList();

            if (!originalText.Trim().EndsWith(CommandDelimiter))
            {
                splitCommands[splitCommands.Count - 1] =
                    splitCommands[splitCommands.Count - 1].Trim(new[] {CommandDelimiter[0]});
            }
            return splitCommands;
        }

        public override List<string> PreProcessContent(List<string> originalContent)
        {
            return new List<string>(CollapseMultiLineCommands(originalContent));
        }

        /// <summary>
        /// Retrieve the value of a PATH parameter in a command, if it exists.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Null if no PATH parameter exists, otherwise the value of the match (which can be an empty string)</returns>
        public string GetPathParameter(string command)
        {
            var match = PathCaptureRegex.Match(command);
            if (!match.Success)
            {
                return null;
            }

            if (match.Groups[1].Success)
            {
                return match.Groups[1].Value;
            }
            else if (match.Groups[2].Success)
            {
                return match.Groups[2].Value;
            }

            return string.Empty;
        }
    }
}