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
    public sealed partial class SelectOutput : Form
    {
        protected List<CodeFile> Files = new List<CodeFile>();
        protected Dictionary<string, Annotation> Annotations = new Dictionary<string, Annotation>(); 

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
                    Annotations.Add(annotation.OutputLabel, annotation);
                }
            }

            LoadList();
        }

        private void LoadList(string filter = "")
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                lvwOutput.Items.Clear();

                foreach (var annotation in Annotations.Where(x => x.Key.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0))
                {
                    var item = lvwOutput.Items.Add(annotation.Key);
                    item.SubItems.AddRange(new[] {annotation.Value.CodeFile.FilePath});
                    item.Tag = annotation.Value;
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
    }
}
