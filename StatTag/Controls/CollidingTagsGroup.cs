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
    public sealed partial class CollidingTagsGroup : UserControl
    {
        public List<Tag> Tags { get; set; }

        private readonly ScintillaEditorPopover scintillaPopOver = new ScintillaEditorPopover();

        public CollidingTagsGroup()
        {
            InitializeComponent();
            this.AutoSize = true;
            lvwTags.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private class TagListItem
        {
            public Tag Tag { get; set; }
            public TagListItem(Tag tag) { Tag = tag; }
            public override string ToString()
            {
                return string.Format("{0} (Lines {1})", Tag.Name, Tag.FormatLineNumberRange());
            }
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

                cboKeepTag.Items.Add(new TagListItem(tag));
            }
            cboKeepTag.SelectedIndex = cboKeepTag.Items.Count - 1;
            lvwTags.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvwTags.Height = height + lvwTags.Margin.Top + lvwTags.Margin.Bottom + 35;
            this.Height = lvwTags.Height + this.Margin.Top + this.Margin.Bottom;
        }

        public Tag GetTagToKeep()
        {
            var selectedItem = cboKeepTag.SelectedItem as TagListItem;
            return (selectedItem == null) ? null : selectedItem.Tag;
        }

        private void cmdPeek_MouseClick(object sender, MouseEventArgs e)
        {
            if (Tags == null || Tags.Count == 0)
            {
                return;
            }

            int? startLine = Tags.Min(x => x.LineStart);
            int? endLine = Tags.Max(x => x.LineEnd);
            if (!startLine.HasValue || !endLine.HasValue)
            {
                return;
            }

            scintillaPopOver.ShowCodeFileLines(Tags.First().CodeFile,startLine.Value, endLine.Value);
            scintillaPopOver.Show(this, e.Location);
        }
    }
}
