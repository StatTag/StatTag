using System.Drawing;
using System.Linq;
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
        private const int CheckColumn = 0;
        private const int StatPackageColumn = 1;
        private const int FilePathColumn = 2;
        private const int FileEditColumn = 3;
        private const int DetailsColumn = 4;

        public List<CodeFile> Files { get; set; }
        public DocumentManager Manager { get; set; }

        public LoadAnalysisCode(DocumentManager manager, List<CodeFile> files = null)
        {
            InitializeComponent();
            Font = UIUtility.CreateScaledFont(Font, CreateGraphics());
            Manager = manager;
            Files = files;
            MinimumSize = Size;
        }

        private void LoadAnalysisCode_Load(object sender, EventArgs e)
        {
            colStatPackage.Items.AddRange(GeneralUtil.StringArrayToObjectArray(Constants.StatisticalPackages.GetList()));
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
            string fileName = UIUtility.GetFileName(Constants.FileFilters.FormatForOpenFileDialog());
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string package = CodeFile.GuessStatisticalPackage(fileName);
                AddItem(new CodeFile {FilePath = fileName, StatisticalPackage = package});
            }
        }

        private CodeFile AddItem(CodeFile file)
        {
            int index = dgvItems.Rows.Add(new object[] { false, file.StatisticalPackage, file.FilePath, Constants.DialogLabels.Elipsis, Constants.DialogLabels.Details });
            dgvItems.Rows[index].Tag = file;
            return file;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            dgvItems.CurrentCell = null;

            // Save off the values that may already be cached for an tag.
            //var existingTags = Manager.GetTags().Select(a => new Tag(a)).ToList();

            var files = new List<CodeFile>();
            for (int index = 0; index < dgvItems.Rows.Count; index++)
            {
                var item = dgvItems.Rows[index];
                var file = new CodeFile
                {
                    FilePath = item.Cells[FilePathColumn].Value.ToString(),
                    StatisticalPackage = (item.Cells[StatPackageColumn].Value == null ? string.Empty : item.Cells[StatPackageColumn].Value.ToString())
                };
                file.LoadTagsFromContent();
                files.Add(file);
                file.SaveBackup();
            }
            Files = files;
            Manager.Files = Files;
            
            Close();
        }

        private void cmdRemove_Click(object sender, EventArgs e)
        {
            UIUtility.RemoveSelectedItems(dgvItems, CheckColumn);
        }

        private void EditFilePath(int rowIndex)
        {
            string fileName = UIUtility.GetFileName(Constants.FileFilters.FormatForOpenFileDialog());
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                dgvItems.Rows[rowIndex].Cells[FilePathColumn].Value = fileName;
            }   
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == FileEditColumn)
            {
                EditFilePath(e.RowIndex);
            }
            else if (e.ColumnIndex == DetailsColumn)
            {
                //var file = dgvItems.Rows[e.RowIndex].Tag as CodeFile;
                //if (file != null)
                //{
                //    file.LoadTagsFromContent();
                //    var dialog = new ManageTags(new List<CodeFile>(new []{ file }));
                //    if (DialogResult.OK == dialog.ShowDialog())
                //    {
                        
                //    }
                //}
            }
        }

        private void dgvItems_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditFilePath(e.RowIndex);
        }
    }
}
