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

        private readonly List<CodeFile> Files;

        private const int ColUnlinkedCodeFile = 0;
        private const int ColUnlinkedTag = 1;
        private const int ColUnlinkedActionToTake = 2;

        private const int ColOriginalTag = 0;
        private const int ColOriginalLineNumbers = 1;
        private const int ColDuplicateTag = 2;
        private const int ColDuplicateLineNumbers = 3;

        private class DuplicateTagPair
        {
            public Tag First { get; set; }
            public Tag Second { get; set; }
        }

        public CheckDocument(Dictionary<string, List<Tag>> unlinkedTags, DuplicateTagResults duplicateTags, List<CodeFile> files)
        {
            InitializeComponent();
            Font = UIUtility.CreateScaledFont(Font, CreateGraphics());
            MinimumSize = Size;
            UnlinkedTags = unlinkedTags;
            DuplicateTags = duplicateTags;
            Files = files;
            UnlinkedTagUpdates = new Dictionary<string, CodeFileAction>();
            DuplicateTagUpdates = new List<UpdatePair<Tag>>();
        }

        private void CheckDocument_Load(object sender, EventArgs e)
        {
            int? defaultTab = null;

            // Track the tags as we add them to the list, so we only display each unique
            // tag one time.  The reason we have duplicates is that this list comes from
            // the fields in a document, where multiple instances of the same tag could
            // exist.
            HashSet<string> addedTags = new HashSet<string>();
            foreach (var item in UnlinkedTags)
            {
                foreach (var tag in item.Value)
                {
                    if (!addedTags.Contains(tag.Id))
                    {
                        int row = dgvUnlinkedTags.Rows.Add(new object[] { item.Key, tag.OutputLabel });
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
                            result.Key.OutputLabel, result.Key.FormatLineNumberRange(),
                            duplicate.OutputLabel, duplicate.FormatLineNumberRange()
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
                defaultTab = 1;
                tabDuplicate.Text += string.Format(" ({0})", dgvDuplicateTags.RowCount);
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
            if (!tag.OutputLabel.Equals(updatedLabel, StringComparison.CurrentCultureIgnoreCase))
            {
                return new UpdatePair<Tag>()
                {
                    Old = tag,
                    New = new Tag(tag) { OutputLabel = updatedLabel }
                };
            }
            return null;
        }
    }
}
