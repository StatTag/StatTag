using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StatTag.Core.Models;
using StatTag.Models;

namespace StatTag
{
    public sealed partial class SelectOutput : Form
    {
        private readonly List<CodeFile> Files = new List<CodeFile>();
        private readonly List<Tag> Tags = new List<Tag>();
        private readonly TagListViewColumnSorter ListViewSorter = new TagListViewColumnSorter();

        public SelectOutput(List<CodeFile> files = null)
        {
            InitializeComponent();
            UIUtility.ScaleFont(this);
            Files = files;
            UIUtility.SetDialogTitle(this);
        }

        public List<Tag> GetSelectedTags()
        {
            return UIUtility.GetCheckedTagsFromListView(lvwOutput).ToList();
        }

        private void SelectOutput_Load(object sender, EventArgs e)
        {
            foreach (var file in Files)
            {
                foreach (var tag in file.Tags)
                {
                    Tags.Add(tag);
                }
            }

            lvwOutput.ListViewItemSorter = ListViewSorter;
            LoadList();
        }

        private void LoadList(string filter = "")
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                lvwOutput.Items.Clear();

                var filteredTags = Tags.Where(x => x.Name.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0).OrderBy(x => x.LineStart);
                foreach (var tag in filteredTags)
                {
                    var item = lvwOutput.Items.Add(tag.Name);
                    item.SubItems.AddRange(new[] {tag.CodeFile.FilePath});
                    item.Tag = tag;
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void txtFilter_FilterChanged(object sender, EventArgs e)
        {
            LoadList(txtFilter.Text);
        }

        private void lvwOutput_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewSorter.HandleSort(e.Column, lvwOutput);
        }
    }
}
