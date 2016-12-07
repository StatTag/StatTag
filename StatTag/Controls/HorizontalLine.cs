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
    public partial class HorizontalLine : UserControl
    {
        public HorizontalLine()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Call the OnPaint method of the base class.
            base.OnPaint(e);

            // Declare and instantiate a new pen and draw the line
            var pen = new Pen(Color.Gray, 1.0f);
            e.Graphics.DrawLine(pen, this.Margin.Left, (this.Height / 2), this.Width - this.Margin.Right, (this.Height / 2));
        }
    }
}
