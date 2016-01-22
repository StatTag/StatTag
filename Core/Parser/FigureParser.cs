using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Parser
{
    public class FigureParser : BaseParameterParser
    {
        public static void Parse(string annotationText, Annotation annotation)
        {
            annotation.ValueFormat = new ValueFormat();
            int paramIndex = annotationText.IndexOf(Constants.AnnotationTags.ParamStart, StringComparison.CurrentCulture);
            // If no parameters are set, fill in default values
            if (paramIndex == -1)
            {
                return;
            }

            annotation.OutputLabel = GetStringParameter(Constants.AnnotationParameters.Label, annotationText);
            annotation.RunFrequency = GetStringParameter(Constants.AnnotationParameters.Frequency, annotationText, Constants.RunFrequency.Default);
        }
    }
}
