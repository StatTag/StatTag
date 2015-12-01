using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisManager.Core.Models
{
    public class Annotation
    {
        public CodeFile CodeFile { get; set; }
        public string Type { get; set; }
        public string OutputLabel { get; set; }
        public string RunFrequency { get; set; }
    }
}
