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

namespace AnalysisManager
{
    public partial class MainRibbon
    {
        public DocumentManager Manager
        {
            get { return Globals.ThisAddIn.Manager; }
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

        private void cmdManageOutput_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new ManageCodeBlocks(Manager.Files);
            if (DialogResult.OK == dialog.ShowDialog())
            {
                //dialog.Annotations;
            }
        }

        private void cmdInsertOutput_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new SelectOutput(Manager.Files);
            if (DialogResult.OK == dialog.ShowDialog())
            {
                foreach (var annotation in dialog.GetSelectedAnnotations())
                {
                    Manager.InsertField(annotation);
                }
            }
        }

        private void ExecuteStatPackage(CodeFile file)
        {
            var automation = new Stata.Automation();
            automation.Initialize();

            // Get all of the commands in the code file that are not marked to be run as "on demand"
            var parser = Factories.GetParser(file);
            if (parser == null)
            {
                return;
            }

            var filteredLines = parser.Filter(file.GetContent(), Constants.ParserFilterMode.ExcludeOnDemand);
            var results = automation.RunCommands(filteredLines);
        }
    }
}
