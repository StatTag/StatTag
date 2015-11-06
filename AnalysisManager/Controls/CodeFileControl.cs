using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Controls
{
    public partial class CodeFileControl : UserControl
    {
        public event EventHandler Delete;

        public CodeFileControl()
        {
            InitializeComponent();
        }

        public CodeFile Details
        {
            get
            {
                return new CodeFile()
                {
                    StatisticalPackage = cboStatPackage.Text,
                    FilePath = txtFilePath.Text
                };
            }

            set
            {
                cboStatPackage.SelectedValue = value.StatisticalPackage;
                lblLastCached.Text = value.LastCached.ToString();
                txtFilePath.Text = value.FilePath;
            }
        }

        private void cmdLoadFile_Click(object sender, EventArgs e)
        {
            FileDialog openFile = new OpenFileDialog();
            if (DialogResult.OK == openFile.ShowDialog())
            {
                txtFilePath.Text = openFile.FileName;
                var hashAlg = MD5.Create();
                var hash = hashAlg.ComputeHash(new FileStream(openFile.FileName, FileMode.Open));
                lblChecksum.Text = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            }
        }

        private void CodeFile_Load(object sender, EventArgs e)
        {
            this.Height = pnlMetadata.Top;
            pnlMetadata.Visible = false;
        }

        private void lnkDetails_Clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlMetadata.Visible = !pnlMetadata.Visible;
            this.Height = pnlMetadata.Visible ? pnlMetadata.Bottom : pnlMetadata.Top;
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            OnDelete(EventArgs.Empty);
        }

        protected virtual void OnDelete(EventArgs e)
        {
            EventHandler handler = Delete;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
