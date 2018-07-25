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
    public sealed partial class CheckDocument : Form
    {
        public enum DefaultTab
        {
            FirstWithData,
            UnlinkedTags,
            DuplicateTags
        }

        /// <summary>
        /// The collection of actions that the user has indicated they wish to proceed with.
        /// </summary>
        public Dictionary<string, CodeFileAction> UnlinkedTagUpdates { get; set; }

        /// <summary>
        /// The collection of updates that should be applied to uniquely name tags across
        /// code files.
        /// </summary>
        public List<UpdatePair<Tag>> DuplicateTagUpdates { get; set; }

        /// <summary>
        /// Used as input, this is the list of tags that are not fully linked to the
        /// current document, organized by the missing code file path.
        /// </summary>
        public Dictionary<string, List<Tag>> UnlinkedTags;

        /// <summary>
        /// Used as input, this is the list of code files that contain tags with
        /// duplicate names.
        /// </summary>
        public DuplicateTagResults DuplicateTags { get; set; }

        /// <summary>
        /// If there are unlinked tags, this will be a list of the unique code files that
        /// were found to have unlinked tags.
        /// </summary>
        public List<string> UnlinkedAffectedCodeFiles { get; set; } 

        private readonly List<CodeFile> Files;
        private DefaultTab InitDefaultTab { get; set; }

        private const int ColUnlinkedTag = 0;
        private const int ColUnlinkedCodeFile = 1;
        private const int ColUnlinkedActionToTake = 2;

        private const int ColOriginalTag = 0;
        private const int ColOriginalLineNumbers = 1;
        private const int ColDuplicateTag = 2;
        private const int ColDuplicateLineNumbers = 3;

        private bool IsSelectingFile = false;

        private class DuplicateTagPair
        {
            public Tag First { get; set; }
            public Tag Second { get; set; }
        }

        public CheckDocument(Dictionary<string, List<Tag>> unlinkedTags, DuplicateTagResults duplicateTags, List<CodeFile> files, DefaultTab defaultTab)
        {
            InitializeComponent();
            UIUtility.ScaleFont(this);
            MinimumSize = Size;
            UnlinkedTags = unlinkedTags;
            DuplicateTags = duplicateTags;
            Files = files;
            UnlinkedTagUpdates = new Dictionary<string, CodeFileAction>();
            DuplicateTagUpdates = new List<UpdatePair<Tag>>();
            UIUtility.SetDialogTitle(this);
            InitDefaultTab = defaultTab;
        }

        private void CheckDocument_Load(object sender, EventArgs e)
        {
            int? defaultTab = null;

            // Track the tags as we add them to the list, so we only display each unique
            // tag one time.  The reason we have duplicates is that this list comes from
            // the fields in a document, where multiple instances of the same tag could
            // exist.s
            var addedTags = new List<string>();
            foreach (var item in UnlinkedTags)
            {
                foreach (var tag in item.Value)
                {
                    if (!addedTags.Contains(tag.Id))
                    {
                        int row = dgvUnlinkedTags.Rows.Add(new object[] { tag.Name, item.Key });
                        dgvUnlinkedTags.Rows[row].Tag = tag;
                        addedTags.Add(tag.Id);
                    }
                }
            }

            if (dgvUnlinkedTags.RowCount > 0)
            {
                defaultTab = 0;
                tabUnlinked.Text += string.Format(" ({0})", dgvUnlinkedTags.RowCount);
            }

            foreach (var item in DuplicateTags)
            {
                foreach (var result in item.Value)
                {
                    foreach (var duplicate in result.Value)
                    {
                        int row = dgvDuplicateTags.Rows.Add(new object[]
                        {
                            result.Key.Name, result.Key.FormatLineNumberRange(),
                            duplicate.Name, duplicate.FormatLineNumberRange()
                        });
                        dgvDuplicateTags.Rows[row].Tag = new DuplicateTagPair()
                        {
                            First = result.Key,
                            Second = duplicate
                        };
                    }
                }
            }

            if (dgvDuplicateTags.RowCount > 0)
            {
                // If the default tab wasn't already assigned, then we will update it.  Since
                // this is the second tab, we don't want to overwrite the default tab if it
                // is supposed to be the first tab.  Otherwise the second tab shows as selected
                // when we want to select the first tab with data.
                if (!defaultTab.HasValue)
                {
                    defaultTab = 1;
                }
                tabDuplicate.Text += string.Format(" ({0})", dgvDuplicateTags.RowCount);
            }

            // If a default tab hasn't been assigned (based on the first to have data), then we need
            // to check if we were initialized to show a specific tab.
            if ((InitDefaultTab != DefaultTab.FirstWithData))
            {
                switch (InitDefaultTab)
                {
                    case DefaultTab.UnlinkedTags:
                        defaultTab = 0;
                        break;
                    case DefaultTab.DuplicateTags:
                        defaultTab = 1;
                        break;
                }
            }

            if (defaultTab.HasValue)
            {
                tabResults.SelectTab(defaultTab.Value);                
            }

            UIUtility.BuildCodeFileActionColumn(Files, dgvUnlinkedTags, ColUnlinkedActionToTake, true);
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            // Make sure we pick up any changes that the data grid view hasn't seen yet
            dgvUnlinkedTags.EndEdit();
            dgvDuplicateTags.EndEdit();
            UnlinkedAffectedCodeFiles = new List<string>();

            // Iterate the list of unlinked tags and determine the actions that we
            // should be taking.
            foreach (var row in dgvUnlinkedTags.Rows.OfType<DataGridViewRow>())
            {
                var fileCell = row.Cells[ColUnlinkedCodeFile] as DataGridViewTextBoxCell;
                var actionCell = row.Cells[ColUnlinkedActionToTake] as DataGridViewComboBoxCell;
                if (actionCell == null || fileCell == null)
                {
                    continue;
                }

                var tag = row.Tag as Tag;
                if (tag != null && !UnlinkedTagUpdates.ContainsKey(tag.Id))
                {
                    UnlinkedTagUpdates.Add(tag.Id, actionCell.Value as CodeFileAction);
                }

                if (!UnlinkedAffectedCodeFiles.Contains(fileCell.Value.ToString()))
                {
                    UnlinkedAffectedCodeFiles.Add(fileCell.Value.ToString());
                }
            }

            // Iterate the list of duplicate named tags and build the list of name changes
            // that are needed.
            foreach (var row in dgvDuplicateTags.Rows.OfType<DataGridViewRow>())
            {
                var duplicatePair = row.Tag as DuplicateTagPair;
                if (duplicatePair != null)
                {
                    DuplicateTagUpdates.Add(GetDuplicateUpdate(row, duplicatePair.First, ColOriginalTag));
                    DuplicateTagUpdates.Add(GetDuplicateUpdate(row, duplicatePair.Second, ColDuplicateTag));
                }
            }

            DuplicateTagUpdates = DuplicateTagUpdates.Where(x => x != null).ToList();
        }

        /// <summary>
        /// Loop through the list of tags that are duplicates, and see what the recommended action is
        /// for each. Build the appropriate update list depending on if one tag (or both) changed in the
        /// pair.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="tag"></param>
        /// <param name="textColumnIndex"></param>
        /// <returns></returns>
        private UpdatePair<Tag> GetDuplicateUpdate(DataGridViewRow row, Tag tag, int textColumnIndex)
        {
            var updatedLabel = row.Cells[textColumnIndex].Value.ToString();
            if (!tag.Name.Equals(updatedLabel, StringComparison.CurrentCultureIgnoreCase))
            {
                return new UpdatePair<Tag>()
                {
                    Old = tag,
                    New = new Tag(tag) { Name = updatedLabel }
                };
            }
            return null;
        }

        private void dgvUnlinkedTags_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvUnlinkedTags.IsCurrentCellDirty)
            {
                dgvUnlinkedTags.CommitEdit(DataGridViewDataErrorContexts.Commit);
                dgvUnlinkedTags.EndEdit(DataGridViewDataErrorContexts.LeaveControl);
                dgvUnlinkedTags.CurrentCell = dgvUnlinkedTags[0, 0];
            }
        }

        private void dgvUnlinkedTags_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (IsSelectingFile)
            {
                return;
            }

            if (e.ColumnIndex != ColUnlinkedActionToTake || e.RowIndex < 0)
            {
                return;
            }

            var combo = dgvUnlinkedTags[e.ColumnIndex, e.RowIndex] as DataGridViewComboBoxCell;
            if (combo == null)
            {
                return;
            }

            IsSelectingFile = true;
            var value = combo.Value as CodeFileAction;
            if (value != null && value.Action == Constants.CodeFileActionTask.SelectFile)
            {
                string fileName = UIUtility.GetOpenFileName(Constants.FileFilters.FormatForOpenFileDialog());
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    string package = CodeFile.GuessStatisticalPackage(fileName);
                    var action = UIUtility.AddOptionToBuildCodeFileActionColumn(
                        new CodeFile { FilePath = fileName, StatisticalPackage = package }, dgvUnlinkedTags, ColUnlinkedActionToTake);
                    combo.Value = action.Data;
                }
                else
                {
                    combo.Value = null;
                }


            }
            IsSelectingFile = false;
        }
    }
}
