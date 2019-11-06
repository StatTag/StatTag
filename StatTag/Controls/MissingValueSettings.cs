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
    public partial class MissingValueSettings : UserControl
    {
        public event EventHandler ValueChanged;

        public MissingValueSettings()
        {
            InitializeComponent();
        }

        public void UpdateDisplay()
        {
            HandleMissingValueRadioChanged(radMissingValueStatDefault.Checked,
                radMissingValueBlankString.Checked,
                radMissingValueCustomString.Checked);
        }

        public void SetCustomMissingValueString(string value)
        {
            txtMissingValueString.Text = value;
        }

        public string GetCustomMissingValueString()
        {
            return txtMissingValueString.Text;
        }
        

        /// <summary>
        /// Helper method to get the (string) constant value that represents the user's
        /// choice for how to represent a missing value.
        /// </summary>
        /// <returns></returns>
        public string GetMissingValuesSelection()
        {
            if (radMissingValueCustomString.Checked)
            {
                return Constants.MissingValueOption.CustomValue;
            }
            else if (radMissingValueStatDefault.Checked)
            {
                return Constants.MissingValueOption.StatPackageDefault;
            }

            // Our default is to use a blank string.
            return Constants.MissingValueOption.BlankString;
        }

        /// <summary>
        /// Helper method to convert a constant string value into the appropriately
        /// selected radio button for missing value handling.
        /// </summary>
        /// <param name="value"></param>
        public void SetMissingValuesSelection(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (value.Equals(Constants.MissingValueOption.CustomValue, StringComparison.CurrentCultureIgnoreCase))
                {
                    radMissingValueCustomString.Checked = true;
                }
                else if (value.Equals(Constants.MissingValueOption.StatPackageDefault, StringComparison.CurrentCultureIgnoreCase))
                {
                    radMissingValueStatDefault.Checked = true;
                }
                else
                {
                    radMissingValueBlankString.Checked = true;
                }
            }

            HandleMissingValueRadioChanged(radMissingValueStatDefault.Checked,
                radMissingValueBlankString.Checked,
                radMissingValueCustomString.Checked);
        }

        private void MissingValueRadio_Changed(object sender, EventArgs e)
        {
            var changedRadioButton = (RadioButton) sender;
            if (!changedRadioButton.Checked)
            {
                return;
            }

            HandleMissingValueRadioChanged((changedRadioButton == radMissingValueStatDefault),
                (changedRadioButton == radMissingValueBlankString),
                (changedRadioButton == radMissingValueCustomString));
        }

        private void HandleMissingValueRadioChanged(bool statDefault, bool blankString, bool customString)
        {
            if (statDefault)
            {
                txtMissingValueString.Enabled = false;
                radMissingValueBlankString.Checked = false;
                radMissingValueCustomString.Checked = false;
            }
            else if (blankString)
            {
                txtMissingValueString.Enabled = false;
                radMissingValueCustomString.Checked = false;
                radMissingValueStatDefault.Checked = false;
            }
            else if (customString)
            {
                txtMissingValueString.Enabled = true;
                radMissingValueBlankString.Checked = false;
                radMissingValueStatDefault.Checked = false;
            }
            else
            {
                // This "should never happen", but if for some reason no radio
                // buttons are selected after a select event, we will force the
                // selection to the first item by default.
                radMissingValueStatDefault.Checked = true;
                radMissingValueBlankString.Checked = false;
                radMissingValueCustomString.Checked = false;
            }

            if (ValueChanged != null)
            {
                ValueChanged(this, new EventArgs());
            }
        }
    }
}
