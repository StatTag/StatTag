using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StatTag.Core.Models;

namespace StatTag.Controls
{
    public partial class CollidingTagsGroup : UserControl
    {
        public List<Tag> Tags { get; set; } 

        public CollidingTagsGroup()
        {
            InitializeComponent();
            this.AutoSize = true;
            lvwTags.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        public void SetData(List<Tag> collidingTags)
        {
            Tags = collidingTags;

            lvwTags.Items.Clear();
            cboKeepTag.Items.Clear();

            if (collidingTags == null || collidingTags.Count == 0)
            {
                return;
            }

            int height = 0;
            foreach (var tag in collidingTags)
            {
                var item = lvwTags.Items.Add(tag.Name);
                item.SubItems.Add(tag.FormatLineNumberRange());
                height += item.Bounds.Height;

                cboKeepTag.Items.Add(tag.Name);
            }
            cboKeepTag.SelectedIndex = cboKeepTag.Items.Count - 1;
            lvwTags.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvwTags.Height = height + lvwTags.Margin.Top + lvwTags.Margin.Bottom + 35;
            this.Height = lvwTags.Height + this.Margin.Top + this.Margin.Bottom;
        }

        public Tag GetTagToKeep()
        {
            var selectedItem = cboKeepTag.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedItem))
            {
                return null;
            }

            return Tags.FirstOrDefault(x => x.Name.Equals(selectedItem));
        }
    }
}
