using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AnalysisManager.Core;
using AnalysisManager.Core.Models;

namespace AnalysisManager
{
    public sealed partial class ManageAnnotation : Form
    {
        public const int SelectedButtonWidth = 83;
        public const int UnselectedButtonWidth = 70;
        public readonly Font SelectedButtonFont = DefaultFont;
        public readonly Font UnselectedButtonFont = DefaultFont;

        public List<CodeFile> Files { get; set; }

        public ManageAnnotation(List<CodeFile> files = null)
        {
            InitializeComponent();
            SelectedButtonFont = Font;
            UnselectedButtonFont = new Font(Font.FontFamily, 8.25f);
            Files = files;
            UpdateForTypeClick(cmdValue);

            cboRunFrequency.Items.AddRange(Utility.StringArrayToObjectArray(Constants.RunFrequency.GetList()));
            if (Files != null)
            {
                cboCodeFiles.Items.AddRange(Utility.StringArrayToObjectArray(Files.Select(x => x.FilePath).ToArray()));
            }
        }

        private void cmdValue_Click(object sender, EventArgs e)
        {
            UpdateForTypeClick(sender as Button);
        }

        private void SelectTypeButton(Button button)
        {
            button.Left = pnlType.Left - SelectedButtonWidth + 2;
            button.Width = SelectedButtonWidth;
            button.BackColor = Color.White;
            button.Font = SelectedButtonFont;
        }

        private void UnselectTypeButton(Button button)
        {
            button.Left = pnlType.Left - UnselectedButtonWidth + 2;
            button.Width = UnselectedButtonWidth;
            button.BackColor = SystemColors.ButtonFace;
            button.Font = UnselectedButtonFont;
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
