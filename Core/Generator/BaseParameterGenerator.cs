using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Generator
{
    public class BaseParameterGenerator
    {
        public string GetLabelParameter(Annotation annotation)
        {
            if (!string.IsNullOrWhiteSpace(annotation.OutputLabel))
            {
                return string.Format("{0}=\"{1}\", ", Constants.AnnotationParameters.Label, annotation.OutputLabel);
            }

            return string.Empty;
        }

        public string GetRunFrequencyParameter(Annotation annotation)
        {
            if (!string.IsNullOrWhiteSpace(annotation.RunFrequency))
            {
                return string.Format("{0}=\"{1}\", ", Constants.AnnotationParameters.Frequency, annotation.RunFrequency);
            }

            return string.Empty;
        }

        public string CleanResult(string result)
        {
            return result.Trim().Trim(new[] {','});
        }
    }
}
