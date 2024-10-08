﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using StatTag.Core.Parser;
using StatTag.Core.Utility;

namespace Stata
{
    public class StataAutomation : IDisposable, IStatAutomation
    {
        public const string LocalMacroPrefix = "_";

        /// <summary>
        /// This is a special local macro name that is being used within StatTag.
        /// </summary>
        public const string StatTagTempMacroName = "__st_tmp_display_value";

        public const string DisablePagingCommand = "set more off";

        public const string StatTagVerbatimLogName = "__stattag_verbatim_log_tmp.log";
        //public const string StatTagVerbatimLogCommand = "log using \"" + StatTagVerbatimLogName + "\", text replace";
        public const string StatTagVerbatimLogIdentifier = "stattag-verbatim";

        public const string EndLoggingCommand = "log close";

        public const string DisplayWorkingDirectoryCommand = "display \"`c(pwd)'\"";

        // The following are constants used to manage the Stata Automation API
        public const string RegisterParameter = "/Register";
        public const string UnregisterParameter = "/Unregister";

        public StatPackageState State { get; set; }

        public string GetInitializationErrorMessage()
        {
            if (!State.EngineConnected)
            {
                return
                    "Could not communicate with Stata.  You will need to enable Stata Automation (not done by default) to run this code in StatTag.\r\n\r\nThis can be done from StatTag > User Settings > Stata, or see http://www.stata.com/automation";
            }
            else if (!State.WorkingDirectorySet)
            {
                return
                    "We were unable to change the working directory to the location of your code file.   If this problem persists, please contact the StatTag team at StatTag@northwestern.edu.";
            }

            return
                "We were able to connect to Stata and change the working directory, but some other unknown error occurred during initialization.   If this problem persists, please contact the StatTag team at StatTag@northwestern.edu.";
        }

        protected stata.StataOLEApp Application { get; set; }
        protected StataParser Parser { get; set; }
        protected List<StataParser.Log> OpenLogs { get; set; }
        protected bool IsTrackingVerbatim { get; set; }
        
        private static class ScalarType
        {
            public const int NotFound = 0;
            public const int Numeric = 1;
            public const int String = 2;
        }

        private const int StataHidden = 1;
        private const int MinimizeStata = 2;
        private const int ShowStata = 3;

        public StataAutomation()
        {
            Parser = new StataParser();
            State = new StatPackageState();
        }

        public void Show()
        {
            if (Application != null)
            {
                Application.UtilShowStata(ShowStata);
            }
        }

        public void Hide()
        {
            if (Application != null)
            {
                Application.UtilShowStata(StataHidden);
            }
        }

        public string FormatErrorMessageFromExecution(Exception exc)
        {
            return exc.Message;
        }

        /// <summary>
        /// Collect information about the Stata installation (if one exists), and provide a formatted string
        /// back to the caller.
        /// </summary>
        /// <returns></returns>
        public static CheckResult InstallationInformation(UserSettings settings)
        {
            var builder = new StringBuilder();
            var infoResult = new CheckResult();
            stata.StataOLEApp application = null;
            try
            {
                if (settings == null || !File.Exists(settings.StataLocation))
                {
                    builder.Append("Stata is not currently configured in this instance of StatTag");
                    infoResult.Result = false;
                }
                else if (IsStataRunning(settings.StataLocation))
                {
                    builder.Append("Stata is installed and running");
                    infoResult.Result = true;
                }
                else
                {
                    application = new stata.StataOLEApp();
                    application.UtilShowStata(StataHidden);
                    application.DoCommandAsync("local _stattag_stata_version = \"`c(version)'\"");
                    PauseStataUntilFree(application);
                    // "__stattag..." is not a typo - the local macro name requires the extra underscore ahead
                    // of what we originally named it in order to be accessed via MacroValue.
                    var version = application.MacroValue("__stattag_stata_version");
                    builder.AppendFormat("Stata {0} detected.", version);
                    infoResult.Result = true;
                }
            }
            catch (Exception exc)
            {
                builder.AppendFormat(
                    "Unable to communicate with Stata. Stata may not be installed or there might be other configuration issues.\r\n");
                builder.AppendFormat("{0}\r\n", exc.Message);
                infoResult.Result = false;
            }
            finally
            {
                application = null;
            }

            infoResult.Details = builder.ToString().Trim();
            return infoResult;
        }

        public bool Initialize(CodeFile file, LogManager logger)
        {
            try
            {
                OpenLogs = new List<StataParser.Log>();
                Application = new stata.StataOLEApp();

                // Make sure that Stata is not busy processing anything.
                // We will only wait up to 5 seconds
                logger.WriteMessage("Preparing to pause until Stata is free...");
                PauseStataUntilFree(500, 10);
                logger.WriteMessage("Stata is free.  Preparing to disable paging...");
                DoCommandWithPauseAndRetry(DisablePagingCommand, 500, 3);
                logger.WriteMessage("Paging disabled.  Preparing to show Stata application window...");
                Show();
                logger.WriteMessage("Stata application window now visible");
                State.EngineConnected = true;

                // Set the working directory to the location of the code file, if it is provided and
                // it isn't a UNC path.
                if (file != null)
                {
                    var path = Path.GetDirectoryName(file.FilePath);
                    if (!string.IsNullOrEmpty(path) && !path.Trim().StartsWith("\\\\"))
                    {
                        logger.WriteMessage(string.Format("Changing working directory to {0}", path));
                        RunCommand(string.Format("cd \"{0}\"", path.Replace("\\", "\\\\")));
                        State.WorkingDirectorySet = true;
                    }
                }

                logger.WriteMessage("Completed initialization");
            }
            catch (Exception exc)
            {
                logger.WriteMessage("An exception was caught when trying to initialize Stata");
                logger.WriteException(exc);
                return false;
            }

            return true;
        }


        /// <summary>
        /// Determine if a command is one that would return a result of some sort.
        /// </summary>
        /// <param name="command">The command to evaluate</param>
        /// <returns></returns>
        public bool IsReturnable(string command)
        {
            return Parser.IsValueDisplay(command) || Parser.IsImageExport(command) || Parser.IsTableResult(command);
        }

        private void DoCommandWithPauseAndRetry(string command, int pauseDuration, int maxRetry)
        {
            int tryCounter = 0;
            while (tryCounter < maxRetry)
            {
                try
                {
                    Application.DoCommand(command);
                    return;
                }
                catch (Exception exc)
                {
                    tryCounter++;
                    Thread.Sleep(pauseDuration);
                }                
            }
        }

        private void EnsureLoggingForVerbatim(Tag tag)
        {
            // If there is no open log, we will start one
            if (OpenLogs.Find(x => x.LogType.Equals("log", StringComparison.CurrentCultureIgnoreCase)) == null)
            {
                string verbatimLogFile = StatTagVerbatimLogName;
                if (tag.CodeFile != null && !string.IsNullOrWhiteSpace(tag.CodeFile.FilePath))
                {
                    verbatimLogFile = Path.Combine(Path.GetDirectoryName(tag.CodeFile.FilePath),
                        StatTagVerbatimLogName);
                }
                RunCommand(string.Format("log using \"{0}\", text replace", verbatimLogFile));
                // We use a special identifier so we know it is a StatTag initiated log during processing, and that
                // it can be closed.  Otherwise we may confuse it with a log the user started.
                OpenLogs.Add(new StataParser.Log() { LogType = StatTagVerbatimLogIdentifier, LogPath = verbatimLogFile });
            }
        }

        /// <summary>
        /// Run a collection of commands and provide all applicable results.
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        public CommandResult[] RunCommands(string[] commands, Tag tag = null)
        {
            try
            {
                var commandResults = new List<CommandResult>();
                IsTrackingVerbatim = (tag != null && tag.Type == Constants.TagType.Verbatim);
                string startingVerbatimCommand = string.Empty;   // Tracks in the log where we begin pulling verbatim output from
                foreach (var command in commands)
                {
                    if (Parser.IsStartingLog(command))
                    {
                        OpenLogs.AddRange(Parser.GetLogs(command));
                    }

                    // Ensure logging is taking place if the user has identified this as verbatim output.
                    if (Parser.IsTagStart(command) && IsTrackingVerbatim)
                    {
                        startingVerbatimCommand = command;
                        EnsureLoggingForVerbatim(tag);
                    }

                    // Perform execution of the command
                    var result = RunCommand(command, tag);

                    // Handle the result normally unless we are in the middle of a verbatim tag.  In that case,
                    // the logging should be enabled at this point and closing it out will be handled later when
                    // the verbatim tag ends.
                    if (result != null && !result.IsEmpty() && !IsTrackingVerbatim)
                    {
                        commandResults.Add(result);
                    }
                    else if (Parser.IsTagEnd(command) && IsTrackingVerbatim)
                    {
                        // Log management can get tricky.  If the user has established their own log file as part of their do file, we have
                        // to manage closing and reopening the log when we need to access content.  Otherwise Stata will keep the log file
                        // locked and we can't access it.
                        StataParser.Log logToRead = null;
                        var verbatimLog = OpenLogs.Find(x => x.LogType.Equals(StatTagVerbatimLogIdentifier,
                            StringComparison.CurrentCultureIgnoreCase));
                        var regularLog = OpenLogs.Find(x => x.LogType.Equals("log",
                            StringComparison.CurrentCultureIgnoreCase));
                        if (verbatimLog != null)
                        {
                            RunCommand(string.Format(EndLoggingCommand));
                            logToRead = verbatimLog;
                        }
                        else if (regularLog != null)
                        {
                            logToRead = regularLog;
                            RunCommand(EndLoggingCommand);
                        }

                        // If we don't have a log for some reason, just continue on (with no verbatim output).
                        if (logToRead == null)
                        {
                            continue;
                        }

                        // Pull the text and parse out the relevant lines that we want
                        var verbatimOutput = CreateVerbatimOutputFromLog(logToRead, startingVerbatimCommand, command);
                        commandResults.Add(new CommandResult() { VerbatimResult = verbatimOutput });

                        // Now that we have the output, we have to perform some cleanup for the log.  If we have a verbatim
                        // log that we created, we will clean it up.  Otherwise we need to re-enable the logging for the
                        // user-defined log.
                        if (verbatimLog != null)
                        {
                            OpenLogs.Remove(verbatimLog);
                            File.Delete(verbatimLog.LogPath);
                        }
                        else
                        {
                            RunCommand(string.Format("log using \"{0}\"", logToRead.LogPath));
                        }
                    }
                }

                ResolveCommandPromises(commandResults);
                return commandResults.ToArray();
            }
            catch (Exception exc)
            {
                // If we catch an exception, the script is going to stop operating.  So we will check to see
                // if any log files are open, and close them for the user.  Otherwise the log will remain
                // open.
                foreach (var openLog in OpenLogs)
                {
                    // We have a special log type identifier we use for tracking verbatim output.  The Replace call
                    // for each log command is a cheat to make sure that type is converted to log (instead of if/elses).
                    RunCommand(string.Format("{0} close", openLog.LogType.Replace(StatTagVerbatimLogIdentifier, "log")));
                }

                // Since we should now have closed all logs, clear the list we were tracking.
                OpenLogs.Clear();

                IsTrackingVerbatim = false;

                throw exc;
            }
        }

        /// <summary>
        /// Iterate through a list of command results and resolve any outstanding
        /// promises on the results.
        /// </summary>
        /// <param name="commandResults">The list of command results</param>
        private void ResolveCommandPromises(List<CommandResult> commandResults)
        {
            foreach (var result in commandResults)
            {
                if (!string.IsNullOrEmpty(result.TableResultPromise))
                {
                    // Yes, we are expanding the file paths twice. The problem is that depending on how the file is written (putexcel
                    // being the first example we ran into), it may not actually be on disk when the first GetExpandedFilePath is called.
                    // Because that method requires a file to exist before it will accept it, we need to do the expansion again since
                    // otherwise we could have just a relative path.
                    result.TableResult = DataFileToTable.GetTableResult(GetExpandedFilePath(result.TableResultPromise));
                    result.TableResultPromise = null;
                }
            }
        }

        /// <summary>
        /// Given an internal reference that StatTag maintains to a log file, pull out the relevant log lines (excluding
        /// the commands that would be written there too).  This returns a string with newlines represented as \r\n.s
        /// </summary>
        /// <param name="logToRead"></param>
        /// <param name="startingVerbatimCommand"></param>
        /// <param name="endingVerbatimCommand"></param>
        /// <returns></returns>
        private string CreateVerbatimOutputFromLog(StataParser.Log logToRead, string startingVerbatimCommand, string endingVerbatimCommand)
        {
            var text = File.ReadAllText(logToRead.LogPath).Replace("\r\n", "\r");
            int startIndex = text.IndexOf(startingVerbatimCommand, StringComparison.CurrentCulture);
            int endIndex = text.IndexOf(endingVerbatimCommand, startIndex, StringComparison.CurrentCulture);
            if (startIndex == -1 || endIndex == -1)
            {
                return text;
            }

            startIndex += startingVerbatimCommand.Length + 1;
            var substring = text.Substring(startIndex, endIndex - startIndex).Trim().Split(new char[] { '\r' });
            // Lines prefixed with ". " or "> " are from Stata to echo our commands and can be removed.  Note that we sometimes end
            // up with a line that is just a period - this is a ". " line that got trimmed and can be removed (but we only
            // do that if it is the last line).
            // Stata will put the first line of the command with a ". " prefix, and if it runs over will then use "> " continuation.
            // So that we don't pull out valid lines starting with "> ", we will sequentially iterate over the list of log lines and
            // track the state so that we only pull out "> " lines if they are preceded by a ". " or "> " line.
            var finalLines = new List<string>();
            bool previousLineCommand = false;
            foreach (var line in substring)
            {
                if (line.StartsWith(". "))
                {
                    previousLineCommand = true;
                }
                else if (previousLineCommand && line.StartsWith("> "))
                {
                    previousLineCommand = true;
                }
                else
                {
                    previousLineCommand = false;
                    finalLines.Add(line);
                }
            }
            if (finalLines.Count > 0 && finalLines.Last().Equals("."))
            {
                finalLines = finalLines.Take(finalLines.Count - 1).ToList();
            }

            // Go through the lines until we find the first non-whitespace line.  That's what we will use as the first actual line in the verbatim
            // results.  If that removes everything, we'll return a single blank line.
            var firstIndex = 0;
            for (int index = 0; index < finalLines.Count; index++)
            {
                if (!string.IsNullOrWhiteSpace(finalLines[index]))
                {
                    firstIndex = index;
                    break;
                }
            }

            // Now go through and find the last non-whitespace line.  That's the extent that we'll pull out for the verbatim results.
            var lastIndex = finalLines.Count - 1;
            for (int index = lastIndex; index >= 0; index--)
            {
                if (!string.IsNullOrWhiteSpace(finalLines[index]))
                {
                    lastIndex = index;
                    break;
                }
            }

            // Order matters here -we're going to Take first to remove the ending blank lines, then remove
            // the starting blank lines via Skip (just so we don't have to recalculate the Take count after
            // we Skip a set number of lines).
            finalLines = finalLines.Take(lastIndex + 1).Skip(firstIndex).ToList();
            return string.Join("\r\n", finalLines);
        }

        /// <summary>
        /// Given a macro name, retrieve the value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetMacroValue(string name)
        {
            string result = Application.MacroValue(name);
            // If we get an empty string, try it with the prefix for local macros
            if (string.IsNullOrEmpty(result))
            {
                result = Application.MacroValue(string.Format("{0}{1}", LocalMacroPrefix, name));
            }

            // Nothing yet?  Stata lets you put saved results into a macro, which are actual commands
            // and not anything named.  Our last attempt is to see if that might be the case, and then
            // pull out the value.
            if (string.IsNullOrEmpty(result) && Parser.IsSavedResultCommand(name))
            {
                result = Application.StReturnString(name);
            }
            return result;
        }

        /// <summary>
        /// Determine the string result for a command that returns a single value.  This includes
        /// macros and scalars.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string GetDisplayResult(string command)
        {
            string name;

            // Display values with calculations in them require special handling.  The API does not
            // process them directly, so our workaround is to introduce a local macro to process the
            // calculcation, and then use the downstream macro result handler to pull out the result.
            // This will work even if the same local macro is defined multiple times in the same
            // execution.  In version 3.2, we introduced this step for ALL display values.  While it
            // is admittedly some execution overhead, it saves time (and potential errors) trying to
            // parse every command to see if it's a calculation or system variable (e.g., _pi, _N, _b[val]).
            name = Parser.GetValueName(command);
            command = string.Format("local {0} = {1}", StatTagTempMacroName, name);
            RunCommand(command);
            command = string.Format("display `{0}'", StatTagTempMacroName);

            if (Parser.IsMacroDisplayValue(command))
            {
                name = Parser.GetMacroValueName(command);
                return GetMacroValue(name);
            }

            name = Parser.GetValueName(command);
            int scalarType = Application.ScalarType(name);
            switch (scalarType)
            {
                case ScalarType.String:
                    return Application.ScalarString(name);
                case ScalarType.Numeric:
                    return Application.ScalarNumeric(name).ToString(CultureInfo.CurrentCulture);
                default:
                    // If it's not a scalar type, it's assumed to be a saved type that can be returned
                    return Application.StReturnString(name);
            }
        }

        /// <summary>
        /// Combines the different components of a matrix command into a single structure.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Table GetTableResult(string command)
        {
            // Check to see if we can identify a file name that contains our table data.  If one
            // exists, we will start by returning that.  If there is no file name specified, we
            // will proceed and assume we are pulling data out of a Stata matrix.
            var dataFile = Parser.GetTableDataPath(command);
            if (!string.IsNullOrWhiteSpace(dataFile))
            {
                return DataFileToTable.GetTableResult(GetExpandedFilePath(dataFile));
            }

            var matrixName = Parser.GetTableName(command);
            try
            {
                // If a matrix is not created (because of an error in naming in the code, or because the code may have
                // failed to run), trying to access the API Matrix* functions will thrown an exception.  We need to catch
                // that and provide a more informative error message.
                var rowNames = (string[]) Application.MatrixRowNames(matrixName);
                var columnNames = Application.MatrixColNames(matrixName);
                var rowCount = Application.MatrixRowDim(matrixName) +
                               ((rowNames != null && rowNames.Length > 0) ? 1 : 0);
                var columnCount = Application.MatrixColDim(matrixName) +
                                  ((columnNames != null && columnNames.Length > 0) ? 1 : 0);
                var data = ProcessForMissingValues(Application.MatrixData(matrixName));

                var arrayData = TableUtil.MergeTableVectorsToArray(rowNames, columnNames, data, rowCount, columnCount);
                var table = new Table(rowCount, columnCount, arrayData);
                return table;
            }
            catch (Exception exc)
            {
                throw new Exception(
                    string.Format(
                        "There was an error accessing the matrix '{0}'.\r\n\r\nThis is most often caused when the Stata do file that generates the matrix fails to run in StatTag.  Please verify that your code runs from Stata itself, and if so report this to StatTag@northwestern.edu",
                        matrixName), exc);
            }
        }

        /// <summary>
        /// Identify missing values from the Stata API and explicitly set them to a null result.
        /// </summary>
        /// <remarks>Stata represents missing values as very large integers.  If we don't account for
        /// those results, we will display large integers as a result.  To clean up the results, we
        /// will detect missing values and set them to null.</remarks>
        /// <param name="data"></param>
        /// <returns></returns>
        private string[] ProcessForMissingValues(double[] data)
        {
            var cleanedData = new string[data.Length];
            var missingValue = Application.UtilGetStMissingValue();
            for (int index = 0; index < data.Length; index++)
            {
                cleanedData[index] = (data[index] >= missingValue) ? null : data[index].ToString();
            }

            return cleanedData;
        }

        /// <summary>
        /// Return an expanded, full file path - accounting for variables, functions, relative paths, etc.
        /// </summary>
        /// <param name="saveLocation">A Stata command that will be translated into a file path.</param>
        /// <returns>The full file path</returns>
        private string GetExpandedFilePath(string saveLocation)
        {
            // If the save location is not a macro, and it appears to be a relative path, translate it into a fully
            // qualified path based on Stata's current environment.
            if (Parser.HasMacroInCommand(saveLocation))
            {
                var macros = Parser.GetMacros(saveLocation);
                foreach (var macro in macros)
                {
                    var result = GetMacroValue(macro);
                    saveLocation = Parser.ReplaceMacroWithValue(saveLocation, macro, result);
                }
            }
            else if (Parser.IsRelativePath(saveLocation))
            {
                // Attempt to find the current working directory.  If we are not able to find it, or the value we end up
                // creating doesn't exist, we will just proceed with whatever image location we had previously.
                var results =
                    RunCommands(new string[] { DisplayWorkingDirectoryCommand },
                        new Tag() {Type = Constants.TagType.Value});
                if (results != null && results.Length > 0)
                {
                    var path = results.First().ValueResult;
                    var correctedPath = Path.GetFullPath(Path.Combine(path, saveLocation));
                    if (File.Exists(correctedPath))
                    {
                        saveLocation = correctedPath;
                    }
                }
            }

            return saveLocation;
        }

        /// <summary>
        /// Utility function to continue looping until Stata appears to be free.
        /// <param name="waitDuration">Milliseconds to wait on each iteration</param>
        /// <param name="maxWait">How many times to check and wait before ending the check.  If set to null, will poll indefinitely.</param>
        /// </summary>
        private void PauseStataUntilFree(int waitDuration = 100, int? maxWait = null)
        {
            PauseStataUntilFree(Application, waitDuration, maxWait);
        }

        private static void PauseStataUntilFree(stata.StataOLEApp application, int waitDuration = 100, int? maxWait = null)
        {
            if (application == null)
            {
                return;
            }

            int waitCounter = 0;
            while (application.UtilIsStataFree() == 0)
            {
                Thread.Sleep(waitDuration);
                waitCounter++;

                // If we have a guard in place for maximum wait time, check it and escape
                // the loop if we're past the threshold.
                if (maxWait.HasValue && waitCounter >= maxWait.Value)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Run a Stata command and provide the result of the command (if one should be returned).
        /// </summary>
        /// <param name="command">The command to run, taken from a Stata do file</param>
        /// <returns>The result of the command, or null if the command does not provide a result.</returns>
        public CommandResult RunCommand(string command, Tag tag = null)
        {
            // If we are tracking verbatim, we don't do any special tag processing.  Likewise, if we don't have
            // a tag, we don't want to do any special handling or processing of results.  This way any non-
            // tagged code is going to go ahead and be executed.
            if (!IsTrackingVerbatim && tag != null)
            {
                if (Parser.IsValueDisplay(command))
                {
                    return new CommandResult() { ValueResult = GetDisplayResult(command) };
                }

                // Because we are being more open what we allow for tables, we are now going
                // to only allow table results when we have a tag that is a table type.  Note that
                // we will short-circuit executing the command ONLY when we have a matrix type
                // of result.  If we think we have a data file, we need to execute the command.  That
                // is why we make two checks within this method for table results, the first one here for
                // matrix results, and the second one for any table result.
                if (tag.Type == Constants.TagType.Table && Parser.IsMatrix(command))
                {
                    return new CommandResult() { TableResult = GetTableResult(command) };
                }
            }

            // Additional special handling for table1.  In order to force the output of table1 to be an XLSX file,
            // we need to trap its creation.  Here we detect if it looks like a non-XLSX file extension (with two
            // common types - this is not an exhaustive list) and change the command appropriately.
            if (!IsTrackingVerbatim && tag != null && tag.Type == Constants.TagType.Table &&
                Parser.IsTableResult(command) && Parser.IsTable1Command(command))
            {
                var result = Parser.GetTableDataPath(command);
                // We want to force XLS -> XLSX for the table1 command, and will convert CSV (which isn't valid, but people can still do it) to XLSX.
                // Since the command has already run, we do have an extra step to rename the file to the extension we want.  Keeping in mind that the
                // table1 can only create Excel files, so we are allowed to make assumptions about formats.
                if (result.EndsWith(".xls"))
                {
                    command = command.Replace(result, result + "x");
                }
                else if (result.EndsWith(".csv"))
                {
                    var newFile = result.Substring(0, result.Length - 4) + ".xlsx";
                    command = command.Replace(result, newFile);
                }
            }

            // We wrap all commands in a capture/noisily block so that every execution will check for errors and make them accessible to us
            // via UtilStataErrorCode.  Without this, DoCommandAsync isn't able to return the error codes and we just blindly assume that
            // everything works fine.
            bool isCapturable = Parser.IsCapturableBlock(command);
            string captureCommand = 
                isCapturable ? string.Format( "capture {{\r\nnoisily {{\r\n{0}\r\n}}\r\n}}", command) : command;

            int returnCode = Application.DoCommandAsync(captureCommand);
            if (returnCode != 0)
            {
                throw new Exception(string.Format("There was an error while executing the Stata command: {0}", command));
            }

            PauseStataUntilFree();

            // Check for the error code.  If there is an error code set, we will throw an exception to report the error message and halt
            // execution.
            int errorCode = Application.UtilStataErrorCode();
            if (errorCode != 0)
            {
                throw new Exception(
                    string.Format("There was an error while executing the Stata command: {0} (error code {1} - {2})",
                        command, errorCode, GetStataErrorDescription(errorCode)));
            }

            if (!IsTrackingVerbatim && tag != null)
            {
                if (Parser.IsImageExport(command))
                {
                    return new CommandResult() { FigureResult = GetExpandedFilePath(Parser.GetImageSaveLocation(command)) };
                }

                // Because we are being more open what we allow for tables, we are now going
                // to only allow table results when we have a tag that is a table type.  See above why we
                // have two checks for table results in this method.
                if (tag.Type == Constants.TagType.Table && Parser.IsTableResult(command))
                {
                    return new CommandResult() { TableResultPromise = GetExpandedFilePath(Parser.GetTableDataPath(command)) };
                }
            }
            
            return null;
        }

        public void Dispose()
        {
            Hide();
            Application = null;
        }

        public static bool UnregisterAutomationAPI(string path)
        {
            return RunProcess(path, UnregisterParameter);
        }

        public static bool RegisterAutomationAPI(string path)
        {
            return RunProcess(path, RegisterParameter);
        }


        public static bool IsAutomationEnabled()
        {
            try
            {
                var application = new stata.StataOLEApp();
                application.UtilShowStata(StataHidden);
                application = null;
                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        /// <summary>
        /// Execute a process as an administrator.  Used for managing the automation API.
        /// </summary>
        /// <param name="path">Path of the process to run</param>
        /// <param name="parameters">Parameters used by the process.s</param>
        /// <returns>true if successful, false otherwise</returns>
        protected static bool RunProcess(string path, string parameters)
        {
            var startInfo = new ProcessStartInfo(path, parameters)
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "runas"
            };
            // Allows running as administrator, needed to change COM registration
            var process = Process.Start(startInfo);
            if (process == null)
            {
                return false;
            }

            process.WaitForExit(30000);

            return (0 == process.ExitCode);
        }

        /// <summary>
        /// Determine if the Stata executable is currently running
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsStataRunning(string path)
        {
            // If we don't know about the Stata EXE (or if what the user provided is not valid),
            // we're not going to be able to detect Stata running so go ahead and claim it's not.
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return false;
            }

            var exeName = Path.GetFileNameWithoutExtension(path);
            var processes = Process.GetProcessesByName(exeName);
            return processes.Any(p => !p.HasExited);
        }

        // Taken from : http://www.stata.com/manuals13/perror.pdf
        // This list is also maintained in StatTagMac - if changed here, please also change there.
        private string GetStataErrorDescription(int errorCode)
        {
            string errorDescription = string.Empty;
  
            switch(errorCode)
            {
                case 1: errorDescription = @"You pressed Break. This is not considered an error."; break;
                case 2: errorDescription = "connection timed out -- see help r(2) for troubleshooting"; break;
                case 3: errorDescription = "no dataset in use"; break;
                case 4: errorDescription = "no; data in memory would be lost"; break;
                case 5: errorDescription = "not sorted"; break;
                case 6: errorDescription = "Return code from confirm existence when string does not exist."; break;
                case 7: errorDescription = "'___' found where ___ expected"; break;
                case 8: errorDescription = "error — Display generic error message and exit"; break;
                case 9: errorDescription = "assertion is false"; break;
                case 10: errorDescription = "error — Display generic error message and exit"; break;
                case 12: errorDescription = "error — Display generic error message and exit"; break;
                case 18: errorDescription = "you must start with an empty dataset"; break;
                case 100: errorDescription = "varlist required = exp required using required"; break;
                case 101: errorDescription = "varlist not allowed weights not allowed in range not allowed if not allowed"; break;
                case 102: errorDescription = "too few variables specified"; break;
                case 103: errorDescription = "too many variables specified"; break;
                case 104: errorDescription = "nothing to input"; break;
                case 106: errorDescription = "variable   is   in master but   in using data"; break;
                case 107: errorDescription = "not possible with numeric variable"; break;
                case 108: errorDescription = "not possible with string variable"; break;
                case 109: errorDescription = "type mismatch"; break;
                case 110: errorDescription = "already defined"; break;
                case 111: errorDescription = "not found no variables defined"; break;
                case 119: errorDescription = "statement out of context"; break;
                case 120: errorDescription = "invalid %format"; break;
                case 121: errorDescription = "invalid numlist"; break;
                case 122: errorDescription = "invalid numlist has"; break;
                case 123: errorDescription = "invalid numlist has"; break;
                case 124: errorDescription = "invalid numlist has"; break;
                case 125: errorDescription = "invalid numlist has"; break;
                case 126: errorDescription = "invalid numlist has"; break;
                case 127: errorDescription = "invalid numlist has"; break;
                case 130: errorDescription = "expression too long too many SUMs"; break;
                case 131: errorDescription = "not possible with test"; break;
                case 132: errorDescription = "too many '(' or '[' or too many ')' or ']'"; break;
                case 133: errorDescription = "unknown function   ()"; break;
                case 135: errorDescription = "not possible with weighted data"; break;
                case 140: errorDescription = "repeated categorical variable in term"; break;
                case 141: errorDescription = "repeated term"; break;
                case 145: errorDescription = "term contains more than 8 variables"; break;
                case 146: errorDescription = "too many variables or values (matsize too small)"; break;
                case 147: errorDescription = "term not in model"; break;
                case 148: errorDescription = "too few categories"; break;
                case 149: errorDescription = "too many categories"; break;
                case 151: errorDescription = "non r-class program may not set r()"; break;
                case 152: errorDescription = "non e-class program may not set e()"; break;
                case 153: errorDescription = "non s-class program may not set s()"; break;
                case 161: errorDescription = "ado-file has commands outside of program define ...end"; break;
                case 162: errorDescription = "ado-file does not define command"; break;
                case 170: errorDescription = "unable to chdir"; break;
                case 175: errorDescription = "factor level out of range"; break;
                case 180: errorDescription = "invalid attempt to modify label"; break;
                case 181: errorDescription = "may not label strings"; break;
                case 182: errorDescription = "not labeled"; break;
                case 184: errorDescription = "options   and   may not be combined"; break;
                case 190: errorDescription = "request may not be combined with by"; break;
                case 191: errorDescription = "request may not be combined with by() option"; break;
                case 196: errorDescription = "could not restore sort order because variables were dropped"; break;
                case 197: errorDescription = "invalid syntax"; break;
                case 198: errorDescription = "invalid syntax"; break;
                case 199: errorDescription = "unrecognized command"; break;
                case 301: errorDescription = "last estimates not found"; break;
                case 302: errorDescription = "last test not found"; break;
                case 303: errorDescription = "equation not found"; break;
                case 304: errorDescription = "ml model not found"; break;
                case 305: errorDescription = "ml model not found Same as 304."; break;
                case 310: errorDescription = "not possible because object(s) in use"; break;
                case 321: errorDescription = "requested action not valid after most recent estimation command"; break;
                case 322: errorDescription = "something that should be true of your estimation results is not"; break;
                case 399: errorDescription = "may not drop constant"; break;
                case 401: errorDescription = "may not use noninteger frequency weights"; break;
                case 402: errorDescription = "negative weights encountered"; break;
                case 404: errorDescription = "not possible with pweighted data"; break;
                case 406: errorDescription = "not possible with analytic weights"; break;
                case 407: errorDescription = "weights must be the same for all observations in a group"; break;
                case 409: errorDescription = "no variance"; break;
                case 411: errorDescription = "nonpositive values encountered has negative values"; break;
                case 412: errorDescription = "redundant or inconsistent constraints"; break;
                case 416: errorDescription = "missing values encountered"; break;
                case 420: errorDescription = "groups found, 2 required"; break;
                case 421: errorDescription = "could not determine between-subject error term; use bse() option"; break;
                case 422: errorDescription = "could not determine between-subject basic unit; use bseunit() option"; break;
                case 430: errorDescription = "convergence not achieved"; break;
                case 450: errorDescription = "is not a 0/1 variable number of successes invalid"; break;
                case 451: errorDescription = "invalid values for time variable"; break;
                case 452: errorDescription = "invalid values for factor variable"; break;
                case 459: errorDescription = "something that should be true of your data is not"; break;
                case 460: errorDescription = "fpc must be >= 0"; break;
                case 461: errorDescription = "fpc for all observations within a stratum must be the same There is a problem with your fpc variable; see [SVY] svyset."; break;
                case 462: errorDescription = "fpc must be <= 1 if a rate, or >= no. sampled PSUs per stratum if PSU totals There is a problem with your fpc variable; see [SVY] svyset."; break;
                case 463: errorDescription = "sum of weights equals zero"; break;
                case 464: errorDescription = "poststratum weights must be constant within poststrata"; break;
                case 465: errorDescription = "poststratum weights must be >= 0"; break;
                case 466: errorDescription = "standardization weights must be constant within standard strata"; break;
                case 467: errorDescription = "standardization weights must be >= 0"; break;
                case 471: errorDescription = "esample() invalid"; break;
                case 480: errorDescription = "starting values invalid or some RHS variables have missing values"; break;
                case 481: errorDescription = "equation/system not identified"; break;
                case 482: errorDescription = "nonpositive value(s) among   , cannot log transform"; break;
                case 491: errorDescription = "could not find feasible values"; break;
                case 498: errorDescription = "various messages"; break;
                case 499: errorDescription = "various messages"; break;
                case 501: errorDescription = "matrix operation not found"; break;
                case 503: errorDescription = "conformability error"; break;
                case 504: errorDescription = "matrix has missing values"; break;
                case 505: errorDescription = "matrix not symmetric"; break;
                case 506: errorDescription = "matrix not positive definite"; break;
                case 507: errorDescription = "name conflict"; break;
                case 508: errorDescription = "matrix has zero values"; break;
                case 509: errorDescription = "matrix operators that return matrices not allowed in this context"; break;
                case 601: errorDescription = "file not found"; break;
                case 602: errorDescription = "file already exists"; break;
                case 603: errorDescription = "file could not be opened"; break;
                case 604: errorDescription = "log file already open"; break;
                case 606: errorDescription = "no log file open"; break;
                case 607: errorDescription = "no cmdlog file open"; break;
                case 608: errorDescription = "file is read-only; cannot be modified or erased"; break;
                case 609: errorDescription = "file xp format"; break;
                case 610: errorDescription = "file   not Stata format"; break;
                case 611: errorDescription = "record too long"; break;
                case 612: errorDescription = "unexpected end of file"; break;
                case 613: errorDescription = "file does not contain dictionary"; break;
                case 614: errorDescription = "dictionary invalid"; break;
                case 616: errorDescription = "wrong number of values in checksum file"; break;
                case 621: errorDescription = "already preserved"; break;
                case 622: errorDescription = "nothing to restore"; break;
                case 631: errorDescription = "host not found"; break;
                case 632: errorDescription = "web filename not supported in this context"; break;
                case 633: errorDescription = "may not write files over Internet"; break;
                case 639: errorDescription = "file transmission error (checksums do not match)"; break;
                case 640: errorDescription = "package file too long"; break;
                case 641: errorDescription = "package file invalid"; break;
                case 651: errorDescription = "may not seek past end of file"; break;
                case 660: errorDescription = "proxy host not found"; break;
                case 662: errorDescription = "proxy server refused request to send"; break;
                case 663: errorDescription = "remote connection to proxy failed"; break;
                case 665: errorDescription = "could not set socket nonblocking"; break;
                case 667: errorDescription = "wrong version winsock.dll"; break;
                case 668: errorDescription = "could not find a valid winsock.dll"; break;
                case 669: errorDescription = "invalid URL"; break;
                case 670: errorDescription = "invalid network port number"; break;
                case 671: errorDescription = "unknown network protocol"; break;
                case 672: errorDescription = "server refused to send file"; break;
                case 673: errorDescription = "authorization required by server"; break;
                case 674: errorDescription = "unexpected response from server"; break;
                case 675: errorDescription = "server reported server error"; break;
                case 676: errorDescription = "server refused request to send"; break;
                case 677: errorDescription = "remote connection failed"; break;
                case 678: errorDescription = "could not open local network socket"; break;
                case 681: errorDescription = "too many open files"; break;
                case 682: errorDescription = "could not connect to odbc dsn"; break;
                case 683: errorDescription = "could not fetch variable in odbc table"; break;
                case 688: errorDescription = "file is corrupt"; break;
                case 691: errorDescription = "I/O error"; break;
                case 692: errorDescription = "file I/O error on read"; break;
                case 693: errorDescription = "file I/O error on write"; break;
                case 694: errorDescription = "could not rename file"; break;
                case 695: errorDescription = "could not copy file"; break;
                case 696: errorDescription = "is temporarily unavailable"; break;
                case 699: errorDescription = "insufficient disk space"; break;
                case 702: errorDescription = "op. sys. refused to start new process"; break;
                case 703: errorDescription = "op. sys. refused to open pipe"; break;
                case 791: errorDescription = "system administrator will not allow you to change this setting"; break;
                case 900: errorDescription = "no room to add more variables"; break;
                case 901: errorDescription = "no room to add more observations"; break;
                case 902: errorDescription = "no room to add more variables because of width"; break;
                case 903: errorDescription = "no room to promote variable (e.g., change int to float) because of width"; break;
                case 907: errorDescription = "maxvar too small"; break;
                case 908: errorDescription = "matsize too small"; break;
                case 909: errorDescription = "op. sys. refuses to provide memory"; break;
                case 910: errorDescription = "value too small"; break;
                case 912: errorDescription = "value too large"; break;
                case 913: errorDescription = "op. sys. refuses to provide sufficient memory"; break;
                case 914: errorDescription = "op. sys. refused to allow Stata to open a temporary file"; break;
                case 920: errorDescription = "too many macros"; break;
                case 950: errorDescription = "insufficient memory"; break;
                case 1000: errorDescription = "system limit exceeded - see manual See [R] limits."; break;
                case 1001: errorDescription = "too many values"; break;
                case 1002: errorDescription = "too many by variables"; break;
                case 1003: errorDescription = "too many options"; break;
                case 1004: errorDescription = "command too long"; break;
                case 1400: errorDescription = "numerical overflow"; break;
                case 2000: errorDescription = "no observations"; break;
                case 2001: errorDescription = "insufficient observations"; break;
                case 3698: errorDescription = "file seek error"; break;
                case 3000: errorDescription = "(message varies)"; break;
                case 3001: errorDescription = "incorrect number of arguments"; break;
                case 3002: errorDescription = "identical arguments not allowed"; break;
                case 3010: errorDescription = "attempt to dereference NULL pointer"; break;
                case 3011: errorDescription = "invalid lval"; break;
                case 3012: errorDescription = "undefined operation on pointer"; break;
                case 3020: errorDescription = "class child/parent compiled at different times"; break;
                case 3021: errorDescription = "class compiled at different times"; break;
                case 3022: errorDescription = "function not supported on this platform"; break;
                case 3101: errorDescription = "matrix found where function required"; break;
                case 3102: errorDescription = "function found where matrix required"; break;
                case 3103: errorDescription = "view found where array required"; break;
                case 3104: errorDescription = "array found where view required"; break;
                case 3200: errorDescription = "conformability error"; break;
                case 3201: errorDescription = "vector required"; break;
                case 3202: errorDescription = "rowvector required"; break;
                case 3203: errorDescription = "colvector required"; break;
                case 3204: errorDescription = "matrix found where scalar required"; break;
                case 3205: errorDescription = "square matrix required"; break;
                case 3206: errorDescription = "invalid use of view containing op.vars"; break;
                case 3250: errorDescription = "type mismatch"; break;
                case 3251: errorDescription = "nonnumeric found where numeric required"; break;
                case 3252: errorDescription = "noncomplex found where complex required"; break;
                case 3253: errorDescription = "nonreal found where real required"; break;
                case 3254: errorDescription = "nonstring found where string required"; break;
                case 3255: errorDescription = "real or string required"; break;
                case 3256: errorDescription = "numeric or string required"; break;
                case 3257: errorDescription = "nonpointer found where pointer required"; break;
                case 3258: errorDescription = "nonvoid found where void required"; break;
                case 3259: errorDescription = "nonstruct found where struct required"; break;
                case 3260: errorDescription = "nonclass found where class required"; break;
                case 3261: errorDescription = "non class/struct found where class/struct required"; break;
                case 3300: errorDescription = "argument out of range"; break;
                case 3301: errorDescription = "subscript invalid"; break;
                case 3302: errorDescription = "invalid %fmt"; break;
                case 3303: errorDescription = "invalid permutation vector"; break;
                case 3304: errorDescription = "struct nested too deeply"; break;
                case 3305: errorDescription = "class nested too deeply"; break;
                case 3351: errorDescription = "argument has missing values"; break;
                case 3352: errorDescription = "singular matrix"; break;
                case 3353: errorDescription = "matrix not positive definite"; break;
                case 3360: errorDescription = "failure to converge"; break;
                case 3492: errorDescription = "resulting string too long"; break;
                case 3498: errorDescription = "(message varies)"; break;
                case 3499: errorDescription = "not found"; break;
                case 3500: errorDescription = "invalid Stata variable name"; break;
                case 3598: errorDescription = "Stata returned error"; break;
                case 3601: errorDescription = "invalid file handle"; break;
                case 3602: errorDescription = "invalid filename"; break;
                case 3603: errorDescription = "invalid file mode"; break;
                case 3611: errorDescription = "too many open files"; break;
                case 3612: errorDescription = "file too large for 32-bit Stata"; break;
                case 3621: errorDescription = "attempt to write read-only file"; break;
                case 3622: errorDescription = "attempt to read write-only file"; break;
                case 3623: errorDescription = "attempt to seek append-only file"; break;
                case 3900: errorDescription = "out of memory"; break;
                case 3901: errorDescription = "macro memory in use"; break;
                case 3930: errorDescription = "error in LAPACK routine"; break;
                case 3995: errorDescription = "unallocated function"; break;
                case 3996: errorDescription = "built-in unallocated"; break;
                case 3997: errorDescription = "unimplemented opcode"; break;
                case 3998: errorDescription = "stack overflow"; break;
                case 3999: errorDescription = "system assertion false"; break;

                default:
                {
                    errorDescription = "Unknown error";
                    break;
                }
            }
  
            if(errorCode >= 4000 && errorCode <= 4999)
            {
                errorDescription = "Unknown class error";
            }
  
            return errorDescription;
        }
    }
}
