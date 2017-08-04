using System.Drawing;
using System.IO;
using System.Linq;
using StatTag.Controls;
using StatTag.Core;
using StatTag.Core.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using StatTag.Core.Utility;
using StatTag.Models;

namespace StatTag
{
    public sealed partial class LoadAnalysisCode : Form
    {
        public List<CodeFile> Files { get; set; }
        public DocumentManager Manager { get; set; }

        public LoadAnalysisCode(DocumentManager manager, List<CodeFile> files = null)
        {
            InitializeComponent();
            UIUtility.ScaleFont(this);
            Manager = manager;
            Files = files;
            MinimumSize = Size;
            UIUtility.SetDialogTitle(this);
        }

        private void LoadAnalysisCode_Load(object sender, EventArgs e)
        {
            if (Files != null)
            {
                foreach (var file in Files)
                {
                    AddItem(file);
                }
            }
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            var fileNames = UIUtility.GetOpenFileNames(Constants.FileFilters.FormatForOpenFileDialog());
            if (fileNames != null)
            {
                var cleanedFiles = fileNames.Where(x => !string.IsNullOrWhiteSpace(x));
                foreach (var fileName in cleanedFiles)
                {
                    string package = CodeFile.GuessStatisticalPackage(fileName);
                    AddItem(new CodeFile { FilePath = fileName, StatisticalPackage = package });                    
                }
            }
        }

        /// <summary>
        /// Helper to add a new code file to our managed collections
        /// </summary>
        /// <param name="file">The code file to add to our managed collections</param>
        private void AddItem(CodeFile file)
        {
            var entry = new CodeFileEntry
            {
                CodeFile = file,
                Width = pnlCodeFiles.Width - pnlCodeFiles.Margin.Left - pnlCodeFiles.Margin.Right - 2,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            entry.CodeFileClick += codeFile_Click;
            entry.CodeFileDoubleClick += codeFile_DoubleClick;
            pnlCodeFiles.Controls.Add(entry);
        }

        /// <summary>
        /// Helper method to deselect all code files in the list
        /// </summary>
        private void UnselectAllCodeFiles()
        {
            foreach (var codeFile in pnlCodeFiles.Controls.OfType<CodeFileEntry>())
            {
                codeFile.Selected = false;
            }
        }

        /// <summary>
        /// Responds to a code file entry being clicked in our panel (list)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void codeFile_Click(object sender, EventArgs e)
        {
            if (Control.ModifierKeys != Keys.Alt)
            {
                UnselectAllCodeFiles();
            }

            (sender as CodeFileEntry).Selected = true;
        }

        /// <summary>
        /// Responds to a code file entry being double-clicked in our panel (list)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void codeFile_DoubleClick(object sender, EventArgs e)
        {
            var selectedEntry = sender as CodeFileEntry;
            if (selectedEntry != null)
            {
                selectedEntry.Selected = true;
                EditFilePath(selectedEntry);
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            var files = new List<CodeFile>();
            foreach (var codeFileEntry in pnlCodeFiles.Controls.OfType<CodeFileEntry>())
            {
                var file = new CodeFile()
                {
                    FilePath = codeFileEntry.CodeFile.FilePath,
                    StatisticalPackage = CodeFile.GuessStatisticalPackage(codeFileEntry.CodeFile.FilePath)
                };
                file.LoadTagsFromContent();
                files.Add(file);
                file.SaveBackup();
            }
            Files = files;
            Manager.SetCodeFileList(Files);
            
            Close();
        }

        private void cmdRemove_Click(object sender, EventArgs e)
        {
            var selectedEntries = pnlCodeFiles.Controls.OfType<CodeFileEntry>().Where(x => x.Selected).ToList();
            foreach (var entry in selectedEntries)
            {
                 pnlCodeFiles.Controls.Remove(entry);
            }
            UpdateCodeFileList();
        }

        /// <summary>
        /// Helper method to manage the action of attempting to change a code file's location.  It will handle
        /// the UI aspects as well as updating the code file data if it is changed.
        /// </summary>
        /// <param name="entry"></param>
        private void EditFilePath(CodeFileEntry entry)
        {
            string fileName = UIUtility.GetOpenFileName(Constants.FileFilters.FormatForOpenFileDialog());
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var file = new CodeFile {FilePath = fileName};
                entry.CodeFile = file;
            }
        }

        /// <summary>
        /// Perform necessary visual updates to the code file list
        /// </summary>
        private void UpdateCodeFileList()
        {
            foreach (var item in pnlCodeFiles.Controls.OfType<CodeFileEntry>())
            {
                item.Index = pnlCodeFiles.Controls.GetChildIndex(item, false);
            }
        }

        private void pnlCodeFiles_ControlAdded(object sender, ControlEventArgs e)
        {
            UpdateCodeFileList();
        }

        private void pnlCodeFiles_ControlRemoved(object sender, ControlEventArgs e)
        {
            UpdateCodeFileList();
        }

        private void pnlCodeFiles_Click(object sender, EventArgs e)
        {
            UnselectAllCodeFiles();
        }
    }
}
