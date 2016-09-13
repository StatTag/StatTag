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
        public new static void Parse(string tagText, Tag tag)
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

            // After v1.0, we decided to move away from explicitly including column/row names, since it wasn't universally
            // consistent (in how data is represented) across all statistical packages.  Instead, we moved to row and column
            // filtering which would let the user skip a certain number of rows or columns.  By default then, this includes
            // row names and column names.
            // For backwards compatibility, we will look to see if these values were provided.  Note that we are no longer
            // including a default value for these, because if the attribute isn't present we want to know that. If the
            // attribute is there, we are going to convert it into a filter.
            // We are making the assumption that if this is an old tag that the Include attribute was set.  If not, we will
            // assume this is a newer tag and no conversion is necessary.
            var includeColumnNames = GetBoolParameter(Constants.TableParameters.ColumnNames, tagText);
            if (includeColumnNames.HasValue && !includeColumnNames.Value)
            {
                tag.TableFormat.ColumnFilter.Enabled = true;
                tag.TableFormat.ColumnFilter.Type = Constants.FilterType.Exclude;
                tag.TableFormat.ColumnFilter.Value = "1";
            }

            var includeRowNames = GetBoolParameter(Constants.TableParameters.RowNames, tagText);
            if (includeRowNames.HasValue && !includeRowNames.Value)
            {
                tag.TableFormat.RowFilter.Enabled = true;
                tag.TableFormat.RowFilter.Type = Constants.FilterType.Exclude;
                tag.TableFormat.RowFilter.Value = "1";
            }

            // We don't allow mix-and-match from v1 and v2 parameter types.  If we have set a filter that was set from our
            // migration code above, we're done.  Otherwise we will continue processing tags (this is typically what will happen).
            if (tag.TableFormat.ColumnFilter.Enabled || tag.TableFormat.RowFilter.Enabled)
            {
                return;
            }

            BuildFilter(Constants.FilterPrefix.Column, tag.TableFormat.ColumnFilter, tagText);
            BuildFilter(Constants.FilterPrefix.Row, tag.TableFormat.RowFilter, tagText);

            //tag.TableFormat.ColumnFilter.Enabled =
            //    GetBoolParameter(Constants.FilterPrefix.Column + Constants.TableParameters.FilterEnabled, tagText,
            //        Constants.TableParameterDefaults.FilterEnabled).Value;
            //// We are only going to look at other parameters if the filter is enabled.
            //if (tag.TableFormat.ColumnFilter.Enabled)
            //{
            //    tag.TableFormat.ColumnFilter.Type =
            //        GetStringParameter(Constants.FilterPrefix.Column + Constants.TableParameters.FilterType, tagText,
            //            Constants.TableParameterDefaults.FilterType);
            //    tag.TableFormat.ColumnFilter.Value =
            //        GetStringParameter(Constants.FilterPrefix.Column + Constants.TableParameters.FilterValue, tagText,
            //            Constants.TableParameterDefaults.FilterValue);
            //}
        }

        protected static void BuildFilter(string filterPrefix, FilterFormat filter, string tagText)
        {
            filter.Enabled =
                GetBoolParameter(filterPrefix + Constants.TableParameters.FilterEnabled, tagText,
                    Constants.TableParameterDefaults.FilterEnabled).Value;
            // We are only going to look at other parameters if the filter is enabled.
            if (filter.Enabled)
            {
                filter.Type =
                    GetStringParameter(filterPrefix + Constants.TableParameters.FilterType, tagText,
                        Constants.TableParameterDefaults.FilterType);
                filter.Value =
                    GetStringParameter(filterPrefix + Constants.TableParameters.FilterValue, tagText,
                        Constants.TableParameterDefaults.FilterValue);
            }
            else
            {
                filter.Type = Constants.TableParameterDefaults.FilterType;
                filter.Value = Constants.TableParameterDefaults.FilterValue;
            }
        }
    }
}
