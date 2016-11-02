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
        private static readonly string[] ValueCommands = new[] { "print.default", "print.noquote", "sprintf", "noquote", "print" };
        private static readonly Regex ValueKeywordRegex = new Regex(string.Format("^\\s*(?:{0})\\s*\\(\\s*\\w+\\s*(?:,[\\s\\S]*)?\\)", string.Join("|", ValueCommands)));
        private static readonly Regex ValueRegex = new Regex(string.Format("^\\s*(?:{0})\\s*\\(\\s*(\\w+)\\s*(?:,[\\s\\S]*)?\\)", string.Join("|", ValueCommands)));


        public override string CommentCharacter
        {
            get { return Constants.CodeFileComment.R; }
        }

        public override bool IsImageExport(string command)
        {
            throw new NotImplementedException();
        }

        public override string GetImageSaveLocation(string command)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override string GetTableName(string command)
        {
            throw new NotImplementedException();
        }

        public override List<string> PreProcessContent(List<string> originalContent)
        {
            return originalContent;
        }
    }
}
