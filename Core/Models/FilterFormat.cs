using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Core.Models
{
    public class FilterFormat
    {
        /// <summary>
        /// The prefix is used when generating the filter out to a tag, so we can have multiple
        /// filters in a single tag that are uniquely identified.
        /// </summary>
        public string Prefix { get; set; }

        public bool Enabled { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public FilterFormat(string prefix)
        {
            Prefix = prefix;
            Type = string.Empty;
            Value = string.Empty;
        }
    }
}
