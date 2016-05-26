using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Core.Models
{
    public class ExecutionStep
    {
        public int Type { get; set; }
        public List<string> Code { get; set; }
        public List<string> Result { get; set; }
        public Tag Tag { get; set; }

        public ExecutionStep()
        {
            Code = new List<string>();
        }
    }
}
