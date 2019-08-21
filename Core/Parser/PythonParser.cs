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
        private static readonly Regex FigureRegex = new Regex(string.Format(".+\\((\\s*?[\\s\\S]*)\\)"));

        public override string CommentCharacter
        {
            get { return Constants.CodeFileComment.Python; }
        }

        public override bool IsImageExport(string command)
        {
            // We consider any tagged command a candidate for an image result
            return true;
        }

        public override string GetImageSaveLocation(string command)
        {
            var match = FigureRegex.Match(command);
            if (!match.Success || match.Groups.Count < 2)
            {
                return string.Empty;
            }

            return match.Groups[1].Value;
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
