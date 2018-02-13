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
using Microsoft.Office.Tools.Word;

namespace StatTag
{
    public sealed partial class UpdateOutput : Form
    {
        public List<Tag> Tags { get; set; }

        private readonly List<Tag> DefaultTags = new List<Tag>();
        private readonly List<Tag> OnDemandTags = new List<Tag>();

        private readonly TagListViewColumnSorter DefaultListSorter = new TagListViewColumnSorter(); 
        private readonly TagListViewColumnSorter OnDemandListSorter = new TagListViewColumnSorter();

        public List<Tag> SelectedTags
        {
            get
            {
                var tags = new List<Tag>();
                tags.AddRange(UIUtility.GetCheckedTagsFromListView(lvwDefault));
                tags.AddRange(UIUtility.GetCheckedTagsFromListView(lvwOnDemand));
                return tags;
            }
        }

        public UpdateOutput(List<Tag> tags)
        {
            Tags = tags;
            InitializeComponent();
            UIUtility.ScaleFont(this);
            UIUtility.SetDialogTitle(this);
        }

        private void ToggleList(ListView box, bool value)
        {
            for (int index = 0; index < box.Items.Count; index++)
            {
                box.Items[index].Checked = value;
            }
        }

        private void cmdOnDemandSelectAll_Click(object sender, EventArgs e)
        {
            ToggleList(lvwOnDemand, true);
        }

        private void cmdOnDemandSelectNone_Click(object sender, EventArgs e)
        {
            ToggleList(lvwOnDemand, false);
        }

        private void cmdDefaultSelectAll_Click(object sender, EventArgs e)
        {
            ToggleList(lvwDefault, true);
        }

        private void cmdDefaultSelectNone_Click(object sender, EventArgs e)
        {
            ToggleList(lvwDefault, false);
        }

        private void UpdateOutput_Load(object sender, EventArgs e)
        {
            if (Tags == null)
            {
                return;
            }

            foreach (var tag in Tags)
            {
                if (tag.RunFrequency.Equals(Constants.RunFrequency.Always))
                {
                    DefaultTags.Add(tag);
                }
                else
                {
                    OnDemandTags.Add(tag);
                }
            }

            lvwDefault.ListViewItemSorter = DefaultListSorter;
            lvwOnDemand.ListViewItemSorter = OnDemandListSorter;

            LoadOnDemandList();
            LoadDefaultList();
        }

        private void LoadOnDemandList(string filter = "")
        {
            LoadList(OnDemandTags, lvwOnDemand, false, filter);
        }

        private void LoadDefaultList(string filter = "")
        {
            LoadList(DefaultTags, lvwDefault, true, filter);
        }

        private void LoadList(List<Tag> tags, ListView listView, bool checkItem, string filter = "")
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                listView.Items.Clear();

                var filteredTags = tags.Where(x => x.Name.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0).OrderBy(x => x.LineStart);
                foreach (var tag in filteredTags)
                {
                    var item = listView.Items.Add(tag.Name);
                    item.SubItems.AddRange(new[] { tag.CodeFile.FilePath });
                    item.Tag = tag;
                    item.Checked = checkItem;
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void txtOnDemandFilter_FilterChanged(object sender, EventArgs e)
        {
            LoadOnDemandList(txtOnDemandFilter.Text);
        }

        private void txtDefaultFilter_FilterChanged(object sender, EventArgs e)
        {
            LoadDefaultList(txtDefaultFilter.Text);
        }

        private void lvwOnDemand_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            OnDemandListSorter.HandleSort(e.Column, lvwOnDemand);
        }

        private void lvwDefault_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            DefaultListSorter.HandleSort(e.Column, lvwDefault);
        }
    }
}
