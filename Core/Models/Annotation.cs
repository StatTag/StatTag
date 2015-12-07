using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AnalysisManager.Core.Models
{
    public class Annotation
    {
        [JsonIgnore]
        public CodeFile CodeFile { get; set; }
        public string Type { get; set; }
        public string OutputLabel { get; set; }
        public string RunFrequency { get; set; }

        public ValueFormat ValueFormat { get; set; }

        /// <summary>
        /// The starting line is the 0-based line index where the opening
        /// annotation tag exists.
        /// </summary>
        public int? LineStart { get; set; }

        /// <summary>
        /// The ending line is the 0-based line index where the closing
        /// annotation tag exists.
        /// </summary>
        public int? LineEnd { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(OutputLabel))
            {
                return OutputLabel;
            }

            return base.ToString();
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
