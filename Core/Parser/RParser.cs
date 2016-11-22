using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        private static readonly Regex FigureRegex = new Regex(string.Format("^\\s*(?:{0})\\s*\\((\\s*?[\\s\\S]*?)\\)", string.Join("|", FigureCommands)));
        private static readonly Regex FigureParameterRegex = new Regex("(?:([\\w]*?)\\s*=\\s*)?(?:([\\w]*?\\s*\\(.*?\\))|([\\w]+))");
        private const char ParameterDelimiter = '=';
        private const char ArgumentDelimiter = ',';
        private const string FileParameterName = "file";

        public override string CommentCharacter
        {
            get { return Constants.CodeFileComment.R; }
        }

        public override bool IsImageExport(string command)
        {
            return FigureRegex.IsMatch(command);
        }

        private class FunctionParam
        {
            public int Index;
            public string Key;
            public string Value;
        }

        public override string GetImageSaveLocation(string command)
        {
            var match = FigureRegex.Match(command);
            var parameters = new List<FunctionParam>();
            if (!match.Success)
            {
                return string.Empty;
            }

            var arguments = match.Groups[1].Value.Split(ArgumentDelimiter);
            for (int index = 0; index < arguments.Length; index++)
            {
                var components = arguments[index].Split(ParameterDelimiter).Select(x => x.Trim()).ToArray();
                parameters.Add(new FunctionParam()
                {
                    Index = index,
                    Key = (components.Length > 1 ? components[0] : string.Empty),
                    Value = (components.Length > 1 ? components[1] : components[0]).Replace("\"", "").Replace("'", "")
                });
            }

            // Follow R's approach to argument matching (http://adv-r.had.co.nz/Functions.html#function-arguments)
            // First, look for exact name (perfect matching)
            var matchingArg =
                parameters.FirstOrDefault(
                    x => x.Key.Equals(FileParameterName, StringComparison.CurrentCultureIgnoreCase));
            if (matchingArg != null)
            {
                return matchingArg.Value;
            }

            // Next look by prefix matching
            matchingArg =
                parameters.FirstOrDefault(
                    x =>
                        !string.IsNullOrWhiteSpace(x.Key) &&
                        FileParameterName.StartsWith(x.Key, StringComparison.CurrentCultureIgnoreCase));
            if (matchingArg != null)
            {
                return matchingArg.Value;
            }

            // Finally position-based matching (1st item)
            return parameters.First(x => x.Index == 0).Value;
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
