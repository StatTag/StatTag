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
    public partial class ManageCodeBlocks : Form
    {
        public ManageCodeBlocks()
        {
            InitializeComponent();

            dgvItems.Rows.Add(new object[] {"Stata", "Value", "Sample mean", Constants.RunFrequency.Always});
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            var dialog = new ManageAnnotation();
            dialog.ShowDialog();
        }
    }
}
