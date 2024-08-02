using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Models
{
    /// <summary>
    /// Specialized result object where a boolean response is desired, but you may want
    /// an optional collection of details about why something passed/failed.  This can
    /// be used in place of throwing an exception in a failure scenario.
    /// </summary>
    public class CheckResult
    {
        public bool Result { get; set; }
        public string Details { get; set; }

        public override string ToString()
        {
            return Details;
        }
    }
}
