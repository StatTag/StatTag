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
        private static string ValueCommand = "display";
        private static Regex ValueRegex = new Regex(string.Format("{0}\\s+(.*)", ValueCommand.Replace(" ", "\\s+")));
        private static string GraphCommand = "graph export";
        private static Regex GraphRegex = new Regex(string.Format("{0}\\s+\\\"(.*)\\\"", GraphCommand.Replace(" ", "\\s+")));

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
            return command.Trim().StartsWith(GraphCommand);
        }

        public override bool IsValueDisplay(string command)
        {
            return command.Trim().StartsWith(ValueCommand);
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
                return match.Groups[1].Value;
            }

            return string.Empty;
        }

        public override string GetValueName(string command)
        {
            var match = ValueRegex.Match(command);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return string.Empty;
        }
    }
}
