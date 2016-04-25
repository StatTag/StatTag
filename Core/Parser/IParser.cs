using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Interfaces;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Parser
{
    public interface IParser
    {
        Annotation[] Parse(CodeFile file, int filterMode = Constants.ParserFilterMode.IncludeAll, List<Annotation> annotationsToRun = null);
        List<ExecutionStep> GetExecutionSteps(CodeFile file, int filterMode = Constants.ParserFilterMode.IncludeAll, List<Annotation> annotationsToRun = null);
        bool IsImageExport(string command);
        string GetImageSaveLocation(string command);
        bool IsValueDisplay(string command);
        string GetValueName(string command);
        bool IsTableResult(string command);
        string GetTableName(string command);
    }
}
