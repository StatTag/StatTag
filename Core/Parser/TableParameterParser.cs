using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Parser
{
    public class TableParameterParser : BaseParameterParser
    {
        public static void Parse(string tagText, Tag tag)
        {
            tag.TableFormat = new TableFormat();
            int paramIndex = tagText.IndexOf(Constants.TagTags.ParamStart, StringComparison.CurrentCulture);
            // If no parameters are set, fill in default values
            if (paramIndex == -1)
            {
                tag.RunFrequency = Constants.RunFrequency.Always;
                return;
            }

            BaseParameterParser.Parse(tagText, tag);
            tag.TableFormat.IncludeColumnNames = GetBoolParameter(Constants.TableParameters.ColumnNames, tagText, Constants.TableParameterDefaults.ColumnNames).Value;
            tag.TableFormat.IncludeRowNames = GetBoolParameter(Constants.TableParameters.RowNames, tagText, Constants.TableParameterDefaults.RowNames).Value;
        }
    }
}
