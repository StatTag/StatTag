using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Generator
{
    public class TableGenerator : BaseParameterGenerator
    {
        public string CreateParameters(Tag tag)
        {
            var builder = new StringBuilder();
            builder.Append(GetLabelParameter(tag));
            builder.Append(GetRunFrequencyParameter(tag));
            builder.Append(CreateTableParameters(tag));

            return CleanResult(builder.ToString());
        }

        public string CreateTableParameters(Tag tag)
        {
            var builder = new StringBuilder();
            if (tag.TableFormat != null)
            {
                builder.AppendFormat("{0}={1}, {2}={3}",
                    Constants.TableParameters.ColumnNames, tag.TableFormat.IncludeColumnNames,
                    Constants.TableParameters.RowNames, tag.TableFormat.IncludeRowNames);
            }

            return CleanResult(builder.ToString());
        }
    }
}
