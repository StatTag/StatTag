using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Models
{
    public class BackgroundWorkerProgressReporter : IProgressReporter
    {
        BackgroundWorker Worker { get; set; }

        public BackgroundWorkerProgressReporter(BackgroundWorker worker)
        {
            Worker = worker;
        }

        public void ReportProgress(int percentage, string message)
        {
            Worker.ReportProgress(percentage, message);
        }

        public bool IsCancelling()
        {
            return Worker.CancellationPending;
        }
    }
}
