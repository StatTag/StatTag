using System.Collections.Generic;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Models
{
    public class DocumentManager
    {
        public List<CodeFile> Files { get; set; }

        public const string ConfigurationAttribute = "Analysis Manager Configuration";

        public DocumentManager()
        {
            Files = new List<CodeFile>();
        }

        public void SaveFileListToDocument()
        {
            var document = Globals.ThisAddIn.Application.ActiveDocument;
            var attribute = CodeFile.SerializeList(Files);
            document.Variables.Add(ConfigurationAttribute, attribute);
            var test = document.Variables[ConfigurationAttribute].Value;
        }
    }
}
