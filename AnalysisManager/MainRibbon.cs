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
//using System = Microsoft.Office.Interop.Word.System;

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
            get { return Globals.ThisAddIn.PropertiesManager;  }
        }

        private void MainRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void cmdLoadCode_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new LoadAnalysisCode(Manager.Files);
            if (DialogResult.OK == dialog.ShowDialog())
            {
                Manager.Files = dialog.Files;
            }
        }

        private void cmdManageAnnotations_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new ManageAnnotations(Manager.Files);
            if (DialogResult.OK == dialog.ShowDialog())
            {
                // Update the code files with their annotations
                foreach (var file in Manager.Files)
                {
                    file.Save();
                }
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
                    var refreshedFiles = new HashSet<CodeFile>();
                    var annotations = dialog.GetSelectedAnnotations();
                    foreach (var annotation in annotations)
                    {
                        if (!refreshedFiles.Contains(annotation.CodeFile))
                        {
                            if (!ExecuteStatPackage(annotation.CodeFile, Constants.ParserFilterMode.AnnotationList, annotations))
                            {
                                break;
                            }

                            refreshedFiles.Add(annotation.CodeFile);
                        }

                        Manager.InsertField(annotation);
                    }
                }
                finally
                {
                    Cursor.Current = Cursors.Default;   
                }
            }
        }

        private bool ExecuteStatPackage(CodeFile file, int filterMode = Constants.ParserFilterMode.ExcludeOnDemand, List<Annotation> annotationsToRun = null)
        {
            var automation = new Stata.Automation();
            if (!automation.Initialize())
            {
                MessageBox.Show(
                    "Could not communicate with Stata.  You will need to enable Stata Automation (not done by default) to run this code in Analysis Manager.\r\n\r\nThis can be done from Analysis Manager > Settings, or see http://www.stata.com/automation");
                return false;
            }

            // Get all of the commands in the code file that are not marked to be run as "on demand"
            var parser = Factories.GetParser(file);
            if (parser == null)
            {
                return false;
            }

            try
            {
                var steps = parser.GetExecutionSteps(file.LoadFileContent(), filterMode, annotationsToRun);
                foreach (var step in steps)
                {
                    var results = automation.RunCommands(step.Code.ToArray());
                    if (step.Annotation != null)
                    {
                        var annotation = Manager.FindAnnotation(step.Annotation.OutputLabel, step.Annotation.Type);
                        if (annotation != null)
                        {
                            annotation.CachedResult = new List<string>(results);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, System.Windows.Forms.Application.ProductName);
                return false;
            }

            return true;
        }

        private void cmdSettings_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new Settings(PropertiesManager.Properties);
            if (DialogResult.OK == dialog.ShowDialog())
            {
                PropertiesManager.Properties = dialog.Properties;
                PropertiesManager.Save();
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
                        if (!ExecuteStatPackage(codeFile, Constants.ParserFilterMode.AnnotationList, annotations))
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
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
