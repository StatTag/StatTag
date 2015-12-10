using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Generator
{
    public class ValueGenerator
    {
        public string CreateParameters(Annotation annotation)
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(annotation.OutputLabel))
            {
                builder.AppendFormat("Label=\"{0}\", ", annotation.OutputLabel);
            }

            if (annotation.ValueFormat == null)
            {
                builder.Append(CreateDefaultParameters());
            }
            else
            {
                switch (annotation.ValueFormat.FormatType)
                {
                    case Constants.ValueFormatType.Numeric:
                        builder.AppendFormat("{0}=\"{1}\", ", Constants.ValueParameters.Type, annotation.ValueFormat.FormatType);
                        builder.Append(CreateNumericParameters(annotation.ValueFormat));
                        break;
                    case Constants.ValueFormatType.DateTime:
                        builder.AppendFormat("{0}=\"{1}\", ", Constants.ValueParameters.Type, annotation.ValueFormat.FormatType);
                        builder.Append(CreateDateTimeParameters(annotation.ValueFormat));
                        break;
                    case Constants.ValueFormatType.Percentage:
                        builder.AppendFormat("{0}=\"{1}\", ", Constants.ValueParameters.Type, annotation.ValueFormat.FormatType);
                        builder.Append(CreatePercentageParameters(annotation.ValueFormat));
                        break;
                    default:
                        builder.Append(CreateDefaultParameters());
                        break;
                }
            }

            return builder.ToString().Trim().Trim(new []{','});
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
                Constants.ValueParameters.UseThousands, format.UseThousands.ToString().ToLower());
        }

        public string CreateDefaultParameters()
        {
            return string.Format("{0}=\"{1}\"", Constants.ValueParameters.Type, Constants.ValueFormatType.Default);
        }
    }
}
