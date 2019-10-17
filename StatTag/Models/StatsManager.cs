using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using R;
using SAS;
using StatTag.Core;
using StatTag.Core.Exceptions;
using StatTag.Core.Models;
using Stata;
using StatTag.Core.Interfaces;
using StatTag.Core.Parser;
using Jupyter;

namespace StatTag.Models
{
    /// <summary>
    /// Manages the execution of code files in the correct statistical package.
    /// </summary>
    public class StatsManager
    {
        /// <summary>
        /// Used exclusively by ExecuteStatPackage as its return value type.
        /// </summary>
        public class ExecuteResult
        {
            /// <summary>
            /// Did the call to ExecuteStatPackage complete without any errors
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// A list of Tags that were detected as having changed values since they
            /// were originally inserted into the document
            /// </summary>
            public List<Tag> UpdatedTags { get; set; }

            /// <summary>
            /// If Success is false, this may contain a description of the error
            /// which can be displayed to the user.
            /// </summary>
            public string ErrorMessage { get; set; }
        }

        /// <summary>
        /// The number of steps after which we will allow the UI to update
        /// </summary>
        private const int RefreshStepInterval = 5;

        public DocumentManager DocumentManager { get; set; }
        public SettingsManager SettingsManager { get; set; }
        public static Configuration Config { get; set; }

        private bool CheckIfStataRunning { get; set; }

        public StatsManager(DocumentManager documentManager, SettingsManager settingsManager, Configuration config)
        {
            DocumentManager = documentManager;
            SettingsManager = settingsManager;
            Config = config;
            CheckIfStataRunning = true;
        }

        /// <summary>
        /// Factory method to return the appropriate statistical automation engine
        /// </summary>
        /// <param name="file">The code file we will be executing</param>
        /// <returns>A stat package automation instance</returns>
        public static IStatAutomation GetStatAutomation(CodeFile file)
        {
            if (file != null)
            {
                switch (file.StatisticalPackage)
                {
                    case Constants.StatisticalPackages.Stata:
                        return new StataAutomation();
                    case Constants.StatisticalPackages.SAS:
                        return new SASAutomation();
                    case Constants.StatisticalPackages.R:
                        return new RAutomation();
                    case Constants.StatisticalPackages.RMarkdown:
                        return new RMarkdownAutomation();
                    case Constants.StatisticalPackages.Python:
                        return new PythonAutomation(Config);
                }
            }

            return null;
        }

        /// <summary>
        /// Customized prompt dialog with variable text and buttons.
        /// </summary>
        /// <remarks>Derived from https://stackoverflow.com/a/9569546/5670646 </remarks>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        /// <param name="confirmationButtonText"></param>
        /// <param name="cancelButtonText"></param>
        /// <returns></returns>
        private DialogResult Prompt(string text, string caption, string confirmationButtonText, string cancelButtonText)
        {
            const int FormWidth = 500;
            const int ControlMargin = 15;
            const int TextMargin = 20;
            const int TextWidth = (FormWidth) - (2*TextMargin);

            var prompt = new Form
            {
                Width = FormWidth,
                Height = 150,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
            };

            var textLabel = new Label() { Left = TextMargin, Top = TextMargin, Width = TextWidth, Text = text };
            var cancel = new Button() { Text = cancelButtonText, Left = 350, Width = 10, Top = 100};
            var confirmation = new Button() { Text = confirmationButtonText, Left = 350, Width = 10, Top = 100 };
            using (Graphics g = prompt.CreateGraphics())
            {
                SizeF size = g.MeasureString(text, textLabel.Font, TextWidth);
                textLabel.Height = (int)Math.Ceiling(size.Height);

                size = g.MeasureString(cancelButtonText, cancel.Font, (FormWidth / 2));
                cancel.Width = (int)Math.Ceiling(size.Width) + TextMargin;
                cancel.Left = FormWidth - cancel.Width - prompt.Margin.Right - ControlMargin;

                size = g.MeasureString(confirmationButtonText, confirmation.Font, (FormWidth / 2));
                confirmation.Width = (int)Math.Ceiling(size.Width) + TextMargin;
                confirmation.Left = cancel.Left - confirmation.Width - ControlMargin;
            }

            prompt.Controls.Add(textLabel);

            cancel.Top = textLabel.Bottom + TextMargin;
            confirmation.Top = cancel.Top;

            confirmation.Click += (sender, e) => { prompt.DialogResult = DialogResult.OK; prompt.Close(); };
            prompt.Height = confirmation.Bottom + ControlMargin + TextMargin;
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            return prompt.ShowDialog();
        }

        /// <summary>
        /// A hook to perform any necessary checks prior to initializing the stat automation
        /// object.  Any notification/interaction with the user should be done from within
        /// this method.
        /// </summary>
        /// <param name="codeFile">The code file that is to be executed</param>
        public bool PreExecutionCheck(CodeFile codeFile)
        {
            // Perform pre-run check for Stata.  Currently this is the only program that
            // requires a pre-run check.
            if (codeFile.StatisticalPackage.Equals(Constants.StatisticalPackages.Stata) && CheckIfStataRunning)
            {
                if (StataAutomation.IsStataRunning(SettingsManager.Settings.StataLocation))
                {
                    var response = Prompt(
                        "StatTag has detected a running instance of Stata.  If you proceed, any unsaved changes to do-files open in the Stata Do-file Editor could be lost.  We recommend that you close the Do-file Editor before continuing.\r\n\r\nDo you want to continue running your Stata code in StatTag?",
                        UIUtility.GetAddInName(), "Run my code", "Cancel");

                    // If they told us to run the code, we're going to stop bugging them and go ahead.
                    if (response == DialogResult.OK)
                    {
                        CheckIfStataRunning = false;
                        return true;
                    }
                    CheckIfStataRunning = true;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Run the statistical package for a given code file.
        /// </summary>
        /// <remarks>
        /// This method should not display any message boxes to the user.  Communication to the user should be handled via exceptions or
        /// ideally through the ExecuteResult response.
        /// </remarks>
        /// <param name="file">The code file to execute</param>
        /// <param name="filterMode">The type of filter to apply on the types of commands to execute</param>
        /// <param name="tagsToRun">An optional list of tags to execute code for.  This is only applied when the filter mode is ParserFilterMode.TagList</param>
        /// <returns></returns>
        public ExecuteResult ExecuteStatPackage(CodeFile file, int filterMode = Constants.ParserFilterMode.ExcludeOnDemand, List<Tag> tagsToRun = null)
        {
            var result = new ExecuteResult() { Success = false, UpdatedTags = new List<Tag>() };
            using (var automation = GetStatAutomation(file))
            {
                if (!automation.Initialize(file, DocumentManager.Logger))
                {
                    result.ErrorMessage = automation.GetInitializationErrorMessage();
                    return result;
                }

                var parser = Factories.GetParser(file);
                if (parser == null)
                {
                    return result;
                }

                var document = Globals.ThisAddIn.SafeGetActiveDocument();
                var metadata = DocumentManager.LoadMetadataFromDocument(document, true);

                try
                {
                    // Get all of the commands in the code file that should be executed given the current filter
                    var steps = parser.GetExecutionSteps(file, automation, filterMode, tagsToRun);
                    for (int index = 0; index < steps.Count; index++)
                    {
                        var step = steps[index];

                        // Every few steps, we will allow the screen to update, otherwise the UI looks like it's
                        // completely hung up.  Note that we will only do this if screen updating is disabled when
                        // we invoke this method.  Otherwise we run the risk of not re-enabling screen updates.
                        if (!Globals.ThisAddIn.Application.ScreenUpdating && (index%RefreshStepInterval == 0))
                        {
                            Globals.ThisAddIn.Application.ScreenUpdating = true;
                            Globals.ThisAddIn.Application.ScreenRefresh();
                            Globals.ThisAddIn.Application.ScreenUpdating = false;
                        }

                        // Call any parser-dependent processing hooks for preparing the code in this step.
                        var codeToExecute = parser.PreProcessExecutionStepCode(step);

                        // If there is no tag, we want to continue our loop with the next step,
                        // since there's no processing needed outside of running the code.
                        if (step.Tag == null)
                        {
                            automation.RunCommands(codeToExecute);
                            continue;
                        }   

                        var results = automation.RunCommands(codeToExecute, step.Tag);

                        var tag = DocumentManager.FindTag(step.Tag.Id);
                        if (tag != null && results != null)
                        {
                            var resultList = new List<CommandResult>(results);

                            // Determine if we had a cached list, and if so if the results have changed.
                            // If the cached list is null, we will always try to refresh.
                            bool resultsChanged = (tag.CachedResult == null) ||
                                                    (tag.CachedResult != null &&
                                                       !resultList.SequenceEqual(tag.CachedResult));
                            tag.CachedResult = resultList;

                            // If the results did change, we need to sweep the document and update all of the results
                            if (resultsChanged)
                            {
                                // For all table tags, update the formatted cells collection
                                if (tag.IsTableTag())
                                {
                                    tag.CachedResult.FindAll(x => x.TableResult != null).ForEach(
                                        x =>
                                            x.TableResult.FormattedCells =
                                                tag.TableFormat.Format(x.TableResult,
                                                    Factories.GetValueFormatter(tag.CodeFile),
                                                    metadata));
                                }

                                result.UpdatedTags.Add(tag);
                            }
                        }
                    }

                    result.Success = true;
                }
                catch (Exception exc)
                {
                    if (DocumentManager != null && DocumentManager.Logger != null)
                    {
                        DocumentManager.Logger.WriteException(exc);
                    }

                    // Hide the statistical program UI (if applicable), and ensure the screen is refreshed once that's
                    // done to avoid any UI artifacts in Word.
                    automation.Hide();
                    if (!Globals.ThisAddIn.Application.ScreenUpdating)
                    {
                        Globals.ThisAddIn.Application.ScreenUpdating = true;
                        Globals.ThisAddIn.Application.ScreenRefresh();
                    }

                    var message = automation.FormatErrorMessageFromExecution(exc);
                    result.ErrorMessage = message;
                    return result;
                }
            }

            return result;
        }
    }
}
