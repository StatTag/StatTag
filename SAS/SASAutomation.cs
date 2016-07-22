using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using StatTag.Core.Parser;

namespace SAS
{
    public class SASAutomation : IStatAutomation
    {
        private const string DisplayMacroValueCommand = "%PUT";

        private SasServer Server = null;
        protected SASParser Parser { get; set; }

        public SASAutomation()
        {
            Parser = new SASParser();
        }

        public string GetInitializationErrorMessage()
        {
            return "Could not communicate with SAS.  SAS may not be fully installed, or be missing some of the automation pieces that StatTag requires.";
        }

        public void Dispose()
        {
            if (Server != null)
            {
                Server.Close();
            }
        }

        public bool Initialize()
        {
            //TODO Do we want to allow remote connections, or just localhost?
            Server = new SasServer()
            {
                UseLocal = true
            };
            Server.Connect();
            return true;
        }

        public CommandResult[] RunCommands(string[] commands)
        {
            var commandResults = new List<CommandResult>();
            foreach (var command in commands)
            {
                var result = RunCommand(command);
                if (result != null && !result.IsEmpty())
                {
                    commandResults.Add(result);
                }
            }

            return commandResults.ToArray();
        }

        /// <summary>
        /// Run a Stata command and provide the result of the command (if one should be returned).
        /// </summary>
        /// <param name="command">The command to run, taken from a Stata do file</param>
        /// <returns>The result of the command, or null if the command does not provide a result.</returns>
        public CommandResult RunCommand(string command)
        {
            Array carriageControls;
            Array lineTypeArray;
            Array logLineArray;

            Server.Workspace.LanguageService.Submit(command);

            // These calls need to be made because they cause SAS to initialize internal structures that
            // are used when FlushLogLines is called.  Even though we're not really doing anything with these
            // values, don't remove these calls.
            SAS.LanguageServiceCarriageControl carriageControl = new SAS.LanguageServiceCarriageControl();
            SAS.LanguageServiceLineType lineType = new SAS.LanguageServiceLineType();

            Server.Workspace.LanguageService.FlushLogLines(10000, out carriageControls, out lineTypeArray,
                out logLineArray);

            // For all of the lines that we got back from SAS, we want to find those that are of the Normal type (meaning they
            // would contain some type of result/output), and that aren't empty.  Filtering empty lines is done because SAS
            // will dump out a bunch of extra output when we run, including blank Normal lines.
            var relevantLines = new List<string>();
            var lineTypes = lineTypeArray.OfType<LanguageServiceLineType>().ToArray();
            var logLines = logLineArray.OfType<string>().ToArray();
            for (int index = 0; index < lineTypes.Length; index++)
            {
                var line = logLines[index];
                if (lineTypes[index] == LanguageServiceLineType.LanguageServiceLineTypeNormal
                    && !string.IsNullOrWhiteSpace(line))
                {
                    relevantLines.Add(line);
                }
            }

            // If we have a value command, we will pull out the last relevant line from the output.
            if (Parser.IsValueDisplay(command))
            {
                return new CommandResult() { ValueResult = relevantLines.LastOrDefault() };
            }

            if (Parser.IsImageExport(command))
            {
                return new CommandResult() { FigureResult = Parser.GetImageSaveLocation(command) };
            }

            return null;
        }

        public bool IsReturnable(string command)
        {
            return Parser.IsValueDisplay(command) || Parser.IsImageExport(command) || Parser.IsTableResult(command);
        }
    }
}
