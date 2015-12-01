using System;
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
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            string fileName = GetFileName();
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                dgvItems.Rows.Add(new object[] { false, "", fileName, Constants.DialogLabels.Elipsis, Constants.DialogLabels.Details});
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            var files = new List<CodeFile>();
            for (int index = 0; index < dgvItems.Rows.Count; index++)
            {
                var item = dgvItems.Rows[index];
                files.Add(new CodeFile()
                {
                    FilePath = item.Cells[FilePathColumn].Value.ToString(),
                    StatisticalPackage = item.Cells[StatPackageColumn].Value.ToString()
                });
            }
            Files = files;

            this.Close();
        }

        private void cmdRemove_Click(object sender, EventArgs e)
        {
            UIUtility.RemoveSelectedItems(dgvItems, CheckColumn);
            //var removeList = new List<DataGridViewRow>();
            //for (int index = 0; index < dgvItems.Rows.Count; index++)
            //{
            //    var item = dgvItems.Rows[index];
            //    var cell = item.Cells[CheckColumn] as DataGridViewCheckBoxCell;
            //    if (cell != null && cell.Value.ToString() == "true")
            //    {
            //        removeList.Add(item);
            //    }
            //}

            //foreach (var item in removeList)
            //{
            //    dgvItems.Rows.Remove(item);
            //}
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
        }

        private string GetFileName()
        {
            FileDialog openFile = new OpenFileDialog();
            if (DialogResult.OK == openFile.ShowDialog())
            {
                return openFile.FileName;
            }

            return null;
        }
    }
}
