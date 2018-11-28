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
            scintilla.MarginOptions = MarginOptions.NoSelect;
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
                case Constants.StatisticalPackages.SAS:
                    ConfigureSASEditor(scintilla);
                    break;
                case Constants.StatisticalPackages.R:
                    ConfigureREditor(scintilla);
                    break;
                case Constants.StatisticalPackages.RMarkdown:
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
        /// Internal method to do the specific configurations for SAS files.
        /// </summary>
        /// <param name="scintilla">The Scintilla control to configure.</param>
        private static void ConfigureSASEditor(Scintilla scintilla)
        {
            // Set the lexer
            scintilla.Lexer = Lexer.SAS;

            // Disable code block folding.
            scintilla.SetProperty("fold", "0");

            // Set the styles
            scintilla.Styles[Style.SAS.Default].ForeColor = Color.Black;
            scintilla.Styles[Style.SAS.Comment].ForeColor = Color.FromArgb(0x00, 0x7F, 0x00);
            scintilla.Styles[Style.SAS.Comment].Italic = true;
            scintilla.Styles[Style.SAS.CommentLine].ForeColor = Color.FromArgb(0x00, 0x7F, 0x00);
            scintilla.Styles[Style.SAS.CommentLine].Italic = true;
            scintilla.Styles[Style.SAS.CommentBlock].ForeColor = Color.FromArgb(0x00, 0x7F, 0x00);
            scintilla.Styles[Style.SAS.CommentBlock].Italic = true;
            scintilla.Styles[Style.SAS.Number].ForeColor = Color.Black;
            scintilla.Styles[Style.SAS.String].ForeColor = Color.FromArgb(0xA3, 0x15, 0x15);
            scintilla.Styles[Style.SAS.Operator].Bold = true;
            scintilla.Styles[Style.SAS.Word].ForeColor = Color.FromArgb(0x00, 0x00, 0x7F);
            scintilla.Styles[Style.SAS.Macro].ForeColor = Color.FromArgb(0x00, 0x00, 0x00);
            scintilla.Styles[Style.SAS.Macro].Italic = true;
            scintilla.Styles[Style.SAS.MacroKeyword].ForeColor = Color.FromArgb(0x00, 0x00, 0xFF);
            scintilla.Styles[Style.SAS.BlockKeyword].ForeColor = Color.FromArgb(0x00, 0x00, 0x7F);
            scintilla.Styles[Style.SAS.BlockKeyword].Bold = true;
            scintilla.Styles[Style.SAS.MacroFunction].ForeColor = Color.FromArgb(0x00, 0x00, 0x00);
            scintilla.Styles[Style.SAS.MacroFunction].Bold = true;
            scintilla.Styles[Style.SAS.Statement].ForeColor = Color.FromArgb(0x00, 0x00, 0xFF);

            // We don't like showing the whitespace markers
            scintilla.ViewWhitespace = WhitespaceMode.Invisible;

            // Keyword lists:
            // 0 "Macro keywords",
            // 1 "Macro block keywords"
            // 2 "Macro function keywords"
            // 3 "Statements"
            var keywords = "%end %length %sysevalf %abort %eval %let %qscan %sysexec %qsubstr %sysfunc %global %qsysfunc %sysget %bquote %go %local %quote %sysrput %by %goto %qupcase %then %if %inc %return %tso %cms %include %nrstr %unquote %index %scan %until %input %put %upcase %nrbquote %str %while %nrquote %syscall %window %display %substr %superq %symdel %do %symexist %else";
            var blockKeywords = "%macro %mend proc data run";
            var functionKeywords = "%abend %qkupcase %act %file %list %activate %listm %clear %resolve %to %close %pause %run %comandr %on %save %unstr %copy %infile %open %deact %stop %del %kcmpres %delete %kindex %kleft %metasym %dmidsply %klength %qkcmpres %dmisplit %kscan %qkleft %ksubstr %qkscan %edit %ktrim %qksubstr %symglobl %kupcase %qktrim %symlocal";
            var statements = "abort array attrib by call cards cards4 catname checkpoint execute_always continue datalines datalines4 declare delete describe display dm do until while drop end endsas error execute file filename footnote format go to if then else infile informat input keep label leave length libname link list lock lostcard merge missing modify null ods options output page put putlog redirect remove rename replace retain return sasfile select set skip stop sum sysecho title update where window x";
            scintilla.SetKeywords(0, keywords);
            scintilla.SetKeywords(1, blockKeywords);
            scintilla.SetKeywords(2, functionKeywords);
            scintilla.SetKeywords(3, statements);
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
            scintilla.Styles[Style.R.Number].ForeColor = Color.Black;
            scintilla.Styles[Style.R.String].ForeColor = Color.FromArgb(0xA3, 0x15, 0x15);
            scintilla.Styles[Style.R.Operator].Bold = true;
            scintilla.Styles[Style.R.KWord].ForeColor = Color.FromArgb(0x00, 0x00, 0x7F);
            scintilla.Styles[Style.R.BaseKWord].ForeColor = Color.FromArgb(0x00, 0x00, 0x7F);
            scintilla.Styles[Style.R.OtherKWord].ForeColor = Color.FromArgb(0x00, 0x00, 0x7F);

            var keywords = @"commandArgs detach length dev.off stop lm library predict lmer 
           plot print display anova read.table read.csv complete.cases dim attach as.numeric seq max 
           min data.frame lines curve as.integer levels nlevels ceiling sqrt ranef order
           AIC summary str head png tryCatch par mfrow interaction.plot qqnorm qqline";

            var keywords2 = @"TRUE FALSE if else for while in break continue function library require source";

            scintilla.SetKeywords(0, keywords);
            scintilla.SetKeywords(1, keywords2);

            scintilla.ViewWhitespace = WhitespaceMode.Invisible;
        }
    }
}
