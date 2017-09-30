using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Core.Models
{
    /// <summary>
    /// This helps us with internal tracking of how statistical packages are
    /// loaded and initialized for use.  This includes steps used across all
    /// statistical packages.
    /// </summary>
    public class StatPackageState
    {
        /// <summary>
        /// We have connected to the statistical software engine/API
        /// </summary>
        public bool EngineConnected { get; set; }

        /// <summary>
        /// We have changed the working directory.
        /// </summary>
        public bool WorkingDirectorySet { get; set; }

        /// <summary>
        /// All initialization steps have been completed
        /// </summary>
        public bool FullyInitialized
        {
            get { return EngineConnected && WorkingDirectorySet; }
        }
    }
}
