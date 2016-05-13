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
        public static void Parse(string tagText, Tag tag)
        {
            tag.FigureFormat = new FigureFormat();
            int paramIndex = tagText.IndexOf(Constants.TagTags.ParamStart, StringComparison.CurrentCulture);
            // If no parameters are set, fill in default values
            if (paramIndex == -1)
            {
                tag.RunFrequency = Constants.RunFrequency.Default;
                return;
            }

            BaseParameterParser.Parse(tagText, tag);
        }
    }
}
