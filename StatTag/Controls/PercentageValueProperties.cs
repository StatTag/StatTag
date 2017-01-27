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
    public sealed partial class PercentageValueProperties : UserControl
    {
        public int DecimalPlaces { get; set; }

        public PercentageValueProperties()
        {
            InitializeComponent();
        }

        private void nudDecimalPlaces_ValueChanged(object sender, EventArgs e)
        {
            DecimalPlaces = (int)nudDecimalPlaces.Value;
        }

        private void PercentageValueProperties_Load(object sender, EventArgs e)
        {
            UpdateValues();
        }

        public void UpdateValues()
        {
            nudDecimalPlaces.Value = DecimalPlaces;
        }
    }
}
