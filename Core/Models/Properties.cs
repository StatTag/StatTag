using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Core.Models
{
    /// <summary>
    /// User preferences and settings for StatTag.
    /// </summary>
    public class Properties
    {
        /// <summary>
        /// The full path to the Satata executable on the user's machine.
        /// </summary>
        public string StataLocation { get; set; }

        /// <summary>
        /// If the user would like to have debug logging enabled.
        /// </summary>
        public bool EnableLogging { get; set; }

        /// <summary>
        /// The path of the log file to write to.
        /// </summary>
        public string LogLocation { get; set; }
    }
}
