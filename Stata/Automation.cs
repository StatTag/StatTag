using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;
using AnalysisManager.Core.Parser;

namespace Stata
{
    public class Automation : IDisposable
    {
        public const string LocalMacroPrefix = "_";

        // The following are constants used to manage the Stata Automation API
        public const string RegisterParameter = "/Register";
        public const string UnregisterParameter = "/Unregister";

        protected stata.StataOLEApp Application { get; set; }
        protected AnalysisManager.Core.Parser.Stata Parser { get; set; }

        private static class ScalarType
        {
            public const int NotFound = 0;
            public const int Numeric = 1;
            public const int String = 2;
        }

        private const int StataHidden = 1;

        public Automation()
        {
            Parser = new AnalysisManager.Core.Parser.Stata();
        }

        public bool Initialize()
        {
            try
            {
                Application = new stata.StataOLEApp();
                Application.UtilShowStata(StataHidden);
            }
            catch (COMException comExc)
            {
                return false;
            }

            return true;
        }

        public bool IsReturnable(string command)
        {
            return Parser.IsValueDisplay(command) || Parser.IsImageExport(command) || Parser.IsTableResult(command);
        }

        public CommandResult[] RunCommands(string[] commands)
        {
            return commands.Select(command => RunCommand(command)).Where(
                result => result != null && !result.IsEmpty()).ToArray();
        }

        public string GetDisplayResult(string command)
        {
            string name;
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
                    return Application.ScalarNumeric(name).ToString();
                default:
                    // If it's not a scalar type, it's assumed to be a saved type that can be returned
                    return Application.StReturnString(name);
            }
        }

        public Table GetTableResult(string command)
        {
            var matrixName = Parser.GetTableName(command);
            var table = new Table(
                Application.MatrixRowNames(matrixName),
                Application.MatrixColNames(matrixName),
                Application.MatrixRowDim(matrixName),
                Application.MatrixColDim(matrixName),
                Application.MatrixData(matrixName)
            );

            return table;
        }

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

            int returnCode = Application.DoCommand(command);
            if (returnCode != 0)
            {
                throw new Exception(string.Format("There was an error while executing the Stata command: {0}", command));
            }

            if (Parser.IsImageExport(command))
            {
                return new CommandResult() { FigureResult = Parser.GetImageSaveLocation(command) };
            }
            
            return null;
        }

        public void Dispose()
        {
            Application = null;
        }

        public static bool UnregisterAutomationAPI(string path)
        {
            return RunCommand(path, UnregisterParameter);
        }

        public static bool RegisterAutomationAPI(string path)
        {
            return RunCommand(path, RegisterParameter);
        }

        protected static bool RunCommand(string path, string parameters)
        {
            var startInfo = new ProcessStartInfo(path, parameters);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Verb = "runas";  // Allows running as administrator, needed to change COM registration
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
