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
using R;
using SAS;
using Stata;
using StatTag.Core.Models;
using StatTag.Models;

namespace StatTag
{
    public sealed partial class Settings : Form
    {
        private const string ExecutableFileFilter = "Application Executable|*.exe";
        private const string LogFileFilter = "Log File|*.log";

        private const string BlankValueLabelTemplate =
            "Note: The current document is set to use [BLANK_VALUE] for missing values.  Changes made here will affect new documents, but not the current document.  The setting for the current document can be changed under \"Document Properties\".";
        
        public Core.Models.UserSettings Properties { get; set; }
        private LogManager Logger { get; set; }
        private DocumentManager Manager { get; set; }

        public Settings(Core.Models.UserSettings properties, DocumentManager manager)
        {
            AutoScaleMode = AutoScaleMode.None;
            InitializeComponent();
            Properties = properties;
            Manager = manager;
            MinimumSize = Size;
            Logger = new LogManager();
            UIUtility.SetDialogTitle(this);

            tabSettings.AutoSize = true;
            tabGeneral.AutoSize = true;
            tabLogging.AutoSize = true;
            tabStata.AutoSize = true;
        }

        private void cmdStataLocation_Click(object sender, EventArgs e)
        {
            var stataPath = UIUtility.GetOpenFileName(ExecutableFileFilter);
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
            chkEnableLogging.Checked = Properties.EnableLogging;
            txtLogLocation.Text = Properties.LogLocation;
            chkRunCodeOnOpen.Checked = Properties.RunCodeOnOpen;
            txtMaxLogSize.Value = ((int)(Properties.GetValueInRange(Properties.MaxLogFileSize,
                Core.Models.UserSettings.MaxLogFileSizeMin, Core.Models.UserSettings.MaxLogFileSizeMax,
                Core.Models.UserSettings.MaxLogFileSizeDefault) / (Constants.BytesToMegabytesConversion * 1.0)));
            txtMaxLogFiles.Value = Properties.GetValueInRange(Properties.MaxLogFiles,
                Core.Models.UserSettings.MaxLogFilesMin, Core.Models.UserSettings.MaxLogFilesMax,
                Core.Models.UserSettings.MaxLogFilesDefault);
            missingValueSettings1.SetMissingValuesSelection(Properties.RepresentMissingValues);
            missingValueSettings1.SetCustomMissingValueString(Properties.CustomMissingValue);

            missingValueSettings1.ValueChanged += missingValueSettings_Changed;
            
            UpdateLoggingControls();
            UpdateStataControls();
            UpdateMissingValueControls();
        }

        private void missingValueSettings_Changed(object sender, EventArgs e)
        {
            UpdateMissingValueControls();
        }

        private void UpdateMissingValueControls()
        {
            var documentMetadata = Manager.LoadMetadataFromCurrentDocument(false);
            var customMissingValue = missingValueSettings1.GetCustomMissingValueString();
            var representMissingValues = missingValueSettings1.GetMissingValuesSelection();
            lblEmptyValueWarning.Visible = false;
            if (documentMetadata != null && (!representMissingValues.Equals(documentMetadata.RepresentMissingValues)
                || (representMissingValues.Equals(Constants.MissingValueOption.CustomValue) && !customMissingValue.Equals(documentMetadata.CustomMissingValue))))
            {
                lblEmptyValueWarning.Text = BlankValueLabelTemplate.Replace("[BLANK_VALUE]", documentMetadata.GetMissingValueReplacementAsString());
                lblEmptyValueWarning.Visible = true;
            }
        }

        /// <summary>
        /// Utility method to confirm that the Stata EXE path provided by the user is valid.  If not,
        /// inform the user of this.
        /// </summary>
        /// <returns>true if the file exists, or false otherwise</returns>
        private bool CheckStataFilePath()
        {
            if (File.Exists(txtStataLocation.Text))
            {
                return true;
            }

            MessageBox.Show(this,
                string.Format("The Stata executable could not be found at {0}.", txtStataLocation.Text),
                UIUtility.GetAddInName(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
        }

        private void cmdRegisterStataAutomation_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CheckStataFilePath())
                {
                    return;
                }

                Cursor = Cursors.WaitCursor;

                // We are going to first try to unregister the automation API, if it was set up.  
                // If this fails, show an error message.  If it succeeds, we don't show anything and
                // move on to enabling automation.
                if (!Stata.StataAutomation.UnregisterAutomationAPI(txtStataLocation.Text))
                {
                    ShowStataCommandError("disable");
                    return;
                }

                if (!Stata.StataAutomation.RegisterAutomationAPI(txtStataLocation.Text))
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
            try
            {
                if (!CheckStataFilePath())
                {
                    return;
                }

                if (DialogResult.Yes !=
                    MessageBox.Show(this,
                        "***WARNING: Disabling Stata Automation will reset your Stata user preferences.\r\n\r\nAlso, if you disable Stata Automation, StatTag will no longer work with Stata results.\r\n\r\nAre you sure you want to proceed?",
                        UIUtility.GetAddInName(), MessageBoxButtons.YesNo))
                {
                    return;
                }

                Cursor = Cursors.WaitCursor;
                if (!Stata.StataAutomation.UnregisterAutomationAPI(txtStataLocation.Text))
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
            Properties.EnableLogging = chkEnableLogging.Checked;
            Properties.LogLocation = txtLogLocation.Text;
            Properties.RunCodeOnOpen = chkRunCodeOnOpen.Checked;
            Properties.MaxLogFileSize = ((uint)(txtMaxLogSize.Value * Constants.BytesToMegabytesConversion));
            Properties.MaxLogFiles = (uint)txtMaxLogFiles.Value;
            Properties.CustomMissingValue = missingValueSettings1.GetCustomMissingValueString();
            Properties.RepresentMissingValues = missingValueSettings1.GetMissingValuesSelection();

            if (Properties.EnableLogging && !Logger.IsValidLogPath(Properties.LogLocation))
            {
                UIUtility.WarningMessageBox("The debug file you have selected appears to be invalid, or you do not have rights to access it.\r\nPlease select a valid path for the debug file, or disable debugging.", null);
                DialogResult = DialogResult.None;
            }
        }

        private void chkEnableLogging_CheckedChanged(object sender, EventArgs e)
        {
            UpdateLoggingControls();
        }

        private void UpdateLoggingControls()
        {
            bool enableControls = chkEnableLogging.Checked;
            cmdLogLocation.Enabled = enableControls;
            txtLogLocation.Enabled = enableControls;
            txtMaxLogFiles.Enabled = enableControls;
            txtMaxLogSize.Enabled = enableControls;
        }

        private void cmdLogLocation_Click(object sender, EventArgs e)
        {
            var logPath = UIUtility.GetSaveFileName(LogFileFilter);
            if (!string.IsNullOrWhiteSpace(logPath))
            {
                txtLogLocation.Text = logPath;
            }
        }

        private void txtLogLocation_TextChanged(object sender, EventArgs e)
        {
            string logFilePath = txtLogLocation.Text;
            if (string.IsNullOrWhiteSpace(logFilePath))
            {
                lblLogWarning.Text = "Please select or enter a file path";
                lblLogWarning.Visible = true;
            }
            else
            {
                lblLogWarning.Visible = false;
            }
        }
    }
}
