using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StatTag
{
    public sealed partial class About : Form
    {
        private string SystemInformation { get; set; }

        public About(string systemInformation)
        {
            InitializeComponent();
            UIUtility.ScaleFont(this);
            SystemInformation = systemInformation;
        }

        private void About_Load(object sender, EventArgs e)
        {
            lblVersion.Text = UIUtility.GetVersionLabel();
            lblCopyright.Text = UIUtility.GetCopyright();
            UpdateSystemDetails();
        }

        private void UpdateSystemDetails()
        {
            rtbSystemDetails.SelectionTabs = new int[] { 10, 20, 30, 40 };
            rtbSystemDetails.Text = SystemInformation;
            rtbSystemDetails.ReadOnly = true;
        }

        private void cmdCopyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(rtbSystemDetails.Text);
        }

        private void cmdCopyCitation_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtCitation.Text);
        }
    }
}
