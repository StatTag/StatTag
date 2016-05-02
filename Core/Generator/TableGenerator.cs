using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Generator
{
    public class TableGenerator : BaseParameterGenerator
    {
        public string CreateParameters(Annotation annotation)
        {
            var builder = new StringBuilder();
            builder.Append(GetLabelParameter(annotation));
            builder.Append(GetRunFrequencyParameter(annotation));
            builder.Append(CreateTableParameters(annotation));

            return CleanResult(builder.ToString());
        }

        public string CreateTableParameters(Annotation annotation)
        {
            var builder = new StringBuilder();
            if (annotation.TableFormat != null)
            {
                builder.AppendFormat("{0}={1}, {2}={3}",
                    Constants.TableParameters.ColumnNames, annotation.TableFormat.IncludeColumnNames,
                    Constants.TableParameters.RowNames, annotation.TableFormat.IncludeRowNames);
            }

            return CleanResult(builder.ToString());
        }
    }
}
