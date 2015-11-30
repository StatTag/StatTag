using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace AnalysisManager
{
    public partial class MainRibbon
    {
        private void MainRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void cmdLoadCode_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new LoadAnalysisCode();
            dialog.ShowDialog();
        }

        private void cmdManageOutput_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new ManageCodeBlocks();
            dialog.ShowDialog();
        }
    }
}
