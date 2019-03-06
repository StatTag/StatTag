using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using StatTag.Core;
using StatTag.Core.Exceptions;
using StatTag.Core.Generator;
using StatTag.Core.Models;
using StatTag.Models;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools.Ribbon;
using Application = System.Windows.Forms.Application;

namespace StatTag
{
    public partial class MainRibbon
    {
        // The ribbon will maintain a reference to all of its dialogs to help track when
        // the dialog is open.  If a dialog is open, the CloseAllOpenDialogs method will
        // then be able to signal all of them to close.  If a dialog is not currently
        // open, its property is set to null.
        private EditTag EditTagDialog = null;
        private LoadAnalysisCode LoadAnalysisCodeDialog = null;
        private LinkCodeFiles LinkCodeFilesDialog = null;
        private TagManagerForm ManageTagsDialog = null;
        private Settings SettingsDialog = null;

        public DocumentManager Manager
        {
            get { return Globals.ThisAddIn.DocumentManager; }
        }

        public SettingsManager SettingsManager
        {
            get { return Globals.ThisAddIn.SettingsManager; }
        }

        public LogManager LogManager
        {
            get { return Globals.ThisAddIn.LogManager; }
        }

        public StatsManager StatsManager
        {
            get { return Globals.ThisAddIn.StatsManager; }
        }

        public Document ActiveDocument
        {
            get { return Globals.ThisAddIn.SafeGetActiveDocument();  }
        }

        public string SystemInformation
        {
            get { return Globals.ThisAddIn.GetSystemInformation(); }
        }

        private void MainRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            try
            {
                UIStatusAfterFileLoad();
            }
            catch (StatTagUserException uex)
            {
                UIUtility.ReportException(uex, uex.Message, LogManager);
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when trying to load the application ribbon.",
                    LogManager);
            }
        }

        public void UIStatusAfterFileLoad()
        {
            var files = Manager.GetCodeFileList(ActiveDocument);
            bool enabled = (files != null && files.Count > 0);
            cmdManageTags.Enabled = enabled;
            cmdDocumentProperties.Enabled = (ActiveDocument != null);
            cmdLoadCode.Enabled = (ActiveDocument != null);
        }

        /// <summary>
        /// Change the visibility of the ManageTags form instance used by the add-in
        /// </summary>
        /// <param name="visible">If the ManageTags form should be visible or not</param>
        /// <param name="onlyIfVisible">If you conditionally only want to change the visibility if the ManageTags form is visible</param>
        public void SetManageTagsFormVisibility(bool visible, bool onlyIfVisible = false)
        {
            // The onlyIfVisible flag only really comes into play if we're attempting to hide the
            // ManageTags form.  This is in response to some odd focus issues across threads when
            // we're trying to hide the form and display another dialog.
            if (ManageTagsDialog != null && !ManageTagsDialog.IsDisposed && (!onlyIfVisible || ManageTagsDialog.Visible))
            {
                // Since we may be invoking forms across threads, we use Invoke to change the visibility and
                // the TopMost setting.
                ManageTagsDialog.Invoke((MethodInvoker)delegate()
                {
                    ManageTagsDialog.TopMost = visible;
                    ManageTagsDialog.Visible = visible;
                });
            }
        }

        /// <summary>
        /// Return if the ManageTags form is visible or not
        /// </summary>
        /// <returns>true if the ManageTags form is currently visible</returns>
        public bool GetManageTagsFormVisibility()
        {
            if (ManageTagsDialog == null || ManageTagsDialog.IsDisposed)
            {
                return false;
            }

            return ManageTagsDialog.Visible;
        }

        private void cmdLoadCode_Click(object sender, RibbonControlEventArgs e)
        {
            bool restoreManageTagsDialog = (ManageTagsDialog != null && !ManageTagsDialog.IsDisposed);
            try
            {
                if (restoreManageTagsDialog)
                {
                    SetManageTagsFormVisibility(false);
                }

                var files = Manager.GetCodeFileList(ActiveDocument);
                LoadAnalysisCodeDialog = new LoadAnalysisCode(Manager, files);
                if (DialogResult.OK == LoadAnalysisCodeDialog.ShowDialog())
                {
                    Manager.SetCodeFileList(LoadAnalysisCodeDialog.Files);

                    var unlinkedResults = Manager.FindAllUnlinkedTags();
                    if (unlinkedResults != null && unlinkedResults.Count > 0)
                    {
                        LinkCodeFilesDialog = new LinkCodeFiles(unlinkedResults, LoadAnalysisCodeDialog.Files);
                        if (DialogResult.OK == LinkCodeFilesDialog.ShowDialog())
                        {
                            Manager.UpdateUnlinkedTagsByCodeFile(LinkCodeFilesDialog.CodeFileUpdates,
                                LinkCodeFilesDialog.UnlinkedAffectedCodeFiles);
                        }
                    }

                    UIStatusAfterFileLoad();
                }
            }
            catch (StatTagUserException uex)
            {
                UIUtility.ReportException(uex, uex.Message, LogManager);
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when trying to manage your code files.",
                    LogManager);
            }
            finally
            {
                LoadAnalysisCodeDialog = null;
                LinkCodeFilesDialog = null;

                if (restoreManageTagsDialog)
                {
                    SetManageTagsFormVisibility(true);
                }
            }
        }

        private void cmdManageTags_Click(object sender, RibbonControlEventArgs e)
        {
            // This is a pseudo-singleton implementation.  We are controlling the single instance from
            // the code here instead of the class because we need to provide it with the reference to
            // the DocumentManager object when we create it the first time.  This also allows us to
            // perform a check so that if the user clicks on this again, the singleton is already created
            // but just isn't shown, we're able to make it visible again.
            try
            {
                if (ManageTagsDialog == null || ManageTagsDialog.IsDisposed)
                {
                    ManageTagsDialog = new TagManagerForm(Manager);
                    ManageTagsDialog.Show();
                }
                else
                {
                    ManageTagsDialog.Visible = true;
                    ManageTagsDialog.TopMost = true;
                    ManageTagsDialog.Focus();
                }
            }
            catch (StatTagUserException uex)
            {
                ManageTagsDialog = null;
                UIUtility.ReportException(uex, uex.Message, LogManager);
            }
            catch (Exception exc)
            {
                ManageTagsDialog = null;
                UIUtility.ReportException(exc,
                    "There was an unexpected error when trying to manage your tags.",
                    LogManager);
            }
        }

        private void cmdSettings_Click(object sender, RibbonControlEventArgs e)
        {
            bool restoreManageTagsDialog = (ManageTagsDialog != null && !ManageTagsDialog.IsDisposed);
            try
            {
                if (restoreManageTagsDialog)
                {
                    SetManageTagsFormVisibility(false, true);
                }

                SettingsDialog = new Settings(SettingsManager.Settings, Manager);
                if (DialogResult.OK == SettingsDialog.ShowDialog())
                {
                    SettingsManager.Settings = SettingsDialog.Properties;
                    SettingsManager.Save();
                    LogManager.UpdateSettings(SettingsDialog.Properties);
                }
            }
            catch (StatTagUserException uex)
            {
                UIUtility.ReportException(uex, uex.Message, LogManager);
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when trying to manage your settings",
                    LogManager);
            }
            finally
            {
                SettingsDialog = null;

                if (restoreManageTagsDialog)
                {
                    SetManageTagsFormVisibility(true);
                }
            }
        }

        /// <summary>
        /// Called when we need to forcibly close any and all open dialogs, such as
        /// for system shutdown, or when a linked code file has been changed.
        /// </summary>
        public void CloseAllOpenDialogs()
        {
            UIUtility.ManageCloseDialog(EditTagDialog);
            UIUtility.ManageCloseDialog(LoadAnalysisCodeDialog);
            UIUtility.ManageCloseDialog(LinkCodeFilesDialog);
            UIUtility.ManageCloseDialog(SettingsDialog);

            if (ManageTagsDialog != null && !ManageTagsDialog.IsDisposed)
            {
                ManageTagsDialog.CloseAllChildDialogs();
            }
        }

        private void cmdHelp_Click(object sender, RibbonControlEventArgs e)
        {
            var userGuidePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StatTag-UserGuide.pdf");
            if (!File.Exists(userGuidePath))
            {
                UIUtility.WarningMessageBox(
                    string.Format("We were unable to find the user guide at {0}.", userGuidePath), 
                    LogManager);
                return;
            }

            LogManager.WriteMessage(string.Format("Opening the user guide file {0}.", userGuidePath));
            System.Diagnostics.Process.Start(userGuidePath);
        }

        private void cmdAbout_Click(object sender, RibbonControlEventArgs e)
        {
            bool restoreManageTagsDialog = (ManageTagsDialog != null && !ManageTagsDialog.IsDisposed);
            try
            {
                if (restoreManageTagsDialog)
                {
                    SetManageTagsFormVisibility(false, true);
                }

                var about = new About(this.SystemInformation);
                about.ShowDialog();
            }
            catch (StatTagUserException uex)
            {
                UIUtility.ReportException(uex, uex.Message, LogManager);
            }
            catch (Exception exc)
            {
                LogManager.WriteException(exc);
            }
            finally
            {
                if (restoreManageTagsDialog)
                {
                    SetManageTagsFormVisibility(true);
                }
            }
        }

        private void cmdDocumentProperties_Click(object sender, RibbonControlEventArgs e)
        {
            bool restoreManageTagsDialog = (ManageTagsDialog != null && !ManageTagsDialog.IsDisposed);

            try
            {
                if (restoreManageTagsDialog)
                {
                    SetManageTagsFormVisibility(false, true);
                }

                var documentProperties = new DocumentProperties(this.ActiveDocument, this.Manager);
                documentProperties.ShowDialog();
            }
            catch (StatTagUserException uex)
            {
                UIUtility.ReportException(uex, uex.Message, LogManager);
            }
            catch (Exception exc)
            {
                LogManager.WriteException(exc);
            }
            finally
            {
                if (restoreManageTagsDialog)
                {
                    SetManageTagsFormVisibility(true);
                }
            }
        }
    }
}
