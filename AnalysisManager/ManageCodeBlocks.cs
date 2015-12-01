using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AnalysisManager.Core.Models;

namespace AnalysisManager
{
    public sealed partial class ManageCodeBlocks : Form
    {
        private const int CheckColumn = 0;

        public List<CodeFile> Files { get; set; }

        public ManageCodeBlocks(List<CodeFile> files)
        {
            InitializeComponent();
//            dgvItems.Rows.Add(new object[] {false, "Stata", "Value", "Sample mean", Constants.RunFrequency.Always, Constants.DialogLabels.Edit });
            this.MinimumSize = this.Size;
            Files = files;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            var dialog = new ManageAnnotation(Files);
            dialog.ShowDialog();
        }

        private void cmdRemove_Click(object sender, EventArgs e)
        {
            UIUtility.RemoveSelectedItems(dgvItems, CheckColumn);
        }
    }
}
