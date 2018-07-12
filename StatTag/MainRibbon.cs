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
        //private ManageTags ManageTagsDialog = null;
        private TagManager ManageTagsDialog = null;
        private SelectOutput SelectOutputDialog = null;
        private Settings SettingsDialog = null;
        private UpdateOutput UpdateOutputDialog = null;
        private CheckDocument CheckDocumentDialog = null;

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
            cmdDefineTag.Enabled = enabled;
            cmdInsertOutput.Enabled = enabled;
            cmdUpdateOutput.Enabled = enabled;
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
            try
            {
                ManageTagsDialog = new TagManager(Manager.GetTags(), Manager);
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

            //try
            //{
            //    ManageTagsDialog = new ManageTags(Manager);
            //    if (DialogResult.OK == ManageTagsDialog.ShowDialog())
            //    {
            //        Manager.SaveAllCodeFiles(ActiveDocument);
            //    }
            //}
            //catch (StatTagUserException uex)
            //{
            //    UIUtility.ReportException(uex, uex.Message, LogManager);
            //}
            //catch (Exception exc)
            //{
            //    UIUtility.ReportException(exc,
            //        "There was an unexpected error when trying to manage your tags.",
            //        LogManager);
            //}
            //finally
            //{
            //    ManageTagsDialog = null;
            //}
        }

        private void cmdInsertOutput_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                var files = Manager.GetCodeFileList(ActiveDocument);
                SelectOutputDialog = new SelectOutput(files);
                if (DialogResult.OK == SelectOutputDialog.ShowDialog())
                {
                    try
                    {
                        var tags = SelectOutputDialog.GetSelectedTags();
                        LogManager.WriteMessage(string.Format("Inserting {0} selected tags", tags.Count));
                        Manager.InsertTagsInDocument(tags);
                    }
                    catch (StatTagUserException uex)
                    {
                        UIUtility.ReportException(uex, uex.Message, LogManager);
                    }
                    catch (Exception exc)
                    {
                        UIUtility.ReportException(exc,
                            "There was an unexpected error when trying to insert the tag output into the document.",
                            LogManager);
                    }
                    finally
                    {
                        Globals.ThisAddIn.Application.ScreenUpdating = true;
                        Cursor.Current = Cursors.Default;
                    }
                }
            }
            finally
            {
                SelectOutputDialog = null;
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

        private void cmdUpdateOutput_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                UpdateOutputDialog = new UpdateOutput(Manager.GetTags());
                if (DialogResult.OK != UpdateOutputDialog.ShowDialog())
                {
                    return;
                }

                var tags = UpdateOutputDialog.SelectedTags;

                // Clear the dialog object once we no longer need it.  This allows us to better track which
                // dialogs are active and open.
                UpdateOutputDialog = null;
                Cursor.Current = Cursors.WaitCursor;
                Globals.ThisAddIn.Application.ScreenUpdating = false;
                
                // First, go through and update all of the code files to ensure we have all
                // refreshed tags.
                var refreshedFiles = new List<CodeFile>();
                var files = Manager.GetCodeFileList(ActiveDocument);
                foreach (var codeFile in files)
                {
                    if (!refreshedFiles.Contains(codeFile))
                    {
                        var result = StatsManager.ExecuteStatPackage(codeFile, Constants.ParserFilterMode.TagList, tags);
                        if (!result.Success)
                        {
                            break;
                        }

                        refreshedFiles.Add(codeFile);
                    }
                }

                // Now we will refresh all of the tags that are fields.  Since we most likely
                // have more fields than tags, we are going to use the approach of looping
                // through all fields and updating them (via the DocumentManager).
                Manager.UpdateFields();
            }
            catch (StatTagUserException uex)
            {
                UIUtility.ReportException(uex, uex.Message, LogManager);
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when trying to update values in your document.",
                    LogManager);
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
                Cursor.Current = Cursors.Default;

                // First, go through and update all of the code files to ensure we have all
                // refreshed tags.
                UpdateOutputDialog = null;
            }
        }

        private void cmdValidateDocument_Click(object sender, RibbonControlEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                Manager.PerformDocumentCheck(ActiveDocument, ref CheckDocumentDialog);
            }
            catch (StatTagUserException uex)
            {
                UIUtility.ReportException(uex, uex.Message, LogManager);
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when performing a validity check on this document.",
                    LogManager);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                CheckDocumentDialog = null;
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
            ManageCloseDialog(SelectOutputDialog);
            ManageCloseDialog(SettingsDialog);
            ManageCloseDialog(UpdateOutputDialog);
            ManageCloseDialog(CheckDocumentDialog);
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

        private void cmdDefineTag_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                EditTagDialog = new EditTag(true, Manager);
                if (DialogResult.OK == EditTagDialog.ShowDialog())
                {
                    Manager.SaveEditedTag(EditTagDialog);
                    var files = Manager.GetCodeFileList(ActiveDocument);
                    foreach (var file in files)
                    {
                        file.LoadTagsFromContent();
                    }

                    Manager.CheckForInsertSavedTag(EditTagDialog);
                }
            }
            finally
            {
                EditTagDialog = null;
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
