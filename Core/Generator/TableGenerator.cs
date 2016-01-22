using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Generator
{
    public class TableGenerator : BaseParameterGenerator
    {
        public string CreateParameters(Annotation annotation)
        {
            // Putting in StringBuilder, assuming more params will be added
            var builder = new StringBuilder();
            builder.Append(GetLabelParameter(annotation));
            builder.Append(GetRunFrequencyParameter(annotation));
            return CleanResult(builder.ToString());
        }
    }
}
