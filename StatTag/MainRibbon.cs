using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using StatTag.Core;
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
        public DocumentManager Manager
        {
            get { return Globals.ThisAddIn.DocumentManager; }
        }

        public PropertiesManager PropertiesManager
        {
            get { return Globals.ThisAddIn.PropertiesManager; }
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
            UIStatusAfterFileLoad();
        }

        public void UIStatusAfterFileLoad()
        {
            var files = Manager.GetCodeFileList(ActiveDocument);
            bool enabled = (files != null && files.Count > 0);
            cmdDefineTag.Enabled = enabled;
            cmdInsertOutput.Enabled = enabled;
            cmdUpdateOutput.Enabled = enabled;
            cmdManageTags.Enabled = enabled;
            cmdValidateDocument.Enabled = enabled;
        }

        private void cmdLoadCode_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                var files = Manager.GetCodeFileList(ActiveDocument);
                var loadDialog = new LoadAnalysisCode(Manager, files);
                if (DialogResult.OK == loadDialog.ShowDialog())
                {
                    Manager.SetCodeFileList(loadDialog.Files);

                    var unlinkedResults = Manager.FindAllUnlinkedTags();
                    if (unlinkedResults != null && unlinkedResults.Count > 0)
                    {
                        var linkDialog = new LinkCodeFiles(unlinkedResults, files);
                        if (DialogResult.OK == linkDialog.ShowDialog())
                        {
                            Manager.UpdateUnlinkedTagsByCodeFile(linkDialog.CodeFileUpdates);
                        }
                    }

                    UIStatusAfterFileLoad();
                }
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when trying to manage your code files.",
                    LogManager);
            }
        }

        private void cmdManageTags_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                var dialog = new ManageTags(Manager);
                if (DialogResult.OK == dialog.ShowDialog())
                {
                    Manager.SaveAllCodeFiles(ActiveDocument);
                }
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when trying to manage your tags.",
                    LogManager);
            }
        }

        private void cmdInsertOutput_Click(object sender, RibbonControlEventArgs e)
        {
            var files = Manager.GetCodeFileList(ActiveDocument);
            var dialog = new SelectOutput(files);
            if (DialogResult.OK == dialog.ShowDialog())
            {
                try
                {
                    var tags = dialog.GetSelectedTags();
                    LogManager.WriteMessage(string.Format("Inserting {0} selected tags", tags.Count));
                    Manager.InsertTagsInDocument(tags);
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

        private void cmdSettings_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                var dialog = new Settings(PropertiesManager.Properties);
                if (DialogResult.OK == dialog.ShowDialog())
                {
                    PropertiesManager.Properties = dialog.Properties;
                    PropertiesManager.Save();
                    LogManager.UpdateSettings(dialog.Properties);
                }
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when trying to manage your settings",
                    LogManager);
            }
        }

        private void cmdUpdateOutput_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new UpdateOutput(Manager.GetTags());
            if (DialogResult.OK != dialog.ShowDialog())
            {
                return;
            }

            var tags = dialog.SelectedTags;
            Cursor.Current = Cursors.WaitCursor;
            Globals.ThisAddIn.Application.ScreenUpdating = false;
            try
            {
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
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when tring to update values in your document.",
                    LogManager);
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
                Cursor.Current = Cursors.Default;
            }
        }

        private void cmdValidateDocument_Click(object sender, RibbonControlEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                Manager.PerformDocumentCheck(ActiveDocument);
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
            }
        }

        private void cmdDefineTag_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new EditTag(true, Manager);
            if (DialogResult.OK == dialog.ShowDialog())
            {
                Manager.SaveEditedTag(dialog);
                var files = Manager.GetCodeFileList(ActiveDocument);
                foreach (var file in files)
                {
                    file.LoadTagsFromContent();
                }

                Manager.CheckForInsertSavedTag(dialog);
            }
        }

        private void cmdHelp_Click(object sender, RibbonControlEventArgs e)
        {

        }
    }
}
