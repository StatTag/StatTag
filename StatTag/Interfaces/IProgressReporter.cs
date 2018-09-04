using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag
{
    /// <summary>
    /// This provides a general interface to progress reporting capabilities.  This is very
    /// closely aligned to the BackgroundWorker, but to avoid tightly coupling to that
    /// implementation, we defined this interaface.
    /// </summary>
    public interface IProgressReporter
    {
        bool IsCancelling();
        void ReportProgress(int percentage, string message);
    }
}
