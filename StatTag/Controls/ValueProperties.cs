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
            Font = UIUtility.CreateScaledFont(Font, CreateGraphics());
        }

        private int PanelTop()
        {
            return pnlNumeric.Top;
        }

        private void ShowProperties()
        {
            var selectedType = (cboType.SelectedValue ?? Constants.ValueFormatType.Default).ToString();
            switch (selectedType)
            {
                case Constants.ValueFormatType.Numeric:
                    HideAllButActivePanel(pnlNumeric);
                    break;
                case Constants.ValueFormatType.Percentage:
                    HideAllButActivePanel(pnlPercentage);
                    break;
                case Constants.ValueFormatType.DateTime:
                    HideAllButActivePanel(pnlDateTime);
                    break;
                default:
                    HideAllButActivePanel(pnlDefault);
                    break;
            }

            AdjustUIForVisiblePanel();
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
                    var numProperties = pnlNumeric.Controls.OfType<NumericValueProperties>().First();
                    format.DecimalPlaces = numProperties.DecimalPlaces;
                    format.UseThousands = numProperties.UseThousands;
                    break;
                case Constants.ValueFormatType.DateTime:
                    var dateTimeProperties = pnlDateTime.Controls.OfType<DateTimeValueProperties>().First();
                    format.DateFormat = dateTimeProperties.DateFormat;
                    format.TimeFormat = dateTimeProperties.TimeFormat;
                    break;
                case Constants.ValueFormatType.Percentage:
                    var pctProperties = pnlPercentage.Controls.OfType<PercentageValueProperties>().First();
                    format.DecimalPlaces = pctProperties.DecimalPlaces;
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
                var numProperties = pnlNumeric.Controls.OfType<NumericValueProperties>().First();
                numProperties.DecimalPlaces = format.DecimalPlaces;
                numProperties.UseThousands = format.UseThousands;
                numProperties.UpdateValues();
            }
            else if (format.FormatType == Constants.ValueFormatType.DateTime)
            {
                var dateTimeProperties = pnlDateTime.Controls.OfType<DateTimeValueProperties>().First();
                dateTimeProperties.DateFormat = format.DateFormat;
                dateTimeProperties.TimeFormat = format.TimeFormat;
                dateTimeProperties.UpdateValues();
            }
            else if (format.FormatType == Constants.ValueFormatType.Percentage)
            {
                var pctProperties = pnlPercentage.Controls.OfType<PercentageValueProperties>().First();
                pctProperties.DecimalPlaces = format.DecimalPlaces;
                pctProperties.UpdateValues();
            }
        }

        private void HideAllButActivePanel(Panel panel)
        {
            pnlDefault.Visible = (panel == pnlDefault);
            pnlNumeric.Visible = (panel == pnlNumeric);
            pnlPercentage.Visible = (panel == pnlPercentage);
            pnlDateTime.Visible = (panel == pnlDateTime);
        }

        private void AdjustUIForVisiblePanel()
        {
            var panels = this.Controls.OfType<Panel>();
            foreach (var panel in panels)
            {
                if (panel.Visible)
                {
                    panel.Top = PanelTop();
                    this.Height = panel.Top + panel.Height + this.Margin.Bottom + this.Margin.Top;
                }
            }
        }

        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowProperties();
        }
    }
}
