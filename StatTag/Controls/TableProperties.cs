using System;
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
            Font = UIUtility.CreateScaledFont(Font, CreateGraphics());
        }

        public void SetTableFormat(TableFormat tableFormat)
        {
            chkExcludeColumns.Checked = tableFormat.IncludeColumnNames;
            chkExcludeRows.Checked = tableFormat.IncludeRowNames;
        }

        public void SetValueFormat(ValueFormat valueFormat)
        {
            numericValueProperties.UseThousands = valueFormat.UseThousands;
            numericValueProperties.DecimalPlaces = valueFormat.DecimalPlaces;
            numericValueProperties.UpdateValues();
        }

        public TableFormat GetTableFormat()
        {
            return new TableFormat()
            {
                IncludeColumnNames = chkExcludeColumns.Checked,
                IncludeRowNames = chkExcludeRows.Checked
            };
        }

        public ValueFormat GetValueFormat()
        {
            return new ValueFormat()
            {
                FormatType = Constants.ValueFormatType.Numeric,
                UseThousands = numericValueProperties.UseThousands,
                DecimalPlaces = numericValueProperties.DecimalPlaces,
                // Tables will have rows and columns, and so we will allow non-numeric types to flow
                // through when inserting results into the document.
                AllowInvalidTypes = true
            };
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
