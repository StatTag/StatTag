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
                    var item = lvwOutput.Items.Add(annotation.OutputLabel);
                    item.SubItems.AddRange(new [] { file.FilePath });
                    item.Tag = annotation;
                }
            }
        }
    }
}
