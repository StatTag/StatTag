using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisManager.Models
{
    public class BaseManager
    {
        public LogManager Logger { get; set; }

        /// <summary>
        /// Wrapper around a LogManager instance.  Since logging is not always enabled/available for this object
        /// the wrapper only writes if a logger is accessible.
        /// </summary>
        /// <param name="text"></param>
        protected void Log(string text)
        {
            if (Logger != null)
            {
                Logger.WriteMessage(text);
            }
        }

        /// <summary>
        /// Wrapper around a LogManager instance.  Since logging is not always enabled/available for this object
        /// the wrapper only writes if a logger is accessible.
        /// </summary>
        /// <param name="exc"></param>
        protected void LogException(Exception exc)
        {
            if (Logger != null)
            {
                Logger.WriteException(exc);
            }
        }
    }
}
