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
                annotations.AddRange(clbDefault.CheckedItems.Cast<Annotation>());
                annotations.AddRange(clbOnDemand.CheckedItems.Cast<Annotation>());
                return annotations;
            }
        }

        public UpdateOutput(List<Annotation> annotations)
        {
            Annotations = annotations;
            InitializeComponent();
        }

        private void ToggleList(CheckedListBox box, bool value)
        {
            for (int index = 0; index < box.Items.Count; index++)
            {
                box.SetItemChecked(index, value);
            }
        }

        private void cmdOnDemandSelectAll_Click(object sender, EventArgs e)
        {
            ToggleList(clbOnDemand, true);
        }

        private void cmdOnDemandSelectNone_Click(object sender, EventArgs e)
        {
            ToggleList(clbOnDemand, false);
        }

        private void cmdDefaultSelectAll_Click(object sender, EventArgs e)
        {
            ToggleList(clbDefault, true);
        }

        private void cmdDefaultSelectNone_Click(object sender, EventArgs e)
        {
            ToggleList(clbDefault, false);
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
                    clbDefault.Items.Add(annotation, true);
                }
                else
                {
                    clbOnDemand.Items.Add(annotation, false);
                }
            }
        }
    }
}
