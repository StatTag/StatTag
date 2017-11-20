using System;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using StatTag.Core.Interfaces;

namespace StatTag.Core.Models
{
    public class LogManager
    {
        public const ulong DefaultMaxLogFileSize = 102400;  // 100MB default
        public const ulong DefaultMaxLogFiles = 1;

        private const string LoggerName = "StatTagLogger";

        public bool Enabled { get; set; }
        public string LogFilePath { get; set; }
        public ulong MaxLogFileSize { get; set; }
        public ulong MaxLogFiles { get; set; }
        protected IFileHandler FileHandler { get; set; }

        // Because we have built-in mechanisms to create a file handler wrapper and a 
        // logger if one isn't specified, this allows us to internally track if we should
        // do that, or if one was provided and we should just use that.
        private bool UseProvidedHandlerAndLogger { get; set; }

        // log4net objects (used as underlying logger as of StatTag v3.2)
        private PatternLayout layout = new PatternLayout("%d [%t] %-5p %m%n");
        private RollingFileAppender logAppender = null;
        private log4net.ILog Logger { get; set; }

        //public LogManager(IFileHandler handler = null)
        //{
        //    FileHandler = handler ?? new FileHandler();
        //}

        public LogManager()
        {
            UseProvidedHandlerAndLogger = false;
            FileHandler = new FileHandler();
        }

        /// <summary>
        /// This constructor is primarily planned for use by unit tests.
        /// </summary>
        /// <param name="handler">The file handler wrapper to use</param>
        /// <param name="logger">The log4net logger to use.</param>
        public LogManager(IFileHandler handler, ILog logger)
        {
            UseProvidedHandlerAndLogger = true;
            FileHandler = handler;
            Logger = logger;
        }

        /// <summary>
        /// Determine if a given path is accessible and can be opened with write access.
        /// </summary>
        /// <param name="logFilePath">The file path to check</param>
        /// <returns>True if the path is valid, false otherwise.</returns>
        public bool IsValidLogPath(string logFilePath)
        {
            if (string.IsNullOrWhiteSpace(logFilePath))
            {
                return false;
            }

            FileStream stream = null;
            try
            {
                // Check write access
                stream = FileHandler.OpenWrite(logFilePath);
            }
            catch (Exception exc)
            {
                return false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
                stream = null;
            }

            return true;
        }

        /// <summary>
        /// Updates the internal settings used by this log manager, when given a set of
        /// application properties.
        /// </summary>
        /// <remarks>This should ba called any time the application properties are loaded or updated.</remarks>
        /// <param name="properties">Application properties</param>
        public void UpdateSettings(Properties properties)
        {
            UpdateSettings(properties.EnableLogging, properties.LogLocation, properties.MaxLogFileSize, properties.MaxLogFiles);
        }

        /// <summary>
        /// Updates the internal settings used by this log manager, when given a set of
        /// application properties.
        /// </summary>
        /// <remarks>This should ba called any time the application properties are loaded or updated.
        /// If the log path is not valid, we will disable logging.</remarks>
        /// <param name="enabled">If logging is enabled by the user</param>
        /// <param name="filePath">The path of the log file to write to.</param>
        /// <param name="maxLogFileSize"></param>
        /// <param name="maxLogFiles"></param>
        public void UpdateSettings(bool enabled, string filePath, ulong? maxLogFileSize, ulong? maxLogFiles)
        {
            LogFilePath = filePath;
            Enabled = (enabled && IsValidLogPath(LogFilePath));
            MaxLogFileSize = (maxLogFileSize.HasValue ? maxLogFileSize.Value : DefaultMaxLogFileSize);
            MaxLogFiles = (maxLogFiles.HasValue ? maxLogFiles.Value : DefaultMaxLogFiles);

            if (!UseProvidedHandlerAndLogger)
            {
                var hierarchy = (Hierarchy)log4net.LogManager.GetRepository();
                hierarchy.Root.RemoveAllAppenders(); // Remove all of the existing appenders since we are re-initializing them

                layout.ActivateOptions();

                logAppender = new RollingFileAppender()
                {
                    AppendToFile = true,
                    File = LogFilePath,
                    MaxFileSize = (long)MaxLogFileSize,
                    MaxSizeRollBackups = (int)MaxLogFiles,
                    RollingStyle = RollingFileAppender.RollingMode.Size,
                    ImmediateFlush = true,
                    LockingModel = new FileAppender.MinimalLock(),
                    Layout = layout
                };
                logAppender.ActivateOptions();
                var result = BasicConfigurator.Configure(logAppender);
                Logger = log4net.LogManager.GetLogger(LoggerName);
            }
        }

        /// <summary>
        /// Writes a message to the log file.
        /// </summary>
        /// <remarks>Can be safely called any time, even if logging is disabled.</remarks>
        /// <param name="text">The text to write to the log file.</param>
        public void WriteMessage(string text)
        {
            if (Enabled)
            {
                Logger.Info(text);
            }
        }

        /// <summary>
        /// Writes the details of an exception to the log file.
        /// </summary>
        /// <remarks>Can be safely called any time, even if logging is disabled.
        /// Recursively called for all inner exceptions.</remarks>
        /// <param name="exc">The exception to write to the log file.</param>
        public void WriteException(Exception exc)
        {
            if (Enabled)
            {
                Logger.ErrorFormat("{0}\r\nStack trace: {1}", exc.Message, exc.StackTrace);
                if (exc.InnerException != null)
                {
                    WriteException(exc.InnerException);
                }
            }
        }
    }
}
