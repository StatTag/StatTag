using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Parser
{
    public class FigureParameterParser : BaseParameterParser
    {
        public static void Parse(string annotationText, Annotation annotation)
        {
            annotation.FigureFormat = new FigureFormat();
            int paramIndex = annotationText.IndexOf(Constants.AnnotationTags.ParamStart, StringComparison.CurrentCulture);
            // If no parameters are set, fill in default values
            if (paramIndex == -1)
            {
                annotation.RunFrequency = Constants.RunFrequency.Default;
                return;
            }

            BaseParameterParser.Parse(annotationText, annotation);
        }
    }
}
