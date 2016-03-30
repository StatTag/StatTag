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
    public partial class UpdateOutput : Form
    {
        public List<Annotation> Annotations { get; set; }

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
                    var item = lvwDefault.Items.Add(annotation.OutputLabel);
                    item.SubItems.AddRange(new[] { annotation.CodeFile.FilePath });
                    item.Tag = annotation;
                    item.Checked = true;
                }
                else
                {
                    var item = lvwOnDemand.Items.Add(annotation.OutputLabel);
                    item.SubItems.AddRange(new[] { annotation.CodeFile.FilePath });
                    item.Tag = annotation;
                }
            }
        }
    }
}
