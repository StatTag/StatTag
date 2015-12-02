using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalysisManager.Controls
{
    public partial class DateTimeValueProperties : UserControl
    {
        public DateTimeValueProperties()
        {
            InitializeComponent();
        }

        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }

        private void chkShowDate_CheckedChanged(object sender, EventArgs e)
        {
            cboDate.Enabled = chkShowDate.Checked;
            UpdateDate();
        }

        private void chkShowTime_CheckedChanged(object sender, EventArgs e)
        {
            cboTime.Enabled = chkShowTime.Checked;
            UpdateTime();
        }

        private void DateTimeValueProperties_Load(object sender, EventArgs e)
        {
            cboDate.Items.AddRange(new object[]
            {
                "03/14/2001",
                "March 14, 2001",
                "Wednesday, March 14, 2001"
            });

            cboTime.Items.AddRange(new object[]
            {
                "7:30:50 pm",
                "19:30",
                "7:30 pm",
                "19:30:50"
            });

            UpdateValues();
        }

        private void UpdateDate()
        {
            DateFormat = (cboDate.Enabled ? cboDate.SelectedItem as string : string.Empty);
        }

        private void UpdateTime()
        {
            TimeFormat = (cboTime.Enabled ? cboTime.SelectedItem as string : string.Empty);
        }

        private void cboDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDate();
        }

        private void cboTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTime();
        }

        public void UpdateValues()
        {
            if (!string.IsNullOrWhiteSpace(DateFormat))
            {
                cboDate.SelectedItem = DateFormat;
                chkShowDate.Checked = true;
            }

            if (!string.IsNullOrWhiteSpace(TimeFormat))
            {
                cboTime.SelectedItem = TimeFormat;
                chkShowTime.Checked = true;
            }
        }
    }
}
