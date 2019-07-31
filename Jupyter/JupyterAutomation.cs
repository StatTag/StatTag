using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JupyterKernelManager;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using StatTag.Core.Parser;
using StatTag.Core.Utility;
using System.Threading;

namespace Jupyter
{
    public abstract class JupyterAutomation : IStatAutomation
    {
        protected string KernelName { get; set; }
        protected abstract ICodeFileParser Parser { get; set; }
        private KernelManager Manager { get; set; }
        private KernelClient Client { get; set; }

        public StatPackageState State { get; set; }

        protected JupyterAutomation(string kernelName)
        {
            State = new StatPackageState();
            KernelName = kernelName;
        }

        public static string InstallationInformation()
        {
            return string.Empty;
        }

        public virtual bool Initialize(CodeFile file, LogManager logger)
        {
            if (Manager == null)
            {
                try
                {
                    logger.WriteMessage(string.Format("Attempting to create KernelManager for {0}", KernelName));
                    Manager = new KernelManager(KernelName);
                    logger.WriteMessage("Starting kernel");
                    Manager.StartKernel();

                    logger.WriteMessage("Creating client for kernel");
                    Client = Manager.CreateClient();
                    State.EngineConnected = true;
                    State.WorkingDirectorySet = true;
                }
                catch (Exception exc)
                {
                    logger.WriteMessage("Exception caught when initializing Jupyter kernel");
                    logger.WriteException(exc);
                    return false;
                }
            }
            return (Manager != null) && (Client != null);
        }

        public virtual void Dispose()
        {
            if (Client != null)
            {
                Client.StopChannels();
                //Client.Dispose();
                Client = null;
            }

            if (Manager != null)
            {
                Manager.Dispose();
                Manager = null;
            }
        }

        public CommandResult[] RunCommands(string[] commands, Tag tag = null)
        {
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
                        //VerbatimLog.StartCache();
                    }
                    // If we're going to be doing a figure, we want to clean out the old images so we know
                    // exactly which ones we're writing to.
                    else if (isFigureTag)
                    {
                        //CleanTemporaryImageFolder();
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
                        //VerbatimLog.StopCache();
                        //commandResults.Add(new CommandResult()
                        //{
                            //VerbatimResult = string.Join("", VerbatimLog.GetCache())
                        //});
                    }
                    // If this is the end of a figure tag, only proceed with this temp file processing if we don't
                    // already have a figure result of some sort.
                    else if (isFigureTag && !commandResults.Any(x => !string.IsNullOrWhiteSpace(x.FigureResult)))
                    {
                        // Make sure the graphics device is closed. We're going with the assumption that a device
                        // was open and a figure was written out.
                        //RunCommand("graphics.off()");

                        // If we don't have the file specified normally, we will use our fallback mechanism of writing to a
                        // temporary directory.
                        //var files = Directory.GetFiles(TemporaryImageFilePath, TemporaryImageFileFilter);
                        //if (files.Length == 0)
                        //{
                        //    continue;
                        //}

                        //// Find the last file in the directory.  We anticipate there would normally only be 1, but since 
                        //// several commands could be run, we will just take the last one.
                        //var tempImageFile = files.OrderBy(x => x).Last();
                        //var correctedPath = Path.GetFullPath(Path.Combine(TemporaryImageFilePath, ".."));
                        //var imageFile = Path.Combine(correctedPath, string.Format("{0}.png", TagUtil.TagNameAsFileName(tag)));
                        //File.Copy(tempImageFile, imageFile, true);

                        //commandResults.Add(new CommandResult() { FigureResult = imageFile });
                    }
                }
            }

            return commandResults.ToArray();
        }

        public CommandResult RunCommand(string command, Tag tag = null)
        {
            Client.Execute(command);
            while (Client.HasPendingExecute())
            {
                Thread.Sleep(100);
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
                        .Where(x => x.Header.MessageType.Equals(MessageType.DisplayData) ||
                                    x.Header.MessageType.Equals(MessageType.Stream))
                        .ToList();

                // Very important to clear out the execution history.  We've pulled stuff off the log
                // that we are interested in, so the next time we access the log we don't want to have
                // to wade through it again.
                Client.ClearExecuteLog();

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
            // We take a hint from the tag type to identify tables.  Because of how open R is with its
            // return of results, a user can just specify a variable and get the result.
            if (tag.Type == Constants.TagType.Table)
            {
                //return new CommandResult() { TableResult = GetTableResult(command, result) };
            }

            return null;
        }

        public virtual CommandResult HandleImageResult(Tag tag, string command, List<Message> result)
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
                //return new CommandResult() { FigureResult = GetExpandedFilePath(saveLocation) };
            }

            return null;
        }

        public virtual CommandResult HandleValueResult(Tag tag, string command, List<Message> result)
        {
            // If we have a value command, we will pull out the last relevant line from the output.
            // Because we treat every type of output as a possible value result, we are only going
            // to capture the result if it's flagged as a tag.
            if (tag.Type == Constants.TagType.Value)
            {
                var valueResult = GetValueResult(result);
                if (valueResult != null)
                {
                    return new CommandResult() {ValueResult = valueResult};
                }
            }

            return null;
        }

        private string GetValueResult(List<Message> result)
        {
            var first = result.FirstOrDefault();
            if (first != null && first.Content != null)
            {
                if (first.Header.MessageType.Equals(MessageType.Stream))
                {
                    if (first.Content.text != null)
                    {
                        return first.Content.text.ToString().Trim();
                    }
                }
                else
                {
                    
                }
                return first.Content.ToString();
            }

            return null;
        }
    }
}
