using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using StatTag.Core.Utility;

namespace StatTag.Core.Parser
{
    public class RParser : BaseParser
    {
        public static readonly string[] FigureCommands = new[] { "pdf", "win.metafile", "png", "jpeg", "bmp", "postscript" };
        private static readonly Regex FigureRegex = new Regex(string.Format("^\\s*(?:{0})\\s*\\((\\s*?[\\s\\S]*)\\)", string.Join("|", FigureCommands)));
        //private static readonly Regex FigureParameterRegex = new Regex("(?:([\\w]*?)\\s*=\\s*)?(?:([\\w]*?\\s*\\(.*?\\))|([\\w]+))");
        private static readonly Regex FigureParameterRegex = new Regex("(?:([\\w]*?)\\s*=\\s*)?(?:([\\w]+\\s*\\(.+\\))|([^\\(\\)]+?))(?:,|$)\\s*", RegexOptions.Multiline);
        private const char ParameterDelimiter = '=';
        private const char ArgumentDelimiter = ',';
        private const string FileParameterName = "file";

        public override string CommentCharacter
        {
            get { return Constants.CodeFileComment.R; }
        }

        public override bool IsImageExport(string command)
        {
            return FigureRegex.IsMatch(command);
        }

        private class FunctionParam
        {
            public int Index;
            public string Key;
            public string Value;
        }

        /// <summary>
        /// This will return the exact parameter that represents the image save location.  This may be an R function (e.g., paste)
        /// to construct the file path, a variable, or a string literal.  String literals will include enclosing quotes.  This is
        /// because the output of this method is sent to R as an expression for evaluation.  That way R handles converting everything
        /// into an exact file path that we can then utilize.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override string GetImageSaveLocation(string command)
        {
            var match = FigureRegex.Match(command);
            var parameters = new List<FunctionParam>();
            if (!match.Success)
            {
                return string.Empty;
            }

            var arguments = match.Groups[1].Value;
            var matches = FigureParameterRegex.Matches(arguments);
            if (matches.Count == 0)
            {
                return string.Empty;
            }

            for (int index = 0; index < matches.Count; index++)
            {
                var paramMatch = matches[index];
                parameters.Add(new FunctionParam()
                {
                    Index = index,
                    Key = paramMatch.Groups[1].Value,
                    Value = (string.IsNullOrWhiteSpace(paramMatch.Groups[2].Value) ? paramMatch.Groups[3].Value : paramMatch.Groups[2].Value)
                });
            }

            // Follow R's approach to argument matching (http://adv-r.had.co.nz/Functions.html#function-arguments)
            // First, look for exact name (perfect matching)
            var matchingArg =
                parameters.FirstOrDefault(
                    x => x.Key.Equals(FileParameterName, StringComparison.CurrentCultureIgnoreCase));
            if (matchingArg != null)
            {
                return matchingArg.Value;
            }

            // Next look by prefix matching
            matchingArg =
                parameters.FirstOrDefault(
                    x =>
                        !string.IsNullOrWhiteSpace(x.Key) &&
                        FileParameterName.StartsWith(x.Key, StringComparison.CurrentCultureIgnoreCase));
            if (matchingArg != null)
            {
                return matchingArg.Value;
            }

            // Finally, look for the first unnamed argument.
            var filteredParameters = parameters.Where(x => string.IsNullOrWhiteSpace(x.Key)).OrderBy(x => x.Index);
            var firstUnnamed = filteredParameters.FirstOrDefault();
            return (firstUnnamed == null) ? string.Empty : firstUnnamed.Value;
        }

        /// <summary>
        /// Because of how R operates, we allow more flexibility such that anything
        /// can be considered a value or a table.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Always returns true</returns>
        public override bool IsValueDisplay(string command)
        {
            return true;
        }

        /// <summary>
        /// Not used (see IsValueDisplay)
        /// </summary>
        /// <param name="command"></param>
        /// <returns>string.Empty</returns>
        public override string GetValueName(string command)
        {
            return string.Empty;
        }

        /// <summary>
        /// Because of how R operates, we allow more flexibility such that anything
        /// can be considered a value or a table.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Always returns true</returns>
        public override bool IsTableResult(string command)
        {
            return true;
        }

        /// <summary>
        /// Not used (see IsTableResult)
        /// </summary>
        /// <param name="command"></param>
        /// <returns>string.Empty</returns>
        public override string GetTableName(string command)
        {
            return string.Empty;
        }

        /// <summary>
        /// To ensure the R API gets the full command, we need to collapse multi-line commands.
        /// This assumes trailing comments have been stripped already.
        /// </summary>
        /// <param name="originalContent">An array of command lines</param>
        /// <returns>An array of commands with multi-line commands on a single line.  The size will be &lt;= the size of originalContent</returns>
        public string[] CollapseMultiLineCommands(string[] originalContent)
        {
            var originalText = string.Join("\r\n", originalContent);
            var modifiedText = originalText;
            int openCount = 0;
            int closedCount = 0;
            int currentStart = -1;
            int currentEnd = -1;

            // Find opening paren, if any exists.
            var next = modifiedText.IndexOf("(", StringComparison.CurrentCultureIgnoreCase);
            while (next > -1)
            {
                if (modifiedText[next] == '(')
                {
                    openCount++;

                    if (openCount == 1)
                    {
                        currentStart = next;
                    }
                }
                else
                {
                    closedCount++;
                    currentEnd = next;
                }

                // Do we have a balanced match?  If so, we will take this range and clear out newlines.
                if (openCount == closedCount)
                {
                    openCount = 0;
                    closedCount = 0;

                    // Re-compose the string.  Note that we independently replace \r and \n with a space.  This allows us
                    // to 1) handle different line endings, and 2) preserve indices so we don't have to account for shrinking
                    // strings.
                    modifiedText = modifiedText.Substring(0, currentStart) +
                                   modifiedText.Substring(currentStart, (currentEnd - currentStart))
                                       .Replace("\r", " ")
                                       .Replace("\n", " ") + modifiedText.Substring(currentEnd);
                }

                next = modifiedText.IndexOfAny(new[] { '(', ')' }, next + 1);
            }
            return modifiedText.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToArray();
        }

        public override List<string> PreProcessContent(List<string> originalContent)
        {
            if (originalContent == null || originalContent.Count == 0)
            {
                return new List<string>();
            }

            var originalText = string.Join("\r\n", originalContent);

            var modifiedText = CodeParserUtil.StripTrailingComments(originalText);
            // Ensure all multi-line function calls are collapsed
            return modifiedText.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
        }
    }
}
