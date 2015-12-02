using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisManager.Core.Models
{
    public class ValueFormat
    {
        public int DecimalPlaces { get; set; }
        public bool UseThousands { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
    }
}
