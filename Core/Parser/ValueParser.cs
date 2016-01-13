﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Parser
{
    public class ValueParser : BaseParameterParser
    {
        public static void Parse(string annotationText, Annotation annotation)
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

            annotation.OutputLabel = GetStringParameter(Constants.AnnotationParameters.Label, annotationText);
            annotation.RunFrequency = GetStringParameter(Constants.AnnotationParameters.Frequency, annotationText, Constants.RunFrequency.Default);
            annotation.ValueFormat.FormatType = GetStringParameter(Constants.ValueParameters.Type, annotationText, Constants.ValueFormatType.Default);
            int? intValue = GetIntParameter(Constants.ValueParameters.Decimals, annotationText, 0);
            annotation.ValueFormat.DecimalPlaces = intValue.Value;  // Since we specify a default, we assume it won't ever be null
            bool? boolValue = GetBoolParameter(Constants.ValueParameters.UseThousands, annotationText, false);
            annotation.ValueFormat.UseThousands = boolValue.Value;  // Since we specify a default, we assume it won't ever be null
            annotation.ValueFormat.DateFormat = GetStringParameter(Constants.ValueParameters.DateFormat, annotationText);
            annotation.ValueFormat.TimeFormat = GetStringParameter(Constants.ValueParameters.TimeFormat, annotationText);
        }
    }
}
