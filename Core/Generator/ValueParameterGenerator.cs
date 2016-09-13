using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Generator
{
    public class ValueParameterGenerator : BaseParameterGenerator
    {
        public string CreateParameters(Tag tag)
        {
            var builder = new StringBuilder();
            builder.Append(GetLabelParameter(tag));
            builder.Append(GetRunFrequencyParameter(tag));
            builder.Append(CreateValueParameters(tag));

            return CleanResult(builder.ToString());
        }

        public string CreateValueParameters(Tag tag)
        {
            var builder = new StringBuilder();
            if (tag.ValueFormat == null)
            {
                builder.Append(CreateDefaultParameters());
            }
            else
            {
                switch (tag.ValueFormat.FormatType)
                {
                    case Constants.ValueFormatType.Numeric:
                        builder.Append(CreateDefaultParameters(tag.ValueFormat.FormatType, tag.ValueFormat.AllowInvalidTypes));
                        builder.Append(CreateNumericParameters(tag.ValueFormat));
                        break;
                    case Constants.ValueFormatType.DateTime:
                        builder.Append(CreateDefaultParameters(tag.ValueFormat.FormatType, tag.ValueFormat.AllowInvalidTypes));
                        builder.Append(CreateDateTimeParameters(tag.ValueFormat));
                        break;
                    case Constants.ValueFormatType.Percentage:
                        builder.Append(CreateDefaultParameters(tag.ValueFormat.FormatType, tag.ValueFormat.AllowInvalidTypes));
                        builder.Append(CreatePercentageParameters(tag.ValueFormat));
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
