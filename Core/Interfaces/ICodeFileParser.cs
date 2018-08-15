using System.Collections.Generic;
using StatTag.Core.Models;

namespace StatTag.Core.Interfaces
{
    public interface ICodeFileParser
    {
        Tag[] Parse(CodeFile file, int filterMode = Constants.ParserFilterMode.IncludeAll, List<Tag> tagsToRun = null);
        Tag[] ParseIncludingInvalidTags(CodeFile file);
        List<ExecutionStep> GetExecutionSteps(CodeFile file, int filterMode = Constants.ParserFilterMode.IncludeAll, List<Tag> tagsToRun = null);
        string[] PreProcessExecutionStepCode(ExecutionStep step);
        bool IsImageExport(string command);
        string GetImageSaveLocation(string command);
        bool IsValueDisplay(string command);
        string GetValueName(string command);
        bool IsTableResult(string command);
        /// <summary>
        /// Return a named object that represents table data within the statistical program
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        string GetTableName(string command);

        /// <summary>
        /// Return a path to where table data is found
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        string GetTableDataPath(string command);
    }
}
