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

        public StatsManager StatsManager
        {
            get { return Globals.ThisAddIn.StatsManager; }
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
                Globals.ThisAddIn.Application.ScreenUpdating = false;
                try
                {
                    var updatedAnnotations = new List<Annotation>();
                    var refreshedFiles = new HashSet<CodeFile>();
                    var annotations = dialog.GetSelectedAnnotations();
                    foreach (var annotation in annotations)
                    {
                        if (!refreshedFiles.Contains(annotation.CodeFile))
                        {
                            var result = StatsManager.ExecuteStatPackage(annotation.CodeFile,
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
                    Globals.ThisAddIn.Application.ScreenUpdating = true;
                    Cursor.Current = Cursors.Default;   
                }
            }
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
            Globals.ThisAddIn.Application.ScreenUpdating = false;
            try
            {
                // First, go through and update all of the code files to ensure we have all
                // refreshed annotations.
                var refreshedFiles = new HashSet<CodeFile>();
                foreach (var codeFile in Manager.Files)
                {
                    if (!refreshedFiles.Contains(codeFile))
                    {
                        var result = StatsManager.ExecuteStatPackage(codeFile, Constants.ParserFilterMode.AnnotationList, annotations);
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
                Globals.ThisAddIn.Application.ScreenUpdating = true;
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
