using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;

namespace StatTag.Core.Parser
{
    public class RParser : BaseParser
    {
        public static readonly string[] FigureCommands = new[] { "pdf", "win.metafile", "png", "jpeg", "bmp", "postscript" };
        private static readonly Regex FigureKeywordRegex = new Regex(string.Format("^\\s*(?:{0})\\s*\\(\\s*?[\\s\\S]*?\\)", string.Join("|", FigureCommands)));
        private static readonly Regex FigureRegex = new Regex(string.Format("^\\s*(?:{0})\\s*\\s*\\([\\s\\S]*?(?:(?:file=)?[\\\"']([\\s\\S]*?)[\\\"'])[\\s\\S]*?\\)", string.Join("|", FigureCommands)));


        public override string CommentCharacter
        {
            get { return Constants.CodeFileComment.R; }
        }

        public override bool IsImageExport(string command)
        {
            return FigureKeywordRegex.IsMatch(command);
        }

        public override string GetImageSaveLocation(string command)
        {
            return MatchRegexReturnGroup(command, FigureRegex, 1);
        }

        /// <summary>
        /// Because of how R operates, we allow more flexibility such that anything
        /// can be considered a value or a table.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Always returns true</returns>
        public override bool IsValueDisplay(string command)
        {
            return true;
        }

        /// <summary>
        /// Not used (see IsValueDisplay)
        /// </summary>
        /// <param name="command"></param>
        /// <returns>string.Empty</returns>
        public override string GetValueName(string command)
        {
            return string.Empty;
        }

        /// <summary>
        /// Because of how R operates, we allow more flexibility such that anything
        /// can be considered a value or a table.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Always returns true</returns>
        public override bool IsTableResult(string command)
        {
            return true;
        }

        /// <summary>
        /// Not used (see IsTableResult)
        /// </summary>
        /// <param name="command"></param>
        /// <returns>string.Empty</returns>
        public override string GetTableName(string command)
        {
            return string.Empty;
        }

        public override List<string> PreProcessContent(List<string> originalContent)
        {
            return originalContent;
        }
    }
}
