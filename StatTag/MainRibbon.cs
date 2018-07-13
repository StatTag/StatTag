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
        private TagManager ManageTagsDialog = null;
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
            //cmdDefineTag.Enabled = enabled;
            //cmdInsertOutput.Enabled = enabled;
            //cmdUpdateOutput.Enabled = enabled;
            cmdManageTags.Enabled = enabled;
        }

        private void cmdLoadCode_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
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
            }
        }

        private void cmdManageTags_Click(object sender, RibbonControlEventArgs e)
        {
            // This is a pseudo-singleton implementation.  We are controlling the single instance from
            // the code here instead of the class because we need to provide it with the reference to
            // the DocumentManager object when we create it the first time.
            if (ManageTagsDialog == null || ManageTagsDialog.IsDisposed)
            {
                try
                {
                    ManageTagsDialog = new TagManager(Manager);
                    ManageTagsDialog.Show();
                }
                catch (StatTagUserException uex)
                {
                    UIUtility.ReportException(uex, uex.Message, LogManager);
                }
                catch (Exception exc)
                {
                    UIUtility.ReportException(exc,
                        "There was an unexpected error when trying to manage your tags.",
                        LogManager);
                }
                finally
                {
                    ManageTagsDialog = null;
                }
            }
        }

        private void cmdSettings_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
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
            }
        }

        /// <summary>
        /// Called when we need to forcibly close any and all open dialogs, such as
        /// for system shutdown, or when a linked code file has been changed.
        /// </summary>
        public void CloseAllOpenDialogs()
        {
            ManageCloseDialog(EditTagDialog);
            ManageCloseDialog(LoadAnalysisCodeDialog);
            ManageCloseDialog(LinkCodeFilesDialog);
            ManageCloseDialog(ManageTagsDialog);
            ManageCloseDialog(SettingsDialog);
        }

        /// <summary>
        /// Utility function to invoke the Close method in a given dialog/form.
        /// </summary>
        /// <remarks>Because this
        /// uses messaging, it isn't an immediate close of the dialog, but will result in it
        /// being closed when all threads and message queues are processed.</remarks>
        /// <param name="dialog">The dialog/form to be closed</param>
        private void ManageCloseDialog(Form dialog)
        {
            try
            {
                if (dialog != null && !dialog.IsDisposed)
                {
                    dialog.Invoke((MethodInvoker) (dialog.Close));
                }
            }
            catch (Exception exc)
            {
                // For now we are eating the exception.
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
            try
            {
                var about = new About();
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
        }

        private void cmdDocumentProperties_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
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
        }
    }
}
