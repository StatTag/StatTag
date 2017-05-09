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
    public sealed partial class LinkCodeFiles : Form
    {
        /// <summary>
        /// The collection of actions that the user has indicated they wish to proceed with.
        /// </summary>
        public Dictionary<string, CodeFileAction> CodeFileUpdates { get; set; }
        public Dictionary<string, List<Tag>> UnlinkedResults;
        private readonly List<CodeFile> Files;

        private const int ColMissingCodeFile = 0;
        private const int ColActionToTake = 1;

        private bool IsSelectingFile = false;

        public LinkCodeFiles(Dictionary<string, List<Tag>> unlinkedResults, List<CodeFile> files)
        {
            InitializeComponent();

            UIUtility.ScaleFont(this);
            UnlinkedResults = unlinkedResults;
            Files = files;
            MinimumSize = Size;
            CodeFileUpdates = new Dictionary<string, CodeFileAction>();
            UIUtility.SetDialogTitle(this);
        }

        private void LinkCodeFiles_Load(object sender, EventArgs e)
        {
            foreach (var item in UnlinkedResults)
            {
                int row = dgvCodeFiles.Rows.Add(item.Key);
                dgvCodeFiles.Rows[row].Tag = item;
            }

            UIUtility.BuildCodeFileActionColumn(Files, dgvCodeFiles, ColActionToTake, false);
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            // Make sure we pick up any changes that the data grid view hasn't seen yet
            dgvCodeFiles.EndEdit();

            foreach (var row in dgvCodeFiles.Rows.OfType<DataGridViewRow>())
            {
                var fileCell = row.Cells[ColMissingCodeFile] as DataGridViewTextBoxCell;
                var actionCell = row.Cells[ColActionToTake] as DataGridViewComboBoxCell;
                if (actionCell == null || fileCell == null)
                {
                    continue;
                }

                CodeFileUpdates.Add(fileCell.Value.ToString(), actionCell.Value as CodeFileAction);
            }
        }

        private void dgvCodeFiles_Leave(object sender, EventArgs e)
        {
            dgvCodeFiles.EndEdit();
        }

        private void dgvCodeFiles_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvCodeFiles.IsCurrentCellDirty)
            {
                dgvCodeFiles.CommitEdit(DataGridViewDataErrorContexts.Commit);
                dgvCodeFiles.EndEdit(DataGridViewDataErrorContexts.LeaveControl);
                dgvCodeFiles.CurrentCell = dgvCodeFiles[0, 0];
            }
        }

        private void dgvCodeFiles_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (IsSelectingFile)
            {
                return;
            }

            if (e.ColumnIndex != ColActionToTake || e.RowIndex < 0)
            {
                return;
            }

            var combo = dgvCodeFiles[e.ColumnIndex, e.RowIndex] as DataGridViewComboBoxCell;
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
                        new CodeFile { FilePath = fileName, StatisticalPackage = package }, dgvCodeFiles, ColActionToTake);
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
