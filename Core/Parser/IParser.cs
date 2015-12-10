using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Parser
{
    public interface IParser
    {
        Annotation[] Parse(IList<string> lines, int filterMode = Constants.ParserFilterMode.IncludeAll);
        string[] Filter(IList<string> lines, int filterMode = Constants.ParserFilterMode.IncludeAll);
    }
}
