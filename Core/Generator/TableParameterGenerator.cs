using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Generator
{
    public class TableParameterGenerator : BaseParameterGenerator
    {
        public string CreateParameters(Tag tag)
        {
            var builder = new StringBuilder();
            builder.Append(GetLabelParameter(tag));
            builder.Append(GetRunFrequencyParameter(tag));
            builder.Append(CreateTableParameters(tag));

            return CleanResult(builder.ToString());
        }

        protected void AppendFilter(FilterFormat filter, StringBuilder builder)
        {
            if (filter != null && filter.Enabled)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                {
                    builder.AppendFormat(", ");
                }

                builder.AppendFormat("{0}{1}={2}, {0}{3}=\"{4}\", {0}{5}=\"{6}\"", filter.Prefix,
                    Constants.TableParameters.FilterEnabled, filter.Enabled,
                    Constants.TableParameters.FilterType, filter.Type,
                    Constants.TableParameters.FilterValue, filter.Value);
            }
        }

        public string CreateTableParameters(Tag tag)
        {
            var builder = new StringBuilder();

            // If the table format is null, or both filters are disabled we are not going to add anything
            // to the output (just to reduce clutter).
            if (tag.TableFormat != null &&
                (tag.TableFormat.ColumnFilter.Enabled || tag.TableFormat.RowFilter.Enabled))
            {
                AppendFilter(tag.TableFormat.ColumnFilter, builder);
                AppendFilter(tag.TableFormat.RowFilter, builder);
            }

            return CleanResult(builder.ToString());
        }
    }
}
