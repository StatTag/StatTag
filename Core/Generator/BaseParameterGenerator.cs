using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Generator
{
    public class BaseParameterGenerator
    {
        public string GetLabelParameter(Annotation annotation)
        {
            if (!string.IsNullOrWhiteSpace(annotation.OutputLabel))
            {
                return string.Format("Label=\"{0}\", ", annotation.OutputLabel);
            }

            return string.Empty;
        }

        public string CleanResult(string result)
        {
            return result.Trim().Trim(new[] {','});
        }
    }
}
