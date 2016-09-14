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
    public partial class VerticalLine : UserControl
    {
        public VerticalLine()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Call the OnPaint method of the base class.
            base.OnPaint(e);

            // Declare and instantiate a new pen.
            var pen = new Pen(Color.Gray, 1.0f);

            // Draw an aqua rectangle in the rectangle represented by the control.
            e.Graphics.DrawLine(pen, (this.Width / 2), 2, (this.Width / 2), this.Height - 2);
        }
    }
}
