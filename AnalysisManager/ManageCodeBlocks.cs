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
            MinimumSize = Size;
            Files = files;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            var dialog = new ManageAnnotation(Files);
            if (DialogResult.OK == dialog.ShowDialog())
            {
                if (dialog.Annotation != null && dialog.Annotation.CodeFile != null)
                {
                    // Add the annotation reference to the code file (which saves it for later use).
                    // If we don't do this, we will lose the reference (which is fine for a Cancel
                    // operation).
                    dialog.Annotation.CodeFile.Annotations.Add(dialog.Annotation);
                    AddRow(dialog.Annotation);
                }
            }
        }

        private void AddRow(Annotation annotation)
        {
            if (annotation == null || annotation.CodeFile == null)
            {
                return;
            }

            int row = dgvItems.Rows.Add(new object[] { false, annotation.CodeFile.StatisticalPackage, annotation.Type, annotation.OutputLabel, annotation.RunFrequency, Constants.DialogLabels.Edit });
            dgvItems.Rows[row].Tag = annotation;
        }

        private void cmdRemove_Click(object sender, EventArgs e)
        {
            UIUtility.RemoveSelectedItems(dgvItems, CheckColumn);
        }

        private void ManageCodeBlocks_Load(object sender, EventArgs e)
        {
            foreach (var file in Files)
            {
                foreach (var annotation in file.Annotations)
                {
                    AddRow(annotation);
                }
            }
        }
    }
}
