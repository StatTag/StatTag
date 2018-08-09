﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using StatTag.Core.Exceptions;
using StatTag.Core.Models;
using StatTag.Models;
using Font = System.Drawing.Font;
using Rectangle = System.Drawing.Rectangle;
using View = System.Windows.Forms.View;

namespace StatTag
{
    public partial class TagManager : Form
    {
        // These internal members are used to track if and when we have child forms
        // open.  This will allow us to manage closing them if a code file changes
        private EditTag EditTagForm { get; set; }

        private readonly List<Tag> FilteredTags = new List<Tag>();
        public DocumentManager Manager { get; set; }
        public Document ActiveDocument { get; set; }

        private const int InnerPadding = 3;
        private const int ItemHeight = 60;
        private const int NumberRows = 3;
        private const int RowHeight = (int)(ItemHeight * (1.0 / NumberRows));
        private const string DefaultFormat = "Default";
        private const string DefaultPreviewText = "(Exactly as Generated)";
        private const int TagTypeImageDimension = 32;
        private const int TagAlertImageDimension = 16;
        private const string BaseDialogTitle = "StatTag - Tag Manager";
        private const string DuplicateTagIndicator = "StatTag|DuplicateTag";

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

        private static readonly Bitmap TagAlertImage = new Bitmap(StatTag.Properties.Resources.warning);


        private readonly TagListViewColumnSorter ListViewSorter = new TagListViewColumnSorter();
        private ExecutionProgressForm CurrentProgress;

        public TagManager(DocumentManager manager)
        {
            InitializeComponent();
            lvwTags.View = View.Details;  // It must always be Details
            lvwTags.ListViewItemSorter = ListViewSorter;
            Manager = manager;

            if (Manager != null)
            {
                SetDialogTitle();
                LoadCodeFileList(this);
                Manager.TagListChanged += ManagerOnTagListChanged;
                Manager.CodeFileListChanged += ManagerOnCodeFileListChanged;
                Manager.ActiveDocumentChanged += ManagerOnActiveDocumentChanged;
                Manager.EditingTag += ManagerOnEditingTag;
                Manager.EditedTag += ManagerOnEditedTag;
            }
        }

        /// <summary>
        /// Solution from: https://stackoverflow.com/a/8849636/
        /// </summary>
        /// <param name="lvwList"></param>
        /// <returns></returns>
        private delegate void SelectListItem(ListView lvwList, Tag tag);
        private void SelectTagInList(ListView lvwList, Tag tag)
        {
            if (!lvwList.InvokeRequired)
            {
                var items = lvwList.Items.OfType<ListViewItem>();
                var selectableItem = items.FirstOrDefault(x => x.Tag.Equals(tag));
                if (selectableItem != null)
                {
                    lvwList.SelectedItems.Clear();
                    selectableItem.Selected = true;
                }
            }
            else
            {
                this.Invoke(new SelectListItem(SelectTagInList), new object[] { lvwList, tag });
            }
        }

        private delegate void ManageVisibility(Form form, bool visible);
        private void SetTagManagerVisibility(Form form, bool visible)
        {
            if (!form.InvokeRequired)
            {
                if (!form.IsDisposed)
                {
                    form.Visible = visible;
                }
            }
            else
            {
                this.Invoke(new ManageVisibility(SetTagManagerVisibility), new object[] { form, visible });
            }
        }

        private void ManagerOnEditingTag(object sender, DocumentManager.TagEventArgs tagEventArgs)
        {
            SetTagManagerVisibility(this, false);
        }

        private void ManagerOnEditedTag(object sender, DocumentManager.TagEventArgs tagEventArgs)
        {
            SetTagManagerVisibility(this, true);
            SelectTagInList(lvwTags, tagEventArgs.Tag);
        }

        /// <summary>
        /// Event handler called when the active Word document changes.  Our response is to update
        /// the document title, code file list and tag list to reflect the new document's metadata.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ManagerOnActiveDocumentChanged(object sender, EventArgs eventArgs)
        {
            SetDialogTitle();
            LoadCodeFileList(this);
            FilterTags(this);
        }

        /// <summary>
        /// Update the title of the dialog to reflect the currently active document name
        /// </summary>
        private void SetDialogTitle()
        {
            var activeDocument = Manager.ActiveDocument;
            Text = string.Format("{0}{1}", BaseDialogTitle,
                (activeDocument != null) ? string.Format(" - {0}", activeDocument.FullName) : string.Empty);
        }

        /// <summary>
        /// Load the list of code files (used for filtering tags) from our DocumentManager reference
        /// </summary>
        private delegate void CodeFileListDelegate(StatTag.TagManager form);
        private void LoadCodeFileList(StatTag.TagManager form)
        {
            if (!form.InvokeRequired)
            {
                if (Manager != null)
                {
                    var selectedItem = string.Empty;
                    if (cboCodeFiles.SelectedIndex >= 0 && cboCodeFiles.SelectedItem != null)
                    {
                        selectedItem = cboCodeFiles.SelectedItem.ToString();
                    }

                    var codeFiles = Manager.GetCodeFileList();
                    cboCodeFiles.Items.Clear();
                    cboCodeFiles.Items.Add("(Tags for all code files)");
                    cboCodeFiles.Items.AddRange(codeFiles.Select(path => Path.GetFileName(path.FilePath)).ToArray());

                    if (string.IsNullOrWhiteSpace(selectedItem))
                    {
                        cboCodeFiles.SelectedIndex = 0;
                    }
                    else
                    {
                        cboCodeFiles.SelectedItem = selectedItem;

                        // If the selection we tried is no longer valid, default to the "all tags" filter selection
                        if (cboCodeFiles.SelectedItem == null)
                        {
                            cboCodeFiles.SelectedIndex = 0;
                        }
                    }
                }
            }
            else
            {
                this.Invoke(new CodeFileListDelegate(LoadCodeFileList), new object[] { form });
            }
        }

        /// <summary>
        /// Handle notifications from the DocumentManager when our list of code files has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ManagerOnCodeFileListChanged(object sender, EventArgs eventArgs)
        {
            LoadCodeFileList(this);
            FilterTags(this);
        }

        /// <summary>
        /// Handle notifications from the DocumentManager when the list and/or content of tags has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ManagerOnTagListChanged(object sender, EventArgs eventArgs)
        {
            FilterTags(this);
        }

        private void TagManager_Load(object sender, EventArgs e)
        {
            var imgList = new ImageList {ImageSize = new Size(1, ItemHeight)};
            lvwTags.SmallImageList = imgList;
            FilterTags(this);
        }

        /// <summary>
        /// Filter the list of tags that is displayed based off of the code file filter, as well as the
        /// search string filter.
        /// </summary>
        private delegate void FilterTagsDelegate(StatTag.TagManager form);
        private void FilterTags(StatTag.TagManager form)
        {
            if (!form.InvokeRequired)
            {
                FilteredTags.Clear();
                lvwTags.Items.Clear();

                bool allCodeFiles = (cboCodeFiles.SelectedIndex == 0 || cboCodeFiles.SelectedItem == null);
                bool noFilter = string.IsNullOrWhiteSpace(txtFilter.Text);
                var filter = txtFilter.Text;
                if (Manager != null)
                {
                    var tags = Manager.GetTags();
                    FilteredTags.AddRange(tags.Where(x =>
                        (allCodeFiles || x.CodeFilePath.EndsWith(cboCodeFiles.SelectedItem.ToString()))
                        && (noFilter || x.Name.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0)).ToArray());
                }

                foreach (var tag in FilteredTags)
                {
                    var isDuplicate = (FilteredTags.Count(x => x.Id.Equals(tag.Id)) > 1);
                    lvwTags.Items.Add(new ListViewItem(new string[] { tag.CodeFile.StatisticalPackage, tag.Name, tag.Type, isDuplicate ? DuplicateTagIndicator : string.Empty }) { Tag = tag });
                }

                UpdateUIForSelection();
            }
            else
            {
                this.Invoke(new FilterTagsDelegate(FilterTags), new object[] { form });
            }
        }

        /// <summary>
        /// Helper to safely return a Bitmap to represent the statistical package used to run
        /// the CodeFile file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private Bitmap GetStatPackageImage(CodeFile file)
        {
            if (file != null && StatPackageImages.ContainsKey(file.StatisticalPackage))
            {
                return StatPackageImages[file.StatisticalPackage];
            }

            return StatPackageImages[string.Empty];
        }

        /// <summary>
        /// For a given tag, return a string that describes how it will be formatted.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
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

        /// <summary>
        /// For a given tag, return a string that represents its preview - this accounts
        /// for things like numeric formatting.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
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
                    e.Graphics.DrawString(Path.GetFileName(tag.CodeFilePath), SubFont, (isSelected ? Brushes.White : Brushes.Gray), row2, TextFormat);
                    break;
                case 2:
                    var selectedSubBrush = (isSelected ? Brushes.White : Brushes.Gray);
                    e.Graphics.DrawString(tag.Type, NormalFont, (isSelected ? Brushes.White : Brushes.Black), topRow, TextFormat);
                    e.Graphics.DrawString(GenerateFormatDescriptionFromTag(tag), SubFont, selectedSubBrush, row2, TextFormat);
                    e.Graphics.DrawString(GeneratePreviewTextFromTag(tag), SubFont, selectedSubBrush, row3, TextFormat);
                    var typeImageRect = e.Bounds;
                    typeImageRect.Offset((e.Bounds.Width - TagTypeImageDimension - (2 * InnerPadding)),
                        (e.Bounds.Height - TagTypeImageDimension) / 2);
                    typeImageRect.Width = TagTypeImageDimension;
                    typeImageRect.Height = TagTypeImageDimension;
                    e.Graphics.DrawImage(TagTypeImages[tag.Type], typeImageRect);
                    break;
                case 3:
                    if (e.SubItem.Text.Equals(DuplicateTagIndicator))
                    {
                        var imageRect = e.SubItem.Bounds;
                        imageRect.Offset(InnerPadding, InnerPadding);
                        imageRect.Height = TagAlertImageDimension;
                        imageRect.Width = TagAlertImageDimension;
                        e.Graphics.DrawImage(TagAlertImage, imageRect);
                    }
                    break;
            }
        }

        private void cboCodeFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterTags(this);
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
                e.DrawBackground();
                e.Graphics.DrawString(e.Header.Text, NormalFont, Brushes.Black, e.Bounds, format);
            }
        }

        private void lvwTags_MouseDoubleClick(object sender, MouseEventArgs e)
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

                var hit = lvwTags.HitTest(e.Location);
                if (hit.SubItem != null && hit.SubItem.Text.Equals(DuplicateTagIndicator))
                {
                    Manager.PerformDocumentCheck(Manager.ActiveDocument, false, CheckDocument.DefaultTab.DuplicateTags);
                }
                else if (Manager.EditTag(existingTag))
                {
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
            cmdRemoveTags.Enabled = hasSelection;
            cmdInsert.Text = string.Format("Insert {0} Selected Tag{1}", selectionCount, (selectionCount != 1 ? "s" : ""));
            cmdUpdate.Text = string.Format("Update {0} Selected Tag{1}", selectionCount, (selectionCount != 1 ? "s" : ""));
            cmdRemoveTags.Text = string.Format("Remove {0} Tag{1}", selectionCount, (selectionCount != 1 ? "s" : ""));
        }

        private void lvwTags_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewSorter.HandleSort(e.Column, lvwTags);
        }

        /// <summary>
        /// Helper function to return the list of Tag objects that are represented by selections
        /// within the ListView
        /// </summary>
        /// <returns></returns>
        private List<Tag> GetSelectedTags()
        {
            // When tags get inserted into the document, the cursor ends up before the newly inserted field.  I tried
            // to move the cursor after the inserted field (I promise!), but to no avail.  Our workaround so that the
            // tags are inserted in the order they are in the list is to do it in reverse order (which is why it's
            // OrderByDescending).
            var selectedItems = lvwTags.SelectedItems;
            return selectedItems.OfType<ListViewItem>()
                .OrderByDescending(x => x.Index)
                .Select(x => x.Tag as Tag)
                .Where(x => x != null).ToList();
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
                InitializeProgressDialog(insertBackgroundWorker);

                var tags = GetSelectedTags();
                insertBackgroundWorker.RunWorkerAsync(tags);
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

        private void InitializeProgressDialog(BackgroundWorker worker)
        {
            if (CurrentProgress != null && !CurrentProgress.IsDisposed)
            {
                CurrentProgress.Close();
                CurrentProgress = null;
            }

            CurrentProgress = new ExecutionProgressForm(worker);
            this.TopMost = false;
            this.Visible = false;
            CurrentProgress.TopMost = true;
            CurrentProgress.Show();
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeProgressDialog(updateBackgroundWorker);

                Cursor.Current = Cursors.WaitCursor;
                Globals.ThisAddIn.Application.ScreenUpdating = false;

                var tags = GetSelectedTags();
                updateBackgroundWorker.RunWorkerAsync(tags);
            }
            catch (StatTagUserException uex)
            {
                UIUtility.ReportException(uex, uex.Message, Manager.Logger);
                CompletedBackgroundWorker();
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when trying to update values in your document.",
                    Manager.Logger);
                CompletedBackgroundWorker();
            }
        }

        private void txtFilter_FilterChanged(object sender, EventArgs e)
        {
            FilterTags(this);
        }

        private void cmdDefineTag_Click(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = false;
                this.Visible = false;

                CodeFile lastSelectedCodeFile = null;
                bool defineAnother = false;
                do
                {
                    EditTagForm = new EditTag(true, Manager, lastSelectedCodeFile);
                    if (DialogResult.OK == EditTagForm.ShowDialog())
                    {
                        Manager.SaveEditedTag(EditTagForm);
                        defineAnother = EditTagForm.DefineAnother;
                        lastSelectedCodeFile = EditTagForm.Tag.CodeFile;
                    }
                    else
                    {
                        defineAnother = false;
                    }
                } while (defineAnother);
            }
            finally
            {
                this.Visible = true;
                this.TopMost = true;
                EditTagForm = null;
            }
        }

        private void cmdRemoveTags_Click(object sender, EventArgs e)
        {
            var removedTags = new List<Tag>();
            var selectedIndices = lvwTags.SelectedIndices;
            for (int index = (selectedIndices.Count - 1); index >= 0; index--)
            {
                removedTags.Add((Tag)lvwTags.Items[selectedIndices[index]].Tag);
                lvwTags.Items.RemoveAt(selectedIndices[index]);
            }

            Manager.RemoveTags(removedTags);
        }

        /// <summary>
        /// Called when we need to forcibly close any and all open dialogs, such as
        /// for system shutdown, or when a linked code file has been changed.
        /// </summary>
        public void CloseAllChildDialogs()
        {
            UIUtility.ManageCloseDialog(EditTagForm);
            Manager.CloseAllChildDialogs();
        }

        private void cmdCheckUnlinkedTags_Click(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = false;
                this.Visible = false;

                Manager.PerformDocumentCheck(Manager.ActiveDocument);
            }
            finally
            {
                this.Visible = true;
                this.TopMost = true;
                EditTagForm = null;
            }
        }

        private void TagManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Manager != null)
            {
                Manager.TagListChanged -= ManagerOnTagListChanged;
                Manager.CodeFileListChanged -= ManagerOnCodeFileListChanged;
                Manager.ActiveDocumentChanged -= ManagerOnActiveDocumentChanged;
                Manager.EditingTag -= ManagerOnEditingTag;
                Manager.EditedTag -= ManagerOnEditedTag;
            }
        }

        /// <summary>
        /// Needed to handle Ctrl+A as a select-all hotkey
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwTags_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control)
            {
                foreach (ListViewItem item in lvwTags.Items)
                {
                    item.Selected = true;
                }
            }
        }

        /// <summary>
        /// General event handler for all background processors to update the progress dialog (if it's shown)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (CurrentProgress != null)
            {
                CurrentProgress.UpdateProgress(e.ProgressPercentage, e.UserState.ToString());
            }
        }

        /// <summary>
        /// Event handler started for the background worker that updates inserted tags within the Word document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // First, go through and update all of the code files to ensure we have all
            // refreshed tags.
            var worker = (BackgroundWorker) sender;
            var progressReporter = new BackgroundWorkerProgressReporter(worker);
            var tags = (List<Tag>)e.Argument;
            var refreshedFiles = new List<CodeFile>();
            var files = tags.Select(x => x.CodeFile).Distinct().ToArray();
            int length = files.Length;
            for (int index = 0; index < length; index++)
            {
                // If the user cancels while running code files, exit right away so we don't do any of
                // the later tag processing/updates.
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                var codeFile = files[index]; 
                int progressBefore = (int)((((index * 1.0 + 1) / length) / 2.0) * 100);
                worker.ReportProgress(progressBefore, string.Format("Running '{0}' (code file {1} of {2})", Path.GetFileName(codeFile.FilePath), (index + 1), length));

                if (!refreshedFiles.Contains(codeFile))
                {
                    var result = Manager.StatsManager.ExecuteStatPackage(codeFile, Constants.ParserFilterMode.TagList, tags);
                    if (!result.Success)
                    {
                        throw new StatTagUserException(result.ErrorMessage);
                    }

                    refreshedFiles.Add(codeFile);
                }

                int progressAfter = (int)(((index * 1.0 + 1) / length) * 100);
                worker.ReportProgress(progressAfter, string.Format("Completed code file {0} of {1}", (index + 1), length));
            }

            // Now we will refresh all of the tags that are fields.  Since we most likely
            // have more fields than tags, we are going to use the approach of looping
            // through all fields and updating them (via the DocumentManager).
            Manager.UpdateFields(tags, progressReporter);
            e.Cancel = worker.CancellationPending;
        }

        /// <summary>
        /// General event handler called when a background worker has completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            CompletedBackgroundWorker(e.Error, e.Cancelled);
        }

        /// <summary>
        /// Helper function to manage cleanup & completion after a background worker is done.
        /// This resets the state of dialogs/forms, and handles any exception reporting from the work.
        /// </summary>
        /// <param name="exception"></param>
        private void CompletedBackgroundWorker(Exception exception = null, bool cancelled = false)
        {
            if (CurrentProgress != null)
            {
                CurrentProgress.Close();
                CurrentProgress = null;
            }

            if (exception != null)
            {
                UIUtility.ReportException(exception, exception.Message, Manager.Logger);
            }

            if (cancelled)
            {
                Manager.Logger.WriteMessage("The code execution was cancelled by the user");
                UIUtility.WarningMessageBox("You have cancelled updates.\r\n\r\nThe values in your document may not be accurate until you update all tags.", Manager.Logger);
            }

            this.TopMost = true;
            this.Visible = true;
            Globals.ThisAddIn.Application.ScreenUpdating = true;
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Event handler when the background worker for inserting tag placeholders is invoked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void insertBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            var progressReporter = new BackgroundWorkerProgressReporter(worker);
            var tags = (List<Tag>)e.Argument;
            Manager.Logger.WriteMessage(string.Format("Inserting {0} selected tags", tags.Count));
            Manager.InsertTagsInDocument(tags, true, progressReporter);
            e.Cancel = worker.CancellationPending;
        }
    }
}
