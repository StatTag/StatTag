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
            actions.Add(CreateActionItem(string.Empty, Constants.CodeFileActionTask.NoAction, null));
            foreach (var file in Files)
            {
                actions.Add(CreateActionItem(string.Format("Use file {0}", file.FilePath), 
                    Constants.CodeFileActionTask.ChangeFile, file));
            }
            actions.Add(CreateActionItem("Remove related annotations from document", 
                Constants.CodeFileActionTask.RemoveAnnotations, null));

            var column = dgvCodeFiles.Columns[ColActionToTake] as DataGridViewComboBoxColumn;
            column.DataSource = actions;
            column.DisplayMember = "Display";
            column.ValueMember = "Data";
        }

        /// <summary>
        /// Utility method to create an action item for our combo box column
        /// </summary>
        /// <remarks>We're wrapping this up to facilitate the DataGridView and how it works.  The GridDataItem lets us
        /// have a display and value property, and the value (the CodeFileAction) can then be picked up and shared when
        /// it is selected.</remarks>
        /// <param name="display">What to display in the combo box</param>
        /// <param name="action">The resulting action to perform</param>
        /// <param name="parameter">Optional - parameter to use with the specified action.</param>
        /// <returns>The created GridDataItem that wraps and contains a CodeFileAction</returns>
        private GridDataItem CreateActionItem(string display, int action, object parameter)
        {
            return new GridDataItem()
            {
                Display = display,
                Data = new CodeFileAction()
                {
                    Label = display,
                    Action = action,
                    Parameter = parameter
                }
            };
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
