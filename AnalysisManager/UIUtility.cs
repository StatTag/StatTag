﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalysisManager
{
    public static class UIUtility
    {
        public static IEnumerable<object> RemoveSelectedItems(DataGridView dgvItems, int checkColumn)
        {
            if (dgvItems == null)
            {
                return null;
            }

            dgvItems.CurrentCell = null;  //Force any changes to save

            var removeList = new List<DataGridViewRow>();
            for (int index = 0; index < dgvItems.Rows.Count; index++)
            {
                var item = dgvItems.Rows[index];
                var cell = item.Cells[checkColumn] as DataGridViewCheckBoxCell;
                if (cell != null && cell.Value.ToString().Equals("true", StringComparison.CurrentCultureIgnoreCase))
                {
                    removeList.Add(item);
                }
            }

            foreach (var item in removeList)
            {
                dgvItems.Rows.Remove(item);
            }

            return removeList.Select(x => x.Tag);
        }

        public static string GetFileName(string filter)
        {
            FileDialog openFile = new OpenFileDialog();
            openFile.Filter = filter;
            if (DialogResult.OK == openFile.ShowDialog())
            {
                return openFile.FileName;
            }

            return null;
        }

        public static string GetAddInName()
        {
            return "Analysis Manager";
        }
    }
}
