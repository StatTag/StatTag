using AnalysisManager.Controls;
using AnalysisManager.Core;
using AnalysisManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AnalysisManager
{
    public sealed partial class ManageAnnotation : Form
    {
        public const int SelectedButtonWidth = 83;
        public const int UnselectedButtonWidth = 70;
        public readonly Font SelectedButtonFont = DefaultFont;
        public readonly Font UnselectedButtonFont = DefaultFont;

        public List<CodeFile> Files { get; set; }
        public Annotation Annotation { get; set; }

        private string AnnotationType { get; set; }

        public ManageAnnotation(List<CodeFile> files = null)
        {
            InitializeComponent();
            SelectedButtonFont = Font;
            UnselectedButtonFont = new Font(Font.FontFamily, 8.25f);
            Files = files;
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
                AnnotationType = Constants.AnnotationType.Value;
            }
            else if (button == cmdFigure)
            {
                UnselectTypeButton(cmdValue);
                UnselectTypeButton(cmdTable);
                AnnotationType = Constants.AnnotationType.Figure;
            }
            else if (button == cmdTable)
            {
                UnselectTypeButton(cmdValue);
                UnselectTypeButton(cmdFigure);
                AnnotationType = Constants.AnnotationType.Table;
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

        private void ManageAnnotation_Load(object sender, EventArgs e)
        {
            UpdateForTypeClick(cmdValue);

            cboRunFrequency.Items.AddRange(Utility.StringArrayToObjectArray(Constants.RunFrequency.GetList()));
            cboCodeFiles.DisplayMember = "FilePath";
            if (Files != null)
            {
                cboCodeFiles.Items.AddRange(Files.Select(x => x as object).ToArray());
            }

            if (Annotation != null)
            {
                cboCodeFiles.SelectedItem = Annotation.CodeFile;
                cboRunFrequency.SelectedItem = Annotation.RunFrequency;
                txtOutputLabel.Text = Annotation.OutputLabel;
                AnnotationType = Annotation.Type;

                switch (AnnotationType)
                {
                    case Constants.AnnotationType.Value:
                        valueProperties.SetValueFormat(Annotation.ValueFormat);
                        break;
                }
            }
            else
            {
                // If there is only one file available, select it by default
                if (Files != null && Files.Count == 1)
                {
                    cboCodeFiles.SelectedIndex = 0;
                }

                // Always default the run frequency to "Always" (always)
                cboRunFrequency.SelectedItem = Constants.RunFrequency.Always;
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (Annotation == null)
            {
                Annotation = new Annotation();
            }

            Annotation.Type = AnnotationType;
            Annotation.OutputLabel = txtOutputLabel.Text;
            Annotation.RunFrequency = cboRunFrequency.SelectedItem as string;
            Annotation.CodeFile = cboCodeFiles.SelectedItem as CodeFile;

            switch (AnnotationType)
            {
                case Constants.AnnotationType.Value:
                    Annotation.ValueFormat = valueProperties.GetValueFormat();
                    break;
            }
        }
    }
}
