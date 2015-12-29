﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Parser
{
    /// <summary>
    /// Reads through a file containing stata commands and identifies the blocks of
    /// code that use the Analysis Manager annotation syntax
    /// </summary>
    public sealed class Stata : BaseParser
    {
        private static readonly char[] MacroDelimiters = {'`', '\''};
        private static string ValueCommand = "display";
        private static Regex ValueKeywordRegex = new Regex(string.Format("^\\s*{0}\\b", ValueCommand));
        private static Regex ValueRegex = new Regex(string.Format("^\\s*{0}\\s+(.*)", ValueCommand));
        private static string GraphCommand = "graph export";
        private static Regex GraphKeywordRegex = new Regex(string.Format("^\\s*{0}\\b", GraphCommand.Replace(" ", "\\s+")));
        private static Regex GraphRegex = new Regex(string.Format("^\\s*{0}\\s+\\\"(.*)\\\"", GraphCommand.Replace(" ", "\\s+")));
        /// <summary>
        /// This is used to test/extract a macro display value.
        /// <remarks>It assumes the rest of the display command has been extracted, 
        /// and only the value name remains.</remarks>
        /// </summary>
        private static Regex MacroValueRegex = new Regex("^\\s*`(.+)'");

        public override string CommentCharacter
        {
            get { return Constants.CodeFileComment.Stata; }
        }

        /// <summary>
        /// Determine if a command is for exporting an image
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override bool IsImageExport(string command)
        {
            return GraphKeywordRegex.IsMatch(command);
        }

        public override bool IsValueDisplay(string command)
        {
            return ValueKeywordRegex.IsMatch(command);
        }

        /// <summary>
        /// Determine if a display value is a macro type.
        /// </summary>
        /// <remarks>Assumes that you have already verified the command contains a display value.</remarks>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool IsMacroDisplayValue(string command)
        {
            string value = GetValueName(command);
            return MacroValueRegex.IsMatch(value);
        }

        /// <summary>
        /// Returns the file path where an image exported from the statistical package
        /// is being saved.
        /// </summary>
        /// <remarks>Assumes that you have verified this is an image export command
        /// using IsImageExport first</remarks>
        /// <param name="command"></param>
        /// <returns></returns>
        public override string GetImageSaveLocation(string command)
        {
            var match = GraphRegex.Match(command);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }

            return string.Empty;
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
            var match = ValueRegex.Match(command);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }

            return string.Empty;
        }

        /// <summary>
        /// A specialized version of GetValueName that prepares a macro display value
        /// for use.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string GetMacroValueName(string command)
        {
            return GetValueName(command).Trim(MacroDelimiters);
        }
    }
}
