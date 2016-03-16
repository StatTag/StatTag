using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AnalysisManager.Core;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Models
{
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
            var automation = new Stata.Automation();
            if (!automation.Initialize())
            {
                MessageBox.Show(
                    "Could not communicate with Stata.  You will need to enable Stata Automation (not done by default) to run this code in Analysis Manager.\r\n\r\nThis can be done from Analysis Manager > Settings, or see http://www.stata.com/automation",
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
                var steps = parser.GetExecutionSteps(file.LoadFileContent(), filterMode, annotationsToRun);
                foreach (var step in steps)
                {
                    var results = automation.RunCommands(step.Code.ToArray());
                    if (step.Annotation == null)
                    {
                        continue;
                    }

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
                                    x => x.TableResult.FormattedCells = annotation.TableFormat.Format(x.TableResult));
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

            return result;
        }
    }
}
