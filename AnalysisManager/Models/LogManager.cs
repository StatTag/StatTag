using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisManager.Models
{
    public class LogManager
    {
        public bool Enabled { get; set; }
        public string LogFilePath { get; set; }

        public LogManager()
        {
        }

        public void UpdateSettings(Properties properties)
        {
            UpdateSettings(properties.EnableLogging, properties.LogLocation);
        }

        public void UpdateSettings(bool enabled, string filePath)
        {
            Enabled = enabled;
            LogFilePath = filePath;
        }

        public void WriteMessage(string text)
        {
            if (Enabled)
            {
                File.AppendAllText(LogFilePath, string.Format("{0} - {1}\r\n", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), text));
            }
        }

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
