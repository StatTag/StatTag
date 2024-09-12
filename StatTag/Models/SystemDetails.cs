using StatTag.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Models
{
    /// <summary>
    /// Allow us to track details about the state of available execution environments within StatTag
    /// </summary>
    public class SystemDetails
    {
        /// <summary>
        /// The fully formatted system information that is available from the About dialog
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// A flag to indicate if the contents of the object are filled out or not
        /// </summary>
        public bool Complete { get; set; }

        /// <summary>
        /// If there was an error while trying to capture the system details.  This means that the
        /// contents may be partially complete.
        /// </summary>
        public bool Error { get; set; }
        public bool  RSupport { get; set; }
        public bool SASSupport { get; set; }
        public bool StataSupport { get; set; }
        public bool PythonSupport { get; set; }

        public SystemDetails()
        {
            Complete = false;
            Error = false;
        }
    }
}
