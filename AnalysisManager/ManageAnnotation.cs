using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalysisManager
{
    public partial class ManageAnnotation : Form
    {
        public const int SelectedButtonWidth = 83;
        public const int UnselectedButtonWidth = 75;

        public ManageAnnotation()
        {
            InitializeComponent();
        }

        private void cmdValue_Click(object sender, EventArgs e)
        {
            UpdateForTypeClick(sender as Button);
        }

        private void SelectTypeButton(Button button)
        {
            button.Left = pnlType.Left - SelectedButtonWidth + 1;
            button.Width = SelectedButtonWidth;
            button.BackColor = Color.LightGray;
        }

        private void UnselectTypeButton(Button button)
        {
            button.Left = pnlType.Left - UnselectedButtonWidth + 1;
            button.Width = UnselectedButtonWidth;
            button.BackColor = SystemColors.ButtonFace;
        }

        private void UpdateForTypeClick(Button button)
        {
            SelectTypeButton(button);

            if (button == cmdValue)
            {
                UnselectTypeButton(cmdFigure);
                UnselectTypeButton(cmdTable);
            }
            else if (button == cmdFigure)
            {
                UnselectTypeButton(cmdValue);
                UnselectTypeButton(cmdTable);
            }
            else if (button == cmdTable)
            {
                UnselectTypeButton(cmdValue);
                UnselectTypeButton(cmdFigure);
            }

        }

        private void cmdFigure_Click(object sender, EventArgs e)
        {
            UpdateForTypeClick(sender as Button);
        }

        private void cmdTable_Click(object sender, EventArgs e)
        {
            UpdateForTypeClick(sender as Button);
        }
    }
}
