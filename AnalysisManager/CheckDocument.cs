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
    public sealed partial class CheckDocument : Form
    {
        /// <summary>
        /// The collection of actions that the user has indicated they wish to proceed with.
        /// </summary>
        public Dictionary<string, CodeFileAction> AnnotationUpdates { get; set; }

        /// <summary>
        /// Used as input, this is the list of annotations that are not fully linked to the
        /// current document, organized by the missing code file path.
        /// </summary>
        public Dictionary<string, List<Annotation>> UnlinkedAnnotations;

        private readonly List<CodeFile> Files;

        private const int ColCodeFile = 0;
        private const int ColAnnotation = 0;
        private const int ColActionToTake = 2;


        public CheckDocument(Dictionary<string, List<Annotation>> unlinkedAnnotations, List<CodeFile> files)
        {
            InitializeComponent();
            Font = UIUtility.CreateScaledFont(Font, CreateGraphics());
            MinimumSize = Size;
            UnlinkedAnnotations = unlinkedAnnotations;
            Files = files;
            AnnotationUpdates = new Dictionary<string, CodeFileAction>();
        }

        private void CheckDocument_Load(object sender, EventArgs e)
        {
            // Track the annotations as we add them to the list, so we only display each unique
            // annotation one time.  The reason we have duplicates is that this list comes from
            // the fields in a document, where multiple instances of the same annotation could
            // exist.
            HashSet<string> addedAnnotations = new HashSet<string>();
            foreach (var item in UnlinkedAnnotations)
            {
                foreach (var annotation in item.Value)
                {
                    if (!addedAnnotations.Contains(annotation.Id))
                    {
                        int row = dgvUnlinkedAnnotations.Rows.Add(new object[] { item.Key, annotation.OutputLabel });
                        dgvUnlinkedAnnotations.Rows[row].Tag = annotation;
                        addedAnnotations.Add(annotation.Id);
                    }
                }
            }

            UIUtility.BuildCodeFileActionColumn(Files, dgvUnlinkedAnnotations, ColActionToTake, true);
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            // Make sure we pick up any changes that the data grid view hasn't seen yet
            dgvUnlinkedAnnotations.EndEdit();

            foreach (var row in dgvUnlinkedAnnotations.Rows.OfType<DataGridViewRow>())
            {
                var fileCell = row.Cells[ColCodeFile] as DataGridViewTextBoxCell;
                var actionCell = row.Cells[ColActionToTake] as DataGridViewComboBoxCell;
                if (actionCell == null || fileCell == null)
                {
                    continue;
                }

                var annotation = row.Tag as Annotation;
                if (annotation != null && !AnnotationUpdates.ContainsKey(annotation.Id))
                {
                    AnnotationUpdates.Add(annotation.Id, actionCell.Value as CodeFileAction);
                }
            }
        }
    }
}
