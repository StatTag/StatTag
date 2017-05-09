using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        protected List<string> LogCache { get; set; }
        protected bool LogCacheEnabled { get; set; }

        public SASAutomation()
        {
            Parser = new SASParser();
        }

        public string GetInitializationErrorMessage()
        {
            return "Could not communicate with SAS.  SAS may not be fully installed, or might be missing some of the automation pieces that StatTag requires.";
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
                    result.TableResult = CSVToTable.GetTableResult(result.TableResultPromise);
                    result.TableResultPromise = null;
                }
            }
        }

        public CommandResult[] RunCommands(string[] commands, Tag tag = null)
        {
            var commandResults = new List<CommandResult>();
            foreach (var command in commands)
            {
                if (tag != null && tag.Type == Constants.TagType.Verbatim && Parser.IsTagStart(command))
                {
                    LogCache = new List<string>();
                    LogCacheEnabled = true;
                }

                var result = RunCommand(command, tag);

                if (tag != null && tag.Type == Constants.TagType.Verbatim && Parser.IsTagEnd(command))
                {
                    if (result == null)
                    {
                        result = new CommandResult();
                    }
                    // SAS writes out some unicode character as a horizontal delimiter.  It looks awful.  We're going to take
                    // the liberty of replacing it with a dash.
                    result.VerbatimResult = string.Join("\r\n", LogCache).Replace("\u0192", "-");
                    LogCacheEnabled = false;
                }

                if (result != null && !result.IsEmpty())
                {
                    commandResults.Add(result);
                }
            }

            ResolveCommandPromises(commandResults);
            return commandResults.ToArray();
        }


        private string GetExpandedLocation(string originalLocation)
        {
            // Because of the order in which we get locations, we may need to
            // expand the location if it has a macro in its name.  An example
            // of macro expansion would be:
            //   %let Pathway = C:\Development\Code;
            //   %put "&Pathway.\Test\";
            // We will do a simple check (it's not perfect, and we may expand more often than we
            // need), but this allows us to keep the code simple while being effective.
            if (Parser.HasMacroIndicator(originalLocation))
            {
                var expandedLocation = RunCommand("%PUT " + originalLocation + ";");
                return expandedLocation.ValueResult;
            }

            return originalLocation;
        }

        /// <summary>
        /// Run a Stata command and provide the result of the command (if one should be returned).
        /// </summary>
        /// <param name="command">The command to run, taken from a Stata do file</param>
        /// <param name="tag">The tag associated with the command (if applicable)</param>
        /// <returns>The result of the command, or null if the command does not provide a result.</returns>
        public CommandResult RunCommand(string command, Tag tag = null)
        {
            Array carriageControls;
            Array lineTypeArray;
            Array logLineArray;

            Server.Workspace.LanguageService.Submit(command);

            // These calls need to be made because they cause SAS to initialize internal structures that
            // are used when FlushLogLines is called.  Even though we're not really doing anything with these
            // values, don't remove these calls.
            SAS.LanguageServiceCarriageControl carriageControlTemp = new SAS.LanguageServiceCarriageControl();
            SAS.LanguageServiceLineType lineTypeTemp = new SAS.LanguageServiceLineType();

            //Server.Workspace.LanguageService.FlushLogLines(10000, out carriageControls, out lineTypeArray,
            //    out logLineArray);

            // For all of the lines that we got back from SAS, we want to find those that are of the Normal type (meaning they
            // would contain some type of result/output), and that aren't empty.  Filtering empty lines is done because SAS
            // will dump out a bunch of extra output when we run, including blank Normal lines.
            var relevantLines = new List<string>();
            do
            {
                Server.Workspace.LanguageService.FlushLogLines(1000, out carriageControls, out lineTypeArray, out logLineArray);
                for (int index = 0; index < logLineArray.GetLength(0); index++)
                {
                    var lineType = (SAS.LanguageServiceLineType)lineTypeArray.GetValue(index);
                    var line = (string)logLineArray.GetValue(index);
                    if (lineType == LanguageServiceLineType.LanguageServiceLineTypeNormal
                        && !string.IsNullOrWhiteSpace(line))
                    {
                        relevantLines.Add(line);
                    }
                }

            }
            while (logLineArray != null && logLineArray.Length > 0);

            // Process the listing, even if we aren't going to use the output (it's only relevant when collecting
            // verbatim results).  This way we know that it's appropriately flushed when it comes time to use it for
            // verbatim tags.
            do
            {
                Server.Workspace.LanguageService.FlushListLines(1000, out carriageControls, out lineTypeArray, out logLineArray);
                if (LogCacheEnabled)
                {
                    for (int index = 0; index < logLineArray.GetLength(0); index++)
                    {
                        var lineType = (SAS.LanguageServiceLineType)lineTypeArray.GetValue(index);
                        var line = (string)logLineArray.GetValue(index);
                        // For verbatim we skip titles, but leave everything else.
                        if (lineType != LanguageServiceLineType.LanguageServiceLineTypeTitle)
                        {
                            LogCache.Add(line);
                        }
                    }                    
                }
            }
            while (logLineArray != null && logLineArray.Length > 0);

            // If we're doing verbatim output, don't bother continuing down and trying
            // to otherwise process the commands.
            if (tag != null && tag.Type == Constants.TagType.Verbatim)
            {
                return null;
            }

            if (Parser.IsValueDisplay(command))
            {
                // If we have a value command, we will pull out the last relevant line from the output.
                return new CommandResult() { ValueResult = relevantLines.LastOrDefault() };
            }

            if (Parser.IsImageExport(command))
            {
                return new CommandResult() { FigureResult = GetExpandedLocation(Parser.GetImageSaveLocation(command)) };
            }

            if (Parser.IsTableResult(command))
            {
                return new CommandResult() { TableResultPromise = GetExpandedLocation(Parser.GetTableName(command)) };
            }

            return null;
        }

        public bool IsReturnable(string command)
        {
            return Parser.IsValueDisplay(command) || Parser.IsImageExport(command) || Parser.IsTableResult(command);
        }
    }
}
