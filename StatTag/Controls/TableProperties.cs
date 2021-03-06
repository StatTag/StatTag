﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StatTag.Core.Models;

namespace StatTag.Controls
{
    public sealed partial class TableProperties : UserControl
    {
        public TableProperties()
        {
            InitializeComponent();
        }

        public void SetTableFormat(TableFormat tableFormat)
        {
            if (tableFormat == null)
            {
                return;
            }

            if (tableFormat.ColumnFilter == null || !tableFormat.ColumnFilter.Enabled)
            {
                chkExcludeColumns.Checked = false;
                txtColumns.Text = Constants.TableParameterDefaults.FilterValue;
            }
            else
            {
                chkExcludeColumns.Checked = tableFormat.ColumnFilter.Enabled;
                txtColumns.Text = tableFormat.ColumnFilter.Value;
            }

            if (tableFormat.RowFilter == null || !tableFormat.RowFilter.Enabled)
            {
                chkExcludeRows.Checked = false;
                txtRows.Text = Constants.TableParameterDefaults.FilterValue;
            }
            else
            {
                chkExcludeRows.Checked = tableFormat.RowFilter.Enabled;
                txtRows.Text = tableFormat.RowFilter.Value;
            }
        }

        public void SetValueFormat(ValueFormat valueFormat)
        {
            valueProperties1.SetValueFormat(valueFormat);
        }

        public TableFormat GetTableFormat()
        {
            var tableFormat = new TableFormat();
            if (chkExcludeColumns.Checked && !string.IsNullOrWhiteSpace(txtColumns.Text))
            {
                tableFormat.ColumnFilter.Enabled = chkExcludeColumns.Checked;
                tableFormat.ColumnFilter.Type = Constants.FilterType.Exclude;
                tableFormat.ColumnFilter.Value = txtColumns.Text;
            }

            if (chkExcludeRows.Checked && !string.IsNullOrWhiteSpace(txtRows.Text))
            {
                tableFormat.RowFilter.Enabled = chkExcludeRows.Checked; 
                tableFormat.RowFilter.Type = Constants.FilterType.Exclude;
                tableFormat.RowFilter.Value = txtRows.Text;
            }
            return tableFormat;
        }

        public ValueFormat GetValueFormat()
        {
            var valueFormat = valueProperties1.GetValueFormat();
            // Tables will have rows and columns, and so we will allow non-numeric types to flow
            // through when inserting results into the document.
            valueFormat.AllowInvalidTypes = true;
            return valueFormat;
        }

        private void chkExcludeRows_CheckedChanged(object sender, EventArgs e)
        {
            txtRows.Enabled = chkExcludeRows.Checked;
        }

        private void chkExcludeColumns_CheckedChanged(object sender, EventArgs e)
        {
            txtColumns.Enabled = chkExcludeColumns.Checked;
        }
    }
}
