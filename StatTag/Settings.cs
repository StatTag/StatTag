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
        private bool StataAutomationEnabledOnEntry { get; set; }
        private SystemDetails SystemInformation { get; set; }
        
        /// <summary>
        /// If set when the dialog closes, this signals that a configuration change took place and
        /// that StatTag should refresh the system information cache.
        /// </summary>
        public bool RefreshSystemInformation { get; private set; }

        public Settings(Core.Models.UserSettings properties, DocumentManager manager, SystemDetails sysInfo)
        {
            AutoScaleMode = AutoScaleMode.None;
            InitializeComponent();
            Properties = properties;
            Manager = manager;
            SystemInformation = sysInfo;
            MinimumSize = Size;
            Logger = new LogManager();
            UIUtility.SetDialogTitle(this);
            RefreshSystemInformation = false;

            tabSettings.AutoSize = true;
            tabGeneral.AutoSize = true;
            tabLogging.AutoSize = true;
            tabStata.AutoSize = true;

            StataAutomationEnabledOnEntry = Stata.StataAutomation.IsAutomationEnabled();
            chkStataAutomation.Checked = StataAutomationEnabledOnEntry;
            UpdateStataSettingsUI();
            UpdateRSettingsUI(SystemInformation.RSupport);
        }

        private void cmdStataLocation_Click(object sender, EventArgs e)
        {
            var stataPath = UIUtility.GetOpenFileName(ExecutableFileFilter);
            if (!string.IsNullOrWhiteSpace(stataPath))
            {
                txtStataLocation.Text = stataPath;
            }
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

        private bool EnableStataAutomation()
        {
            try
            {
                if (!CheckStataFilePath())
                {
                    return false;
                }

                Cursor = Cursors.WaitCursor;

                // We are going to first try to unregister the automation API, if it was set up.  
                // If this fails, we don't show an error message and will just move on trying to enable
                // automation.
                Stata.StataAutomation.UnregisterAutomationAPI(txtStataLocation.Text);

                if (!Stata.StataAutomation.RegisterAutomationAPI(txtStataLocation.Text))
                {
                    ShowStataCommandError("enable");
                    return false;
                }

                ShowStataCommandSuccess("enabled");
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            RefreshSystemInformation = true;
            return true;
        }
        
        private bool DisableStataAutomation()
        {
            try
            {
                if (!CheckStataFilePath())
                {
                    return false;
                }

                if (DialogResult.Yes !=
                    MessageBox.Show(this,
                        "***WARNING: Disabling Stata Automation will reset your Stata user preferences.\r\n\r\nAlso, if you disable Stata Automation, StatTag will no longer work with Stata results.\r\n\r\nAre you sure you want to proceed?",
                        UIUtility.GetAddInName(), MessageBoxButtons.YesNo))
                {
                    return false;
                }

                Cursor = Cursors.WaitCursor;
                if (!Stata.StataAutomation.UnregisterAutomationAPI(txtStataLocation.Text))
                {
                    ShowStataCommandError("disable");
                    return false;
                }

                ShowStataCommandSuccess("disabled");
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            RefreshSystemInformation = true;
            return true;
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
            // If the Stata location has changed, and the user clicked 'OK' on this dialog, we're going to assume they
            // want to enable the Stata automation API, or disable it if they unchecked the automation box.
            if (!HandleStataAutomationAPIChanges())
            {
                DialogResult = DialogResult.None;
                return;
            }

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

        private bool HandleStataAutomationAPIChanges()
        {
            // If the Stata location is empty, there's really nothing we can do.  We're just going
            // to accept the change and proceed.  This will avoid warning the user that the file
            // location "" (blank string) couldn't be found.
            if (string.IsNullOrWhiteSpace(txtStataLocation.Text))
            {
                return true;
            }

            // If there was a change in the file location, and automation is still enabled, we
            // need to handle unregistering and re-registering
            if (chkStataAutomation.Checked && !string.Equals(Properties.StataLocation, txtStataLocation.Text))
            {
                return EnableStataAutomation();
            }
            else if (chkStataAutomation.Checked != StataAutomationEnabledOnEntry)
            {
                if (chkStataAutomation.Checked)
                {
                    return EnableStataAutomation();
                }
                else
                {
                    return DisableStataAutomation();
                }
            }

            // No changes, so everything is fine.
            return true;
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

        private void chkStataAutomation_CheckedChanged(object sender, EventArgs e)
        {
            UpdateStataSettingsUI();
        }

        private void UpdateStataSettingsUI()
        {
            var enabled = chkStataAutomation.Checked;
            txtStataLocation.Enabled = enabled;
            cmdStataLocation.Enabled = enabled;
        }

        private void UpdateRSettingsUI(bool supported)
        {
            lblRSupportStatus.Text = supported ? "Enabled" : "Not detected";
            lblRSupportStatus.ForeColor = supported ? Color.Green : Color.Red;
        }
        private void cmdInstallRSupport_Click(object sender, EventArgs e)
        {
            Logger.WriteMessage("Beginning installation of R support");

            var result = MessageBox.Show("StatTag will attempt to install and configure the IRkernel package in R.  This will make changes to your R setup.  Do you wish to continue?",
                        UIUtility.GetAddInName(), MessageBoxButtons.YesNoCancel);
            if (DialogResult.Yes != result)
            {
                Logger.WriteMessage("User cancelled setting up IRkernel");
                LogRStatusFailure();
                return;
            }

            LogRStatusAndLogger("Proceeding with IRkernel setup. Locating R...");
            var rPath = RAutomation.DetectRPath();
            if (string.IsNullOrWhiteSpace(rPath))
            {
                LogRStatusAndLogger("Unable to locate an R installation using the registry");
                LogRStatusFailure();
                return;
            }
            else if (!Directory.Exists(rPath))
            {
                LogRStatusAndLogger(string.Format("R path in registry but could not be found: {0}", rPath));
                LogRStatusFailure();
                return;
            }
            else
            {
                LogRStatusAndLogger(string.Format("R path found: {0}", rPath));
            }

            rPath = Path.Combine(rPath, "R.exe");
            if (!File.Exists(rPath))
            {
                LogRStatusAndLogger(string.Format("Could not find R.exe at {0}", rPath));
                LogRStatusFailure();
                return;
            }

            LogRStatusAndLogger("Attempting to install IRkernel...");

            var irKernelResult = RAutomation.RunRFromCommandLine(rPath, "-e \"install.packages('IRkernel', repos = 'https://cloud.r-project.org')\"");
            if (irKernelResult.Result)
            {
                LogRStatusAndLogger("Successfully installed (or confirmed installation of) IRkernel");
                LogRStatusAndLogger(irKernelResult.Details);
            }
            else
            {
                LogRStatusAndLogger("Failed to install IRkernel.  Please see the StatTag User Guide for more instructions on how to set up R support.");
                LogRStatusAndLogger(irKernelResult.Details);
                LogRStatusFailure();
                return;
            }

            LogRStatusAndLogger("Attempging to configure IRkernel...");

            var embeddedJupyterPath = "";
            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            if (Uri.IsWellFormedUriString(executingAssembly.CodeBase, UriKind.Absolute))
            {
                var codeBasePath = Path.GetDirectoryName(new Uri(executingAssembly.CodeBase).LocalPath);
                var embeddedJupyterBase = Path.Combine(codeBasePath, "python-embed");
                if (Directory.Exists(embeddedJupyterBase))
                {
                    LogRStatusAndLogger(string.Format("Using embedded Jupyter at: {0}", embeddedJupyterBase));
                    embeddedJupyterPath = string.Format("{0};{1}",
                        embeddedJupyterBase,
                        Path.Combine(embeddedJupyterBase, "Scripts"));
                }
            }

            var configResult = RAutomation.RunRFromCommandLine(rPath, "-e \"IRkernel::installspec()\"", embeddedJupyterPath);
            if (configResult.Result)
            {
                LogRStatusAndLogger("Successfully configured IRkernel");
                UpdateRSettingsUI(true);
                RefreshSystemInformation = true;
            }
            else
            {
                LogRStatusAndLogger("Failed to configure IRkernel.  Please see the StatTag User Guide for more instructions on how to set up R support.");
                LogRStatusAndLogger(configResult.Details);
                LogRStatusFailure();
            }
        }

        /// <summary>
        /// Utility function to write a final "failed" message during R support setup
        /// </summary>
        private void LogRStatusFailure()
        {
            LogRStatusAndLogger("\r\n*** SETUP FAILED ***");
        }

        /// <summary>
        /// Utility function to log the same text both to the dialog text field, as well as the log (if enabled)
        /// </summary>
        /// <param name="text"></param>
        private void LogRStatusAndLogger(string text)
        {
            txtRSupportProgress.Text += text + "\r\n";
            txtRSupportProgress.SelectionStart = txtRSupportProgress.Text.Length;
            txtRSupportProgress.ScrollToCaret();
            Logger.WriteMessage(text);
        }
    }
}
