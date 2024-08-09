using JupyterKernelManager;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using StatTag.Core.Utility;
using StatTag.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

        private static readonly Regex SingleQuoteWrappedString = new Regex("^'([\\s\\S]*)'$");

        /// <summary>
        /// MIME types for images that can be returned by a Jupyter kernel
        /// </summary>
        private class ImageMimeTypes
        {
            public const string PNG = "image/png";
            public const string JPEG = "image/jpeg";
            public const string BMP = "image/bmp";
            public const string GIF = "image/gif";
            public const string PDF = "application/pdf";
            public const string SVG = "image/svg+xml";
            public const string TIFF = "image/tiff";
        }

        public StatPackageState State { get; set; }

        protected JupyterAutomation(string kernelName)
        {
            State = new StatPackageState();
            KernelName = kernelName;
        }

        /// <summary>
        /// Determine if Jupyter is set up on the user's system in a way that we can already
        /// access the jupyter commands.  This means no customization would be 
        /// </summary>
        /// <returns>A CheckResult indicating if Jupyter was found or not.  If the result is
        /// false, details will be available on why it failed.</returns>
        public static CheckResult DetectJupyter(string appendPath = null)
        {
            try
            {
                var info = new ProcessStartInfo("jupyter", "kernelspec --version")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                // If the caller has asked us to append something to the PATH environment variable,
                // do so.  This can be used for when we are using the embedded Python environment.
                if (!string.IsNullOrWhiteSpace(appendPath))
                {
                    if (info.EnvironmentVariables.ContainsKey("PATH"))
                    {
                        var updatedPath = string.Format("{0};{1}", info.EnvironmentVariables["PATH"], appendPath);
                        info.EnvironmentVariables.Remove("PATH");
                        info.EnvironmentVariables.Add("PATH", updatedPath);
                    }
                }

                // Don't allow PYTHONEXECUTABLE to be passed to kernel process. If set, it can bork all
                // the things.  This comes from the Jupyter kernel manager code.
                if (info.EnvironmentVariables.ContainsKey("PYTHONEXECUTABLE"))
                {
                    info.EnvironmentVariables.Remove("PYTHONEXECUTABLE");
                }

                bool anacondaDetected = false;
                var modifiedInfo = KernelManager.AdjustLaunchCommandForAnaconda(info);
                if (!modifiedInfo.FileName.Equals(info.FileName))
                {
                    info = modifiedInfo;
                    anacondaDetected = true;
                }

                var process = Process.Start(info);
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                var processWaitResult = process.WaitForExit(3000);  // Arbitrarily selected timeout of 3 seconds
                if (!processWaitResult)
                {
                    return new CheckResult() { Result = false, Details = "jupyter kernelspec did not return within the expected time." };
                }

                if (0 == process.ExitCode)
                {
                    if (anacondaDetected)
                    {
                        output = string.Format("Detected Anaconda - using for Jupyter execution\r\n{0}", output);
                    }
                    return new CheckResult() { Result = true, Details = output };
                }

                return new CheckResult() { Result = false, Details = "Jupyter was not found on the system" };
            }
            catch (Exception exc)
            {
                return new CheckResult() { Result = false, Details = exc.Message };
            }
        }

        public static CheckResult InstallationInformation()
        {
            var specManager = new KernelSpecManager();
            var specs = specManager.GetAllSpecs();
            var builder = new StringBuilder();
            var result = new CheckResult();
            if (specs != null && specs.Count > 0)
            {
                foreach (var spec in specs)
                {
                    builder.AppendFormat("Kernel: {0}\r\n", spec.Key);
                    builder.AppendFormat("   Display name: {0}\r\n", spec.Value.DisplayName);
                    builder.AppendFormat("   Arguments: {0}\r\n", string.Join(", ", spec.Value.Arguments));
                    builder.AppendFormat("   Resource directory: {0}\r\n", spec.Value.ResourceDirectory);
                }
                result.Result = true;
            }
            else
            {
                builder.Append("No Jupyter kernels could be found.");
                result.Result = false;
            }

            result.Details = builder.ToString();
            return result;
        }

        public virtual bool Initialize(CodeFile file, LogManager logger)
        {
            if (Manager == null)
            {
                try
                {
                    logger.WriteMessage(string.Format("Attempting to create KernelManager for {0}", KernelName));
                    Manager = new KernelManager(KernelName, new StatTagJupyterLogger(logger))
                    {
                        Debug = false
                    };

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

        /// <summary>
        /// Utility function to take an array of commands and collapse it into an array of commands with at least
        /// 3 elements.  This assumes (without checking) that if there are 3 or more lines, that the first line
        /// represents the starting StatTag tag comment, the last line represents the ending StatTag tag comment,
        /// and everything in the middle is code that should be collapsed to a single string, separated by newline.
        /// If there are fewer than 3 elements, the original array is returned.
        /// </summary>
        /// <param name="commands">Array of command strings to collapse</param>
        /// <returns>A collapsed array of maximum 3 command strings</returns>
        public static string[] CollapseTagCommandsArray(string[] commands)
        {
            if (commands == null)
            {
                return commands;
            }

            if (commands.Length >= 3)
            {
                string[] newCommands = new[]
                {
                        commands[0],
                        string.Join("\r\n", commands.Skip(1).Take(commands.Length - 2)),
                        commands[commands.Length - 1]
                };
                return newCommands;
            }

            return commands;
        }

        public virtual CommandResult HandleTableResult(Tag tag, string command, List<Message> result)
        {
            return null;
        }

        /// <summary>
        /// Utility class to make it easier to ship around data from GetImageData
        /// </summary>
        private class ImageDataResult
        {
            public dynamic Data = null;
            public string MimeType = null;
            public string Extension = null;
        }

        /// <summary>
        /// Pull image data (if available) from a Jupyter kernel message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private ImageDataResult GetImageData(Message message)
        {
            if (message == null)
            {
                return null;
            }

            var imageData = message.SafeGetData(ImageMimeTypes.PNG);
            if (imageData != null)
            {
                return new ImageDataResult()
                {
                    Data = imageData,
                    MimeType = ImageMimeTypes.PNG,
                    Extension = "png"
                };
            }

            imageData = message.SafeGetData(ImageMimeTypes.JPEG);
            if (imageData != null)
            {
                return new ImageDataResult()
                {
                    Data = imageData,
                    MimeType = ImageMimeTypes.JPEG,
                    Extension = "jpg"
                };
            }

            imageData = message.SafeGetData(ImageMimeTypes.BMP);
            if (imageData != null)
            {
                return new ImageDataResult()
                {
                    Data = imageData,
                    MimeType = ImageMimeTypes.BMP,
                    Extension = "bmp"
                };
            }

            imageData = message.SafeGetData(ImageMimeTypes.GIF);
            if (imageData != null)
            {
                return new ImageDataResult()
                {
                    Data = imageData,
                    MimeType = ImageMimeTypes.GIF,
                    Extension = "gif"
                };
            }

            imageData = message.SafeGetData(ImageMimeTypes.PDF);
            if (imageData != null)
            {
                return new ImageDataResult()
                {
                    Data = imageData,
                    MimeType = ImageMimeTypes.PDF,
                    Extension = "pdf"
                };
            }

            imageData = message.SafeGetData(ImageMimeTypes.SVG);
            if (imageData != null)
            {
                return new ImageDataResult()
                {
                    Data = imageData,
                    MimeType = ImageMimeTypes.SVG,
                    Extension = "svg"
                };
            }

            imageData = message.SafeGetData(ImageMimeTypes.TIFF);
            if (imageData != null)
            {
                return new ImageDataResult()
                {
                    Data = imageData,
                    MimeType = ImageMimeTypes.TIFF,
                    Extension = "tiff"
                };
            }

            return null;
        }

        public virtual CommandResult HandleImageResult(Tag tag, string command, List<Message> result)
        {
            // If we have a tag, it's not the end of the tag, and the tag is for a figure, see if the Jupyter
            // kernel returned some sort of image data.
            if (tag != null && tag.Type == Constants.TagType.Figure && !Parser.IsTagEnd(command))
            {
                // If the temporary image file path hasn't been initialized for this execution, we can't proceed.
                // We go off the assumption that the initialization for the stat automation code will take care
                // of creating this directory, so if it fails we don't try again.
                if (!Directory.Exists(TemporaryImageFilePath))
                {
                    return null;
                }

                // A result can have multiple messages, which may include things like a warning before the actual
                // image.  We will process all of the messages in the result until we find one that gives us an
                // image.  If we don't find anything, we'll return a null result.
                foreach (var message in result)
                {
                    var imageData = GetImageData(message);
                    if (imageData == null)
                    {
                        continue;
                    }

                    // Part one is to write this to our temporary image folder
                    var tempImageFile = Path.Combine(TemporaryImageFilePath, "tmp." + imageData.Extension);
                    File.WriteAllBytes(tempImageFile, Convert.FromBase64String(imageData.Data));

                    // We know this is a tag, so we know we need to save the file.  We will just go ahead and save this
                    // up a level to a valid directory so the file can be referenced later.
                    var correctedPath = Path.GetFullPath(Path.Combine(TemporaryImageFilePath, ".."));
                    var imageFile = Path.Combine(correctedPath, string.Format("{0}.png", TagUtil.TagNameAsFileName(tag)));
                    File.Copy(tempImageFile, imageFile, true);

                    return new CommandResult() { FigureResult = imageFile };
                }
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
                // Try to get the first non-stderr result.  We can get stderr messages (which may appear first in the list)
                // if there is a warning in R.  We want to get the first actual result, where possible.  If our first filter
                // returns nothing, then we will fall back to just grabbing whatever is in the first result.
                var resultResponseToUse = result.FirstOrDefault(r => r != null &&
                        r.Content != null &&
                        r.Content.name != null &&
                        !r.Content.name.ToString().Trim().Equals("stderr"));
                var valueResult = GetTextValueResult(resultResponseToUse ?? result.FirstOrDefault());
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

        /// <summary>
        /// We need to apply some additional formatting to text results that come back from the R kernel.  Because
        /// these are represented in HTML, we need to consider decoding HTML reserved symbols (e.g., &gt; = >).
        /// We have also noticed that the R kernel will wrap strings in single quotes.  This is different from how
        /// results came back before in StatTag, and so we need to strip those.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string FormatStringFromHtml(string result)
        {
            if (string.IsNullOrWhiteSpace(result))
            {
                return result;
            }

            var interim = result;

            // Identify if the value is wrapped in single quotes.  If so, remove them.
            var match = SingleQuoteWrappedString.Match(interim);
            if (match.Success)
            {
                interim = match.Groups[1].Value;
            }

            // Final step is to HTML-decode the string.
            return WebUtility.HtmlDecode(interim);
        }
    }
}
