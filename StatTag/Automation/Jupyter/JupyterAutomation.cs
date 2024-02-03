using JupyterKernelManager;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using StatTag.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Jupyter
{
    public abstract class JupyterAutomation : IStatAutomation
    {
        private const string TemporaryImageFileFilter = "*.png";
        protected string KernelName { get; set; }
        protected abstract ICodeFileParser Parser { get; set; }
        protected KernelManager Manager { get; set; }
        protected KernelClient Client { get; set; }
        protected List<string> VerbatimResultCache { get; set; }
        protected bool VerbatimResultCacheEnabled { get; set; }
        protected string TemporaryImageFilePath = "";


        public StatPackageState State { get; set; }

        protected JupyterAutomation(string kernelName)
        {
            State = new StatPackageState();
            KernelName = kernelName;
        }

        public static string InstallationInformation()
        {
            var specManager = new KernelSpecManager();
            var specs = specManager.GetAllSpecs();
            var builder = new StringBuilder();
            if (specs != null && specs.Count > 0)
            {
                foreach (var spec in specs)
                {
                    builder.AppendFormat("Kernel: {0}\r\n", spec.Key);
                    builder.AppendFormat("   Display name: {0}\r\n", spec.Value.DisplayName);
                    builder.AppendFormat("   Arguments: {0}\r\n", string.Join(", ", spec.Value.Arguments));
                    builder.AppendFormat("   Resource directory: {0}\r\n", spec.Value.ResourceDirectory);
                }
            }
            else
            {
                builder.Append("No Jupyter kernels could be found.");
            }

            return builder.ToString();
        }

        public virtual bool Initialize(CodeFile file, LogManager logger)
        {
            if (Manager == null)
            {
                try
                {
                    logger.WriteMessage(string.Format("Attempting to create KernelManager for {0}", KernelName));
                    Manager = new KernelManager(KernelName, new StatTagJupyterLogger(logger));

                    Manager.Debug = false;

                    logger.WriteMessage("Starting kernel");
                    Manager.StartKernel();

                    logger.WriteMessage("Creating client for kernel");
                    Client = Manager.CreateClientAndWaitForConnection();
                    if (Client == null)
                    {
                        throw new Exception("Jupyter Kernel failed to start up");
                    }

                    State.EngineConnected = true;
                    State.WorkingDirectorySet = true;

                    if (Manager.Debug) { logger.WriteMessage(Manager.DebugLog.ToString()); }

                    TemporaryImageFilePath = AutomationUtil.InitializeTemporaryImageDirectory(file, logger);
                }
                catch (Exception exc)
                {
                    if (Manager != null && Manager.Debug) { logger.WriteMessage(Manager.DebugLog.ToString()); }
                    logger.WriteMessage("Exception caught when initializing Jupyter kernel");
                    logger.WriteException(exc);
                    return false;
                }
            }
            return (Manager != null) && (Client != null);
        }

        /// <summary>
        /// Helper method to clean out the temporary folder used for storing images
        /// </summary>
        /// <param name="deleteFolder"></param>
        private void CleanTemporaryImageFolder(bool deleteFolder = false)
        {
            AutomationUtil.CleanTemporaryImageFolder(this, null, TemporaryImageFilePath, TemporaryImageFileFilter, deleteFolder);
        }

        public virtual void Dispose()
        {
            if (Client != null)
            {
                Client.StopChannels();
                Client = null;
            }

            if (Manager != null)
            {
                Manager.Dispose();
                Manager = null;
            }

            CleanTemporaryImageFolder(true);
        }

        public virtual CommandResult[] RunCommands(string[] commands, Tag tag = null)
        {
            var commandResults = new List<CommandResult>();
            bool isVerbatimTag = (tag != null && tag.Type == Constants.TagType.Verbatim);
            bool isFigureTag = (tag != null && tag.Type == Constants.TagType.Figure);
            var cachedCommands = new List<string>();
            foreach (var command in commands)
            {
                bool isTagStart = Parser.IsTagStart(command);
                if (isVerbatimTag && isTagStart)
                {
                    VerbatimResultCache = new List<string>();
                    VerbatimResultCacheEnabled = true;
                }
                else if (isFigureTag && isTagStart)
                {
                    // If we're going to be doing a figure, we want to clean out the old images so we know
                    // exactly which ones we're writing to.
                    CleanTemporaryImageFolder();
                }

                CommandResult result = null;
                if (VerbatimResultCacheEnabled)
                {
                    // If the verbatim log cache is enabled, and we're now at the closing verbatim tag, we need to run
                    // all of our cached code and get the results into our verbatim collection.
                    if (isVerbatimTag && Parser.IsTagEnd(command))
                    {
                        RunCommand(string.Join("\r\n", cachedCommands), tag);
                        result = new CommandResult()
                        {
                            VerbatimResult = string.Join("\r\n", VerbatimResultCache)
                        };
                        VerbatimResultCacheEnabled = false;
                    }
                    // Otherwise, we know we're within a verbatim tag so we will cache the current command and save
                    // it for later execution.
                    else
                    {
                        cachedCommands.Add(command);
                    }
                }
                else
                {
                    result = RunCommand(command, tag);
                }
                
                // Whenever we have a result, add it to our list of results for all the commands.
                if (result != null && !result.IsEmpty())
                {
                    commandResults.Add(result);
                }
            }

            return commandResults.ToArray();
        }

        public CommandResult RunCommand(string command, Tag tag = null)
        {
            if (Client == null)
            {
                return null;
            }

            Client.Execute(command.Trim().Replace("\r\n", "\n"));

            // Make sure not only that we're waiting for a command to finish, but that the client
            // is still alive and communicating with the kernel.  Otherwise if the kernel or client
            // diest, this will hang.
            while (Client.HasPendingExecute() && Client.IsAlive)
            {
                Thread.Sleep(100);
            }
            
            // Check if there are any errors that were sent back from the kernel.
            if (Client.HasExecuteError())
            {
                var errors = Client.GetExecuteErrors();
                throw new Exception(string.Format("{0} {1} {2} found when running your code.  Please ensure your code runs to completion in its native editor.\r\n\r\n{3}",
                    errors.Count, "error".Pluralize(errors.Count), "was".Pluralize(errors.Count, "were"),
                    string.Join("\r\n", errors)));
            }

            // If there is no tag associated with the command that was run, we aren't going to bother
            // parsing and processing the results.  This is for blocks of codes in between tags where
            // all we need is for the code to run.
            if (tag != null)
            {
                // Now retrieve all of the completed execution results that we are interested in
                var executeLog =
                    Client.ExecuteLog.Values.OrderBy(x => x.ExecutionIndex)
                        .SelectMany(x => x.Response)
                        .Where(x => x.IsDataMessageType())
                        .ToList();

                // Very important to clear out the execution history.  We've pulled stuff off the log
                // that we are interested in, so the next time we access the log we don't want to have
                // to wade through it again.
                Client.ClearExecuteLog();

                if (VerbatimResultCacheEnabled)
                {
                    VerbatimResultCache.AddRange(GetValueResults(executeLog));
                    return null;
                }

                // Start with tables
                var commandResult = HandleTableResult(tag, command, executeLog);
                if (commandResult != null)
                {
                    return commandResult;
                }

                // Image comes next, because everything else we will count as a value type.
                commandResult = HandleImageResult(tag, command, executeLog);
                if (commandResult != null)
                {
                    return commandResult;
                }

                commandResult = HandleValueResult(tag, command, executeLog);
                if (commandResult != null)
                {
                    return commandResult;
                }
            }
            else
            {
                // Very important to clear out the execution history.  We don't need anything in the log,
                // so we will reset it before processing the next block.
                Client.ClearExecuteLog();
            }

            return null;
        }

        public bool IsReturnable(string command)
        {
            return true;
        }

        public string GetInitializationErrorMessage()
        {
            if (!State.EngineConnected)
            {
                return
                    string.Format("Could not communicate with the {0} Jupyter kernel.  Please make sure the kernel has been installed on your machine, and initialized in your user directory.  See the User's Guide for more information.", KernelName);
            }
            else if (!State.WorkingDirectorySet)
            {
                return
                    "We were unable to change the working directory to the location of your code file.   If this problem persists, please contact the StatTag team at StatTag@northwestern.edu.";
            }

            return
                string.Format("We were able to connect to the {0} Jupyter kernel, but some other unknown error occurred during initialization.   If this problem persists, please contact the StatTag team at StatTag@northwestern.edu.", KernelName);
        }

        public void Hide()
        {
        }

        public string FormatErrorMessageFromExecution(Exception exc)
        {
            return exc.Message;
        }

        public virtual CommandResult HandleTableResult(Tag tag, string command, List<Message> result)
        {
            return null;
        }

        public virtual CommandResult HandleImageResult(Tag tag, string command, List<Message> result)
        {
            return null;
        }

        public virtual CommandResult HandleValueResult(Tag tag, string command, List<Message> result)
        {
            // If we have a value command, we will pull out the last relevant line from the output.
            // Because we treat every type of output as a possible value result, we are only going
            // to capture the result if it's flagged as a tag.
            if (tag.Type == Constants.TagType.Value)
            {
                var valueResult = GetTextValueResult(result.FirstOrDefault());
                if (valueResult != null)
                {
                    return new CommandResult() {ValueResult = valueResult};
                }
            }

            return null;
        }

        protected List<string> GetValueResults(List<Message> results)
        {
            var resultValues = results.Select(GetTextValueResult).ToList();
            return resultValues;
        }

        protected string GetTextValueResult(Message result)
        {
            if (result != null && result.Content != null)
            {
                if (result.IsDataMessageType())
                {
                    if (result.Content.data != null)
                    {
                        return result.SafeGetData("text/plain").Trim();
                    }
                    else if (result.Header.MessageType.Equals(MessageType.Stream))
                    {
                        // We only want to include stdout messages.  If we have a name to
                        // identify the stream, and it's not stdout, skip it.  We will need
                        // to see if there are situations where we don't have the stream name
                        // and it's something we'd want to filter out.
                        if (result.DoesPropertyExist(result.Content, "name")
                            && !result.Content.name.ToString().Equals(StreamName.StdOut))
                        {
                            return null;
                        }

                        if (result.Content.text != null)
                        {
                            return result.Content.text.ToString().Trim();
                        }
                    }
                }
                return result.Content.ToString();
            }

            return null;
        }

        protected string GetHtmlValueResult(Message result)
        {
            if (result != null && result.Content != null)
            {
                if (result.IsDataMessageType())
                {
                    var data = result.SafeGetData("text/html");
                    if (data != null)
                    {
                        return data.Trim();
                    }
                }
            }

            return null;
        }
    }
}
