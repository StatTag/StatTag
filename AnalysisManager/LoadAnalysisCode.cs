using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AnalysisManager.Controls;
using AnalysisManager.Core.Models;
using Microsoft.Office.Interop.Word;

namespace AnalysisManager
{
    public partial class LoadAnalysisCode : Form
    {
        public LoadAnalysisCode()
        {
            InitializeComponent();
        }

        private void LoadAnalysisCode_Load(object sender, EventArgs e)
        {
            UpdateForControlListChange();
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            var file = new CodeFileControl();
            pnlFiles.Controls.Add(file);
            file.Width = pnlFiles.Width - pnlFiles.Margin.Left - pnlFiles.Margin.Right - file.Margin.Left - file.Margin.Right;
            file.Delete += CodeFileControl_Deleted;
        }

        private void CodeFileControl_Deleted(object sender, EventArgs e)
        {
            var control = sender as CodeFileControl;
            if (control != null)
            {
                control.Delete -= CodeFileControl_Deleted;
                pnlFiles.Controls.Remove(control);
            }
        }

        private void pnlFiles_SizeChanged(object sender, EventArgs e)
        {
            foreach (var control in pnlFiles.Controls.OfType<CodeFileControl>())
            {
                control.Width = pnlFiles.Width - pnlFiles.Margin.Left - pnlFiles.Margin.Right - control.Margin.Left - control.Margin.Right;                
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            var files = pnlFiles.Controls.OfType<CodeFileControl>().Select(control => control.Details).Where(x => !string.IsNullOrWhiteSpace(x.FilePath)).ToList();
            this.Close();
        }

        private void pnlFiles_ControlRemoved(object sender, ControlEventArgs e)
        {
            UpdateForControlListChange();
        }

        private void UpdateForControlListChange()
        {
            if (pnlFiles.Controls.OfType<CodeFileControl>().Any())
            {
                lblEmpty.Visible = false;
            }
            else
            {
                lblEmpty.Visible = true;
                lblEmpty.Top = pnlFiles.Top;
                lblEmpty.Left = pnlFiles.Left;
                lblEmpty.Width = pnlFiles.Width;
                lblEmpty.Height = pnlFiles.Height;
            }
        }

        private void pnlFiles_ControlAdded(object sender, ControlEventArgs e)
        {
            UpdateForControlListChange();
        }
    }
}
