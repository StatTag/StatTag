using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Parser
{
    public class ValueParameterParser : BaseParameterParser
    {
        public new static void Parse(string annotationText, Annotation annotation)
        {
            annotation.ValueFormat = new ValueFormat();
            int paramIndex = annotationText.IndexOf(Constants.AnnotationTags.ParamStart, StringComparison.CurrentCulture);
            // If no parameters are set, fill in default values
            if (paramIndex == -1)
            {
                annotation.ValueFormat.FormatType = Constants.ValueFormatType.Default;
                annotation.RunFrequency = Constants.RunFrequency.Default;
                return;
            }

            BaseParameterParser.Parse(annotationText, annotation);
            annotation.ValueFormat.FormatType = GetStringParameter(Constants.ValueParameters.Type, annotationText, Constants.ValueFormatType.Default);
            int? intValue = GetIntParameter(Constants.ValueParameters.Decimals, annotationText, 0);
            annotation.ValueFormat.DecimalPlaces = intValue.Value;  // Since we specify a default, we assume it won't ever be null
            bool? boolValue = GetBoolParameter(Constants.ValueParameters.UseThousands, annotationText, false);
            annotation.ValueFormat.UseThousands = boolValue.Value;  // Since we specify a default, we assume it won't ever be null
            annotation.ValueFormat.DateFormat = GetStringParameter(Constants.ValueParameters.DateFormat, annotationText);
            annotation.ValueFormat.TimeFormat = GetStringParameter(Constants.ValueParameters.TimeFormat, annotationText);
            boolValue = GetBoolParameter(Constants.ValueParameters.AllowInvalidTypes, annotationText, false);
            annotation.ValueFormat.AllowInvalidTypes = boolValue.Value;  // Since we specify a default, we assume it won't ever be null
        }
    }
}
