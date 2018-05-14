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
        private const string CloseAllODS = "ODS _ALL_ CLOSE; ODS NORESULTS; ODS LISTING;";
        //private const string CloseAllODS = "ODS NORESULTS;";

        private SasServer Server = null;
        protected SASParser Parser { get; set; }
        protected List<string> LogCache { get; set; }
        protected bool LogCacheEnabled { get; set; }

        public StatPackageState State { get; set; }

        public SASAutomation()
        {
            Parser = new SASParser();
            State = new StatPackageState();
        }

        public string GetInitializationErrorMessage()
        {
            if (!State.EngineConnected)
            {
                return
                    "Could not communicate with SAS.  SAS may not be fully installed, or might be missing some of the automation pieces that StatTag requires.";
            }
            else if (!State.WorkingDirectorySet)
            {
                return
                    "We were unable to change the working directory to the location of your code file.   If this problem persists, please contact the StatTag team at StatTag@northwestern.edu.";
            }

            return
                "We were able to connect to SAS and change the working directory, but some other unknown error occurred during initialization.   If this problem persists, please contact the StatTag team at StatTag@northwestern.edu.";
        }

        public void Hide()
        {
            // Since the UI is not shown, no action is needed here.
        }

        public string FormatErrorMessageFromExecution(Exception exc)
        {
            return exc.Message;
        }

        public void Dispose()
        {
            if (Server != null)
            {
                Server.Close();
            }
        }

        public bool Initialize(CodeFile file, LogManager logger)
        {
            try
            {
                //TODO Do we want to allow remote connections, or just localhost?
                Server = new SasServer()
                {
                    UseLocal = true
                };
                logger.WriteMessage("Preparing to connect to local SAS instance...");
                Server.Connect();
                State.EngineConnected = true;
                logger.WriteMessage("Connected to local SAS instance");

                // To protect against blank PDFs being created in different situations (mostly around
                // when and how we are executing code and how SAS works when submitting code as a 
                // batch), we will explicitly close all ODS before the execution begins.
                logger.WriteMessage("Preparing to close all ODS...");
                RunCommand(CloseAllODS);
                logger.WriteMessage("Closed all ODS");

                // Set the working directory to the location of the code file, if it is provided and
                // isn't a UNC path.
                if (file != null)
                {
                    var path = Path.GetDirectoryName(file.FilePath);
                    if (!string.IsNullOrEmpty(path) && !path.Trim().StartsWith("\\\\"))
                    {
                        logger.WriteMessage(string.Format("Changing working directory to {0}", path));
                        RunCommand(string.Format("data _null_; call system('cd \"{0}\"'); run;", path));
                        State.WorkingDirectorySet = true;
                    }
                }

                logger.WriteMessage("Completed initialization");
            }
            catch (Exception exc)
            {
                logger.WriteMessage("Caught an exception when attempting to initialize SAS");
                logger.WriteException(exc);
                Server = null;
                return false;
            }

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
                    result.TableResult = DataFileToTable.GetTableResult(result.TableResultPromise);
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
            if (Parser.HasMacroIndicator(originalLocation) || Parser.HasFunctionIndicator(originalLocation))
            {
                var expandedLocation =
                    RunCommand(string.Format("%PUT {0}{1}", originalLocation, SASParser.CommandDelimiter),
                        new Tag() {Type = Constants.TagType.Value});
                if (expandedLocation != null)
                {
                    originalLocation = expandedLocation.ValueResult;
                }

                originalLocation = originalLocation.Replace("\"", "");
            }
            
            // If a macro expansion has taken place, we should still check to see if it resulted
            // in a relative path (instead of assuming all macro expansions result in a fully
            // qualiifed path).
            if (Parser.IsRelativePath(originalLocation))
            {
                // Attempt to find the current working directory.  If we are not able to find it, or the value we end up
                // creating doesn't exist, we will just proceed with whatever image location we had previously.
                var results =
                    RunCommands(
                        new string[]
                        {
                            "filename dummy '.';", "%let __stattag_pwd=%sysfunc(pathname(dummy));",
                            "%put &__stattag_pwd; RUN;"
                        }, new Tag() {Type = Constants.TagType.Value});
                if (results != null && results.Length > 0)
                {
                    var path = results.First().ValueResult;
                    var correctedPath = Path.GetFullPath(Path.Combine(path, originalLocation));
                    if (File.Exists(correctedPath))
                    {
                        originalLocation = correctedPath;
                    }
                }
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

            // If there is no tag associated with the command that was run, we aren't going to bother
            // parsing and processing the results.  This is for blocks of codes in between tags where
            // all we need is for the code to run.
            if (tag != null)
            { 
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
                    return new CommandResult() { TableResultPromise = GetExpandedLocation(Parser.GetTableDataPath(command)) };
                }
            }

            return null;
        }

        public bool IsReturnable(string command)
        {
            return Parser.IsValueDisplay(command) || Parser.IsImageExport(command) || Parser.IsTableResult(command);
        }
    }
}
