using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Parser
{
    public class TableParser : BaseParameterParser
    {
        public static void Parse(string annotationText, Annotation annotation)
        {
            annotation.TableFormat = new TableFormat();
            int paramIndex = annotationText.IndexOf(Constants.AnnotationTags.ParamStart, StringComparison.CurrentCulture);
            // If no parameters are set, fill in default values
            if (paramIndex == -1)
            {
                annotation.RunFrequency = Constants.RunFrequency.Default;
                return;
            }

            annotation.OutputLabel = GetStringParameter(Constants.AnnotationParameters.Label, annotationText);
            annotation.RunFrequency = GetStringParameter(Constants.AnnotationParameters.Frequency, annotationText, Constants.RunFrequency.Default);
            annotation.TableFormat.IncludeColumnNames = GetBoolParameter(Constants.TableParameters.ColumnNames, annotationText, Constants.TableParameterDefaults.ColumnNames).Value;
            annotation.TableFormat.IncludeRowNames = GetBoolParameter(Constants.TableParameters.RowNames, annotationText, Constants.TableParameterDefaults.RowNames).Value;
        }
    }
}
