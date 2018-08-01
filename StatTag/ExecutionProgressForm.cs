using System.ComponentModel;
using System.Windows.Forms;
using StatTag.Models;

namespace StatTag
{
    public partial class ExecutionProgressForm : Form
    {
        private BackgroundWorker Worker { get; set; }
        public ExecutionProgressForm(BackgroundWorker worker)
        {
            InitializeComponent();
            Worker = worker;
        }

        public void UpdateProgress(int percent, string description)
        {
            pbrProgress.Value = percent;
            pbrProgress.Maximum = 100;
            lblDescription.Text = description;
        }

        private void cmdCancel_Click(object sender, System.EventArgs e)
        {
            if (Worker != null)
            {
                Worker.CancelAsync();
            }
        }
    }
}
