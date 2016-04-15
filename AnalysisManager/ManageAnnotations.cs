using System.Collections.Generic;
using AnalysisManager.Core.Models;
using AnalysisManager.Models;
using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;

namespace AnalysisManager
{
    public sealed partial class ManageAnnotations : Form
    {
        private const int CheckColumn = 0;
        private const int StatPackageColumn = 1;
        private const int TypeColumn = 2;
        private const int LabelColumn = 3;
        private const int WhenToRunColumn = 4;
        private const int EditColumn = 5;

        public DocumentManager Manager { get; set; }

        private Dictionary<string, Annotation> Annotations = new Dictionary<string, Annotation>();

        public ManageAnnotations(DocumentManager manager)
        {
            InitializeComponent();
            Font = UIUtility.CreateScaledFont(Font, CreateGraphics());
            MinimumSize = Size;
            Manager = manager;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            var dialog = new EditAnnotation(Manager);
            if (DialogResult.OK == dialog.ShowDialog())
            {
                Manager.SaveEditedAnnotation(dialog);
                AddRow(dialog.Annotation);
                ReloadAnnotations();
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
            var removedTags = UIUtility.RemoveSelectedItems(dgvItems, CheckColumn);
            if (removedTags != null)
            {
                var removedItems = removedTags.Select(x => x as Annotation);
                foreach (var item in removedItems)
                {
                    item.CodeFile.RemoveAnnotation(item);
                }
            }
        }

        private void ManageCodeBlocks_Load(object sender, EventArgs e)
        {
            ReloadAnnotations();
        }

        private void LoadList(string filter = "")
        {
            dgvItems.Rows.Clear();
            foreach (var annotation in Annotations.Where(x => x.Key.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0))
            {
                AddRow(annotation.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReloadAnnotations()
        {
            Annotations.Clear();
            foreach (var file in Manager.Files)
            {
                file.LoadAnnotationsFromContent();
                file.Annotations.ForEach(x => Annotations.Add(x.OutputLabel, x));
            }
            LoadList(txtFilter.Text);
        }

        private void EditAnnotation(int rowIndex)
        {
            var existingAnnotation = dgvItems.Rows[rowIndex].Tag as Annotation;
            if (Manager.EditAnnotation(existingAnnotation))
            {
                ReloadAnnotations();
            }
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == EditColumn)
            {
                EditAnnotation(e.RowIndex);
            }
        }

        private void dgvItems_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditAnnotation(e.RowIndex);
        }

        private void txtFilter_FilterChanged(object sender, EventArgs e)
        {
            LoadList(txtFilter.Text);
        }
    }
}
