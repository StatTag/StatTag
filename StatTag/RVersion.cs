using R;
using StatTag.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StatTag
{
    public partial class RVersion : Form
    {
        private string CustomRPath { get; set; }
        public string SelectedRPath { get; set; }

        public Core.Models.UserSettings Properties { get; set; }

        public RVersion(UserSettings properties)
        {
            AutoScaleMode = AutoScaleMode.None;
            InitializeComponent();
            UIUtility.SetDialogTitle(this);
            Properties = (UserSettings)properties.Clone();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            Properties.RDetection = radDefaultR.Checked ? Constants.RDetectionOption.System : Constants.RDetectionOption.Selected;
            // Ensure these string values are empty (not null) if they are not set.  Otherwise the values won't save in the registry.
            Properties.RCustomPath = CustomRPath ?? string.Empty;
            Properties.RLocation = lstRVersions.SelectedItem != null ? lstRVersions.SelectedItem.ToString() : string.Empty;
        }

        private void cmdCustomRPath_Click(object sender, EventArgs e)
        {
            var openFile = new OpenFileDialog
            {
                Filter = "R Executable|*.exe",
                Multiselect = false,
                FileName = "R.exe",
                Title = "Select R.exe"
            };

            if (DialogResult.OK == openFile.ShowDialog())
            {
                var rPath = Path.GetDirectoryName(openFile.FileName);
                Properties.RCustomPath = rPath;
                Properties.RLocation = rPath;
                LoadRPaths();
            }
        }

        private void radCustomR_CheckedChanged(object sender, EventArgs e)
        {
            lstRVersions.Enabled = radCustomR.Checked;
        }

        private void radDefaultR_CheckedChanged(object sender, EventArgs e)
        {
            lstRVersions.Enabled = radCustomR.Checked;
        }

        private void LoadRPaths()
        {
            lstRVersions.Items.Clear();

            var rPaths = RAutomation.DetectAllRPaths();
            if (rPaths != null)
            {
                foreach (var rPath in rPaths)
                {
                    lstRVersions.Items.Add(rPath);
                }

                // If the user has defined a custom path for R, we are going to add it to
                // our list of R versions.  Note that we are not confirming that the path
                // is still valid - if the user had it at one point, we are going to preserve
                // it.
                if (!string.IsNullOrWhiteSpace(Properties.RCustomPath))
                {
                    CustomRPath = Properties.RCustomPath;

                    // Only add the path if it doesn't already exist
                    if (!rPaths.Contains(Properties.RCustomPath))
                    {
                        lstRVersions.Items.Add(Properties.RCustomPath);
                    }
                }
            }

            // If the user has defined a selected R location that they wish to use, we
            // will restore it.  Even if the user isn't currently using that, we want
            // to remember what they had selected at any point in time.
            if (!string.IsNullOrWhiteSpace(Properties.RLocation))
            {
                foreach (var item in lstRVersions.Items)
                {
                    if (item.ToString().Equals(Properties.RLocation))
                    {
                        lstRVersions.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void RVersion_Load(object sender, EventArgs e)
        {
            LoadRPaths();

            // Only if the R detection option is set to "Selected" do we need to worry
            // about changing the default. Even if the RDetection value is something
            // unexpected (blank, etc.) we want to just use the default selection then.
            if (Properties.RDetection.Equals(Constants.RDetectionOption.Selected))
            {
                radDefaultR.Checked = false;
                radCustomR.Checked = true;
            }
        }
    }
}
