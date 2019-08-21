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
        // TODO: Implement figure handling
        public static readonly string[] FigureCommands = new[] { "NOT IMPLEMENTED" };

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
            // We consider any tagged command a candidate for a value result
            return true;
        }

        /// <summary>
        /// Not implemented for Python
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
            // We consider any command a candidate for a table result
            return true;
        }

        /// <summary>
        /// Not implement for Python
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
