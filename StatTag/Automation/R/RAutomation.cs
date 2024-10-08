﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Jupyter;
using JupyterKernelManager;
using Microsoft.Win32;
using StatTag.Core.Exceptions;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using StatTag.Core.Parser;
using StatTag.Core.Utility;
using StatTag.Models;

namespace R
{
    public class RAutomation : JupyterAutomation
    {
        public const int MAX_ARRAY_DEPTH = 2;
        private const char ArrayStart = '[';
        private const char ArrayEnd = ']';
        private const char Escape = '\\';
        private const char Delimiter = ',';
        private const char SingleQuote = '\'';
        private const char DoubleQuote = '"';
        private static readonly Regex ValidArrayString = new Regex("^\\s*\\[.*\\]\\s*$", RegexOptions.Multiline | RegexOptions.Singleline);
        private static readonly char[] Quotes = { SingleQuote, DoubleQuote };
        private static readonly char[] LibPathTrimChars = { '\r', '\n', '\t', ' ', '"' };

        private bool IsInitialized = false;

        private const string TemporaryImageFileFilter = "*.png";
        private const string BRACKETED_NA_VALUE = "<NA>";
        private const string NA_VALUE = "NA";
        private const string R_INSTALL_PATH_KEY = "InstallPath";

        private static readonly Dictionary<string, string> RProfileCommands = new Dictionary<string, string>()
        {
            { "sessionInfo()", "Session" },
            { "R.home()", "R Home" },
            { ".libPaths()",  "Lib Path" },
            { "with(as.data.frame(installed.packages(fields = c(\"Package\", \"Version\"))), paste(Package, Version, sep = \"\", collapse = \", \" ))", "Packages" }
        };

        private static readonly Regex STRIP_RESULT_RESPONSE_NUMBER = new Regex("^(?:\\[\\d+\\]\\s*)(.*)");

        private string TemporaryImageFilePath = "";

        protected override sealed ICodeFileParser Parser { get; set; }

        protected Configuration Config { get; set; }
        public RAutomation(Configuration config) : base(Configuration.DefaultPythonKernel)
        {
            Config = config;
            Parser = new RParser();
            SelectKernel();
        }

        /// <summary>
        /// Find the first installed kernel that matches our configured whitelist of R kernels.
        /// </summary>
        private void SelectKernel()
        {
            var specs = new KernelSpecManager().GetAllSpecs().Keys;
            foreach (var kernel in Config.RKernels)
            {
                var match = specs.FirstOrDefault(x => x.Equals(kernel, StringComparison.CurrentCultureIgnoreCase));
                if (match != null)
                {
                    KernelName = match;
                    return;
                }
            }

            // It shouldn't have changed, but we will set the kernel explicitly to the default just in case.
            KernelName = Configuration.DefaultRKernel;
        }

        /// <summary>
        /// Provides information about the R installation on the user's machine.  This uses an instance of the 
        /// automation object to issue commands in order to gather system information.
        /// </summary>
        /// <param name="config">The configuration object needed to initialize the automation object</param>
        /// <returns>A formatted string of system information, which can be displayed to the user</returns>
        public static CheckResult InstallationInformation(Configuration config)
        {
            var builder = new StringBuilder();
            var infoResult = new CheckResult();
            try
            {
                var engine = new RAutomation(config);
                if (engine.Initialize(null, new LogManager()))
                {
                    foreach (var command in RProfileCommands)
                    {
                        var result = engine.RunCommand(command.Key, new Tag() { Type = Constants.TagType.Value });
                        if (result != null && result.ValueResult != null)
                        {
                            builder.AppendFormat("{0} : {1}\r\n", command.Value, string.Join("\r\n", result.ValueResult.Trim()));
                        } else
                        {
                            builder.AppendFormat("{0} : returned an empty result\r\n", command.Value);
                        }
                    }
                    infoResult.Result = true;
                }
                else
                {
                    builder.AppendFormat("No R environment with IRkernel support could be found");
                    infoResult.Result = false;
                }
            }
            catch (Exception exc)
            {
                builder.AppendFormat(
                    "Unable to communicate with R. R may not be installed or there might be other configuration issues.\r\n");
                builder.AppendFormat("{0}\r\n", exc.Message);
                infoResult.Result = false;
            }

            infoResult.Details = builder.ToString().Trim();
            return infoResult;
        }

        public override bool Initialize(CodeFile file, LogManager logger)
        {
            var baseInitialized = base.Initialize(file, logger);
            if (baseInitialized && !IsInitialized)
            {
                try
                {
                    logger.WriteMessage("Preparing to configure R environment...");

                    // Set the working directory to the location of the code file, if it is provided and the
                    // R engine has been initialized.
                    if (file != null)
                    {
                        var path = Path.GetDirectoryName(file.FilePath);
                        if (!string.IsNullOrEmpty(path))
                        {
                            logger.WriteMessage(string.Format("Changing working directory to {0}", path));
                            path = path.Replace("\\", "\\\\").Replace("'", "\\'");
                            RunCommand(string.Format("setwd('{0}')", path), new Tag() { Type = Constants.TagType.Value });  // Escape the path for R
                        }
                    }

                    TemporaryImageFilePath = AutomationUtil.InitializeTemporaryImageDirectory(file, logger);

                    // Now, set up the R environment so it uses the PNG graphic device by default (if no other device
                    // is specified).
                    logger.WriteMessage("Setting R option for default graphics device");
                    RunCommands(new[]
                    {
                        string.Format(".stattag_png = function() {{ png(filename=paste(\"{0}\\\\\", \"StatTagFigure%03d.png\", sep=\"\")) }}",
                            TemporaryImageFilePath.Replace("\\", "\\\\").Replace("\"", "\\\"")),
                        "options(device=\".stattag_png\")"
                    });
                    logger.WriteMessage("Updated R option for default graphics device");

                    logger.WriteMessage("Completed R initialization");
                    IsInitialized = true;
                }
                catch (Exception exc)
                {
                    logger.WriteMessage("Caught an exception while trying to initalize R");
                    logger.WriteException(exc);
                    State.WorkingDirectorySet = false;
                    IsInitialized = false;
                    return false;
                }
            }

            return baseInitialized;
        }

        public override void Dispose()
        {
            if (Manager != null)
            {
                // Part of our cleanup is ensuring all graphic devices are closed out.  Not everyone will do this
                // in their code, and if not it can cause our process to stay running.
                try
                {
                    RunCommand("graphics.off()");
                }
                catch (Exception exc)
                {
                    // We are attempting to close graphic devices, but if it fails, it may not be catastrophic.
                    // For now, we are supressing notification to the user, since it may be a false alarm.
                }
            }

            CleanTemporaryImageFolder(true);
            base.Dispose();
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
                commands = ((RParser)Parser).CollapseMultiLineCommands(commands);

                // When processing a tag, we need to keep it so the tag comments are at the beginning and end of the
                // command array, and all actual code then needs to live in the middle in a single (combined) string.
                // This is because Jupyter won't do incremental code execution, we need to send it our full block of
                // commands at once, in a single string.
                commands = CollapseTagCommandsArray(commands);
            }

            var commandResults = new List<CommandResult>();
            bool isVerbatimTag = (tag != null && tag.Type == Constants.TagType.Verbatim);
            bool isFigureTag = (tag != null && tag.Type == Constants.TagType.Figure);
            foreach (var command in commands)
            {
                if (Parser.IsTagStart(command))
                {
                    // Start the verbatim logging cache, if that is what the user wants for this output.
                    if (isVerbatimTag)
                    {
                        VerbatimResultCache = new List<string>();
                        VerbatimResultCacheEnabled = true;
                    }
                    // If we're going to be doing a figure, we want to clean out the old images so we know
                    // exactly which ones we're writing to.
                    else if (isFigureTag)
                    {
                        CleanTemporaryImageFolder();
                    }

                    // No need to process the code if it's a StatTag tag
                    continue;
                }

                var result = RunCommand(command, tag);
                if (result != null && !result.IsEmpty() && !isVerbatimTag)
                {
                    commandResults.Add(result);
                }
                else if (Parser.IsTagEnd(command))
                {
                    if (isVerbatimTag && VerbatimResultCacheEnabled)
                    {
                        VerbatimResultCacheEnabled = false;
                        commandResults.Add(new CommandResult()
                        {
                            VerbatimResult = string.Join("\r\n", VerbatimResultCache)
                        });
                    }
                    // If this is the end of a figure tag, only proceed with this temp file processing if we don't
                    // already have a figure result of some sort.
                    else if (isFigureTag && !commandResults.Any(x => !string.IsNullOrWhiteSpace(x.FigureResult)))
                    {
                        // Make sure the graphics device is closed. We're going with the assumption that a device
                        // was open and a figure was written out.
                        RunCommand("graphics.off()");

                        // If we don't have the file specified normally, we will use our fallback mechanism of writing to a
                        // temporary directory.
                        var files = Directory.GetFiles(TemporaryImageFilePath, TemporaryImageFileFilter);
                        if (files.Length == 0)
                        {
                            continue;
                        }

                        // Find the last file in the directory.  We anticipate there would normally only be 1, but since 
                        // several commands could be run, we will just take the last one.
                        var tempImageFile = files.OrderBy(x => x).Last();
                        var correctedPath = Path.GetFullPath(Path.Combine(TemporaryImageFilePath, ".."));
                        var imageFile = Path.Combine(correctedPath, string.Format("{0}.png", TagUtil.TagNameAsFileName(tag)));
                        File.Copy(tempImageFile, imageFile, true);

                        commandResults.Add(new CommandResult() {FigureResult = imageFile});
                    }
                }
            }

            return commandResults.ToArray();
        }

        private CommandResult CleanPathInResult(CommandResult result)
        {
            if (result != null && result.ValueResult != null)
            {
                result.ValueResult = result.ValueResult.Trim(Quotes);
            }
            return result;
        }

        /// <summary>
        /// Return an expanded, full file path - accounting for variables, functions, relative paths, etc.
        /// </summary>
        /// <param name="saveLocation">An R command that will be translated into a file path.</param>
        /// <returns>The full file path</returns>
        protected string GetExpandedFilePath(string saveLocation)
        {
            var fileLocation = CleanPathInResult(RunCommand(saveLocation, new Tag() { Type = Constants.TagType.Value }));
            if (((RParser)Parser).IsRelativePath(fileLocation.ValueResult))
            {
                // Attempt to find the current working directory.  If we are not able to find it, or the value we end up
                // creating doesn't exist, we will just proceed with whatever image location we had previously.
                var workingDirResult = CleanPathInResult(RunCommand("getwd()", new Tag() { Type = Constants.TagType.Value }));
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

        public override CommandResult HandleTableResult(Tag tag, string command, List<Message> result)
        {
            // We take a hint from the tag type to identify tables.  Because of how open R is with its
            // return of results, a user can just specify a variable and get the result.
            if (tag.Type.Equals(Constants.TagType.Table) && Parser.IsTableResult(command))
            {
                var message = result.FirstOrDefault();

                // Check to see if we can identify a file name that contains our table data.  If one
                // exists, we will start by returning that.  If there is no file name specified, we
                // will proceed and assume we are pulling data out of some R type.
                var dataFile = Parser.GetTableDataPath(command);
                if (!string.IsNullOrWhiteSpace(dataFile))
                {
                    return new CommandResult() { TableResult = DataFileToTable.GetTableResult(GetExpandedFilePath(dataFile)) };
                }

                var htmlValue = GetHtmlValueResult(message);
                if (string.IsNullOrWhiteSpace(htmlValue))
                {
                    // We know that print() returns just a plain text representation of the table data.  Because the user wants
                    // this formatted in Word as an actual table, and we can't reliably parse the text-based table, we will display
                    // a warning to the user to be shown after execution is done.
                    if (((RParser)Parser).CommandContainsPrint(command))
                    {
                        return new CommandResult() {
                            WarningResult = string.Format("StatTag did not receive a table result for the '{0}' tag. If you have wrapped your result in a print() statement, please remove it in your R code and try again.",
                            tag.Name)
                        };
                    }

                    return new CommandResult() { TableResult = ParseTableResult(GetTextValueResult(message)) };
                }
                else
                {
                    return new CommandResult() { TableResult = ParseHtmlTableResult(htmlValue) };
                }

            }

            return null;
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

            var flattenedData = new string[rows * cols];
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
            if (table != null)
            {
                return HandleAsHTMLTable(table);
            }

            var list = htmlFragment.DocumentNode.SelectSingleNode(".//ol");
            if (list != null)
            {
                return HandleAsHTMLList(list);
            }

            // If there is no table or list node present, we will return an empty table instead of an error.
            return new Table();
        }

        /// <summary>
        /// Helper function to run our pipeline of value processing fields for an HTML result
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ProcessHtmlValue(string original)
        {
            if (string.IsNullOrWhiteSpace(original))
            {
                return original;
            }

            // If the HTML result is just "NA" we assume it represents a real NA value.  If it
            // were a string containing 'NA', we would expect it to be captured in quotes.
            // Note that only for the "NA" value we will account for whitespace.  Later on we will
            // require it to not have any flanking whitespace.
            if (original.Trim().Equals(NA_VALUE))
            {
                return string.Empty;
            }

            var modified = FormatStringFromHtml(original);

            // If the modified (after HTML decoding) exactly equals <NA>, we assume that is
            // another special representation by the R kernel for an NA result.
            if (modified.Equals(BRACKETED_NA_VALUE))
            {
                return string.Empty;
            }

            return modified;
        }

        private Table HandleAsHTMLTable(HtmlNode table)
        {
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
                    data[rowIndex].Add(ProcessHtmlValue(col.GetDirectInnerText()));
                }
            }

            var dataTable = new Table(numRows, maxCols, TableUtil.MergeTableVectorsToArray(null, null, FlattenDataToArray(numRows, maxCols, data), numRows, maxCols));
            return dataTable;
        }

        private Table HandleAsHTMLList(HtmlNode list)
        {
            var items = list.SelectNodes(".//li");
            if (items == null || items.Count == 0)
            {
                return new Table();
            }

            var numItems = items.Count;
            var data = new List<string>();
            for (int itemIndex = 0; itemIndex < numItems; itemIndex++)
            {
                data.Add(ProcessHtmlValue(items[itemIndex].GetDirectInnerText()));
            }

            var dataTable = new Table(numItems, 1, TableUtil.MergeTableVectorsToArray(null, null, data.ToArray(), numItems, 1));
            return dataTable;
        }

        public override CommandResult HandleImageResult(Tag tag, string command, List<Message> result)
        {
            // Checking for an image export command is our first attempt, since it is specific to our R automation
            // workflow.  Otherwise, we will fallback to default behavior.
            if (Parser.IsImageExport(command) && !Parser.IsTagStart(command))
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

            // Try pulling out a base64-encoded image from the response
            return base.HandleImageResult(tag, command, result);
        }
        protected Table GetTableResult(string command)
        {
            /*
            // Check to see if we can identify a file name that contains our table data.  If one
            // exists, we will start by returning that.  If there is no file name specified, we
            // will proceed and assume we are pulling data out of some R type.
            var dataFile = Parser.GetTableDataPath(command);
            if (!string.IsNullOrWhiteSpace(dataFile))
            {
                return DataFileToTable.GetTableResult(GetExpandedFilePath(dataFile));
            }

            // You'll notice that regardless of the type, we convert everything to a string.  This
            // is just a simplification that StatTag makes about the results.  Often times we are
            // doing additional formatting and will do a conversion from the string type to the right
            // numeric type and format it appropriately.
            if (result.IsDataFrame())
            {
                var data = result.AsDataFrame();
                int rowCount = data.RowCount + 1;
                int columnCount = data.ColumnCount + 1;
                return new Table()
                {
                    ColumnSize = columnCount, RowSize = rowCount,
                    Data = TableUtil.MergeTableVectorsToArray(data.RowNames, data.ColumnNames,
                        FlattenDataFrame(data), rowCount, columnCount)
                };
            }
            else if (result.IsList())
            {
                // A list can be a collection of anything, including other vectors.
                // We will do our best to expand these.  If they are deeply nested
                // though, it becomes difficult to put these in a table structure in
                // a Word document.  For now, we'll limit to 2D structures.
                var list = result.AsList();
                int maxSize = 0;
                var data = new List<List<string>>();
                foreach (var item in list)
                {
                    var characterData = item.AsCharacter();
                    if (characterData == null)
                    {
                        data.Add(new List<string>());
                    }
                    else
                    {
                        var dataAsArray = characterData.Select(x => (x == null ? null : x.ToString())).ToList();
                        data.Add(dataAsArray);
                        maxSize = Math.Max(dataAsArray.Count, maxSize);
                    }
                }

                // Build data into a vector
                var vectorData = new List<string>();
                for (int row = 0; row < maxSize; row++)
                {
                    foreach (var column in data)
                    {
                        vectorData.Add(row < column.Count ? column[row] : null);
                    }
                }

                var rowSize = (list.Names == null || list.Names.Length == 0) ? maxSize : (maxSize + 1);
                return new Table()
                {
                    ColumnSize = data.Count,
                    RowSize = rowSize,
                    Data = TableUtil.MergeTableVectorsToArray(
                        null, list.Names, vectorData.ToArray(), rowSize, data.Count)
                };
            }
            else if (result.IsMatrix())
            {
                var matrix = result.AsCharacterMatrix();
                var rowNames = GetMatrixRowNames(result);
                var columnNames = GetMatrixColumnNames(result);

                // Just to note this isn't an error checking columnNames for rowCount and vice-versa.  Remember that
                // if we have column names, that will take up a row of data.  Likewise, row names are an additional
                // column.
                int rowCount = matrix.RowCount + (columnNames == null ? 0 : 1);
                int columnCount = matrix.ColumnCount + (rowNames == null ? 0 : 1);
                return new Table()
                {
                    ColumnSize = columnCount,
                    RowSize = rowCount,
                    Data = TableUtil.MergeTableVectorsToArray(rowNames, columnNames,
                        FlattenMatrix(matrix), rowCount, columnCount)
                };
            }

            if (result.Type == SymbolicExpressionType.NumericVector
                || result.Type == SymbolicExpressionType.IntegerVector
                || result.Type == SymbolicExpressionType.CharacterVector
                || result.Type == SymbolicExpressionType.LogicalVector)
            {
                var data = result.AsCharacter();
                return new Table()
                {
                    ColumnSize = 1, RowSize = data.Length, Data = TableUtil.MergeTableVectorsToArray(
                        null, null, data.Select(x => (x == null ? null : x.ToString())).ToArray(), data.Length, 1)
                };
            }*/

            return null;
        }

        public new bool IsReturnable(string command)
        {
            return Parser.IsValueDisplay(command) || Parser.IsImageExport(command) || Parser.IsTableResult(command);
        }

        public new string GetInitializationErrorMessage()
        {
            if (!State.EngineConnected)
            {
                return
                    "Could not communicate with R.  R may not be fully installed, or might be missing some of the automation pieces that StatTag requires.";
            }
            else if (!State.WorkingDirectorySet)
            {
                return
                    "We were unable to change the working directory to the location of your code file.   If this problem persists, please contact the StatTag team at StatTag@northwestern.edu.";
            }

            return
                "We were able to connect to R and change the working directory, but some other unknown error occurred during initialization.   If this problem persists, please contact the StatTag team at StatTag@northwestern.edu.";
        }

        public void Hide()
        {
            // Since the UI is not shown, no action is needed here.
        }

        /// <summary>
        /// Clean Jupyter-specific markings from a value result's text
        /// </summary>
        /// <param name="original">Plain text result from Jupyter kernel</param>
        /// <returns>Cleaned string</returns>
        public static string CleanValueResult(string original)
        {
            if (string.IsNullOrWhiteSpace(original))
            {
                return original;
            }

            var result = STRIP_RESULT_RESPONSE_NUMBER.Match(original);
            if (result.Success)
            {
                return result.Groups[1].Value;
            }

            return original;
        }


        /// <summary>
        /// Prepare a value result by doing any necessary post-processing.
        /// </summary>
        /// <param name="tag">The tag we are processing</param>
        /// <param name="command">The original command that was run</param>
        /// <param name="result">Result value(s) from Jupyter</param>
        /// <returns></returns>
        public override CommandResult HandleValueResult(Tag tag, string command, List<Message> result)
        {
            var commandResult = base.HandleValueResult(tag, command, result);
            if (commandResult == null || commandResult.ValueResult == null)
            {
                return commandResult;
            }

            // R results for values are going to be prefixed with "[X]" to indicate the execution order.
            // We could try another Jupyter response type, but those can be formatted, and we want the
            // plain text result.  So instead, we will strip that from the beginning of the result.
            var cleanResult = CleanValueResult(commandResult.ValueResult);
            commandResult.ValueResult = cleanResult;
            return commandResult;
        }

        /// <summary>
        /// Helper method to clean out the temporary folder used for storing images
        /// </summary>
        /// <param name="deleteFolder"></param>
        private void CleanTemporaryImageFolder(bool deleteFolder = false)
        {
            // TODO: Can we make passing graphics.off less brute-force?  We want to ensure we're not trying to access a file that's associated with an open graphics device.
            AutomationUtil.CleanTemporaryImageFolder(this, new[] { "graphics.off()" }, TemporaryImageFilePath, TemporaryImageFileFilter, deleteFolder);
        }

        /// <summary>
        /// Utility method to detect all known paths to R installations on the user's machine.
        /// This includes from environment variables (R_HOME) and the registry.
        /// </summary>
        /// <returns></returns>
        public static List<string> DetectAllRPaths()
        {
            var rPaths = new List<string>();

            var rHome = Environment.GetEnvironmentVariable("R_HOME");
            if (!string.IsNullOrWhiteSpace(rHome) && Directory.Exists(rHome))
            {
                rPaths.Add(rHome);
            }

            rPaths.AddRange(GetAllRPathsFromRegistry(@"SOFTWARE\R-core"));
            rPaths.AddRange(GetAllRPathsFromRegistry(@"SOFTWARE\WOW6432Node\R-core"));

            // Although this should typically just give us a unique list of R paths, we want
            // to do what we can to avoid duplication.  This is admiittedly imperfect - we're
            // ignoring where case differences may exist, etc., but the chances of that seem
            // low enough that we're reducing the overhead of implementing an exhaustive check.
            rPaths = rPaths.Distinct<string>().ToList<string>();
            var expandedRPaths = new List<string>();
            foreach (var rPath in rPaths)
            {
                var path = Path.Combine(rPath, "bin", Environment.Is64BitProcess ? "x64" : "i386");
                if (Directory.Exists(path))
                {
                    expandedRPaths.Add(path);
                }
            }

            return expandedRPaths;
        }

        /// <summary>
        /// Determine the best active R installation location.
        /// 
        /// This is a modification of the original RDotNet library code.
        /// </summary>
        /// <returns></returns>
        public static string DetectRPath()
        {
            var rHome = Environment.GetEnvironmentVariable("R_HOME");
            if (!string.IsNullOrWhiteSpace(rHome) && Directory.Exists(rHome))
            {
                return rHome;
            }

            rHome = GetRHomeFromRegistry(@"SOFTWARE\R-core");
            if (rHome == null)
            {
                rHome = GetRHomeFromRegistry(@"SOFTWARE\WOW6432Node\R-core");
            }

            if (string.IsNullOrWhiteSpace(rHome))
            {
                return null;
            }

            rHome = Path.Combine(rHome, "bin", Environment.Is64BitProcess ? "x64" : "i386");
            if (Directory.Exists(rHome))
            {
                return rHome;
            }

            return null;
        }

        /// <summary>
        /// Find all R installations registered in the registry under the base key provided.
        /// </summary>
        /// <param name="baseKey">The base registry key to search under - this must be some route to R-core</param>
        /// <returns>A list of strings containing the R path (empty if no paths are found)</returns>
        private static List<string> GetAllRPathsFromRegistry(string baseKey)
        {
            var rPaths = new List<string>();
            rPaths.AddRange(GetAllRPathsFromRegistryRoot(Registry.LocalMachine, baseKey));
            rPaths.AddRange(GetAllRPathsFromRegistryRoot(Registry.CurrentUser, baseKey));
            return rPaths;
        }


        /// <summary>
        /// Find all R installations registered in the registry under the root and base key provided.
        /// </summary>
        /// <param name="rootKey">The root registry key to search under</param>
        /// <param name="baseKey">The base registry key to search under - this must be some route to R-core</param>
        /// <returns>A list of strings containing the R path (empty if no paths are found)</returns>
        private static List<string> GetAllRPathsFromRegistryRoot(RegistryKey rootKey, string baseKey)
        {
            var rPaths = new List<string>();
            var rCoreKey = rootKey.OpenSubKey(baseKey);
            if (rCoreKey == null)
            {
                return rPaths;
            }

            // Although R32 and R64 sub-keys also exist, each should be under "R".  We are just going to
            // scan that, which also should also help avoid duplication.
            var rListKey = rCoreKey.OpenSubKey("R");
            if (rListKey == null)
            {
                return rPaths;
            }

            var subKeyNames = rListKey.GetSubKeyNames();
            foreach (var subKeyName in subKeyNames)
            {
                var rVersionKey = rListKey.OpenSubKey(subKeyName);
                if (rVersionKey == null)
                {
                    continue;
                }

                var rInstallPathKey = rVersionKey.GetValue(R_INSTALL_PATH_KEY);
                if (rInstallPathKey != null)
                {
                    rPaths.Add(rInstallPathKey.ToString());
                }
            }

            return rPaths;
        }

        /// <summary>
        /// Determine if we can find the R installation location using an anchor in the registry.
        /// This will allow us to try multiple anchor registry keys, as it can vary based on
        /// platform.
        /// </summary>
        /// <param name="baseKey"></param>
        /// <returns></returns>
        private static string GetRHomeFromRegistry(string baseKey)
        {
            var rCore = Registry.LocalMachine.OpenSubKey(baseKey);
            if (rCore == null)
            {
                rCore = Registry.CurrentUser.OpenSubKey(baseKey);
                if (rCore == null)
                {
                    return null;
                }
            }

            var subKey = Environment.Is64BitProcess ? "R64" : "R";
            var rKey = rCore.OpenSubKey(subKey);
            if (rKey == null)
            {
                return null;
            }

            return GetRInstallPathFromRCoreKegKey(rKey);
        }

        /// <summary>
        /// Utility function adapted from RDotNet code to identify the installation path for R
        /// </summary>
        /// <param name="rCoreKey"></param>
        /// <returns></returns>
        private static string GetRInstallPathFromRCoreKegKey(RegistryKey rCoreKey)
        {
            string installPath = null;
            string[] subKeyNames = rCoreKey.GetSubKeyNames();
            string[] valueNames = rCoreKey.GetValueNames();
            if (valueNames.Length == 0)
            {
                return RecurseFirstSubkey(rCoreKey);
            }
            else
            {
                if (valueNames.Contains(R_INSTALL_PATH_KEY))
                {
                    installPath = (string)rCoreKey.GetValue(R_INSTALL_PATH_KEY);
                }
                else
                {
                    if (valueNames.Contains("Current Version"))
                    {
                        string currentVersion = (string)rCoreKey.GetValue("Current Version");
                        if (currentVersion != null && subKeyNames.Contains(currentVersion))
                        {
                            RegistryKey rVersionCoreKey = rCoreKey.OpenSubKey(currentVersion);
                            return GetRInstallPathFromRCoreKegKey(rVersionCoreKey);
                        }
                    }
                    else
                    {
                        return RecurseFirstSubkey(rCoreKey);
                    }
                }
            }
            return installPath;
        }

        private static string GetRVersionFromRegistry(string baseKey)
        {
            var rCore = Registry.LocalMachine.OpenSubKey(baseKey);
            if (rCore == null)
            {
                rCore = Registry.CurrentUser.OpenSubKey(baseKey);
                if (rCore == null)
                {
                    return null;
                }
            }

            var subKey = Environment.Is64BitProcess ? "R64" : "R";
            var rKey = rCore.OpenSubKey(subKey);
            if (rKey == null)
            {
                return null;
            }

            return GetRInstallPathFromRCoreKegKey(rKey);
        }

        /// <summary>
        /// Utility function adapted from RDotNet code to identify the version of the
        /// active version of R
        /// </summary>
        /// <param name="rCoreKey"></param>
        /// <returns></returns>
        private static string GetRVersionFromRCoreKegKey(RegistryKey rCoreKey)
        {
            string[] subKeyNames = rCoreKey.GetSubKeyNames();
            string[] valueNames = rCoreKey.GetValueNames();
            if (valueNames.Length == 0)
            {
                return RecurseFirstSubkey(rCoreKey);
            }
            else
            {
                if (valueNames.Contains("Current Version"))
                {
                    string currentVersion = (string)rCoreKey.GetValue("Current Version");
                    return currentVersion;
                }
                else
                {
                    return RecurseFirstSubkey(rCoreKey);
                }
            }
            return null;
        }

        /// <summary>
        /// Utility function adapted from RDotNet code base to recurse and check for R install
        /// paths in sub-keys of a registry entry.
        /// </summary>
        /// <param name="rCoreKey"></param>
        /// <returns></returns>
        private static string RecurseFirstSubkey(RegistryKey rCoreKey)
        {
            string[] subKeyNames = rCoreKey.GetSubKeyNames();
            if (subKeyNames.Length > 0)
            {
                var versionNum = subKeyNames.First();
                var rVersionCoreKey = rCoreKey.OpenSubKey(versionNum);
                return GetRInstallPathFromRCoreKegKey(rVersionCoreKey);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Using the same approach as R: https://github.com/r-lib/r-svn/blob/440c5430cc247949caf9d109d18cfed04aba09b8/src/library/utils/R/packages2.R#L315-L347
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string[] GetAccessibleLibPaths(string result)
        {
            var accessiblePaths = new List<string>();
            var paths = RAutomation.ParseLibPathResults(result);
            if (paths.Length == 0)
            {
                return paths;
            }

            foreach (var path in paths)
            {
                try
                {
                    // Not our job to create it - if it doesn't exist, we just skip it from consideration
                    if (!Directory.Exists(path))
                    {
                        continue;
                    }

                    var testDir = Path.Combine(path, "_test_dir_");
                    Directory.CreateDirectory(testDir);
                    Directory.Delete(testDir, true);

                    // If we made it this far, it's valid
                    accessiblePaths.Add(path);
                }
                catch
                {
                    // Eat the exception - it means we can't use this path
                }
            }

            return accessiblePaths.ToArray();
        }

        /// <summary>
        /// Special utility function to take the results of the .libPaths() command and return an
        /// array of all libPath entries that are set.
        /// </summary>
        /// <param name="result">The string result from executing .libPaths() in R</param>
        /// <returns>Array of stringing containing libPath entries, if any exist</returns>
        public static string[] ParseLibPathResults(string result)
        {
            var paths = new List<string>();
            if (string.IsNullOrWhiteSpace(result))
            {
                return paths.ToArray();
            }

            var parts = Regex.Split(result, "^>", RegexOptions.Multiline).Where(x => !string.IsNullOrWhiteSpace(x));
            if (parts.Count() == 0)
            {
                return paths.ToArray();
            }

            // We now need to split by newline each response and clean it up to get back the path
            foreach (var part in parts)
            {
                var lines = part.Split('\n')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => CleanValueResult(x).Trim(LibPathTrimChars));
                if (lines.Count() > 0)
                {
                    // The first one has our command, so we can skip it
                    lines = lines.Skip(1);
                }
                paths.AddRange(lines.Where(x => !string.IsNullOrWhiteSpace(x)));
            }

            //return parts;

            return paths.ToArray();
        }

        /// <summary>
        /// Determine possible path, same as R does: https://github.com/r-lib/r-svn/blob/440c5430cc247949caf9d109d18cfed04aba09b8/src/library/base/R/library.R#L988
        /// </summary>
        /// <returns></returns>
        public static string GetUserLibPath()
        {
            var version = GetRVersionFromRegistry(@"SOFTWARE\R-core");
            if (version == null)
            {
                version = GetRHomeFromRegistry(@"SOFTWARE\WOW6432Node\R-core");
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                return null;
            }

            var majorMinor = Regex.Match(version, @"([\d]+\.[\d]+)");
            if (!majorMinor.Success)
            {
                return null;
            }

            return Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "R", "win-library", majorMinor.Captures[0].Value);
        }
    }
}
