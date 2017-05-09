using System.Collections.Generic;
using StatTag.Core.Models;
using StatTag.Models;
using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;

namespace StatTag
{
    public sealed partial class ManageTags : Form
    {
        private const int CheckColumn = 0;
        private const int StatPackageColumn = 1;
        private const int TypeColumn = 2;
        private const int LabelColumn = 3;
        private const int WhenToRunColumn = 4;
        private const int EditColumn = 5;

        public DocumentManager Manager { get; set; }

        private readonly List<Tag> Tags = new List<Tag>();

        public ManageTags(DocumentManager manager)
        {
            InitializeComponent();
            UIUtility.ScaleFont(this);
            MinimumSize = Size;
            Manager = manager;
            UIUtility.SetDialogTitle(this);
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            var dialog = new EditTag(false, Manager);
            if (DialogResult.OK == dialog.ShowDialog())
            {
                Manager.SaveEditedTag(dialog);
                AddRow(dialog.Tag);
                ReloadTags();
            }
        }

        private void AddRow(Tag tag)
        {
            if (tag == null || tag.CodeFile == null)
            {
                return;
            }

            int row = dgvItems.Rows.Add(new object[] { false, tag.CodeFile.StatisticalPackage, tag.Type, tag.Name, tag.RunFrequency, Constants.DialogLabels.Edit });
            dgvItems.Rows[row].Tag = tag;
        }

        private void cmdRemove_Click(object sender, EventArgs e)
        {
            var removedTags = UIUtility.RemoveSelectedItems(dgvItems, CheckColumn);
            if (removedTags != null)
            {
                var removedItems = removedTags.Select(x => x as Tag);
                foreach (var item in removedItems)
                {
                    item.CodeFile.RemoveTag(item);
                }
            }
        }

        private void ManageCodeBlocks_Load(object sender, EventArgs e)
        {
            ReloadTags();
        }

        private void LoadList(string filter = "")
        {
            dgvItems.Rows.Clear();
            foreach (var tag in Tags.Where(x => x.Name.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0))
            {
                AddRow(tag);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReloadTags()
        {
            Tags.Clear();
            foreach (var file in Manager.GetCodeFileList())
            {
                file.LoadTagsFromContent();
                file.Tags.ForEach(x => Tags.Add(x));
            }
            LoadList(txtFilter.Text);
        }

        private void EditTag(int rowIndex)
        {
            var existingTag = dgvItems.Rows[rowIndex].Tag as Tag;
            if (Manager.EditTag(existingTag))
            {
                ReloadTags();
            }
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == EditColumn)
            {
                EditTag(e.RowIndex);
            }
        }

        private void dgvItems_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditTag(e.RowIndex);
        }

        private void txtFilter_FilterChanged(object sender, EventArgs e)
        {
            LoadList(txtFilter.Text);
        }
    }
}
