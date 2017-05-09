using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Generator
{
    public class FigureGenerator : BaseParameterGenerator
    {
        public string CreateParameters(Tag tag)
        {
            // Putting in StringBuilder, assuming more params will be added
            var builder = new StringBuilder();
            builder.Append(GetLabelParameter(tag));
            builder.Append(GetRunFrequencyParameter(tag));
            return CleanResult(builder.ToString());
        }
    }
}
