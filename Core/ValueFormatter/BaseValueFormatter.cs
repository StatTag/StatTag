using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisManager.Core.ValueFormatter
{
    public class BaseValueFormatter : IValueFormatter
    {
        public const string MissingValue = "";

        public virtual string GetMissingValue()
        {
            return MissingValue;
        }

        public virtual string Finalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? GetMissingValue() : value;
        }
    }
}
