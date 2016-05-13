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
            get { return Globals.ThisAddIn.Manager; }
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

        private void MainRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void cmdLoadCode_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                var loadDialog = new LoadAnalysisCode(Manager, Manager.Files);
                if (DialogResult.OK == loadDialog.ShowDialog())
                {
                    Manager.Files = loadDialog.Files;

                    var unlinkedResults = Manager.FindAllUnlinkedTags();
                    if (unlinkedResults != null && unlinkedResults.Count > 0)
                    {
                        var linkDialog = new LinkCodeFiles(unlinkedResults, Manager.Files);
                        if (DialogResult.OK == linkDialog.ShowDialog())
                        {
                            Manager.UpdateUnlinkedTagsByCodeFile(linkDialog.CodeFileUpdates);
                        }
                    }
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
                    Manager.SaveAllCodeFiles();
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
            var dialog = new SelectOutput(Manager.Files);
            if (DialogResult.OK == dialog.ShowDialog())
            {
                Cursor.Current = Cursors.WaitCursor;
                Globals.ThisAddIn.Application.ScreenUpdating = false;
                try
                {
                    var updatedTags = new List<Tag>();
                    var refreshedFiles = new List<CodeFile>();
                    var tags = dialog.GetSelectedTags();
                    foreach (var tag in tags)
                    {
                        if (!refreshedFiles.Contains(tag.CodeFile))
                        {
                            var result = StatsManager.ExecuteStatPackage(tag.CodeFile,
                                Constants.ParserFilterMode.TagList, tags);
                            if (!result.Success)
                            {
                                break;
                            }

                            updatedTags.AddRange(result.UpdatedTags);
                            refreshedFiles.Add(tag.CodeFile);
                        }

                        Manager.InsertField(tag);
                    }

                    // Now that all of the fields have been inserted, sweep through and update any existing
                    // tags that changed.  We do this after the fields are inserted to better manage
                    // the cursor position in the document.
                    updatedTags = updatedTags.Distinct().ToList();
                    foreach (var updatedTag in updatedTags)
                    {
                        Manager.UpdateFields(new UpdatePair<Tag>(updatedTag, updatedTag));
                    }
                }
                catch (Exception exc)
                {
                    UIUtility.ReportException(exc,
                        "There was an unexpected error when trying to insert a value into the document.",
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
                foreach (var codeFile in Manager.Files)
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
                Manager.PerformDocumentCheck();
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
            var dialog = new EditTag(Manager);
            if (DialogResult.OK == dialog.ShowDialog())
            {
                Manager.SaveEditedTag(dialog);
                foreach (var file in Manager.Files)
                {
                    file.LoadTagsFromContent();
                }

            }
        }

        private void cmdHelp_Click(object sender, RibbonControlEventArgs e)
        {

        }
    }
}
