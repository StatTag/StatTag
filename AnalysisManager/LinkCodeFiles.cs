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
using AnalysisManager.Models;

namespace AnalysisManager
{
    public sealed partial class LinkCodeFiles : Form
    {
        /// <summary>
        /// The collection of actions that the user has indicated they wish to proceed with.
        /// </summary>
        public Dictionary<string, CodeFileAction> CodeFileUpdates { get; set; }
        public Dictionary<string, List<Annotation>> UnlinkedResults;
        private readonly List<CodeFile> Files;

        private const int ColMissingCodeFile = 0;
        private const int ColActionToTake = 1;

        public LinkCodeFiles(Dictionary<string, List<Annotation>> unlinkedResults, List<CodeFile> files)
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
    }
}
