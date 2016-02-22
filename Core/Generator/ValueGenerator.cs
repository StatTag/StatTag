using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Generator
{
    public class ValueGenerator : BaseParameterGenerator
    {
        public string CreateParameters(Annotation annotation)
        {
            var builder = new StringBuilder();
            builder.Append(GetIdParameter(annotation));
            builder.Append(GetLabelParameter(annotation));
            builder.Append(GetRunFrequencyParameter(annotation));
            builder.Append(CreateValueParameters(annotation));

            return CleanResult(builder.ToString());
        }

        public string CreateValueParameters(Annotation annotation)
        {
            var builder = new StringBuilder();
            if (annotation.ValueFormat == null)
            {
                builder.Append(CreateDefaultParameters());
            }
            else
            {
                switch (annotation.ValueFormat.FormatType)
                {
                    case Constants.ValueFormatType.Numeric:
                        builder.Append(CreateDefaultParameters(annotation.ValueFormat.FormatType, annotation.ValueFormat.AllowInvalidTypes));
                        builder.Append(CreateNumericParameters(annotation.ValueFormat));
                        break;
                    case Constants.ValueFormatType.DateTime:
                        builder.Append(CreateDefaultParameters(annotation.ValueFormat.FormatType, annotation.ValueFormat.AllowInvalidTypes));
                        builder.Append(CreateDateTimeParameters(annotation.ValueFormat));
                        break;
                    case Constants.ValueFormatType.Percentage:
                        builder.Append(CreateDefaultParameters(annotation.ValueFormat.FormatType, annotation.ValueFormat.AllowInvalidTypes));
                        builder.Append(CreatePercentageParameters(annotation.ValueFormat));
                        break;
                    default:
                        builder.Append(CreateDefaultParameters());
                        break;
                }
            }

            return CleanResult(builder.ToString());
        }

        public string CreatePercentageParameters(ValueFormat format)
        {
            return string.Format("{0}={1}", Constants.ValueParameters.Decimals, format.DecimalPlaces);
        }

        public string CreateDateTimeParameters(ValueFormat format)
        {
            var elements = new List<string>();
            if (!string.IsNullOrWhiteSpace(format.DateFormat))
            {
                elements.Add(string.Format("{0}=\"{1}\"", Constants.ValueParameters.DateFormat, format.DateFormat));
            }

            if (!string.IsNullOrWhiteSpace(format.TimeFormat))
            {
                elements.Add(string.Format("{0}=\"{1}\"", Constants.ValueParameters.TimeFormat, format.TimeFormat));
            }

            return string.Join(", ", elements);
        }

        public string CreateNumericParameters(ValueFormat format)
        {
            return string.Format("{0}={1}, {2}={3}",
                Constants.ValueParameters.Decimals, format.DecimalPlaces,
                Constants.ValueParameters.UseThousands, format.UseThousands);
        }

        /// <summary>
        /// Establishes the default
        /// </summary>
        /// <returns></returns>
        public string CreateDefaultParameters(string type = Constants.ValueFormatType.Default, bool invalidTypes = false)
        {
            var builder = new StringBuilder(string.Format("{0}=\"{1}\", ", Constants.ValueParameters.Type, type));
            if (invalidTypes != Constants.ValueParameterDefaults.AllowInvalidTypes)
            {
                builder.Append(string.Format("{0}={1}, ", Constants.ValueParameters.AllowInvalidTypes, invalidTypes));
            }
            return builder.ToString();
        }
    }
}
