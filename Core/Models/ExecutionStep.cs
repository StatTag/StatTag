using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisManager.Core.Models
{
    public class ExecutionStep
    {
        public int Type { get; set; }
        public List<string> Code { get; set; }
        public List<string> Result { get; set; }
        public Annotation Annotation { get; set; }

        public ExecutionStep()
        {
            Code = new List<string>();
        }
    }
}
