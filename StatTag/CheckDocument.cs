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
        public Dictionary<string, CodeFileAction> UnlinkedAnnotationUpdates { get; set; }

        /// <summary>
        /// The collection of updates that should be applied to uniquely name annotations across
        /// code files.
        /// </summary>
        public List<UpdatePair<Tag>> DuplicateAnnotationUpdates { get; set; }

        /// <summary>
        /// Used as input, this is the list of annotations that are not fully linked to the
        /// current document, organized by the missing code file path.
        /// </summary>
        public Dictionary<string, List<Tag>> UnlinkedAnnotations;

        /// <summary>
        /// Used as input, this is the list of code files that contain annotations with
        /// duplicate names.
        /// </summary>
        public DuplicateTagResults DuplicateAnnotations { get; set; }

        private readonly List<CodeFile> Files;

        private const int ColUnlinkedCodeFile = 0;
        private const int ColUnlinkedAnnotation = 1;
        private const int ColUnlinkedActionToTake = 2;

        private const int ColOriginalAnnotation = 0;
        private const int ColOriginalLineNumbers = 1;
        private const int ColDuplicateAnnotation = 2;
        private const int ColDuplicateLineNumbers = 3;

        private class DuplicateAnnotationPair
        {
            public Tag First { get; set; }
            public Tag Second { get; set; }
        }

        public CheckDocument(Dictionary<string, List<Tag>> unlinkedAnnotations, DuplicateTagResults duplicateAnnotations, List<CodeFile> files)
        {
            InitializeComponent();
            Font = UIUtility.CreateScaledFont(Font, CreateGraphics());
            MinimumSize = Size;
            UnlinkedAnnotations = unlinkedAnnotations;
            DuplicateAnnotations = duplicateAnnotations;
            Files = files;
            UnlinkedAnnotationUpdates = new Dictionary<string, CodeFileAction>();
            DuplicateAnnotationUpdates = new List<UpdatePair<Annotation>>();
        }

        private void CheckDocument_Load(object sender, EventArgs e)
        {
            int? defaultTab = null;

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

            if (dgvUnlinkedAnnotations.RowCount > 0)
            {
                defaultTab = 0;
                tabUnlinked.Text += string.Format(" ({0})", dgvUnlinkedAnnotations.RowCount);
            }

            foreach (var item in DuplicateAnnotations)
            {
                foreach (var result in item.Value)
                {
                    foreach (var duplicate in result.Value)
                    {
                        int row = dgvDuplicateAnnotations.Rows.Add(new object[]
                        {
                            result.Key.OutputLabel, result.Key.FormatLineNumberRange(),
                            duplicate.OutputLabel, duplicate.FormatLineNumberRange()
                        });
                        dgvDuplicateAnnotations.Rows[row].Tag = new DuplicateAnnotationPair()
                        {
                            First = result.Key,
                            Second = duplicate
                        };
                    }
                }
            }

            if (dgvDuplicateAnnotations.RowCount > 0)
            {
                defaultTab = 1;
                tabDuplicate.Text += string.Format(" ({0})", dgvDuplicateAnnotations.RowCount);
            }

            if (defaultTab.HasValue)
            {
                tabResults.SelectTab(defaultTab.Value);                
            }

            UIUtility.BuildCodeFileActionColumn(Files, dgvUnlinkedAnnotations, ColUnlinkedActionToTake, true);
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            // Make sure we pick up any changes that the data grid view hasn't seen yet
            dgvUnlinkedAnnotations.EndEdit();
            dgvDuplicateAnnotations.EndEdit();

            // Iterate the list of unlinked annotations and determine the actions that we
            // should be taking.
            foreach (var row in dgvUnlinkedAnnotations.Rows.OfType<DataGridViewRow>())
            {
                var fileCell = row.Cells[ColUnlinkedCodeFile] as DataGridViewTextBoxCell;
                var actionCell = row.Cells[ColUnlinkedActionToTake] as DataGridViewComboBoxCell;
                if (actionCell == null || fileCell == null)
                {
                    continue;
                }

                var annotation = row.Tag as Annotation;
                if (annotation != null && !UnlinkedAnnotationUpdates.ContainsKey(annotation.Id))
                {
                    UnlinkedAnnotationUpdates.Add(annotation.Id, actionCell.Value as CodeFileAction);
                }
            }

            // Iterate the list of duplicate named annotations and build the list of name changes
            // that are needed.
            foreach (var row in dgvDuplicateAnnotations.Rows.OfType<DataGridViewRow>())
            {
                var duplicatePair = row.Tag as DuplicateAnnotationPair;
                if (duplicatePair != null)
                {
                    DuplicateAnnotationUpdates.Add(GetDuplicateUpdate(row, duplicatePair.First, ColOriginalAnnotation));
                    DuplicateAnnotationUpdates.Add(GetDuplicateUpdate(row, duplicatePair.Second, ColDuplicateAnnotation));
                }
            }

            DuplicateAnnotationUpdates = DuplicateAnnotationUpdates.Where(x => x != null).ToList();
        }

        /// <summary>
        /// Loop through the list of annotations that are duplicates, and see what the recommended action is
        /// for each. Build the appropriate update list depending on if one annotation (or both) changed in the
        /// pair.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="annotation"></param>
        /// <param name="textColumnIndex"></param>
        /// <returns></returns>
        private UpdatePair<Annotation> GetDuplicateUpdate(DataGridViewRow row, Annotation annotation, int textColumnIndex)
        {
            var updatedLabel = row.Cells[textColumnIndex].Value.ToString();
            if (!annotation.OutputLabel.Equals(updatedLabel, StringComparison.CurrentCultureIgnoreCase))
            {
                return new UpdatePair<Annotation>()
                {
                    Old = annotation,
                    New = new Annotation(annotation) { OutputLabel = updatedLabel }
                };
            }
            return null;
        }
    }
}
