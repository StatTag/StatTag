using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using R;
using SAS;
using StatTag.Core;
using StatTag.Core.Models;
using Stata;
using StatTag.Core.Interfaces;
using StatTag.Core.Parser;

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
        }

        /// <summary>
        /// The number of steps after which we will allow the UI to update
        /// </summary>
        private const int RefreshStepInterval = 5;

        public DocumentManager DocumentManager { get; set; }
        public PropertiesManager PropertiesManager { get; set; }

        public StatsManager(DocumentManager documentManager, PropertiesManager propertiesManager)
        {
            DocumentManager = documentManager;
            PropertiesManager = propertiesManager;
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
                }
            }

            return null;
        }

        public static ICodeFileParser GetCodeFileParser(CodeFile file)
        {
            if (file != null)
            {
                switch (file.StatisticalPackage)
                {
                    case Constants.StatisticalPackages.Stata:
                        return new StataParser();
                }
            }

            return null;
        }

        /// <summary>
        /// Run the statistical package for a given code file.
        /// </summary>
        /// <param name="file">The code file to execute</param>
        /// <param name="filterMode">The type of filter to apply on the types of commands to execute</param>
        /// <param name="tagsToRun">An optional list of tags to execute code for.  This is only applied when the filter mode is ParserFilterMode.TagList</param>
        /// <returns></returns>
        public ExecuteResult ExecuteStatPackage(CodeFile file, int filterMode = Constants.ParserFilterMode.ExcludeOnDemand, List<Tag> tagsToRun = null)
        {
            var result = new ExecuteResult() { Success = false, UpdatedTags = new List<Tag>() };
            using (var automation = GetStatAutomation(file))
            {
                if (!automation.Initialize(file))
                {
                    MessageBox.Show(automation.GetInitializationErrorMessage(), UIUtility.GetAddInName());
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
                    var steps = parser.GetExecutionSteps(file, filterMode, tagsToRun);
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

                        // If there is no tag, we will join all of the command code together.  This allows us to have
                        // multi-line statements, such as a for loop.  Because we don't check for return results, we just
                        // process the command and continue.
                        if (step.Tag == null)
                        {
                            string combinedCommand = string.Join("\r\n", step.Code.ToArray());
                            automation.RunCommands(new[] {combinedCommand});
                            continue;
                        }


                        var results = automation.RunCommands(step.Code.ToArray(), step.Tag);

                        var tag = DocumentManager.FindTag(step.Tag.Id);
                        if (tag != null)
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

                    MessageBox.Show(exc.Message, UIUtility.GetAddInName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return result;
                }
            }

            return result;
        }
    }
}
