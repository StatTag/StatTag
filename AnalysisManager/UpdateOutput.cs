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
using Microsoft.Office.Tools.Word;

namespace AnalysisManager
{
    public sealed partial class UpdateOutput : Form
    {
        public List<Annotation> Annotations { get; set; }

        private readonly Dictionary<string, Annotation> DefaultAnnotations = new Dictionary<string, Annotation>();
        private readonly Dictionary<string, Annotation> OnDemandAnnotations = new Dictionary<string, Annotation>();

        public List<Annotation> SelectedAnnotations
        {
            get
            {
                var annotations = new List<Annotation>();
                annotations.AddRange(UIUtility.GetCheckedAnnotationsFromListView(lvwDefault));
                annotations.AddRange(UIUtility.GetCheckedAnnotationsFromListView(lvwOnDemand));
                return annotations;
            }
        }

        public UpdateOutput(List<Annotation> annotations)
        {
            Annotations = annotations;
            InitializeComponent();
            Font = UIUtility.CreateScaledFont(Font, CreateGraphics());
        }

        private void ToggleList(ListView box, bool value)
        {
            for (int index = 0; index < box.Items.Count; index++)
            {
                box.Items[index].Checked = value;
            }
        }

        private void cmdOnDemandSelectAll_Click(object sender, EventArgs e)
        {
            ToggleList(lvwOnDemand, true);
        }

        private void cmdOnDemandSelectNone_Click(object sender, EventArgs e)
        {
            ToggleList(lvwOnDemand, false);
        }

        private void cmdDefaultSelectAll_Click(object sender, EventArgs e)
        {
            ToggleList(lvwDefault, true);
        }

        private void cmdDefaultSelectNone_Click(object sender, EventArgs e)
        {
            ToggleList(lvwDefault, false);
        }

        private void UpdateOutput_Load(object sender, EventArgs e)
        {
            if (Annotations == null)
            {
                return;
            }

            foreach (var annotation in Annotations)
            {
                if (annotation.RunFrequency.Equals(Constants.RunFrequency.Default))
                {
                    DefaultAnnotations.Add(annotation.OutputLabel, annotation);
                }
                else
                {
                    OnDemandAnnotations.Add(annotation.OutputLabel, annotation);
                }
            }

            LoadOnDemandList();
            LoadDefaultList();
        }

        private void LoadOnDemandList(string filter = "")
        {
            LoadList(OnDemandAnnotations, lvwOnDemand, false, filter);
        }

        private void LoadDefaultList(string filter = "")
        {
            LoadList(DefaultAnnotations, lvwDefault, true, filter);
        }

        private void LoadList(Dictionary<string, Annotation> annotations, ListView listView, bool checkItem, string filter = "")
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                listView.Items.Clear();

                foreach (var annotation in annotations.Where(x => x.Key.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0))
                {
                    var item = listView.Items.Add(annotation.Key);
                    item.SubItems.AddRange(new[] { annotation.Value.CodeFile.FilePath });
                    item.Tag = annotation.Value;
                    item.Checked = checkItem;
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void txtOnDemandFilter_FilterChanged(object sender, EventArgs e)
        {
            LoadOnDemandList(txtOnDemandFilter.Text);
        }

        private void txtDefaultFilter_FilterChanged(object sender, EventArgs e)
        {
            LoadDefaultList(txtDefaultFilter.Text);
        }
    }
}
