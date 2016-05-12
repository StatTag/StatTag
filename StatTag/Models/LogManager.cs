using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Models
{
    public class LogManager
    {
        public bool Enabled { get; set; }
        public string LogFilePath { get; set; }

        /// <summary>
        /// Determine if a given path is accessible and can be opened with write access.
        /// </summary>
        /// <param name="logFilePath">The file path to check</param>
        /// <returns>True if the path is valid, false otherwise.</returns>
        public static bool IsValidLogPath(string logFilePath)
        {
            if (string.IsNullOrWhiteSpace(logFilePath))
            {
                return false;
            }

            FileStream stream = null;
            try
            {
                // Check write access
                stream = File.OpenWrite(logFilePath);
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
            UpdateSettings(properties.EnableLogging, properties.LogLocation);
        }

        /// <summary>
        /// Updates the internal settings used by this log manager, when given a set of
        /// application properties.
        /// </summary>
        /// <remarks>This should ba called any time the application properties are loaded or updated.
        /// If the log path is not valid, we will disable logging.</remarks>
        /// <param name="enabled">If logging is enabled by the user</param>
        /// <param name="filePath">The path of the log file to write to.</param>
        public void UpdateSettings(bool enabled, string filePath)
        {
            LogFilePath = filePath;
            Enabled = (enabled && IsValidLogPath(LogFilePath));
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
                File.AppendAllText(LogFilePath, string.Format("{0} - {1}\r\n", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), text));
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
            WriteMessage(string.Format("Error: {0}\r\nStack trace: {1}", exc.Message, exc.StackTrace));
            if (exc.InnerException != null)
            {
                WriteException(exc.InnerException);
            }
        }
    }
}
