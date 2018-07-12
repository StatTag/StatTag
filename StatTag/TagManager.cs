﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using StatTag.Core.Exceptions;
using StatTag.Core.Models;
using StatTag.Models;

namespace StatTag
{
    public partial class TagManager : Form
    {
        private List<Tag> Tags { get; set; }
        private readonly List<Tag> FilteredTags = new List<Tag>();
        private int sortColumn = -1;
        private bool sortAscending = true;
        public DocumentManager Manager { get; set; }

        private const int InnerPadding = 3;
        private const int ItemHeight = 60;
        private const int NumberRows = 3;
        private const int RowHeight = (int)(ItemHeight * (1.0 / NumberRows));
        private const string DefaultFormat = "Default";
        private const string DefaultPreviewText = "(Exactly as Generated)";
        private const int TagTypeImageDimension = 32;

        private static readonly Brush AlternateBackgroundBrush = new SolidBrush(Color.FromArgb(255, 245, 245, 245));
        private static readonly Brush HighlightBrush = new SolidBrush(Color.FromArgb(255, 17, 108, 214));
        private static readonly Font NameFont = new Font("Segoe UI", 10.0F, FontStyle.Bold);
        private static readonly Font NormalFont = new Font(NameFont, FontStyle.Regular);
        private static readonly Font SubFont = new Font(NameFont.FontFamily, 8.0F, FontStyle.Regular);
        private static readonly StringFormat TextFormat = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Near,
            Trimming = StringTrimming.EllipsisCharacter,
            FormatFlags = StringFormatFlags.NoWrap
        };

        private static readonly Dictionary<string, Bitmap> StatPackageImages = new Dictionary<string, Bitmap>()
        {
            { Constants.StatisticalPackages.R, new Bitmap(StatTag.Properties.Resources.stats_package_r) },
            { Constants.StatisticalPackages.SAS, new Bitmap(StatTag.Properties.Resources.stats_package_sas) },
            { Constants.StatisticalPackages.Stata, new Bitmap(StatTag.Properties.Resources.stats_package_stata) },
            { string.Empty, new Bitmap(StatTag.Properties.Resources.stats_package_unknown) }
        };

        private static readonly Dictionary<string, Bitmap> TagTypeImages = new Dictionary<string, Bitmap>()
        {
            {Constants.TagType.Figure, new Bitmap(StatTag.Properties.Resources.figure_preview)},
            {Constants.TagType.Table, new Bitmap(StatTag.Properties.Resources.table_preview)},
            {Constants.TagType.Value, new Bitmap(1,1)},
            {Constants.TagType.Verbatim, new Bitmap(1,1)},
            {string.Empty, new Bitmap(1,1)}
        };


        public TagManager(List<Tag> tags, DocumentManager manager)
        {
            InitializeComponent();
            lvwTags.View = View.Details;  // It must always be Details
            Tags = tags;
            Manager = manager;

            if (Tags != null)
            {
                cboCodeFiles.Items.Add("(Tags for all code files)");
                cboCodeFiles.Items.AddRange(Tags.GroupBy(t => t.CodeFilePath).Select(path => Path.GetFileName(path.Key)).ToArray());
                cboCodeFiles.SelectedIndex = 0;
            }
        }

        private void TagManager_Load(object sender, EventArgs e)
        {
            var imgList = new ImageList {ImageSize = new Size(1, ItemHeight)};
            lvwTags.SmallImageList = imgList;
            FilterTags();
        }

        private void FilterTags()
        {
            FilteredTags.Clear();
            lvwTags.Items.Clear();

            bool allCodeFiles = (cboCodeFiles.SelectedIndex == 0);
            bool noFilter = string.IsNullOrWhiteSpace(txtFilter.Text);
            var filter = txtFilter.Text;
            if (Tags != null)
            {
                FilteredTags.AddRange(Tags.Where(x => 
                    (allCodeFiles || x.CodeFilePath.EndsWith(cboCodeFiles.SelectedItem.ToString())) 
                    && (noFilter || x.Name.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0)).ToArray());
            }

            foreach (var tag in FilteredTags)
            {
                lvwTags.Items.Add(new ListViewItem(new string[] { tag.CodeFile.StatisticalPackage, tag.Name, tag.Type }) { Tag = tag });
            }

            UpdateUIForSelection();
        }

        //private static bool IsSelected(ListViewItemStates state)
        //{
        //    return ((state & ListViewItemStates.Selected) != 0);
        //}

        private Bitmap GetStatPackageImage(CodeFile file)
        {
            if (file != null && StatPackageImages.ContainsKey(file.StatisticalPackage))
            {
                return StatPackageImages[file.StatisticalPackage];
            }

            return StatPackageImages[string.Empty];
        }

        private string GenerateFormatDescriptionFromTag(Tag tag)
        {
            if (tag == null)
            {
                return DefaultFormat;
            }

            switch (tag.Type)
            {
                case Constants.TagType.Verbatim:
                case Constants.TagType.Figure:
                    return DefaultFormat;
                case Constants.TagType.Table:
                case Constants.TagType.Value:
                    if (tag.ValueFormat == null)
                    {
                        return DefaultFormat;
                    }

                    return tag.ValueFormat.FormatType;
                default:
                    return DefaultFormat;
            }
        }

        private string GeneratePreviewTextFromTag(Tag tag)
        {
            if (tag == null)
            {
                return string.Empty;
            }

            switch (tag.Type)
            {
                case Constants.TagType.Verbatim:
                case Constants.TagType.Figure:
                    return DefaultPreviewText;
                case Constants.TagType.Table:
                case Constants.TagType.Value:
                    if (tag.ValueFormat == null)
                    {
                        return DefaultPreviewText;
                    }

                    switch (tag.ValueFormat.FormatType)
                    {
                        case Constants.ValueFormatType.DateTime:
                            return tag.ValueFormat.Format("January 24, 1984 19:30:50");
                        case Constants.ValueFormatType.Numeric:
                            return tag.ValueFormat.Format("100000");
                        case Constants.ValueFormatType.Percentage:
                            return tag.ValueFormat.Format("1");
                        default:
                            return DefaultPreviewText;
                    }
                default:
                    return string.Empty;
            }
        }

        private void lvwTags_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ItemIndex < 0)
            {
                return;
            }

            // Have to look at Item.Selected status when HideSelection is false - see https://stackoverflow.com/a/2394142
            var isSelected = e.Item.Selected;
            if (isSelected)
            {
                e.Graphics.FillRectangle(HighlightBrush, e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(e.ItemIndex%2 == 0 ? Brushes.White : AlternateBackgroundBrush, e.Bounds);
            }

            var topRow = e.Bounds;
            topRow.Offset(InnerPadding, InnerPadding);
            topRow.Height = RowHeight;
            var row2 = new Rectangle(e.Bounds.X + InnerPadding, topRow.Y + topRow.Height, topRow.Width, RowHeight);
            var row3 = new Rectangle(e.Bounds.X + InnerPadding, row2.Y + row2.Height, topRow.Width, RowHeight);
            var tag = e.Item.Tag as Tag;
            if (tag == null)
            {
                return;
            }

            //var isSelected = IsSelected(e.ItemState);
            //var isSelected = e.Item.Selected;
            switch (e.ColumnIndex)
            {
                case 0:
                    var image = GetStatPackageImage(tag.CodeFile);
                    var rect = e.Bounds;
                    rect.Offset((rect.Width - image.Width) / 2, (rect.Height - image.Height) / 2);
                    e.Graphics.DrawImageUnscaled(image, rect);
                    break;
                case 1:
                    e.Graphics.DrawString(tag.Name, NameFont, (isSelected ? Brushes.White : Brushes.Black), topRow, TextFormat);
                    e.Graphics.DrawString(Path.GetFileName(tag.CodeFilePath), SubFont, Brushes.Gray, row2, TextFormat);
                    break;
                case 2:
                    e.Graphics.DrawString(tag.Type, NormalFont, (isSelected ? Brushes.White : Brushes.Black), topRow, TextFormat);
                    e.Graphics.DrawString(GenerateFormatDescriptionFromTag(tag), SubFont, Brushes.Gray, row2, TextFormat);
                    e.Graphics.DrawString(GeneratePreviewTextFromTag(tag), SubFont, Brushes.Gray, row3, TextFormat);
                    var typeImageRect = e.Bounds;
                    typeImageRect.Offset((e.Bounds.Width - TagTypeImageDimension - (2 * InnerPadding)),
                        (e.Bounds.Height - TagTypeImageDimension) / 2);
                    typeImageRect.Width = TagTypeImageDimension;
                    typeImageRect.Height = TagTypeImageDimension;
                    e.Graphics.DrawImage(TagTypeImages[tag.Type], typeImageRect);
                    break;
            }
        }

        private void cboCodeFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterTags();
        }

        private void lvwTags_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            // Code derived from https://msdn.microsoft.com/en-us/library/system.windows.forms.listview.drawcolumnheader.aspx
            using (var format = new StringFormat())
            {
                switch (e.Header.TextAlign)
                {
                    case HorizontalAlignment.Center:
                        format.Alignment = StringAlignment.Center;
                        break;
                    case HorizontalAlignment.Right:
                        format.Alignment = StringAlignment.Far;
                        break;
                }

                format.LineAlignment = StringAlignment.Center;

                // Draw the standard header background.
                e.DrawBackground();

                // Draw the header text.
                e.Graphics.DrawString(e.Header.Text, NormalFont, Brushes.Black, e.Bounds, format);
            }
        }

        private void lvwTags_DoubleClick(object sender, EventArgs e)
        {
            if (lvwTags.SelectedItems.Count == 0)
            {
                return;
            }

            int firstSelectedIndex = lvwTags.SelectedItems[0].Index;
            var existingTag = lvwTags.Items[firstSelectedIndex].Tag as Tag;
            try
            {
                this.TopMost = false;
                this.Visible = false;
                if (Manager.EditTag(existingTag))
                {
                    //ReloadTags();
                    lvwTags.Refresh();
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            finally
            {
                this.TopMost = true;
                this.Visible = true;
            }
        }

        private void lvwTags_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUIForSelection();
        }

        private void UpdateUIForSelection()
        {
            int selectionCount = lvwTags.SelectedItems.Count;
            bool hasSelection = (selectionCount > 0);
            cmdInsert.Enabled = hasSelection;
            cmdUpdate.Enabled = hasSelection;
            cmdInsert.Text = string.Format("Insert {0} Selected Tag{1}", selectionCount, (selectionCount > 1 ? "s" : ""));
            cmdUpdate.Text = string.Format("Update {0} Selected Tag{1}", selectionCount, (selectionCount > 1 ? "s" : ""));
        }

        protected class TagListViewItemComparer : IComparer
        {
            private readonly int sortColumn = -1;
            private readonly bool sortAscending = true;

            public TagListViewItemComparer(int column, bool ascending)
            {
                sortColumn = column;
                sortAscending = ascending;
            }

            public int Compare(object x, object y)
            {
                var firstItem = x as ListViewItem;
                var secondItem = y as ListViewItem;
                int value = 0;
                if (firstItem == null && secondItem == null)
                {
                    value = 0;
                }
                else if (firstItem == null)
                {
                    value = 1;
                }
                else if (secondItem == null)
                {
                    value = -1;
                }
                else
                {
                    value = string.Compare(firstItem.SubItems[sortColumn].Text, secondItem.SubItems[sortColumn].Text, StringComparison.CurrentCulture);
                }

                return (sortAscending ? value : (-1*value));
            }
        }

        private void lvwTags_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == sortColumn)
            {
                sortAscending = !sortAscending;
            }
            else
            {
                sortColumn = e.Column;
                sortAscending = true;
            }

            lvwTags.ListViewItemSorter = new TagListViewItemComparer(e.Column, sortAscending);
            lvwTags.Sort();
        }

        /// <summary>
        /// Helper function to return the list of Tag objects that are represented by selections
        /// within the ListView
        /// </summary>
        /// <returns></returns>
        private List<Tag> GetSelectedTags()
        {
            var selectedItems = lvwTags.SelectedItems;
            return selectedItems.OfType<ListViewItem>().Select(x => x.Tag as Tag).Where(x => x != null).ToList();
        }

        /// <summary>
        /// Handler to insert selected tags at the current position within the document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdInsert_Click(object sender, EventArgs e)
        {
            try
            {
                var tags = GetSelectedTags();
                Manager.Logger.WriteMessage(string.Format("Inserting {0} selected tags", tags.Count));
                Manager.InsertTagsInDocument(tags);
            }
            catch (StatTagUserException uex)
            {
                UIUtility.ReportException(uex, uex.Message, Manager.Logger);
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when trying to insert the tag output into the document.",
                    Manager.Logger);
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
                Cursor.Current = Cursors.Default;
            }
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                var tags = GetSelectedTags();

                Cursor.Current = Cursors.WaitCursor;
                Globals.ThisAddIn.Application.ScreenUpdating = false;

                // First, go through and update all of the code files to ensure we have all
                // refreshed tags.
                var refreshedFiles = new List<CodeFile>();
                //var files = Manager.GetCodeFileList(ActiveDocument);
                var files = tags.Select(x => x.CodeFile).Distinct();
                foreach (var codeFile in files)
                {
                    if (!refreshedFiles.Contains(codeFile))
                    {
                        var result = Manager.StatsManager.ExecuteStatPackage(codeFile, Constants.ParserFilterMode.TagList, tags);
                        if (!result.Success)
                        {
                            break;
                        }

                        refreshedFiles.Add(codeFile);
                    }
                }

                // Now we will refresh all of the tags that are fields.  Since we most likely
                // have more fields than tags, we are going to use the approach of looping
                // through all fields and updating them (via the DocumentManager).
                Manager.UpdateFields();
            }
            catch (StatTagUserException uex)
            {
                UIUtility.ReportException(uex, uex.Message, Manager.Logger);
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when trying to update values in your document.",
                    Manager.Logger);
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
                Cursor.Current = Cursors.Default;
            }
        }

        //private void LoadList(string filter = "")
        //{
        //    dgvItems.Rows.Clear();
        //    var filteredTags = Tags.Where(x => x.Name.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0).OrderBy(x => x.LineStart);
        //    foreach (var tag in filteredTags)
        //    {
        //        AddRow(tag);
        //    }

        //    // If the list was sorted explicitly, retain that sort order after we've refreshed the data
        //    if (dgvItems.SortedColumn != null)
        //    {
        //        var sortOrder = (dgvItems.SortOrder == SortOrder.Descending)
        //            ? ListSortDirection.Descending
        //            : ListSortDirection.Ascending;
        //        dgvItems.Sort(dgvItems.SortedColumn, sortOrder);
        //    }
        //}

        private void txtFilter_FilterChanged(object sender, EventArgs e)
        {
            //LoadList(txtFilter.Text);
            FilterTags();
        }
    }
}
