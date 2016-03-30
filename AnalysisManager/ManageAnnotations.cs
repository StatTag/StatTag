using AnalysisManager.Core.Models;
using AnalysisManager.Models;
using System;
using System.Linq;
using System.Windows.Forms;

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

        /// <summary>
        /// 
        /// </summary>
        private void ReloadAnnotations()
        {
            dgvItems.Rows.Clear();

            // Save off the values that may already be cached for an annotation.
            var existingAnnotations = Manager.GetAnnotations().Select(a => new Annotation(a)).ToList();

            foreach (var file in Manager.Files)
            {
                file.LoadAnnotationsFromContent();
                file.Annotations.ForEach(x => AddRow(x));
            }
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
    }
}
