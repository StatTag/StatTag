using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AnalysisManager.Core.Models;

namespace AnalysisManager
{
    public sealed partial class LinkCodeFiles : Form
    {
        public Dictionary<string, int> UnlinkedResults = new Dictionary<string, int>();
        public List<CodeFile> Files = new List<CodeFile>();
        /// <summary>
        /// The collection of actions that the user has indicated they wish to proceed with.
        /// </summary>
        public Dictionary<string, CodeFileAction> CodeFileUpdates { get; set; }

        private int ColMissingCodeFile = 0;
        private int ColActionToTake = 1;

        public class GridDataItem
        {
            public string Display { get; set; }
            public CodeFileAction Data { get; set; }
        }

        public LinkCodeFiles(Dictionary<string, int> unlinkedResults, List<CodeFile> files)
        {
            InitializeComponent();

            Font = UIUtility.CreateScaledFont(Font, CreateGraphics());
            UnlinkedResults = unlinkedResults;
            Files = files;
            MinimumSize = Size;
            CodeFileUpdates = new Dictionary<string, CodeFileAction>();
        }

        private void LinkCodeFiles_Load(object sender, EventArgs e)
        {
            foreach (var item in UnlinkedResults)
            {
                int row = dgvCodeFiles.Rows.Add(item.Key);
                dgvCodeFiles.Rows[row].Tag = item;
            }

            var actions = new List<GridDataItem>();
            foreach (var file in Files)
            {
                actions.Add(new GridDataItem() {
                    Display = string.Format("Use file {0}", file.FilePath),
                    Data = new CodeFileAction()
                    {
                        Label = string.Format("Use file {0}", file.FilePath),
                        Action = CodeFileAction.Task.ChangeFile,
                        Parameter = file
                    }
                });
            }
            actions.Add(new GridDataItem() {
                Display = "Remove all annotations in document",
                Data = new CodeFileAction()
                {
                    Label = "Remove all annotations in document",
                    Action = CodeFileAction.Task.RemoveAnnotations
                }
            });

            var column = dgvCodeFiles.Columns[ColActionToTake] as DataGridViewComboBoxColumn;
            column.DataSource = actions;
            column.DisplayMember = "Display";
            column.ValueMember = "Data";
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
    }
}
