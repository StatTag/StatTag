﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AnalysisManager.Controls;
using AnalysisManager.Core;
using AnalysisManager.Core.Models;
using Microsoft.Office.Interop.Word;

namespace AnalysisManager
{
    public sealed partial class LoadAnalysisCode : Form
    {
        private const int CheckColumn = 0;
        private const int StatPackageColumn = 1;
        private const int FilePathColumn = 2;
        private const int FileEditColumn = 3;
        private const int DetailsColumn = 4;

        public List<CodeFile> Files { get; set; } 

        public LoadAnalysisCode(List<CodeFile> files = null)
        {
            InitializeComponent();
            Files = files;
            this.MinimumSize = this.Size;
        }

        private void LoadAnalysisCode_Load(object sender, EventArgs e)
        {
            colStatPackage.Items.AddRange(Utility.StringArrayToObjectArray(Constants.StatisticalPackages.GetList()));
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
            string fileName = GetFileName();
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                AddItem(new CodeFile() { FilePath = fileName });
            }
        }

        private void AddItem(CodeFile file)
        {
            int index = dgvItems.Rows.Add(new object[] { false, file.StatisticalPackage, file.FilePath, Constants.DialogLabels.Elipsis, Constants.DialogLabels.Details });
            dgvItems.Rows[index].Tag = file;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            dgvItems.CurrentCell = null;

            var files = new List<CodeFile>();
            for (int index = 0; index < dgvItems.Rows.Count; index++)
            {
                var item = dgvItems.Rows[index];
                var file = new CodeFile()
                {
                    FilePath = item.Cells[FilePathColumn].Value.ToString(),
                    StatisticalPackage = (item.Cells[StatPackageColumn].Value == null ? string.Empty : item.Cells[StatPackageColumn].Value.ToString())
                };
                file.LoadAnnotationsFromContent();
                files.Add(file);
                file.SaveBackup();
            }
            Files = files;

            this.Close();
        }

        private void cmdRemove_Click(object sender, EventArgs e)
        {
            UIUtility.RemoveSelectedItems(dgvItems, CheckColumn);
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == FileEditColumn)
            {
                string fileName = GetFileName();
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    dgvItems.Rows[e.RowIndex].Cells[FilePathColumn].Value = fileName;
                }    
            }
            else if (e.ColumnIndex == DetailsColumn)
            {
                var file = dgvItems.Rows[e.RowIndex].Tag as CodeFile;
                if (file != null)
                {
                    file.LoadAnnotationsFromContent();
                    var dialog = new ManageCodeBlocks(new List<CodeFile>(new []{ file }));
                    if (DialogResult.OK == dialog.ShowDialog())
                    {
                        
                    }
                }
            }
        }

        private string GetFileName()
        {
            FileDialog openFile = new OpenFileDialog();
            openFile.Filter = Constants.FileFilters.FormatForOpenFileDialog();
            if (DialogResult.OK == openFile.ShowDialog())
            {
                return openFile.FileName;
            }

            return null;
        }
    }
}
