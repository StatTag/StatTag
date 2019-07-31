using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Parser
{
    public class PythonParser : BaseParser
    {
        public override string CommentCharacter
        {
            get { return Constants.CodeFileComment.Python; }
        }

        public override bool IsImageExport(string command)
        {
            return false;
        }

        public override string GetImageSaveLocation(string command)
        {
            return string.Empty;
        }

        public override bool IsValueDisplay(string command)
        {
            return true;
        }

        public override string GetValueName(string command)
        {
            return string.Empty;
        }

        public override bool IsTableResult(string command)
        {
            return false;
        }

        public override string GetTableName(string command)
        {
            return string.Empty;
        }

        public override string GetTableDataPath(string command)
        {
            return string.Empty;
        }

        public override List<string> PreProcessContent(List<string> originalContent, Interfaces.IStatAutomation automation = null)
        {
            return originalContent;
        }
    }
}
