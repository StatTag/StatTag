﻿using AnalysisManager.Controls;
using AnalysisManager.Core;
using AnalysisManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AnalysisManager.Models;

namespace AnalysisManager
{
    public sealed partial class EditAnnotation : Form
    {
        public const int SelectedButtonWidth = 83;
        public const int UnselectedButtonWidth = 70;
        public readonly Font SelectedButtonFont = DefaultFont;
        public readonly Font UnselectedButtonFont = DefaultFont;

        public DocumentManager Manager { get; set; }
        public Annotation Annotation { get; set; }

        private string AnnotationType { get; set; }

        public EditAnnotation(DocumentManager manager = null)
        {
            InitializeComponent();
            SelectedButtonFont = Font;
            UnselectedButtonFont = new Font(Font.FontFamily, 8.25f);
            Manager = manager;
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
                valueProperties.Visible = true;
                figureProperties.Visible = false;
                tableProperties.Visible = false;
                UnselectTypeButton(cmdFigure);
                UnselectTypeButton(cmdTable);
                AnnotationType = Constants.AnnotationType.Value;
            }
            else if (button == cmdFigure)
            {
                valueProperties.Visible = false;
                figureProperties.Visible = true;
                tableProperties.Visible = false;
                UnselectTypeButton(cmdValue);
                UnselectTypeButton(cmdTable);
                AnnotationType = Constants.AnnotationType.Figure;
            }
            else if (button == cmdTable)
            {
                valueProperties.Visible = false;
                figureProperties.Visible = false;
                tableProperties.Visible = true;
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
            OverrideCenterToScreen();

            UpdateForTypeClick(cmdValue);

            cboRunFrequency.Items.AddRange(Utility.StringArrayToObjectArray(Constants.RunFrequency.GetList()));
            cboCodeFiles.DisplayMember = "FilePath";
            if (Manager != null && Manager.Files != null)
            {
                cboCodeFiles.Items.AddRange(Manager.Files.Select(x => x as object).ToArray());
            }

            if (Annotation != null)
            {
                cboCodeFiles.SelectedItem = Annotation.CodeFile;
                cboCodeFiles.Enabled = false;  // We don't allow switching code files
                cboRunFrequency.SelectedItem = Annotation.RunFrequency;
                txtOutputLabel.Text = Annotation.OutputLabel;
                AnnotationType = Annotation.Type;
                LoadCodeFile(Annotation.CodeFile);
                if (Annotation.LineStart.HasValue && Annotation.LineEnd.HasValue)
                {
                    int maxIndex = lstCode.Items.Count - 1;
                    int startIndex = Math.Max(0, Annotation.LineStart.Value);
                    startIndex = Math.Min(startIndex, maxIndex);
                    int endIndex = Math.Min(Annotation.LineEnd.Value, maxIndex);
                    for (int index = startIndex; index <= endIndex; index++)
                    {
                        lstCode.SelectedIndices.Add(index);
                    }
                }

                switch (AnnotationType)
                {
                    case Constants.AnnotationType.Value:
                        UpdateForTypeClick(cmdValue);
                        valueProperties.SetValueFormat(Annotation.ValueFormat);
                        break;
                    case Constants.AnnotationType.Figure:
                        UpdateForTypeClick(cmdFigure);
                        figureProperties.SetFigureFormat(Annotation.FigureFormat);
                        break;
                    case Constants.AnnotationType.Table:
                        UpdateForTypeClick(cmdTable);
                        tableProperties.SetTableFormat(Annotation.TableFormat);
                        tableProperties.SetValueFormat(Annotation.ValueFormat);
                        break;
                }
            }
            else
            {
                // If there is only one file available, select it by default
                if (Manager != null && Manager.Files != null && Manager.Files.Count == 1)
                {
                    cboCodeFiles.SelectedIndex = 0;
                }

                // Default the default run frequency to "Default" (by default)
                cboRunFrequency.SelectedItem = Constants.RunFrequency.Default;
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (Annotation == null)
            {
                Annotation = new Annotation();
            }

            Annotation.Type = AnnotationType;
            Annotation.OutputLabel = Annotation.NormalizeOutputLabel(txtOutputLabel.Text);
            Annotation.RunFrequency = cboRunFrequency.SelectedItem as string;
            Annotation.CodeFile = cboCodeFiles.SelectedItem as CodeFile;
            var selectedIndices = lstCode.SelectedIndices.OfType<int>().ToArray();
            if (!selectedIndices.Any())
            {
                Annotation.LineStart = null;
                Annotation.LineEnd = null;
            }
            else if (selectedIndices.Length == 1)
            {
                Annotation.LineStart = selectedIndices[0];
                Annotation.LineEnd = Annotation.LineStart;
            }
            else
            {
                Annotation.LineStart = selectedIndices.Min();
                Annotation.LineEnd = selectedIndices.Max();
            }

            switch (AnnotationType)
            {
                case Constants.AnnotationType.Value:
                    Annotation.ValueFormat = valueProperties.GetValueFormat();
                    break;
                case Constants.AnnotationType.Figure:
                    Annotation.FigureFormat = figureProperties.GetFigureFormat();
                    break;
                case Constants.AnnotationType.Table:
                    Annotation.TableFormat = tableProperties.GetTableFormat();
                    Annotation.ValueFormat = tableProperties.GetValueFormat();
                    break;
                default:
                    throw new NotSupportedException("This annotation type is not yet supported");
            }
        }

        private void cboCodeFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCodeFile(cboCodeFiles.SelectedItem as CodeFile);
        }

        private void LoadCodeFile(CodeFile file)
        {
            lstCode.Items.Clear();
            if (file != null)
            {
                lstCode.Items.AddRange(Utility.StringArrayToObjectArray(file.Content.ToArray()));
            }
        }

        private void txtOutputLabel_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ignore reserved characters
            if (e.KeyChar == Constants.ReservedCharacters.AnnotationTableCellDelimiter)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Given that we are invoking this dialog from another thread in some instances, we have workarounds
        /// for how the dialog is displayed.  That includes using this method to center the dialog in the
        /// parent, as it does not center otherwise.
        /// <remarks>From: http://stackoverflow.com/questions/6837463/how-come-centertoscreen-method-centers-the-form-on-the-screen-where-the-cursor-i/6837499#6837499</remarks>
        /// </summary>
        private void OverrideCenterToScreen()
        {
            Screen screen = Screen.FromControl(this);

            Rectangle workingArea = screen.WorkingArea;
            this.Location = new Point()
            {
                X = Math.Max(workingArea.X, workingArea.X + (workingArea.Width - this.Width) / 2),
                Y = Math.Max(workingArea.Y, workingArea.Y + (workingArea.Height - this.Height) / 2)
            };
        }

        private void EditAnnotation_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                var result = Utility.CheckForDuplicateLabels(Annotation, Manager.Files);
                if (result != null && result.Count > 0)
                {
                    if (DialogResult.Yes != MessageBox.Show(
                        string.Format(
                            "The output label you have entered ('{0}') appears in {1} other {2}.  Are you sure you want to use the same label?",
                            Annotation.OutputLabel, result.Count, "file".Pluralize(result.Count)),
                        UIUtility.GetAddInName(), MessageBoxButtons.YesNo))
                    {
                        this.DialogResult = DialogResult.None;
                        e.Cancel = true;
                    }
                }
            }
        }
    }
}
