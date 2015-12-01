using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AnalysisManager.Core.Models;
using Microsoft.Office.Tools.Ribbon;

namespace AnalysisManager
{
    public partial class MainRibbon
    {
        public List<CodeFile> Files = new List<CodeFile>();

        private void MainRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void cmdLoadCode_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new LoadAnalysisCode(Files);
            if (DialogResult.OK == dialog.ShowDialog())
            {
                Files = dialog.Files;
            }
        }

        private void cmdManageOutput_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new ManageCodeBlocks(Files);
            dialog.ShowDialog();
        }
    }
}
