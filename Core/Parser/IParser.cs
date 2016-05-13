using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;

namespace StatTag.Core.Parser
{
    public interface IParser
    {
        Tag[] Parse(CodeFile file, int filterMode = Constants.ParserFilterMode.IncludeAll, List<Tag> tagsToRun = null);
        List<ExecutionStep> GetExecutionSteps(CodeFile file, int filterMode = Constants.ParserFilterMode.IncludeAll, List<Tag> tagsToRun = null);
        bool IsImageExport(string command);
        string GetImageSaveLocation(string command);
        bool IsValueDisplay(string command);
        string GetValueName(string command);
        bool IsTableResult(string command);
        string GetTableName(string command);
    }
}
