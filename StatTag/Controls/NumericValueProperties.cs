using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StatTag.Controls
{
    public sealed partial class NumericValueProperties : UserControl
    {
        public int DecimalPlaces { get; set; }
        public bool UseThousands { get; set; }

        public NumericValueProperties()
        {
            InitializeComponent();
            Font = UIUtility.CreateScaledFont(Font, CreateGraphics());
        }

        private void NumericValueProperties_Load(object sender, EventArgs e)
        {
            UpdateValues();
        }

        public void UpdateValues()
        {
            nudDecimalPlaces.Value = DecimalPlaces;
            chkThousandSeparator.Checked = UseThousands;
        }

        private void nudDecimalPlaces_ValueChanged(object sender, EventArgs e)
        {
            DecimalPlaces = (int)nudDecimalPlaces.Value;
        }

        private void chkThousandSeparator_CheckedChanged(object sender, EventArgs e)
        {
            UseThousands = chkThousandSeparator.Checked;
        }
    }
}
