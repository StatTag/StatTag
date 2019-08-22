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
        /// <summary>
        /// Identifies a string that represents the path to a figure/image
        /// </summary>
        /// <remarks>Because Python has several plotting libraries, we are not going to try and identify
        /// a set of built-in method names to detect that it is a figure.  It will be up to the user
        /// to tag correctly, and for us to handle errors correctly.  Instead, we're just going to detect
        /// anything within parentheses as valid.</remarks>
        private static readonly Regex FigureRegex = new Regex(string.Format(".+?\\((\\s*?[\\s\\S]+)\\)"));

        public override string CommentCharacter
        {
            get { return Constants.CodeFileComment.Python; }
        }

        public override bool IsImageExport(string command)
        {
            // If the command is blank/null, there's no way it's an image export command.
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return FigureRegex.IsMatch(command);
        }

        /// <summary>
        /// Return the variable name or file path used to save an image
        /// </summary>
        /// <param name="command">The command to extract the image save path from</param>
        /// <returns>A string containing the Python expression to define the image save location, or an empty string if one cannot be found.</returns>
        public override string GetImageSaveLocation(string command)
        {
            // If the command is blank/null, return our default value of a blank string.
            if (string.IsNullOrWhiteSpace(command))
            {
                return string.Empty;
            }

            var match = FigureRegex.Match(command);
            if (!match.Success || match.Groups.Count < 2)
            {
                return string.Empty;
            }

            return match.Groups[1].Value.Trim();
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
