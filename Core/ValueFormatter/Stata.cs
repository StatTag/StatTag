using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Core.ValueFormatter
{
    public class Stata : BaseValueFormatter
    {
        public new const string MissingValue = ".";

        public override string GetMissingValue()
        {
            return MissingValue;
        }
    }
}
