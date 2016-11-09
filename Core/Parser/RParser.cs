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
        public static readonly string[] ValueCommands = new[] { "print.default", "print.noquote", "sprintf", "noquote", "print" };
        private static readonly Regex ValueKeywordRegex = new Regex(string.Format("^\\s*(?:{0})\\s*\\(\\s*[\\S]+?\\s*(?:,[\\s\\S]*)?\\)", string.Join("|", ValueCommands.Select(x => x.Replace(".", "\\.")))));
        private static readonly Regex ValueRegex = new Regex(string.Format("^\\s*(?:{0})\\s*\\(\\s*([\\S]+?)\\s*(?:,[\\s\\S]*)?\\)", string.Join("|", ValueCommands.Select(x => x.Replace(".", "\\.")))));

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
        /// Determine if a command is for displaying a result
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override bool IsValueDisplay(string command)
        {
            return ValueKeywordRegex.IsMatch(command);
        }

        /// <summary>
        /// Returns the name of the variable/scalar to display.
        /// </summary>
        /// <remarks>Assumes that you have verified this is a display command using
        /// IsValueDisplay first.</remarks>
        /// <param name="command"></param>
        /// <returns></returns>
        public override string GetValueName(string command)
        {
            return MatchRegexReturnGroup(command, ValueRegex, 1);
        }

        public override bool IsTableResult(string command)
        {
            return true;
        }

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
