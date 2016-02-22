using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Controls
{
    public partial class TableProperties : UserControl
    {
        public TableProperties()
        {
            InitializeComponent();
        }

        public void SetTableFormat(TableFormat tableFormat)
        {
            chkIncludeColumnNames.Checked = tableFormat.IncludeColumnNames;
            chkIncludeRowNames.Checked = tableFormat.IncludeRowNames;
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
                IncludeColumnNames = chkIncludeColumnNames.Checked,
                IncludeRowNames = chkIncludeRowNames.Checked
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
    }
}
