using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StatTag.Core;
using StatTag.Core.Models;
using Stata;

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
            /// A list of Annotations that were detected as having changed values since they
            /// were originally inserted into the document
            /// </summary>
            public List<Annotation> UpdatedAnnotations { get; set; }
        }

        /// <summary>
        /// The number of steps after which we will allow the UI to update
        /// </summary>
        private const int RefreshStepInterval = 5;

        public DocumentManager Manager { get; set; }

        public StatsManager(DocumentManager manager)
        {
            Manager = manager;
        }

        /// <summary>
        /// Run the statistical package for a given code file.
        /// </summary>
        /// <param name="file">The code file to execute</param>
        /// <param name="filterMode">The type of filter to apply on the types of commands to execute</param>
        /// <param name="annotationsToRun">An optional list of annotations to execute code for.  This is only applied when the filter mode is ParserFilterMode.AnnotationList</param>
        /// <returns></returns>
        public ExecuteResult ExecuteStatPackage(CodeFile file, int filterMode = Constants.ParserFilterMode.ExcludeOnDemand, List<Annotation> annotationsToRun = null)
        {
            var result = new ExecuteResult() { Success = false, UpdatedAnnotations = new List<Annotation>() };
            using (var automation = new Automation())
            {
                if (!automation.Initialize())
                {
                    MessageBox.Show(
                        "Could not communicate with Stata.  You will need to enable Stata Automation (not done by default) to run this code in StatTag.\r\n\r\nThis can be done from StatTag > Settings, or see http://www.stata.com/automation",
                        UIUtility.GetAddInName());
                    return result;
                }

                var parser = Factories.GetParser(file);
                if (parser == null)
                {
                    return result;
                }

                try
                {
                    // Get all of the commands in the code file that should be executed given the current filter
                    var steps = parser.GetExecutionSteps(file, filterMode, annotationsToRun);
                    //foreach (var step in steps)
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

                        // If there is no annotation, we will join all of the command code together.  This allows us to have
                        // multi-line statements, such as a for loop.  Because we don't check for return results, we just
                        // process the command and continue.
                        if (step.Annotation == null)
                        {
                            string combinedCommand = string.Join("\r\n", step.Code.ToArray());
                            automation.RunCommands(new[] {combinedCommand});
                            continue;
                        }


                        var results = automation.RunCommands(step.Code.ToArray());

                        var annotation = Manager.FindAnnotation(step.Annotation.Id);
                        if (annotation != null)
                        {
                            var resultList = new List<CommandResult>(results);

                            // Determine if we had a cached list, and if so if the results have changed.
                            bool resultsChanged = (annotation.CachedResult != null &&
                                                   !resultList.SequenceEqual(annotation.CachedResult));
                            annotation.CachedResult = resultList;

                            // If the results did change, we need to sweep the document and update all of the results
                            if (resultsChanged)
                            {
                                // For all table annotations, update the formatted cells collection
                                if (annotation.IsTableAnnotation())
                                {
                                    annotation.CachedResult.FindAll(x => x.TableResult != null).ForEach(
                                        x =>
                                            x.TableResult.FormattedCells =
                                                annotation.TableFormat.Format(x.TableResult,
                                                    Factories.GetValueFormatter(annotation.CodeFile)));
                                }

                                result.UpdatedAnnotations.Add(annotation);
                            }
                        }
                    }

                    result.Success = true;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, UIUtility.GetAddInName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return result;
                }
            }

            return result;
        }
    }
}
