using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StatTag.Core.Models;
using StatTag.Core.Parser;

namespace Stata
{
    public class Automation : IDisposable
    {
        public const string LocalMacroPrefix = "_";

        /// <summary>
        /// This is a special local macro name that is being used within StatTag.
        /// </summary>
        public const string StatTagTempMacroName = "__st_tmp_display_value";

        public const string DisablePagingCommand = "set more off";

        // The following are constants used to manage the Stata Automation API
        public const string RegisterParameter = "/Register";
        public const string UnregisterParameter = "/Unregister";

        protected stata.StataOLEApp Application { get; set; }
        protected StatTag.Core.Parser.Stata Parser { get; set; }
        protected List<string> OpenLogs { get; set; } 

        /// <summary>
        /// The collection of all possible Stata process names.  These are converted to
        /// lower case here because the comparison we do later depends on conversion to
        /// lower case.
        /// </summary>
        private static readonly List<string> StataProcessNames = new List<string>(new []
        {
            "statase-64",
            "statamp-64",
            "stata-64",
            "statase",
            "statamp",
            "statase"
        });

        private static class ScalarType
        {
            public const int NotFound = 0;
            public const int Numeric = 1;
            public const int String = 2;
        }

        private const int StataHidden = 1;
        private const int MinimizeStata = 2;
        private const int ShowStata = 3;

        public Automation()
        {
            Parser = new StatTag.Core.Parser.Stata();
        }

        /// <summary>
        /// Determine if a copy of Stata is running
        /// </summary>
        /// <returns></returns>
        public static bool IsAppRunning()
        {
            return Process.GetProcesses().Any(process => StataProcessNames.Contains(process.ProcessName.ToLower()));
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
                OpenLogs = new List<string>();
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

        public CommandResult[] CombineAndRunCommands(string[] commands)
        {
            string combinedCommand = string.Join("\r\n", commands);
            return RunCommands(new[] { combinedCommand });
        }

        /// <summary>
        /// Run a collection of commands and provide all applicable results.
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        public CommandResult[] RunCommands(string[] commands)
        {
            try
            {
                var commandResults = new List<CommandResult>();
                foreach (var command in commands)
                {
                    if (Parser.IsStartingLog(command))
                    {
                        OpenLogs.AddRange(Parser.GetLogType(command));
                    }

                    var result = RunCommand(command);
                    if (result != null && !result.IsEmpty())
                    {
                        commandResults.Add(result);
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
                    RunCommand(string.Format("{0} close", openLog));
                }

                // Since we have closed all logs, clear the list we were tracking.
                OpenLogs.Clear();

                throw exc;
            }
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
                string result = Application.MacroValue(name);
                // If we get an empty string, try it with the prefix for local macros
                if (string.IsNullOrEmpty(result))
                {
                    result = Application.MacroValue(string.Format("{0}{1}", LocalMacroPrefix, name));
                }
                return result;
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
            var table = new Table(
                Application.MatrixRowNames(matrixName),
                Application.MatrixColNames(matrixName),
                Application.MatrixRowDim(matrixName),
                Application.MatrixColDim(matrixName),
                ProcessForMissingValues(Application.MatrixData(matrixName))
            );

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
        private double?[] ProcessForMissingValues(double[] data)
        {
            var cleanedData = new double?[data.Length];
            var missingValue = Application.UtilGetStMissingValue();
            for (int index = 0; index < data.Length; index++)
            {
                cleanedData[index] = (data[index] >= missingValue) ? (double?) null : data[index];
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
            if (Parser.IsValueDisplay(command))
            {
                return new CommandResult() { ValueResult = GetDisplayResult(command)};
            }

            if (Parser.IsTableResult(command))
            {
                return new CommandResult() { TableResult = GetTableResult(command) };
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

            if (Parser.IsImageExport(command))
            {
                return new CommandResult() { FigureResult = Parser.GetImageSaveLocation(command) };
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
