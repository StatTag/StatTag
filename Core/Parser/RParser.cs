using System;
using System.Collections.Generic;
using System.IO;
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
        public static readonly string[] TableCommands = {"write.csv", "write.csv2", "write.table"};
        private static readonly Regex TableRegex = new Regex(string.Format("^\\s*(?:{0})\\s*\\((\\s*?[\\s\\S]*)\\)", string.Join("|", TableCommands)));
        private static readonly Regex MultiLinePlotRegex = new Regex("\\)\\s*\\+[ \t]*[\\r\\n]+[ \t]*");
        private static readonly Regex MultiLinePipeRegex = new Regex("(%(?:[<T]?>|\\$)%)[ \t]*[\\r\\n]+[ \t]*");
        private static readonly Regex PrintRegex = new Regex("^\\s*print\\s*\\(", RegexOptions.Multiline);
        //private static readonly Regex TableKeywordRegex = new Regex(string.Format("^\\s*{0}\\b[\\S\\s]*file", FormatCommandListAsNonCapturingGroup(TableCommands)), RegexOptions.IgnoreCase);
        //private static readonly Regex TableRegex = new Regex(string.Format("^\\s*{0}\\b[\\S\\s]*file\\s*=\\s*[\"'](.*?)[\"'][\\S\\s]*;", FormatCommandListAsNonCapturingGroup(TableCommands)), RegexOptions.IgnoreCase);
        private const char KeyValueDelimiter = '=';
        private const char ArgumentDelimiter = ',';
        private const char CommandDelimiter = ';';
        private const string ImageFileParameterName = "filename";
        private const string TableFileParameterName = "file";

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
                    // If we are in an open double-quote, the single quote should be ignored as it is most likely
                    // part of a path.  We will detect that situation, and continue processing the next character.
                    if (isInQuote && doubleQuoteCounter > 0)
                    {
                        continue;
                    }

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
            return GetSaveLocation(command, FigureRegex, ImageFileParameterName, 1);
        }

        /// <summary>
        /// This will return the exact parameter that represents a file save location.  This may be an R function (e.g., paste)
        /// to construct the file path, a variable, or a string literal.  String literals will include enclosing quotes.  This is
        /// because the output of this method is sent to R as an expression for evaluation.  That way R handles converting everything
        /// into an exact file path that we can then utilize.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="regex"></param>
        /// <param name="fileParameter"></param>
        /// <param name="fileParameterIndex"></param>
        /// <returns></returns>
        private string GetSaveLocation(string command, Regex regex, string fileParameter, int fileParameterIndex)
        {
            // In order to account for a command string that actually has multiple commands embedded in it, we want to find the right
            // fragment that has the image command.  We will take the first one we find.
            Match match = null;
            var commandLines = command.Split(CommandDelimiter);
            foreach (var commandLine in commandLines)
            {
                match = regex.Match(commandLine);
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
                    x => x.Key.Equals(fileParameter, StringComparison.CurrentCultureIgnoreCase));
            if (matchingArg != null)
            {
                return matchingArg.Value;
            }

            // Next look by prefix matching
            matchingArg =
                parameters.FirstOrDefault(
                    x =>
                        !string.IsNullOrWhiteSpace(x.Key) &&
                        fileParameter.StartsWith(x.Key, StringComparison.CurrentCultureIgnoreCase));
            if (matchingArg != null)
            {
                return matchingArg.Value;
            }

            // Finally, look for the nth unnamed argument (defined in the method parameters)
            var filteredParameters = parameters.Where(x => string.IsNullOrWhiteSpace(x.Key)).OrderBy(x => x.Index);
            var firstUnnamed = filteredParameters.Skip(fileParameterIndex - 1).FirstOrDefault();
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

        public override string GetTableName(string command)
        {
            return null;
        }

        /// <summary>
        /// This will return the exact parameter that represents the save location of a file written in R.  This may be an R function (e.g., paste)
        /// to construct the file path, a variable, or a string literal.  String literals will include enclosing quotes.  This is
        /// because the output of this method is sent to R as an expression for evaluation.  That way R handles converting everything
        /// into an exact file path that we can then utilize.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>A string containing the file path, if a file path is specified in the command.  If not, an empty string is returned.</returns>
        public override string GetTableDataPath(string command)
        {
            var result = GetSaveLocation(command, TableRegex, TableFileParameterName, 2);
            return result;
        }

        /// <summary>
        /// Determine if a given command has within it a call to R's print() function.
        /// This does not assume correct/valid R is present, only detecting that the
        /// start of the print function is invoked.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool CommandContainsPrint(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return PrintRegex.IsMatch(command);
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

            // Take any plotting commands that span multiple lines and string them onto a single line.
            modifiedText = MultiLinePlotRegex.Replace(modifiedText, ") + ");
            // Likewise, take any piped commands and string them onto a single line.
            modifiedText = MultiLinePipeRegex.Replace(modifiedText, "$1 ");

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

        /// <summary>
        /// Internal function to take a string and parse it, removing any trailing comments from the end of each command line.
        /// </summary>
        /// <param name="text">The command string to process</param>
        /// <returns>The command string with all trailing comments replaced with spaces</returns>
        private string StripTrailingComments(string text)
        {
            var characters = text.ToCharArray();
            bool inString = false;
            bool inComment = false;
            bool isEscaped = false;
            char stringChar = ' ';
            for (int index = 0; index < characters.Length; index++)
            {
                var chr = characters[index];

                // We will first handle escaped character scenarios.  Note that if we know we are in a comment, we want
                // to bypass any of these checks.  That will force our escaped text within the comment to get cleared
                // out (which is our desired goal).
                if (!inComment)
                {
                    // If we previously saw the escape character, we're going to skip this one and reset the escaped
                    // character flag.
                    if (isEscaped)
                    {
                        isEscaped = false;
                        continue;
                    }

                    // Start by looking for the escape character.  If we find it, set our flag so we know to skip the next
                    // character from special handling.
                    if (chr == '\\')
                    {
                        isEscaped = true;
                        continue;
                    }
                }

                // Next, set the state of our parsing of the string.  Detect different scenarios with quotes and comment
                // markers, as well as newlines, which modify our parsing state.
                switch (chr)
                {
                    case '\'':
                    case '"':
                        // We've found a quote char.  We can only consider ourselves in a string if we're not
                        // in a comment and if we're not in a string or we didn't find the matching closing quote.
                        inString = (!inComment && (!inString || chr != stringChar));
                        if (inString)
                        {
                            stringChar = chr;
                            continue;
                        }
                        break;
                    case '#':
                        // Here's our comment character - if we're not in a string, then we are in a comment at this
                        // point and need to start tracking that.
                        if (!inString)
                        {
                            inComment = true;
                        }
                        break;
                    case '\r':
                    case '\n':
                        // We're no longer in a comment or string once we find the newline
                        inComment = false;
                        inString = false;
                        continue;
                }

                // Finally - if we reach this point and we are in a comment we want to clean it out.  This is tightly
                // coupled to other behavior in PreProcessContent which is going to trim strings.  However, by setting
                // the character to a space and relying on the trim, we simplify this work (since we don't need to
                // resize the string or build a new one).
                if (inComment)
                {
                    characters[index] = ' ';
                }
            }
            return new string(characters).Trim();
        }

        /// <summary>
        /// Override to handle processing of the lines of code associated with an execution step before it is
        /// sent to the R engine.
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public override string[] PreProcessExecutionStepCode(ExecutionStep step)
        {
            if (step == null || step.Code == null)
            {
                return null;
            }

            var code = step.Code.ToArray();
            if (code.Length == 0)
            {
                return code;
            }

            for (int index = 0; index < code.Length; index++)
            {
                // If we have a tag associated with this execution step, we don't want to strip out our StatTag comments.
                // We'll detect that scenario, and skip any processing to ensure the comments are preserved.
                if (step.Tag != null && (IsTagStart(code[index]) || IsTagEnd(code[index])))
                {
                    continue;
                }
                code[index] = StripTrailingComments(code[index]);
            }

            step.Code = new List<string>(code);
            return base.PreProcessExecutionStepCode(step);
        }

        /// <summary>
        /// Override to handle processing of the code file (en masse) before other processing is done.
        /// </summary>
        /// <param name="originalContent"></param>
        /// <param name="automation"></param>
        /// <returns></returns>
        public override List<string> PreProcessContent(List<string> originalContent, IStatAutomation automation = null)
        {
            if (originalContent == null || originalContent.Count == 0)
            {
                return new List<string>();
            }

            return originalContent.Select(x => x.Trim()).ToList();
        }
    }
}
