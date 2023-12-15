using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JupyterKernelManager;
using StatTag.Core.Models;

namespace Jupyter
{
    public class StatTagJupyterLogger : ILogger
    {
        private LogManager Logger { get; set; }

        public StatTagJupyterLogger(LogManager logger)
        {
            Logger = logger;
        }

        public void Write(string message, params object[] parameters)
        {
            Write(LogLevel.Default, message, parameters);
        }

        public void Write(int logLevel, string message, params object[] parameters)
        {
            Logger.WriteMessage(string.Format(message, parameters));
        }
    }
}
