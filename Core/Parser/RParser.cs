using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using StatTag.Core.Utility;

namespace StatTag.Core.Parser
{
    public class RParser : BaseParser
    {
        public static readonly string[] FigureCommands = new[] { "pdf", "win.metafile", "png", "jpeg", "bmp", "postscript" };
        private static readonly Regex FigureRegex = new Regex(string.Format("^\\s*(?:{0})\\s*\\((\\s*?[\\s\\S]*)\\)", string.Join("|", FigureCommands)));
        private const char KeyValueDelimiter = '=';
        private const char ArgumentDelimiter = ',';
        private const char CommandDelimiter = ';';
        private const string FileParameterName = "filename";

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
        /// Split apart a string containing the function parameters into individual parameter components.  These may be
        /// named or unnamed parameters, so the positional index is important.
        /// </summary>
        /// <remarks>This method assumes we have stripped away the outer function call, and that all we have left are the
        /// actual parameters sent to that function call.  We need to account for a few things:
        /// 1. Named parameters (e.g., a = b)
        /// 2. Functions as parameters (e.g., max(c))
        /// 3. String parameters with parameter list-related characters in them (e.g., "my, test.pdf")
        /// 4. Combinations of 1-3</remarks>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private List<FunctionParam> ParseFunctionParameters(string arguments)
        {
            // Move along the sequence of characters until we reach a delimiter (",") or the end of the string
            int parameterIndex = 0;
            bool isInQuote = false;
            int functionCounter = 0;
            bool isInFunction = false;
            int parameterStartIndex = 0;
            int? parameterNameDelimiterIndex = null;
            int doubleQuoteCounter = 0;
            int singleQuoteCounter = 0;
            bool isNamedParameter = false;
            bool noState = true; // Set at the beginning, just to track we haven't done any other tracking
            var parameters = new List<FunctionParam>();
            for (int index = 0; index < arguments.Length; index++)
            {
                char argChar = arguments[index];
                if (argChar == '\'')
                {
                    isInQuote = true;
                    singleQuoteCounter++;
                    if (singleQuoteCounter%2 == 0)
                    {
                        // We have closed out the single quote.  If we are not also in a double
                        // quote sequence, we can reset our quote tracker.
                        if (doubleQuoteCounter == 0)
                        {
                            isInQuote = false;
                        }

                    }
                }
                else if (argChar == '"')
                {
                    isInQuote = true;
                    doubleQuoteCounter++;
                    if (doubleQuoteCounter%2 == 0)
                    {
                        // We have closed out the double quote.  If we are not also in a single
                        // quote sequence, we can reset our quote tracker.
                        if (singleQuoteCounter == 0)
                        {
                            isInQuote = false;
                        }
                    }
                }
                else if (argChar == '(')
                {
                    functionCounter++;
                    isInFunction = true;
                }
                else if (isInFunction && argChar == ')')
                {
                    functionCounter--;
                    isInFunction = (functionCounter != 0);
                }

                // If we are in a quote or in a function, we are not going to allow processing other characters (since they
                // should be treated as literal characters).
                if (isInQuote)
                {
                    continue;
                }
                
                if (argChar == KeyValueDelimiter && !isInFunction)
                {
                    isNamedParameter = true;
                    parameterNameDelimiterIndex = index;
                }
                // Don't allow the argument delimiter to be processed if we are in the middle of a function, since we want
                // to keep that function in its entirety as a parameter for the image command.
                else if (((!isInFunction) && argChar == ArgumentDelimiter) || argChar == CommandDelimiter || index == (arguments.Length - 1))
                {
                    int valueEndIndex = (index == (arguments.Length - 1)) ? arguments.Length : index;
                    int valueStartIndex = (isNamedParameter ? (parameterNameDelimiterIndex.Value + 1) : parameterStartIndex);

                    // We're at an ending sequence, and we need to close out what we've been tracking.
                    parameters.Add(new FunctionParam()
                    {
                        Index = parameterIndex,
                        Key = (isNamedParameter ? arguments.Substring(parameterStartIndex, (parameterNameDelimiterIndex.Value - parameterStartIndex)) : "").Trim(),
                        Value = arguments.Substring(valueStartIndex, (valueEndIndex - valueStartIndex)).Trim()
                    });

                    // Reset our state, since we are going to begin on a new parameter (or be done)
                    isNamedParameter = false;
                    doubleQuoteCounter = 0;
                    singleQuoteCounter = 0;
                    parameterStartIndex = index + 1;
                    parameterNameDelimiterIndex = null;
                    parameterIndex++;
                }
            }

            return parameters;
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
            // In order to account for a command string that actually has multiple commands embedded in it, we want to find the right
            // fragment that has the image command.  We will take the first one we find.
            Match match = null;
            var commandLines = command.Split(CommandDelimiter);
            foreach (var commandLine in commandLines)
            {
                match = FigureRegex.Match(commandLine);
                if (match.Success)
                {
                    break;
                }
            }

            // If there was no successful match, there's no known image command that we should try to process so we'll exit
            // and return an empty string.
            if (match == null || !match.Success)
            {
                return string.Empty;
            }

            var arguments = match.Groups[1].Value;
            var parameters = ParseFunctionParameters(arguments);

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
