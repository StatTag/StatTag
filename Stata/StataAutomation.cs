using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        // The following are constants used to manage the Stata Automation API
        public const string RegisterParameter = "/Register";
        public const string UnregisterParameter = "/Unregister";

        public string GetInitializationErrorMessage()
        {
            return
                "Could not communicate with Stata.  You will need to enable Stata Automation (not done by default) to run this code in StatTag.\r\n\r\nThis can be done from StatTag > Settings, or see http://www.stata.com/automation";
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

        public bool Initialize()
        {
            try
            {
                OpenLogs = new List<StataParser.Log>();
                Application = new stata.StataOLEApp();
                Application.DoCommand(DisablePagingCommand);
                Show();
            }
            catch (COMException comExc)
            {
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
                    var result = RunCommand(command);

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

            int additionalOffset = 1;
            startIndex += startingVerbatimCommand.Length + additionalOffset;
            if (text[startIndex] == '\r')
            {
                additionalOffset++;
                startIndex++;
            }
            var substring = text.Substring(startIndex, endIndex - startIndex - additionalOffset).TrimEnd('\r').Split(new char[] { '\r' });
            var finalLines = substring.Where(line => !line.StartsWith(". ")).ToList();
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
            // execution.
            if (Parser.IsCalculatedDisplayValue(command))
            {
                name = Parser.GetValueName(command);
                command = string.Format("local {0} = {1}", StatTagTempMacroName, name);
                RunCommand(command);
                command = string.Format("display `{0}'", StatTagTempMacroName);
            }

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
            var matrixName = Parser.GetTableName(command);
            var rowNames = (string[])Application.MatrixRowNames(matrixName);
            var columnNames = Application.MatrixColNames(matrixName);
            var rowCount = Application.MatrixRowDim(matrixName) + ((rowNames != null && rowNames.Length > 0) ? 1 : 0);
            var columnCount = Application.MatrixColDim(matrixName) + ((columnNames != null && columnNames.Length > 0) ? 1 : 0);
            var data = ProcessForMissingValues(Application.MatrixData(matrixName));

            var arrayData = TableUtil.MergeTableVectorsToArray(rowNames, columnNames, data, rowCount, columnCount);
            var table = new Table(rowCount, columnCount, arrayData);
            return table;
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
        /// Run a Stata command and provide the result of the command (if one should be returned).
        /// </summary>
        /// <param name="command">The command to run, taken from a Stata do file</param>
        /// <returns>The result of the command, or null if the command does not provide a result.</returns>
        public CommandResult RunCommand(string command)
        {
            if (!IsTrackingVerbatim)
            {
                if (Parser.IsValueDisplay(command))
                {
                    return new CommandResult() { ValueResult = GetDisplayResult(command) };
                }

                if (Parser.IsTableResult(command))
                {
                    return new CommandResult() { TableResult = GetTableResult(command) };
                }
            }

            int returnCode = Application.DoCommandAsync(command);
            if (returnCode != 0)
            {
                throw new Exception(string.Format("There was an error while executing the Stata command: {0}", command));
            }

            while (Application.UtilIsStataFree() == 0)
            {
                Thread.Sleep(100);
            }

            if (Parser.IsImageExport(command) && !IsTrackingVerbatim)
            {
                var imageLocation = Parser.GetImageSaveLocation(command);
                if (imageLocation.Contains(StataParser.MacroDelimiters[0]))
                {
                    var macros = Parser.GetMacros(imageLocation);
                    foreach (var macro in macros)
                    {
                        var result = GetMacroValue(macro);
                        imageLocation = ReplaceMacroWithValue(imageLocation, macro, result);
                    }
                }

                return new CommandResult() { FigureResult = imageLocation };
            }
            
            return null;
        }

        /// <summary>
        /// Given a macro name that appears in a command string, replace it with its expanded value
        /// </summary>
        /// <param name="originalString"></param>
        /// <param name="macro"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ReplaceMacroWithValue(string originalString, string macro, string value)
        {
            return originalString.Replace(
                string.Format("{0}{1}{2}", StataParser.MacroDelimiters[0], macro, StataParser.MacroDelimiters[1]), value);
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
    }
}
