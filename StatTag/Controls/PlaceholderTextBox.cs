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
    public sealed partial class PlaceholderTextBox : UserControl
    {
        public event EventHandler FilterChanged;

        [Description("The placeholder text that should appear when the textbox is empty"), DisplayName("Placholder Text")]
        public string PlaceholderText
        {
            get { return placeholderText; }
            set
            {
                placeholderText = value;
                if (IsPlaceholderShown)
                {
                    ShowPlaceholder();
                }
            }
        }

        public override string Text
        {
            get
            {
                return IsPlaceholderShown ? string.Empty : textBox.Text;
            }

            set { textBox.Text = value; }
        }

        private bool IsPlaceholderShown { get; set; }
        private string placeholderText = "";

        public PlaceholderTextBox()
        {
            InitializeComponent();
            textBox.Text = PlaceholderText;
            IsPlaceholderShown = true;
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            HidePlaceholder();
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                ShowPlaceholder();
            }
            else
            {
                HidePlaceholder();
            }
        }

        private void ShowPlaceholder()
        {
            IsPlaceholderShown = true; 
            textBox.Font = new Font(textBox.Font, FontStyle.Italic);
            textBox.ForeColor = Color.Gray;
            textBox.Text = PlaceholderText;
        }

        private void HidePlaceholder()
        {
            if (IsPlaceholderShown)
            {
                textBox.Text = string.Empty;
            }

            IsPlaceholderShown = false;
            textBox.Font = new Font(textBox.Font, FontStyle.Regular);
            textBox.ForeColor = Color.Black;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (FilterChanged != null)
            {
                FilterChanged(this, e);
            }
        }
    }
}
