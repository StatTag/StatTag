using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StatTag.Core.Models;
using StatTag.Core.Utility;

namespace StatTag.Core.Parser
{
    /// <summary>
    /// Reads through a file containing stata commands and identifies the blocks of
    /// code that use the StatTag tag syntax
    /// </summary>
    public sealed class StataParser : BaseParser
    {
        public static readonly char[] MacroDelimiters = {'`', '\'', '$'};
        private static readonly char[] CalculationOperators = { '*', '/', '-', '+' };
        public static readonly string[] ValueCommands = { "display", "dis", "di" };
        private static readonly Regex ValueKeywordRegex = new Regex(string.Format("^\\s*(?:{0})\\b", FormatCommandListAsNonCapturingGroup(ValueCommands)));
        private static readonly Regex ValueRegex = new Regex(string.Format("^\\s*(?:{0})((\\s*\\()|(\\s+))(.*)(?(2)\\))", FormatCommandListAsNonCapturingGroup(ValueCommands)));
        public static readonly string[] GraphCommands = {"gr(?:aph)? export"};
        private static readonly Regex GraphKeywordRegex = new Regex(string.Format("^\\s*{0}\\b", FormatCommandListAsNonCapturingGroup(GraphCommands)));
        private static readonly Regex GraphRegex = new Regex(string.Format("^\\s*{0}\\s+\\\"?([^\\\",]*)[\\\",]?", FormatCommandListAsNonCapturingGroup(GraphCommands)));
        public static readonly string[] TableCommands = {"mat(?:rix)? l(?:ist)?"};
        private static readonly Regex TableKeywordRegex = new Regex(string.Format("^\\s*{0}\\b", FormatCommandListAsNonCapturingGroup(TableCommands)));
        private static readonly Regex TableRegex = new Regex(string.Format("^\\s*{0}\\s+([^,]*?)(?:\\r|\\n|$)", FormatCommandListAsNonCapturingGroup(TableCommands)));
        private static readonly Regex LogKeywordRegex = new Regex("^\\s*((?:cmd)?log)\\s*using\\b([\\w\\W]*?)(?:$|[\\r\\n,])", RegexOptions.Multiline);
        private static readonly Regex MultiLineIndicator = new Regex("[/]{3,}.*\\s*", RegexOptions.Multiline);
        private static readonly Regex MacroRegex = new Regex(string.Format("`([\\S]*?)'"), RegexOptions.Multiline);
        private const string CommentStart = "/*";
        private const string CommentEnd = "*/";
        //private static readonly Regex TrailingLineComment = new Regex("(?<!\\*)\\/\\/[^\\r\\n]*");

        public class Log
        {
            public string LogType { get; set; }
            public string LogPath { get; set; }
            public string LiteralLogEntry { get; set; }
        }

        /// <summary>
        /// This is used to test/extract a macro display value.
        /// <remarks>It assumes the rest of the display command has been extracted, 
        /// and only the value name remains.</remarks>
        /// </summary>
        private static readonly Regex MacroValueRegex = new Regex("^\\s*(?:`(.+)'|\\$([\\S]+))");

        /// <summary>
        /// Determine if a string represents a numeric constant.  Used when testing display
        /// value results to determine appropriate processing.
        /// <remarks>It assumes the rest of the display command has been extracted, 
        /// and only the value name remains.</remarks>
        /// </summary>
        private static readonly Regex NumericConstantRegex = new Regex("^(\\d*)(\\.)?(\\d+)?(e-?(0|[1-9]\\d*))?$", RegexOptions.Multiline);

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
            return GlobalMatchRegexReturnGroup(command, LogKeywordRegex, 1);
        }

        public string[] GetLogFile(string command)
        {
            return GlobalMatchRegexReturnGroup(command, LogKeywordRegex, 2);
        }

        public Log[] GetLogs(string command)
        {
            var logs = new List<Log>();
            var matches = LogKeywordRegex.Matches(command);
            if (matches.Count == 0)
            {
                return null;
            }

            return
                matches.OfType<Match>().Select(match => new Log()
                    {
                        LogType = match.Groups[1].Value.Trim(),
                        LogPath = match.Groups[2].Value.Replace("\"", "").Trim(),
                        LiteralLogEntry = match.Groups[2].Value
                    }).ToArray();
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
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            var valueName = GetValueName(command);
            return !string.IsNullOrWhiteSpace(valueName) &&
                (valueName.IndexOfAny(CalculationOperators) != -1
                || NumericConstantRegex.IsMatch(valueName));
        }

        /// <summary>
        /// Given a command string, extract all macros that are present.  This will remove
        /// macro delimiters from the macro names returned.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string[] GetMacros(string command)
        {
            var macroNames = new List<string>();
            if (!string.IsNullOrEmpty(command))
            {
                var matches = MacroRegex.Matches(command);
                if (matches.Count > 0)
                {
                    for (int index = 0; index < matches.Count; index++)
                    {
                        macroNames.Add(matches[index].Groups[1].Value);
                    }
                }
            }
            return macroNames.ToArray();
        }

        /// <summary>
        /// Specialized "IndexOfAny" that accepts strings (in this case - our comment
        /// start and end values).
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private int IndexOfAnyCommentChar(string text, int startIndex)
        {
            int nextStart = text.IndexOf(CommentStart, startIndex, StringComparison.CurrentCulture);
            int nextEnd = text.IndexOf(CommentEnd, startIndex, StringComparison.CurrentCulture);
            if (nextStart == -1 && nextEnd == -1)
            {
                return -1;
            }
            else if (nextStart == -1)
            {
                return nextEnd;
            }
            else if (nextEnd == -1)
            {
                return nextStart;
            }

            return Math.Min(nextStart, nextEnd);
        }

        /// <summary>
        /// Utility method to take a string of Stata code and remove all of the comment blocks that use
        /// /* */ syntax.  This is needed because a regex-based solution did not seem feasible (or advised)
        /// given the type of parsing this requires.
        /// </summary>
        /// <param name="text">The original Stata code</param>
        /// <returns>The modified Stata code with all /* */ comments removed </returns>
        private string RemoveNestedComments(string text)
        {
            // Go through the string, finding any comment starts.  Those get pushed onto a
            // stack, and we keep looking for ending pairs.  When that's all done we will
            // reconcile and pull out the comment blocks.
            var startIndex = text.IndexOf(CommentStart, StringComparison.CurrentCulture);
            if (startIndex == -1)
            {
                return text;
            }

            var startIndices = new Stack<int>();
            startIndices.Push(startIndex);
            var commentBlocks = new List<Tuple<int, int, bool>>();
            var nextIndex = IndexOfAnyCommentChar(text, startIndex + 1);
            while (nextIndex >= 0)
            {
                if (text.IndexOf(CommentStart, nextIndex, StringComparison.CurrentCulture) == nextIndex)
                {
                    // We have another comment starting
                    startIndices.Push(nextIndex);
                }
                else
                {
                    // We found the end of the current comment block.  We add 1 to the nextIndex (as the end index)
                    // to capture the "/" character (since our find just gets "*" from "*/").
                    bool isNested = startIndices.Count > 1;
                    commentBlocks.Add(new Tuple<int, int, bool>(startIndices.Pop(), (nextIndex + 1), isNested));
                }
                nextIndex = IndexOfAnyCommentChar(text, nextIndex + 1);
            }

            // If we get a block of code that has unmatched nested comments, we are going to ignore it
            // and return the original text.  Most likely the code will fail in Stata anyway so we are
            // going to pass it to the API as-is.
            if (startIndices.Count != 0)
            {
                return text;
            }

            // Sort the comment blocks by the starting index (descending order).  That way we can remove comment blocks
            // starting at the end and not have to worry about updating string indices.
            commentBlocks.Sort(delegate(Tuple<int, int, bool> x, Tuple<int, int, bool> y)
            {
                if (x == null && y == null) { return 0; }
                if (x == null) { return 1; }
                if (y == null) { return -1; }
                return y.Item1.CompareTo(x.Item1);
            });

            // Finally, loop through the comment blocks and apply them by removing the commented text.  This makes
            // the assumption commentBlocks is sorted in descending order, so we can just remove text and not need
            // to worry about adjusting indices.
            for (int index = 0; index < commentBlocks.Count; index++)
            {
                // Our commentBlocks contains all unique comment locations.  Some locations many be nested inside
                // other comments (not sure why you would, but it could happen!)  We will check to see if we have
                // some type of inner comment and skip it if that's the case, since the outer one will remove it.
                if (commentBlocks[index].Item3)
                {
                    continue;
                }

                var length = commentBlocks[index].Item2 - commentBlocks[index].Item1 + 1;
                text = text.Remove(commentBlocks[index].Item1, length);
            }

            return text;
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
            var modifiedText = MultiLineIndicator.Replace(originalText, " ");
            // Why are we stripping out trailing lines?  There is apparently an issue with the Stata Automation API where having a trailing
            // comment on a valid line causes that command to fail (e.g.: "sysuse(bpwide)  // comment").  Our workaround is to strip those
            // comments so users don't have to modify their code.  The issue has been reported to Stata.
            //modifiedText = TrailingLineComment.Replace(modifiedText, "");
            modifiedText = CodeParserUtil.StripTrailingComments(modifiedText);
            modifiedText = RemoveNestedComments(modifiedText).Trim();
            return modifiedText.Split(new string[]{"\r\n"}, StringSplitOptions.None).ToList();
        }
    }
}
