using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AnalysisManager.Core.Models;
using AnalysisManager.Models;
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
        }
    }
}
