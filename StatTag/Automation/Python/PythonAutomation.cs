using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using JupyterKernelManager;
using StatTag.Core.Exceptions;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using StatTag.Core.Parser;
using StatTag.Core.Utility;

namespace Jupyter
{
    public class PythonAutomation : JupyterAutomation
    {
        protected override sealed ICodeFileParser Parser { get; set; }

        public const int MAX_ARRAY_DEPTH = 2;
        private const char ArrayStart = '[';
        private const char ArrayEnd = ']';
        private const char Escape = '\\';
        private const char Delimiter = ',';
        private const char SingleQuote = '\'';
        private const char DoubleQuote = '"';

        private static readonly Regex ValidArrayString = new Regex("^\\s*\\[.*\\]\\s*$", RegexOptions.Multiline | RegexOptions.Singleline);

        private static readonly Dictionary<string, string> PythonProfileCommands = new Dictionary<string, string>()
        {
            { "import sys; print(sys.version)", "Version" },
            { "sys.executable", "Path" }
        };

        protected Configuration Config { get; set; }

        public PythonAutomation(Configuration config) : base(Configuration.DefaultPythonKernel)
        {
            Config = config;
            Parser = new PythonParser();
            SelectKernel();
        }

        /// <summary>
        /// Find the first installed kernel that matches our configured whitelist of Python kernels.
        /// </summary>
        private void SelectKernel()
        {
            var specs = new KernelSpecManager().GetAllSpecs().Keys;
            foreach (var kernel in Config.PythonKernels)
            {
                var match = specs.FirstOrDefault(x => x.Equals(kernel, StringComparison.CurrentCultureIgnoreCase));
                if (match != null)
                {
                    KernelName = match;
                    return;
                }
            }

            // It shouldn't have changed, but we will set the kernel explicitly to the default just in case.
            KernelName = Configuration.DefaultPythonKernel;
        }

        /// <summary>
        /// Provides information about the Python installation on the user's machine.  This uses an instance of the 
        /// automation object to issue commands in order to gather system information.
        /// </summary>
        /// <param name="config">The configuration object needed to initialize the automation object</param>
        /// <returns>A formatted string of system information, which can be displayed to the user</returns>
        public static string InstallationInformation(Configuration config)
        {
            var builder = new StringBuilder();
            try
            {
                var engine = new PythonAutomation(config);
                if (engine.Initialize(null, new LogManager()))
                {
                    foreach (var command in PythonProfileCommands)
                    {
                        var result = engine.RunCommand(command.Key, new Tag() { Type = Constants.TagType.Value });
                        if (result != null && result.ValueResult != null)
                        {
                            builder.AppendFormat("{0} : {1}\r\n", command.Value, string.Join("\r\n", result.ValueResult.Trim()));
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                builder.AppendFormat(
                    "Unable to communicate with Python. Python or its kernel may not be installed or there might be other configuration issues.\r\n");
                builder.AppendFormat("{0}\r\n", exc.Message);
            }

            return builder.ToString().Trim();
        }

        public override CommandResult[] RunCommands(string[] commands, Tag tag = null)
        {
            // If there is no tag, and we're just running a big block of code, it's much easier if we can send that to
            // the R engine at once.  Otherwise we have to worry about collapsing commands, function definitions, etc.
            if (tag == null)
            {
                commands = new[] { string.Join("\r\n", commands) };
            }
            else
            {
                // commands = ((RParser)Parser).CollapseMultiLineCommands(commands);

                // When processing a tag, we need to keep it so the tag comments are at the beginning and end of the
                // command array, and all actual code then needs to live in the middle in a single (combined) string.
                // This is because Jupyter won't do incremental code execution, we need to send it our full block of
                // commands at once, in a single string.
                commands = CollapseTagCommandsArray(commands);
            }

            return base.RunCommands(commands, tag);
        }

        public override CommandResult HandleImageResult(Tag tag, string command, List<Message> result)
        {
            // If it's not an image tag, we won't even try to do any other checks.
            if (tag.Type != Constants.TagType.Figure)
            {
                return null;
            }

            // If it's not the start tag (which wouldn't have results), try to get an image result
            if (!Parser.IsTagStart(command))
            {
                // First, try pulling out a base64-encoded image from the response.  If that doesn't work, 
                // check to see if there was a saved file.
                var commandResult = base.HandleImageResult(tag, command, result);
                if (commandResult != null)
                {
                    return commandResult;
                }

                if (Parser.IsImageExport(command))
                {
                    // Attempt to extract the save location (either a file name, relative path, or absolute path)
                    // If it is empty, we will assign one to the image based on the tag name, and use that so
                    // the image is properly imported.
                    var saveLocation = Parser.GetImageSaveLocation(command);
                    if (string.IsNullOrWhiteSpace(saveLocation))
                    {
                        saveLocation = "\"tmp\"";
                    }
                    return new CommandResult() { FigureResult = GetExpandedFilePath(saveLocation) };

                }
            }

            return null;
        }

        /// <summary>
        /// Return an expanded, full file path - accounting for variables, functions, relative paths, etc.
        /// </summary>
        /// <param name="saveLocation">An R command that will be translated into a file path.</param>
        /// <returns>The full file path</returns>
        protected string GetExpandedFilePath(string saveLocation)
        {
            //var fileLocation = RunCommand(saveLocation, new Tag() { Type = Constants.TagType.Value });
            var fileLocation = RunCommand(string.Format("print({0})", saveLocation), new Tag() { Type = Constants.TagType.Value });
            if (fileLocation == null)
            {
                return null;
            }

            var baseParser = (BaseParser) Parser;
            if (baseParser != null && baseParser.IsRelativePath(fileLocation.ValueResult))
            {
                // Attempt to find the current working directory.  If we are not able to find it, or the value we end up
                // creating doesn't exist, we will just proceed with whatever image location we had previously.
                var workingDirResult = RunCommand("import os; print(os.getcwd())", new Tag() { Type = Constants.TagType.Value });
                if (workingDirResult != null)
                {
                    var path = workingDirResult.ValueResult;
                    var correctedPath = Path.GetFullPath(Path.Combine(path, fileLocation.ValueResult));
                    if (File.Exists(correctedPath))
                    {
                        fileLocation.ValueResult = correctedPath;
                    }
                }
            }

            return fileLocation.ValueResult;
        }

        /// <summary>
        /// For a tag, handle the results from processing a command to determine if a table result can be derived.
        /// </summary>
        /// <param name="tag">The tag we are processing (if applicable)</param>
        /// <param name="command">The Python command that was run</param>
        /// <param name="result">A collection of Jupyter Message objects representing the results</param>
        /// <returns>A Table object containing the table data, if a table can be extracted.  Null otherwise.</returns>
        public override CommandResult HandleTableResult(Tag tag, string command, List<Message> result)
        {
            if (tag.Type.Equals(Constants.TagType.Table) && !Parser.IsTagStart(command) && Parser.IsTableResult(command))
            {
                var message = result.FirstOrDefault();
                var htmlValue = GetHtmlValueResult(message);
                if (string.IsNullOrWhiteSpace(htmlValue))
                {
                    return new CommandResult() {TableResult = ParseTableResult(GetTextValueResult(message))};
                }
                else
                {
                    return new CommandResult() { TableResult = ParseHtmlTableResult(htmlValue) };
                }
                
            }

            return null;
        }

        /// <summary>
        /// Given a string containing HTML, extract a balanced table.
        /// NOTE: This does not account for rowspan/colspan!
        /// </summary>
        /// <param name="valueString"></param>
        /// <returns></returns>
        public Table ParseHtmlTableResult(string valueString)
        {
            if (string.IsNullOrWhiteSpace(valueString))
            {
                return new Table();
            }

            var htmlFragment = new HtmlDocument();
            htmlFragment.LoadHtml(valueString);
            var table = htmlFragment.DocumentNode.SelectSingleNode(".//table");
            if (table == null)
            {
                // If there is no table node present, we will return an empty table instead of an error.
                return new Table();
            }

            // There's a lot of variation in tables - they can have THEAD and TBODY or not.  They can use TH instead of TD in different places.
            // We need to account for all of those scenarios.
            var rows = table.SelectNodes(".//tr");
            if (rows == null || rows.Count == 0)
            {
                return new Table();
            }

            // Keep in mind that rows and columns can be uneven.  We're going to start with the assumption that the rows are right, and determine
            // which column count (the max) best represents the total for this data.
            int numRows = rows.Count;
            int maxCols = 0;
            var data = new List<List<string>>();
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                var row = rows[rowIndex];
                var cols = row.SelectNodes("./td | ./th");
                maxCols = Math.Max(maxCols, cols.Count);
                data.Add(new List<string>(cols.Count));
                foreach (var col in cols)
                {
                    data[rowIndex].Add(FormatStringFromHtml(col.GetDirectInnerText()));
                }
            }

            var dataTable = new Table(numRows, maxCols, TableUtil.MergeTableVectorsToArray(null, null, FlattenDataToArray(numRows, maxCols, data), numRows, maxCols));
            return dataTable;
        }

        /// <summary>
        /// Take a string result from the Python kernel and convert it into a Table structure that StatTag
        /// can use to populate a table in Word.
        /// </summary>
        /// <param name="valueString">The string result from the Python kernel</param>
        /// <returns>A populated Table structure</returns>
        public Table ParseTableResult(string valueString)
        {
            // If the string has no data, we want to safely return an empty table.  Similarly, if the table string does not start with
            // an opening array char, we don't know how to process it.
            if (string.IsNullOrWhiteSpace(valueString) || !ValidArrayString.IsMatch(valueString))
            {
                return new Table(0, 0, null);
            }

            // Go through each character and process the state change for each symbol, collecting data
            // along the way.  And yes, we are ignoring the nuances of Python collections and just referring
            // to them as "arrays" within the code.
            int arrayDepth = 0;
            int rows = 0;
            int cols = 0;
            int maxCols = 0;
            string currentValue = "";
            char? activeQuoteChar = null;
            bool rowDataTracked = false;
            bool inOpenArray = false;
            bool isEscaped = false;
            var data = new List<List<string>>();
            foreach (var letter in valueString.ToCharArray())
            {
                if (isEscaped)
                {
                    isEscaped = false;
                    // Fall through so we pick up the escaped character
                }
                else if (letter.Equals(Escape))
                {
                    isEscaped = true;
                    continue;
                }
                else if (letter.Equals(ArrayStart))
                {
                    rowDataTracked = false;
                    inOpenArray = true;
                    currentValue = "";
                    arrayDepth++;
                    cols = 0;

                    if (arrayDepth > MAX_ARRAY_DEPTH)
                    {
                        throw new StatTagUserException("StatTag is only able to handle 2-dimensional collections within Python");
                    }
                    continue;
                }
                else if (letter.Equals(ArrayEnd))
                {
                    // Close out the curent array if we have some data.
                    if (rowDataTracked)
                    {
                        cols++;
                        maxCols = Math.Max(maxCols, cols);
                        data[rows - 1].Add(currentValue);
                    }
                    currentValue = "";
                    arrayDepth--;
                    cols = 0;
                    inOpenArray = false;
                    rowDataTracked = false;
                    continue;
                }
                else if (letter.Equals(Delimiter))
                {
                    // If we are outside of the array and find a delimiter between sub-arrays, we don't want to track this as
                    // having found data.
                    if (!inOpenArray)
                    {
                        continue;
                    }

                    cols++;
                    maxCols = Math.Max(maxCols, cols);
                    data[rows - 1].Add(currentValue);
                    currentValue = "";
                    continue;
                }
                else if ((letter.Equals(SingleQuote) || letter.Equals(DoubleQuote)))
                {
                    if (!activeQuoteChar.HasValue)
                    {
                        // We have come across a quote of some sort, and we're 
                        activeQuoteChar = letter;
                        continue;
                    }
                    else if (activeQuoteChar.Value.Equals(letter))
                    {
                        // Only close out the quote if it's a match.
                        activeQuoteChar = null;
                        continue;
                    }

                    // Otherwise, fall through and pick up the quote character because it's part of a string literal
                    // that we are tracking.
                }
                else if ((letter.Equals(' ') || letter.Equals('\t')) && !activeQuoteChar.HasValue)
                {
                    // If we're not in a quoted string, we don't want to capture any whitespace.  Because we need to
                    // respect the whitespace if it's quoted, we can't just trim the string at the end, so we have this
                    // check in place.
                    continue;
                }

                // If we haven't ruled it out, this character is part of a value that we are tracking and should
                // be included in the string.
                currentValue += letter;

                // If this is our first time tracking data for our row, we need to initialize it
                if (!rowDataTracked)
                {
                    rowDataTracked = true;
                    rows++;
                    data.Add(new List<string>());
                }
            }

            var table = new Table(rows, maxCols, TableUtil.MergeTableVectorsToArray(null, null, FlattenDataToArray(rows, maxCols, data), rows, maxCols));
            return table;
        }

        /// <summary>
        /// Because of how we built StatTag, we have some expectations and existing routines to flatten data to a 1D array.
        /// This is the implementation for Python results.  In the future we should consider revisiting this to avoid
        /// the extra processing.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string[] FlattenDataToArray(int rows, int cols, List<List<string>> data)
        {
            if (rows == 0 || cols == 0)
            {
                return null;
            }

            var flattenedData = new string[rows*cols];
            for (int row = 0; row < rows; row++)
            {
                var rowCols = data[row].Count;
                for (int col = 0; col < cols; col++)
                {
                    // Not every row is guaranteed to have the same number of columns.  We will perform checks to see if
                    // that's the case, and if so we will provide a null placeholder.
                    flattenedData[(row * cols) + col] = col >= rowCols ? null : data[row][col];
                }
            }

            return flattenedData;
        }
    }
}
