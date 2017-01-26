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
    public sealed partial class DateTimeValueProperties : UserControl
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
            var dateFormats = new Dictionary<string, string>
            {
                { "", "" },
                { "03/14/2001", Constants.DateFormats.MMDDYYYY },
                { "March 14, 2001", Constants.DateFormats.MonthDDYYYY },
            };
            cboDate.DataSource = new BindingSource(dateFormats, null);
            cboDate.DisplayMember = "Key";
            cboDate.ValueMember = "Value";

            var timeFormats = new Dictionary<string, string>
            {
                {"", ""},
                {"19:30", Constants.TimeFormats.HHMM},
                {"19:30:50", Constants.TimeFormats.HHMMSS}
            };
            cboTime.DataSource = new BindingSource(dateFormats, null);
            cboTime.DisplayMember = "Key";
            cboTime.ValueMember = "Value";

            UpdateValues();
        }

        private void UpdateDate()
        {
            DateFormat = (cboDate.Enabled ? cboDate.SelectedValue as string : string.Empty);
        }

        private void UpdateTime()
        {
            TimeFormat = (cboTime.Enabled ? cboTime.SelectedValue as string : string.Empty);
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
                cboDate.SelectedValue = DateFormat;
                chkShowDate.Checked = true;
            }

            if (!string.IsNullOrWhiteSpace(TimeFormat))
            {
                cboTime.SelectedValue = TimeFormat;
                chkShowTime.Checked = true;
            }
        }
    }
}
