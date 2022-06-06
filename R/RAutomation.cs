using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using RDotNet.Internals;
using RDotNet.NativeLibrary;
using StatTag.Core.Exceptions;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using StatTag.Core.Parser;
using StatTag.Core.Utility;

namespace R
{
    public class RAutomation : IStatAutomation
    {
        private const string MATRIX_DIMENSION_NAMES_ATTRIBUTE = "dimnames";
        private const string TemporaryImageFileFilter = "*.png";

        private static readonly Dictionary<string, string> RProfileCommands = new Dictionary<string, string>()
        {
            { "sessionInfo()", "Session" },
            { "R.home()", "R Home" },
            { ".libPaths()",  "Lib Path" },
            { "with(as.data.frame(installed.packages(fields = c(\"Package\", \"Version\"))), paste(Package, Version, sep = \"\", collapse = \", \" ))", "Packages" }
        };

        private string TemporaryImageFilePath = "";

        public StatPackageState State { get; set; }

        protected REngine Engine = null;
        protected RParser Parser { get; set; }
        protected static VerbatimDevice VerbatimLog = new VerbatimDevice();

        public RAutomation()
        {
            Parser = new RParser();
            State = new StatPackageState();
        }

        /// <summary>
        /// Simple wrapper to get the R version from the registry
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static Version GetRVersion(StringBuilder logger = null)
        {
            var util = new NativeUtility();
            var ver = util.GetRVersionFromRegistry(logger);
            return ver;
        }

        /// <summary>
        /// Wrapper function to provide commonly used parameters to the main IsAffectedByCFGIssue
        /// method.  These are split out so the other one can be automatically tested.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static bool IsAffectedByCFGIssue(StringBuilder logger = null)
        {
            return IsAffectedByCFGIssue(Environment.Is64BitProcess, Environment.OSVersion, GetRVersion(), logger);
        }

        /// <summary>
        /// Determine if the user is affected by the Control Flow Guard issue.  We know it affects R 4.0.3 - 4.1.X,
        /// but only the 64-bit version of R.  If the user is running a 32-bit version, it should be fine.
        /// This method will use registry keys to determine the version of R running.  Any attempts to use REngine from
        /// RDotNet right now would cause a crash.
        /// The issue with R was resolved in R 4.2.
        /// </summary>
        /// <param name="is64BitProcess">If this process is 64-bit or not</param>
        /// <param name="version">The R version to check</param>
        /// <param name="logger">Optional logger to append messages to</param>
        /// <returns>True if the user is affected by the CFG issue, or false otherwise</returns>
        public static bool IsAffectedByCFGIssue(bool is64BitProcess, OperatingSystem osVersion, Version version, StringBuilder logger = null)
        {
            bool affected = false;
            try
            {
                if ((version == null || osVersion == null || osVersion.Version == null))
                {
                    if (logger != null)
                    {
                        logger.AppendFormat("Unable to determine if R version is affected by Control Flow Guard issue\r\n");
                    }
                    return affected;
                }

                // As of now this only affects Windows 10 x64.  Not sure if this will be the same in Windows 11, so we
                // are explicitly checking against Windows 10, and if it is any other version we assume it works.
                if (osVersion.Version.Major != 10)
                {
                    if (logger != null)
                    {
                        logger.Append("Not running Windows 10\r\n");
                    }
                    return affected;
                }

                if (!is64BitProcess)
                {
                    if (logger != null)
                    {
                        logger.Append("Not running a 64-bit process - assuming 32-bit version of R\r\n");
                    }
                    return affected;
                }

                if ((version.Major == 4 && version.Minor == 1) || (version.Major == 4 && version.Minor == 0 && version.Build > 2))
                {
                    affected = true;
                }

                if (affected && logger != null)
                {
                    logger.AppendFormat(
                        "There are known issues with R 4.0.3 and higher.  Current R version {0} is not supported at this time.\r\n",
                        version.ToString());
                }
            }
            catch (Exception exc)
            {
                if (logger != null)
                {
                    logger.AppendFormat("Failed to determine if R version is affected by Control Flow Guard issue: {0}\r\n{1}\r\n", exc.Message, exc.StackTrace);
                }

            }

            return affected;
        }

        public static string InstallationInformation()
        {
            var builder = new StringBuilder();
            try
            {
                if (IsAffectedByCFGIssue(builder))
                {
                    return builder.ToString().Trim();
                }

                var engine = REngine.GetInstance(null, true, null, VerbatimLog);
                foreach (var command in RProfileCommands)
                {
                    var result = engine.Evaluate(command.Key);
                    builder.AppendFormat("{0} : {1}\r\n", command.Value, string.Join("\r\n", result.AsCharacter().ToArray()).Trim());
                }
            }
            catch (Exception exc)
            {
                builder.AppendFormat(
                    "Unable to communicate with R. R may not be installed or there might be other configuration issues.\r\n");
                builder.AppendFormat("{0}\r\n", exc.Message);
            }

            return builder.ToString().Trim();
        }

        public virtual bool Initialize(CodeFile file, LogManager logger)
        {
            if (Engine == null)
            {
                logger.WriteMessage("R Engine instance is null - going through full initialization");

                try
                {
                    if (IsAffectedByCFGIssue())
                    {
                        logger.WriteMessage("Client is affected by CFG issue - will not be able to run R");
                        return false;
                    }

                    logger.WriteMessage("Preparing to set R environment...");
                    REngine.SetEnvironmentVariables(); // <-- May be omitted; the next line would call it.
                    logger.WriteMessage("Set R environment.  Preparing to get R instances...");
                    Engine = REngine.GetInstance(null, true, null, VerbatimLog);

                    State.EngineConnected = (Engine != null);
                    logger.WriteMessage(string.Format("R instance creation {0}",
                        (State.EngineConnected ? "succeeded" : "failed")));

                    // Set the working directory to the location of the code file, if it is provided and the
                    // R engine has been initialized.
                    if (Engine != null && file != null)
                    {
                        var path = Path.GetDirectoryName(file.FilePath);
                        if (!string.IsNullOrEmpty(path))
                        {
                            logger.WriteMessage(string.Format("Changing working directory to {0}", path));
                            path = path.Replace("\\", "\\\\").Replace("'", "\\'");
                            RunCommand(string.Format("setwd('{0}')", path), new Tag() { Type = Constants.TagType.Value });  // Escape the path for R
                            State.WorkingDirectorySet = true;
                        }
                    }

                    logger.WriteMessage("Completed initialization");
                }
                catch (Exception exc)
                {
                    logger.WriteMessage("Caught an exception while trying to initalize R");
                    logger.WriteException(exc);
                    Engine = null;
                    return false;
                }
            }

            if (Engine != null)
            {
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

                return true;
            }

            return false;
        }

        public virtual void Dispose()
        {
            if (Engine != null)
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
        }

        
        public CommandResult[] RunCommands(string[] commands, Tag tag = null)
        {
            // If there is no tag, and we're just running a big block of code, it's much easier if we can send that to
            // the R engine at once.  Otherwise we have to worry about collapsing commands, function definitions, etc.
            if (tag == null)
            {
                commands = new[] { string.Join("\r\n", commands) };
            }
            else
            {
                commands = Parser.CollapseMultiLineCommands(commands);
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
                        VerbatimLog.StartCache();
                    }
                    // If we're going to be doing a figure, we want to clean out the old images so we know
                    // exactly which ones we're writing to.
                    else if (isFigureTag)
                    {
                        CleanTemporaryImageFolder();
                    }
                }

                var result = RunCommand(command, tag);
                if (result != null && !result.IsEmpty() && !isVerbatimTag)
                {
                    commandResults.Add(result);
                }
                else if (Parser.IsTagEnd(command))
                {
                    if (isVerbatimTag)
                    {
                        VerbatimLog.StopCache();
                        commandResults.Add(new CommandResult()
                        {
                            VerbatimResult = string.Join("", VerbatimLog.GetCache())
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

        /// <summary>
        /// Return an expanded, full file path - accounting for variables, functions, relative paths, etc.
        /// </summary>
        /// <param name="saveLocation">An R command that will be translated into a file path.</param>
        /// <returns>The full file path</returns>
        protected string GetExpandedFilePath(string saveLocation)
        {
            var fileLocation = RunCommand(saveLocation, new Tag() { Type = Constants.TagType.Value });
            if (Parser.IsRelativePath(fileLocation.ValueResult))
            {
                // Attempt to find the current working directory.  If we are not able to find it, or the value we end up
                // creating doesn't exist, we will just proceed with whatever image location we had previously.
                var workingDirResult = RunCommand("getwd()", new Tag() { Type = Constants.TagType.Value });
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

        public virtual CommandResult HandleTableResult(Tag tag, string command, SymbolicExpression result)
        {
            // We take a hint from the tag type to identify tables.  Because of how open R is with its
            // return of results, a user can just specify a variable and get the result.
            if (tag.Type == Constants.TagType.Table)
            {
                return new CommandResult() { TableResult = GetTableResult(command, result) };
            }

            return null;
        }

        public virtual CommandResult HandleImageResult(Tag tag, string command, SymbolicExpression result)
        {
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

            return null;
        }

        public virtual CommandResult HandleValueResult(Tag tag, string command, SymbolicExpression result)
        {
            // If we have a value command, we will pull out the last relevant line from the output.
            // Because we treat every type of output as a possible value result, we are only going
            // to capture the result if it's flagged as a tag.
            if (tag.Type == Constants.TagType.Value)
            {
                return new CommandResult() { ValueResult = GetValueResult(result) };
            }

            return null;
        }

        public CommandResult RunCommand(string command, Tag tag = null)
        {
            SymbolicExpression result = null;
            try
            {
                result = Engine.Evaluate(command);
            }
            catch (Exception exc)
            {
                var asciiBytes = Encoding.ASCII.GetBytes(command);
                var utf8Bytes = Encoding.UTF8.GetBytes(command);
                if ((asciiBytes != null && utf8Bytes != null) && (asciiBytes.Length != utf8Bytes.Length))
                {
                    var message = string.Format("{0}\r\n\r\n**NOTE**: There is a known issue where StatTag does not handle non-ASCII variable names in R (e.g., ä <- 100).  You will need to change this within your R file to an ASCII variable name.",
                        exc.Message);
                    throw new StatTagUserException(message, exc);
                }
                else
                {
                    throw exc;
                }
            }

            if (result == null || result.IsInvalid)
            {
                return null;
            }

            // If there is no tag associated with the command that was run, we aren't going to bother
            // parsing and processing the results.  This is for blocks of codes in between tags where
            // all we need is for the code to run.
            if (tag != null)
            {
                // Start with tables
                var commandResult = HandleTableResult(tag, command, result);
                if (commandResult != null)
                {
                    return commandResult;
                }

                // Image comes next, because everything else we will count as a value type.
                commandResult = HandleImageResult(tag, command, result);
                if (commandResult != null)
                {
                    return commandResult;
                }

                commandResult = HandleValueResult(tag, command, result);
                if (commandResult != null)
                {
                    return commandResult;
                }
            }

            return null;
        }

        /// <summary>
        /// Given a 2D data frame, flatten it into a 1D array that is organized as data by row.
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <returns></returns>
        private string[] FlattenDataFrame(DataFrame dataFrame)
        {
            // Because we can only cast columns (not individual cells), we will go through all columns
            // first and cast them to characters so things like NA values are represented appropriately.
            // If we use the default format for these columns/cells, we end up with large negative int
            // values where NA exists.
            var castColumns = new List<CharacterVector>();
            for (int column = 0; column < dataFrame.ColumnCount; column++)
            {
                castColumns.Add(dataFrame[column].AsCharacter());
            }

            var data = new List<string>(dataFrame.RowCount * dataFrame.ColumnCount);
            for (int row = 0; row < dataFrame.RowCount; row++)
            {
                for (int column = 0; column < dataFrame.ColumnCount; column++)
                {
                    data.Add(castColumns[column][row]);
                }
            }

            return data.ToArray();
        }

        /// <summary>
        /// General function to extract dimension (row/column) names for an R matrix.  This deals with
        /// the specific of how R packages matrix results, which is different from a data frame.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="rowNames"></param>
        /// <returns></returns>
        private string[] GetMatrixDimensionNames(SymbolicExpression exp, bool rowNames)
        {
            var attributeName = exp.GetAttributeNames().FirstOrDefault(x => x.Equals(MATRIX_DIMENSION_NAMES_ATTRIBUTE, StringComparison.InvariantCultureIgnoreCase));
            if (string.IsNullOrEmpty(attributeName))
            {
                return null;
            }

            var dimnames = exp.GetAttribute(attributeName).AsList();
            if (dimnames == null)
            {
                return null;
            }

            // Per the R specification, the dimnames will contain 0 to 2 vectors that contain
            // dimension labels.  The first entry is for rows, and the second is for columns.
            // https://stat.ethz.ch/R-manual/R-devel/library/base/html/matrix.html
            switch (dimnames.Length)
            {
                case 1:
                {
                    // If we want columns and we only have one dimname entry, we don't have column names.
                    if (!rowNames)
                    {
                        return null;
                    }
                    var nameVector = dimnames[0].AsCharacter();
                    return (nameVector == null ? null : nameVector.ToArray());
                }
                case 2:
                {
                    // Sometimes the rownames are null, and so we can try and cast to a character vector,
                    // but we need to check if that is null before converting to an array.
                    var nameVector = dimnames[rowNames ? 0 : 1].AsCharacter();
                    return (nameVector == null ? null : nameVector.ToArray());
                }
            }

            return null;
        }

        /// <summary>
        /// Return the row names (if they exist) for a matrix.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns>An array of strings containing the row names, or null if not present.</returns>
        private string[] GetMatrixRowNames(SymbolicExpression exp)
        {
            return GetMatrixDimensionNames(exp, true);
        }

        /// <summary>
        /// Return the column names (if they exist) for a matrix.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns>An array of strings containing the column names, or null if not present.</returns>
        private string[] GetMatrixColumnNames(SymbolicExpression exp)
        {
            return GetMatrixDimensionNames(exp, false);
        }

        /// <summary>
        /// Take the 2D data in a matrix and flatten it to a 1D representation (by row)
        /// which we use internally in StatTag.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private string[] FlattenMatrix(CharacterMatrix matrix)
        {
            var data = new List<string>();
            for (int row = 0; row < matrix.RowCount; row++)
            {
                for (int column = 0; column < matrix.ColumnCount; column++)
                {
                    data.Add(matrix[row, column]);
                }
            }
            return data.ToArray();
        }

        protected Table GetTableResult(string command, SymbolicExpression result)
        {
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
            }

            return null;
        }

        public bool IsReturnable(string command)
        {
            return Parser.IsValueDisplay(command) || Parser.IsImageExport(command) || Parser.IsTableResult(command);
        }

        public string GetInitializationErrorMessage()
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

        public string FormatErrorMessageFromExecution(Exception exc)
        {
            if (Engine == null)
            {
                return exc.Message;
            }

            // There is a known issue with R 3.4.3 and R.NET (https://github.com/jmp75/rdotnet/issues/62).  We will attempt
            // to detect if this setup is consistent with that, and offer targeted guidance if so.
            if (Engine.DllVersion.Contains("3.4.3"))
            {
                // Our check is the presenece of the etc/Renviron.site file.  That was incorrectly deployed with R 3.4.3
                // installers on Windows, and is causing problems when we run code from R.NET.  If that file exists, we
                // provide some additional guidance on what to do.
                var rHome = Environment.GetEnvironmentVariable("R_HOME");
                var builder = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(rHome) && File.Exists(Path.Combine(rHome, "etc/Renviron.site")))
                {
                    builder.Append(
                        "There is a known issue with R 3.4.3 that may be causing your code to fail in StatTag.\r\n\r\nIf you have confirmed that your code runs fine from R or RStudio, please check where R is installed to see if the file 'etc/Renviron.site' exists.  If so, renaming or removing the file may fix this issue.");
                    builder.AppendFormat("\r\n\r\n{0}", exc.Message);
                    return builder.ToString();
                }
            }

            return exc.Message;
        }

        protected string GetValueResult(SymbolicExpression result)
        {
            if (result.IsDataFrame())
            {
                return result.AsDataFrame()[0].AsCharacter().FirstOrDefault();
            }
            else if (result.IsList())
            {
                return result.AsList()[0].AsCharacter().FirstOrDefault();
            }

            switch (result.Type)
            {
                case SymbolicExpressionType.NumericVector:
                case SymbolicExpressionType.IntegerVector:
                case SymbolicExpressionType.CharacterVector:
                case SymbolicExpressionType.LogicalVector:
                    return result.AsCharacter().FirstOrDefault();
            }
            return null;
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
    }
}
