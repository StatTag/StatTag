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
        public static readonly string[] TableCommands = ValueCommands;

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

        /// <summary>
        /// Not implemented for Python - Return the name of the value variable
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override string GetValueName(string command)
        {
            return string.Empty;
        }

        /// <summary>
        /// Determine if the command contains an expression that StatTag can use to extract a table
        /// result from.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override bool IsTableResult(string command)
        {
            // We are reusing the IsValueDisplay check here because we currently have the assumption that the print
            // command will identify both values and tables.
            return IsValueDisplay(command);
        }

        /// <summary>
        /// Not implement for Python - Return the name of the table variable
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
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
