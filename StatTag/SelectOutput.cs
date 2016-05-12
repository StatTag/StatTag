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
    public sealed partial class SelectOutput : Form
    {
        protected List<CodeFile> Files = new List<CodeFile>();
        protected List<Annotation> Annotations = new List<Annotation>();
        private AnnotationListViewColumnSorter ListViewSorter = new AnnotationListViewColumnSorter();

        public SelectOutput(List<CodeFile> files = null)
        {
            InitializeComponent();
            Font = UIUtility.CreateScaledFont(Font, CreateGraphics());
            Files = files;
        }

        public List<Annotation> GetSelectedAnnotations()
        {
            return UIUtility.GetCheckedAnnotationsFromListView(lvwOutput).ToList();
        }

        private void SelectOutput_Load(object sender, EventArgs e)
        {
            foreach (var file in Files)
            {
                foreach (var annotation in file.Annotations)
                {
                    Annotations.Add(annotation);
                }
            }

            lvwOutput.ListViewItemSorter = ListViewSorter;
            LoadList();
        }

        private void LoadList(string filter = "")
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                lvwOutput.Items.Clear();

                foreach (var annotation in Annotations.Where(x => x.OutputLabel.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0))
                {
                    var item = lvwOutput.Items.Add(annotation.OutputLabel);
                    item.SubItems.AddRange(new[] {annotation.CodeFile.FilePath});
                    item.Tag = annotation;
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void txtFilter_FilterChanged(object sender, EventArgs e)
        {
            LoadList(txtFilter.Text);
        }

        private void lvwOutput_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewSorter.HandleSort(e.Column, lvwOutput);
        }
    }
}
