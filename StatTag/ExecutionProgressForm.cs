using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StatTag.Models;

namespace StatTag
{
    public partial class ExecutionProgressForm : Form
    {
        private DocumentManager Manager { get; set; }
        public ExecutionProgressForm(DocumentManager manager)
        {
            InitializeComponent();
            Manager = manager;

            if (Manager != null)
            {
                Manager.ExecutionUpdated += ManagerOnExecutionUpdated;
            }
        }

        private void ManagerOnExecutionUpdated(object sender, DocumentManager.ProgressEventArgs progressEventArgs)
        {
            lblDescription.Text = progressEventArgs.Description;
            pbrProgress.Maximum = progressEventArgs.TotalItems;
            pbrProgress.Value = progressEventArgs.Index;
        }
    }
}
