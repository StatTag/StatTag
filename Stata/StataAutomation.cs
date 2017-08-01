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

            // We wrap all commands in a capture/noisily block so that every execution will check for errors and make them accessible to us
            // via UtilStataErrorCode.  Without this, DoCommandAsync isn't able to return the error codes and we just blindly assume that
            // everything works fine.
            string captureCommand = "capture {\r\nnoisily {\r\n" + command + "\r\n}\r\n}";

            int returnCode = Application.DoCommandAsync(captureCommand);
            if (returnCode != 0)
            {
                throw new Exception(string.Format("There was an error while executing the Stata command: {0}", command));
            }

            while (Application.UtilIsStataFree() == 0)
            {
                Thread.Sleep(100);
            }

            // Check for the error code.  If there is an error code set, we will throw an exception to report the error message and halt
            // execution.
            int errorCode = Application.UtilStataErrorCode();
            if (errorCode != 0)
            {
                throw new Exception(
                    string.Format("There was an error while executing the Stata command: {0} (error code {1} - {2})",
                        command, errorCode, GetStataErrorDescription(errorCode)));
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
