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
                            if (!ExecuteStatPackage(annotation.CodeFile))
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

        private bool ExecuteStatPackage(CodeFile file)
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
                var steps = parser.GetExecutionSteps(file.LoadFileContent(), Constants.ParserFilterMode.ExcludeOnDemand);
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
    }
}
