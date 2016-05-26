using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;
using ScintillaNET;

namespace StatTag.Models
{
    /// <summary>
    /// Manages the configuration of the Scintilla editor control to use the correct lexer and
    /// syntax highlighting style.
    /// </summary>
    public static class ScintillaManager
    {
        /// <summary>
        /// Configure the editor for the specified code file.
        /// </summary>
        /// <param name="scintilla">The editor control to be configured</param>
        /// <param name="codeFile">The code file that will be loaded.</param>
        public static void ConfigureEditor(Scintilla scintilla, CodeFile codeFile)
        {
            //Reset the styles
            scintilla.StyleResetDefault();
            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 10;
            scintilla.StyleClearAll(); // i.e. Apply to all

            if (codeFile == null)
            {
                return;
            }

            switch (codeFile.StatisticalPackage)
            {
                case Constants.StatisticalPackages.Stata:
                    ConfigureStataEditor(scintilla);
                    break;
                case Constants.StatisticalPackages.R:
                    ConfigureREditor(scintilla);
                    break;
            }
        }

        /// <summary>
        /// Internal method to do the specific configurations for Stata do files.
        /// </summary>
        /// <param name="scintilla">The Scintilla control to configure.</param>
        private static void ConfigureStataEditor(Scintilla scintilla)
        {
            // Set the lexer
            scintilla.Lexer = Lexer.Stata;

            // Disable code block folding.
            scintilla.SetProperty("fold", "0");

            // Set the styles
            scintilla.Styles[Style.Stata.Default].ForeColor = Color.Black;
            scintilla.Styles[Style.Stata.Comment].ForeColor = Color.FromArgb(0x00, 0x7F, 0x00);
            scintilla.Styles[Style.Stata.Comment].Italic = true;
            scintilla.Styles[Style.Stata.CommentLine].ForeColor = Color.FromArgb(0x00, 0x7F, 0x00);
            scintilla.Styles[Style.Stata.CommentLine].Italic = true;
            scintilla.Styles[Style.Stata.CommentBlock].ForeColor = Color.FromArgb(0x00, 0x7F, 0x00);
            scintilla.Styles[Style.Stata.CommentBlock].Italic = true;
            scintilla.Styles[Style.Stata.Number].ForeColor = Color.Black;
            scintilla.Styles[Style.Stata.String].ForeColor = Color.FromArgb(0xA3, 0x15, 0x15);
            scintilla.Styles[Style.Stata.Operator].Bold = true;
            scintilla.Styles[Style.Stata.Word].ForeColor = Color.FromArgb(0x00, 0x00, 0x7F);

            // We don't like showing the whitespace markers
            scintilla.ViewWhitespace = WhitespaceMode.Invisible;

            // Keyword lists:
            // 0 "Keywords",
            // 1 "Highlighted identifiers"
            var keywords = "foreach if in of for any pause more set on off by bysort sort use save saveold insheet using global local gen egen mean median replace graph export inlist keep drop legend label la var clear compress mem memory duplicates di display substring substr subinstring subinstr twoway line scatter estimates estout xi:regress tabulate scalar ttest histogram return summarize count matrix sysuse log";
            scintilla.SetKeywords(0, keywords);
            scintilla.SetKeywords(1, keywords);
        }

        /// <summary>
        /// Internal method to do the specific configurations for R code files.
        /// </summary>
        /// <param name="scintilla">The Scintilla control to configure.</param>
        private static void ConfigureREditor(Scintilla scintilla)
        {
            // Set the lexer
            scintilla.Lexer = Lexer.R;

            // Disable code block folding.
            scintilla.SetProperty("fold", "0");

            // Set the styles
            scintilla.Styles[Style.R.Default].ForeColor = Color.Black;
            scintilla.Styles[Style.R.Comment].ForeColor = Color.FromArgb(0x00, 0x7F, 0x00);
            scintilla.Styles[Style.R.Comment].Italic = true;
            scintilla.Styles[Style.R.Number].ForeColor = Color.FromArgb(0x00, 0x7F, 0x7F);
            scintilla.Styles[Style.R.String].ForeColor = Color.FromArgb(0x7F, 0x00, 0x7F);
            scintilla.Styles[Style.R.Operator].Bold = true;
            scintilla.Styles[Style.R.KWord].ForeColor = Color.FromArgb(0x00, 0x7F, 0x7F);

            scintilla.ViewWhitespace = WhitespaceMode.Invisible;
        }
    }
}
