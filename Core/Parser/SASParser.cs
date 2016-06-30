using System.Collections.Generic;
using StatTag.Core.Models;

namespace StatTag.Core.Parser
{
    public class SASParser : BaseParser
    {
        public override string CommentCharacter
        {
            get { return Constants.CodeFileComment.SAS; }
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
            return false;
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

        public override List<string> PreProcessContent(List<string> originalContent)
        {
            return originalContent;
            ;
        }
    }
}