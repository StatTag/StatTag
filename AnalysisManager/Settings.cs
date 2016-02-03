using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalysisManager
{
    public partial class Settings : Form
    {
        private const string ExecutableFileFilter = "Application Executable|*.exe";

        public Models.Properties Properties { get; set; }

        public Settings(Models.Properties properties)
        {
            Properties = properties;
            InitializeComponent();
        }

        private void cmdStataLocation_Click(object sender, EventArgs e)
        {
            var stataPath = UIUtility.GetFileName(ExecutableFileFilter);
            if (!string.IsNullOrWhiteSpace(stataPath))
            {
                txtStataLocation.Text = stataPath;
            }
        }

        private void txtStataLocation_TextChanged(object sender, EventArgs e)
        {
            UpdateStataControls();
        }

        private void UpdateStataControls()
        {
            var enable = !string.IsNullOrWhiteSpace(txtStataLocation.Text);
            cmdRegisterStataAutomation.Enabled = enable;
            cmdDisableStataAutomation.Enabled = enable;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            txtStataLocation.Text = Properties.StataLocation;
            UpdateStataControls();
            lblVersion.Text = UIUtility.GetVersionLabel();
            lblCopyright.Text = UIUtility.GetCopyright();
        }

        private void cmdRegisterStataAutomation_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                if (!Stata.Automation.RegisterAutomationAPI(txtStataLocation.Text))
                {
                    ShowStataCommandError("enable");
                    return;
                }

                ShowStataCommandSuccess("enabled");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void cmdDisableStataAutomation_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes !=
                MessageBox.Show(this,
                    "If you disable Stata Automation, Analysis Manager will no longer work with Stata results.\r\n\r\nAre you sure you want to proceed?",
                    UIUtility.GetAddInName(), MessageBoxButtons.YesNo))
            {
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                if (!Stata.Automation.UnregisterAutomationAPI(txtStataLocation.Text))
                {
                    ShowStataCommandError("disable");
                    return;
                }

                ShowStataCommandSuccess("disabled");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ShowStataCommandError(string action)
        {
            Cursor = Cursors.Default;
            MessageBox.Show(
                string.Format(
                    "There was an error trying to {0} the Stata Automation API.\r\nMore information about Stata Automation can be found at: http://www.stata.com/automation",
                action), UIUtility.GetAddInName(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void ShowStataCommandSuccess(string action)
        {
            Cursor = Cursors.Default;
            MessageBox.Show(
                string.Format(
                    "The Stata Automation API has been successfully {0}.",
                action), UIUtility.GetAddInName());
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            Properties.StataLocation = txtStataLocation.Text;
        }
    }
}
