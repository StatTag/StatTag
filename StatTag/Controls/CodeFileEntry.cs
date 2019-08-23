using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StatTag.Core.Models;

namespace StatTag.Controls
{
    public partial class CodeFileEntry : UserControl
    {
        public event EventHandler CodeFileClick;
        public event EventHandler CodeFileDoubleClick;

        private bool selected = false;
        private int index = 0;
        private CodeFile codeFile = null;

        public CodeFileEntry()
        {
            InitializeComponent();
            new ToolTip().SetToolTip(imgWarning, "The file could not be found at the specified location");
        }

        [Description("The code file being displayed"), Category("Data")]
        public CodeFile CodeFile
        {
            get { return codeFile; }

            set
            {
                codeFile = value;
                if (codeFile == null)
                {
                    FileName = string.Empty;
                    FilePath = string.Empty;
                }
                else
                {
                    FileName = Path.GetFileName(codeFile.FilePath);
                    FilePath = Path.GetDirectoryName(codeFile.FilePath);
                    imgWarning.Visible = (!File.Exists(codeFile.FilePath));
                }
            }
        }

        [Description("The name of the code file"), Category("Data")]
        public string FileName
        {
            get { return lblFileName.Text; }
            set
            {
                lblFileName.Text = value;
                UpdateIconForFile(value);
            }
        }

        [Description("The path to the code file"), Category("Data")]
        public string FilePath
        {
            get { return lblFilePath.Text; }
            set { lblFilePath.Text = value; }
        }

        [Description("Flag to set if the control is considered selected or not"), Category("Data")]
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                UpdateBackgroundColor();
            }
        }

        [Description("The index of the code file entry within a list"), Category("Data")]
        public int Index
        {
            get { return index; }
            set 
            {
                index = value;
                UpdateBackgroundColor();
            }
        }

        private void UpdateBackgroundColor()
        {
            if (selected)
            {
                ForeColor = Color.White;
                BackColor = Color.DodgerBlue;
            }
            else if (index%2 == 0)
            {
                ForeColor = Color.Black;
                BackColor = Color.White;
            }
            else
            {
                ForeColor = Color.Black;
                BackColor = Color.GhostWhite;
            }
        }

        private void UpdateIconForFile(string fileName)
        {
            var statPackage = CodeFile.GuessStatisticalPackage(fileName);
            imgR.Visible = false;
            imgRmd.Visible = false;
            imgSAS.Visible = false;
            imgStata.Visible = false;
            imgPython.Visible = false;
            switch (statPackage)
            {
                case Constants.StatisticalPackages.Stata:
                    {
                        imgStata.Visible = true;
                        break;
                    }
                case Constants.StatisticalPackages.SAS:
                    {
                        imgSAS.Visible = true;
                        break;
                    }
                case Constants.StatisticalPackages.R:
                    {
                        imgR.Visible = true;
                        break;
                    }
                case Constants.StatisticalPackages.RMarkdown:
                    {
                        imgRmd.Visible = true;
                        break;
                    }
                case Constants.StatisticalPackages.Python:
                    {
                        imgPython.Visible = true;
                        break;
                    }
            }
        }

        private void CodeFileEntry_Click(object sender, EventArgs e)
        {
            if (this.CodeFileClick != null)
            {
                CodeFileClick(this, e);
            }
        }

        private void CodeFileEntry_DoubleClick(object sender, EventArgs e)
        {
            if (this.CodeFileDoubleClick != null)
            {
                CodeFileDoubleClick(this, e);
            }
        }
    }
}
