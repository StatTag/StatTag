using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StatTag.Core.Exceptions;
using StatTag.Core.Generator;
using StatTag.Core.Models;

namespace StatTag.Core.Parser
{
    public class PythonParser : BaseParser
    {
        public static readonly string[] ValueCommands = { "print" };
        private static readonly Regex Python2ValueKeywordRegex = new Regex(string.Format("^\\s*(?:{0})[ \\t]+[^\\(\\)\\n\\r]+", FormatCommandListAsNonCapturingGroup(ValueCommands)));
        private static readonly Regex Python3ValueKeywordRegex = new Regex(string.Format("^\\s*(?:{0})[ \\t]*\\([\\w\\s]*\\)", FormatCommandListAsNonCapturingGroup(ValueCommands)));

        // TODO - implement checks for figures and tables
        public static readonly string[] FigureCommands = { "" };
        public static readonly string[] TableCommands = { "" };

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
            return Python2ValueKeywordRegex.IsMatch(command) || Python3ValueKeywordRegex.IsMatch(command);
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
