using System.Collections.Generic;
using StatTag.Core.Models;

namespace StatTag.Core.Interfaces
{
    public interface ICodeFileParser
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
