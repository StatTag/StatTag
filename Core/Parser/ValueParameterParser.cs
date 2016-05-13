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
        public new static void Parse(string tagText, Tag tag)
        {
            tag.ValueFormat = new ValueFormat();
            int paramIndex = tagText.IndexOf(Constants.TagTags.ParamStart, StringComparison.CurrentCulture);
            // If no parameters are set, fill in default values
            if (paramIndex == -1)
            {
                tag.ValueFormat.FormatType = Constants.ValueFormatType.Default;
                tag.RunFrequency = Constants.RunFrequency.Default;
                return;
            }

            BaseParameterParser.Parse(tagText, tag);
            tag.ValueFormat.FormatType = GetStringParameter(Constants.ValueParameters.Type, tagText, Constants.ValueFormatType.Default);
            int? intValue = GetIntParameter(Constants.ValueParameters.Decimals, tagText, 0);
            tag.ValueFormat.DecimalPlaces = intValue.Value;  // Since we specify a default, we assume it won't ever be null
            bool? boolValue = GetBoolParameter(Constants.ValueParameters.UseThousands, tagText, false);
            tag.ValueFormat.UseThousands = boolValue.Value;  // Since we specify a default, we assume it won't ever be null
            tag.ValueFormat.DateFormat = GetStringParameter(Constants.ValueParameters.DateFormat, tagText);
            tag.ValueFormat.TimeFormat = GetStringParameter(Constants.ValueParameters.TimeFormat, tagText);
            boolValue = GetBoolParameter(Constants.ValueParameters.AllowInvalidTypes, tagText, false);
            tag.ValueFormat.AllowInvalidTypes = boolValue.Value;  // Since we specify a default, we assume it won't ever be null
        }
    }
}
