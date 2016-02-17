using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using AnalysisManager.Core;
using AnalysisManager.Core.Generator;
using AnalysisManager.Core.Models;
using AnalysisManager.Models;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools.Ribbon;
using Application = System.Windows.Forms.Application;

namespace AnalysisManager
{
    public partial class MainRibbon
    {
        public DocumentManager Manager
        {
            get { return Globals.ThisAddIn.Manager; }
        }

        public PropertiesManager PropertiesManager
        {
            get { return Globals.ThisAddIn.PropertiesManager; }
        }

        public LogManager LogManager
        {
            get { return Globals.ThisAddIn.LogManager; }
        }

        private void MainRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void cmdLoadCode_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                var dialog = new LoadAnalysisCode(Manager, Manager.Files);
                if (DialogResult.OK == dialog.ShowDialog())
                {
                    Manager.Files = dialog.Files;
                }
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when trying to manage your code files.",
                    LogManager);
            }
        }

        private void cmdManageAnnotations_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                var dialog = new ManageAnnotations(Manager);
                if (DialogResult.OK == dialog.ShowDialog())
                {
                    Manager.SaveAllCodeFiles();
                }
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when trying to manage your annotations.",
                    LogManager);
            }
        }

        private void cmdInsertOutput_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new SelectOutput(Manager.Files);
            if (DialogResult.OK == dialog.ShowDialog())
            {
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    var updatedAnnotations = new List<Annotation>();
                    var refreshedFiles = new HashSet<CodeFile>();
                    var annotations = dialog.GetSelectedAnnotations();
                    foreach (var annotation in annotations)
                    {
                        if (!refreshedFiles.Contains(annotation.CodeFile))
                        {
                            var result = ExecuteStatPackage(annotation.CodeFile,
                                Constants.ParserFilterMode.AnnotationList, annotations);
                            if (!result.Success)
                            {
                                break;
                            }

                            updatedAnnotations.AddRange(result.UpdatedAnnotations);
                            refreshedFiles.Add(annotation.CodeFile);
                        }

                        Manager.InsertField(annotation);
                    }

                    // Now that all of the fields have been inserted, sweep through and update any existing
                    // annotations that changed.  We do this after the fields are inserted to better manage
                    // the cursor position in the document.
                    updatedAnnotations = updatedAnnotations.Distinct().ToList();
                    foreach (var updatedAnnotation in updatedAnnotations)
                    {
                        Manager.UpdateFields(new UpdatePair<Annotation>(updatedAnnotation, updatedAnnotation));
                    }
                }
                catch (Exception exc)
                {
                    UIUtility.ReportException(exc,
                        "There was an unexpected error when trying to insert a value into the document.",
                        LogManager);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;   
                }
            }
        }

        /// <summary>
        /// Used exclusively by ExecuteStatPackage as its return value type.
        /// </summary>
        private class ExecuteResult
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
        /// Run the statistical package for a given code file.
        /// </summary>
        /// <param name="file">The code file to execute</param>
        /// <param name="filterMode">The type of filter to apply on the types of commands to execute</param>
        /// <param name="annotationsToRun">An optional list of annotations to execute code for.  This is only applied when the filter mode is ParserFilterMode.AnnotationList</param>
        /// <returns></returns>
        private ExecuteResult ExecuteStatPackage(CodeFile file, int filterMode = Constants.ParserFilterMode.ExcludeOnDemand, List<Annotation> annotationsToRun = null)
        {
            var result = new ExecuteResult() {Success = false, UpdatedAnnotations = new List<Annotation>()};
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

        private void cmdSettings_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                var dialog = new Settings(PropertiesManager.Properties);
                if (DialogResult.OK == dialog.ShowDialog())
                {
                    PropertiesManager.Properties = dialog.Properties;
                    PropertiesManager.Save();
                    LogManager.UpdateSettings(dialog.Properties);
                }
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when trying to manage your settings",
                    LogManager);
            }
        }

        private void cmdUpdateOutput_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new UpdateOutput(Manager.GetAnnotations());
            if (DialogResult.OK != dialog.ShowDialog())
            {
                return;
            }

            var annotations = dialog.SelectedAnnotations;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                // First, go through and update all of the code files to ensure we have all
                // refreshed annotations.
                var refreshedFiles = new HashSet<CodeFile>();
                foreach (var codeFile in Manager.Files)
                {
                    if (!refreshedFiles.Contains(codeFile))
                    {
                        var result = ExecuteStatPackage(codeFile, Constants.ParserFilterMode.AnnotationList, annotations);
                        if (!result.Success)
                        {
                            break;
                        }

                        refreshedFiles.Add(codeFile);
                    }
                }

                // Now we will refresh all of the annotations that are fields.  Since we most likely
                // have more fields than annotations, we are going to use the approach of looping
                // through all fields and updating them (via the DocumentManager).
                Manager.UpdateFields();
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when tring to update values in your document.",
                    LogManager);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
