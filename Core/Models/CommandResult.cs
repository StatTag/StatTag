using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisManager.Core.Models
{
    public class CommandResult
    {
        public string ValueResult { get; set; }
        public string FigureResult { get; set; }
        public Table TableResult { get; set; }

        public bool IsEmpty()
        {
            return (string.IsNullOrWhiteSpace(ValueResult)
                    && string.IsNullOrWhiteSpace(FigureResult)
                    && (TableResult == null || TableResult.IsEmpty()));
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(ValueResult))
            {
                return ValueResult;
            }

            if (!string.IsNullOrWhiteSpace(FigureResult))
            {
                return FigureResult;
            }

            if (TableResult != null)
            {
                return TableResult.ToString();
            }

            return string.Empty;
        }
    }
}
