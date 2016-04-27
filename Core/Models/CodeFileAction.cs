using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisManager.Core.Models
{
    public class CodeFileAction
    {
        public string Label { get; set; }
        public int Action { get; set; }
        public object Parameter { get; set; }

        public override string ToString()
        {
            return Label;
        }
    }
}
