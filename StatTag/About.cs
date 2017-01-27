using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StatTag
{
    public sealed partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            UIUtility.ScaleFont(this);
        }

        private void About_Load(object sender, EventArgs e)
        {
            lblVersion.Text = UIUtility.GetVersionLabel();
            lblCopyright.Text = UIUtility.GetCopyright();
        }
    }
}
