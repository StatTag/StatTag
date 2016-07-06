using System.Collections.Generic;
using System.Text.RegularExpressions;
using StatTag.Core.Models;

namespace StatTag.Core.Parser
{
    public class SASParser : BaseParser
    {
        private static string ValueCommand = "%put";
        private static readonly Regex ValueKeywordRegex = new Regex(string.Format("^\\s*{0}\\b", ValueCommand));
        private static readonly Regex ValueRegex = new Regex(string.Format("^\\s*{0}\\s+([^;]*);", ValueCommand));

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