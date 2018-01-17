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
    public partial class MissingValueSettings : UserControl
    {
        public MissingValueSettings()
        {
            InitializeComponent();
        }

        public void UpdateDisplay()
        {
            HandleMissingValueRadioChanged();
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
            else if (radMissingValueBlankString.Checked)
            {
                return Constants.MissingValueOption.BlankString;
            }

            // For previous versions, or just as a last restort, our default is
            // to use the statistical package default value.
            return Constants.MissingValueOption.StatPackageDefault;
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
                else if (value.Equals(Constants.MissingValueOption.BlankString, StringComparison.CurrentCultureIgnoreCase))
                {
                    radMissingValueBlankString.Checked = true;
                }
                else
                {
                    radMissingValueStatDefault.Checked = true;
                }
            }

            HandleMissingValueRadioChanged();
        }

        private void MissingValueRadio_Changed(object sender, EventArgs e)
        {
            HandleMissingValueRadioChanged();
        }

        private void HandleMissingValueRadioChanged()
        {
            if (radMissingValueStatDefault.Checked)
            {
                txtMissingValueString.Enabled = false;
            }
            else if (radMissingValueBlankString.Checked)
            {
                txtMissingValueString.Enabled = false;
            }
            else if (radMissingValueCustomString.Checked)
            {
                txtMissingValueString.Enabled = true;
            }
            else
            {
                // This "should never happen", but if for some reason no radio
                // buttons are selected after a select event, we will force the
                // selection to the first item by default.
                radMissingValueStatDefault.Checked = true;
            }
        }
    }
}