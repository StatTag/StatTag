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
using Microsoft.SqlServer.Server;

namespace StatTag.Controls
{
    public sealed partial class ValueProperties : UserControl
    {
        public ValueProperties()
        {
            InitializeComponent();
        }

        private void CheckChanged(object sender, EventArgs e)
        {
            ShowProperties();
        }

        private void ShowProperties()
        {
            for (int index = 0; index < pnlDetails.Controls.Count; index++)
            {
                pnlDetails.Controls[index].Visible = false;
            }

            Control selectedControl = null;
            if (radNumeric.Checked)
            {
                selectedControl = pnlDetails.Controls.OfType<NumericValueProperties>().FirstOrDefault();
            }
            else if (radDateTime.Checked)
            {
                selectedControl = pnlDetails.Controls.OfType<DateTimeValueProperties>().FirstOrDefault();
            }
            else if (radPercentage.Checked)
            {
                selectedControl = pnlDetails.Controls.OfType<PercentageValueProperties>().FirstOrDefault();
            }

            if (selectedControl != null)
            {
                selectedControl.Visible = true;
            }
        }

        private void ValueProperties_Load(object sender, EventArgs e)
        {
            CreatePropertiesControl<NumericValueProperties>(pnlDetails);
            CreatePropertiesControl<DateTimeValueProperties>(pnlDetails);
            CreatePropertiesControl<PercentageValueProperties>(pnlDetails);
            ShowProperties();
        }

        private T CreatePropertiesControl<T>(Panel panel) where T : Control, new()
        {
            var control = new T();
            panel.Controls.Add(control);
            control.Dock = DockStyle.Fill;
            UIUtility.ScaleContainerFont(control);
            return control;
        }

        public ValueFormat GetValueFormat()
        {
            var format = new ValueFormat();
            if (radDefault.Checked)
            {
                format.FormatType = Constants.ValueFormatType.Default;
            }
            else if (radNumeric.Checked)
            {
                var numProperties = pnlDetails.Controls.OfType<NumericValueProperties>().First();
                format.DecimalPlaces = numProperties.DecimalPlaces;
                format.UseThousands = numProperties.UseThousands;
                format.FormatType = Constants.ValueFormatType.Numeric;
            }
            else if (radDateTime.Checked)
            {
                var dateTimeProperties = pnlDetails.Controls.OfType<DateTimeValueProperties>().First();
                format.DateFormat = dateTimeProperties.DateFormat;
                format.TimeFormat = dateTimeProperties.TimeFormat;
                format.FormatType = Constants.ValueFormatType.DateTime;
            }
            else if (radPercentage.Checked)
            {
                var pctProperties = pnlDetails.Controls.OfType<PercentageValueProperties>().First();
                format.DecimalPlaces = pctProperties.DecimalPlaces;
                format.FormatType = Constants.ValueFormatType.Percentage;
            }
            return format;
        }

        public void SetValueFormat(ValueFormat format)
        {
            if (format.FormatType == Constants.ValueFormatType.Default)
            {
                radDefault.Checked = true;
            }
            else if (format.FormatType == Constants.ValueFormatType.Numeric)
            {
                radNumeric.Checked = true;
                var numProperties = pnlDetails.Controls.OfType<NumericValueProperties>().First();
                numProperties.DecimalPlaces = format.DecimalPlaces;
                numProperties.UseThousands = format.UseThousands;
                numProperties.UpdateValues();
            }
            else if (format.FormatType == Constants.ValueFormatType.DateTime)
            {
                radDateTime.Checked = true;
                var dateTimeProperties = pnlDetails.Controls.OfType<DateTimeValueProperties>().First();
                dateTimeProperties.DateFormat = format.DateFormat;
                dateTimeProperties.TimeFormat = format.TimeFormat;
                dateTimeProperties.UpdateValues();
            }
            else if (format.FormatType == Constants.ValueFormatType.Percentage)
            {
                radPercentage.Checked = true;
                var pctProperties = pnlDetails.Controls.OfType<PercentageValueProperties>().First();
                pctProperties.DecimalPlaces = format.DecimalPlaces;
                pctProperties.UpdateValues();
            }
        }
    }
}
