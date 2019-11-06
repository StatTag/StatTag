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

        private void ShowProperties()
        {
            var selectedType = (cboType.SelectedValue ?? Constants.ValueFormatType.Default).ToString();
            switch (selectedType)
            {
                case Constants.ValueFormatType.Numeric:
                    numericValueProperties1.Visible = true;
                    percentageValueProperties1.Visible = false;
                    dateTimeValueProperties1.Visible = false;
                    lblDefault.Visible = false;
                    break;
                case Constants.ValueFormatType.Percentage:
                    numericValueProperties1.Visible = false;
                    percentageValueProperties1.Visible = true;
                    dateTimeValueProperties1.Visible = false;
                    lblDefault.Visible = false;
                    break;
                case Constants.ValueFormatType.DateTime:
                    numericValueProperties1.Visible = false;
                    percentageValueProperties1.Visible = false;
                    dateTimeValueProperties1.Visible = true;
                    lblDefault.Visible = false;
                    break;
                default:
                    numericValueProperties1.Visible = false;
                    percentageValueProperties1.Visible = false;
                    dateTimeValueProperties1.Visible = false;
                    lblDefault.Visible = true;
                    break;
            }

            //AdjustUIForVisiblePanel();
        }

        private void ValueProperties_Load(object sender, EventArgs e)
        {
            var valueFormats = new Dictionary<string, string>
            {
                {"Default", Constants.ValueFormatType.Default},
                {"Numeric", Constants.ValueFormatType.Numeric},
                {"Percentage", Constants.ValueFormatType.Percentage},
                {"Date/Time", Constants.ValueFormatType.DateTime}
            };
            cboType.DataSource = new BindingSource(valueFormats, null);
            cboType.DisplayMember = "Key";
            cboType.ValueMember = "Value";
            cboType.SelectedValue = Constants.ValueFormatType.Default;
            ShowProperties();
        }

        public ValueFormat GetValueFormat()
        {
            var format = new ValueFormat();
            format.FormatType = cboType.SelectedValue.ToString();
            switch (format.FormatType)
            {
                case Constants.ValueFormatType.Numeric:
                    format.DecimalPlaces = numericValueProperties1.DecimalPlaces;
                    format.UseThousands = numericValueProperties1.UseThousands;
                    break;
                case Constants.ValueFormatType.DateTime:
                    format.DateFormat = dateTimeValueProperties1.DateFormat;
                    format.TimeFormat = dateTimeValueProperties1.TimeFormat;
                    break;
                case Constants.ValueFormatType.Percentage:
                    format.DecimalPlaces = percentageValueProperties1.DecimalPlaces;
                    break;
            }
            return format;
        }

        public void SetValueFormat(ValueFormat format)
        {
            cboType.SelectedValue = format.FormatType;
            ShowProperties();

            if (format.FormatType == Constants.ValueFormatType.Numeric)
            {
                numericValueProperties1.DecimalPlaces = format.DecimalPlaces;
                numericValueProperties1.UseThousands = format.UseThousands;
                numericValueProperties1.UpdateValues();
            }
            else if (format.FormatType == Constants.ValueFormatType.DateTime)
            {
                dateTimeValueProperties1.DateFormat = format.DateFormat;
                dateTimeValueProperties1.TimeFormat = format.TimeFormat;
                dateTimeValueProperties1.UpdateValues();
            }
            else if (format.FormatType == Constants.ValueFormatType.Percentage)
            {
                percentageValueProperties1.DecimalPlaces = format.DecimalPlaces;
                percentageValueProperties1.UpdateValues();
            }
        }
        //private void AdjustUIForVisiblePanel()
        //{
        //    var panels = this.Controls.OfType<Panel>();
        //    foreach (var panel in panels)
        //    {
        //        if (panel.Visible)
        //        {
        //            panel.Top = PanelTop();
        //            this.Height = panel.Top + panel.Height + this.Margin.Bottom + this.Margin.Top;
        //        }
        //    }
        //}

        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowProperties();
        }
    }
}
