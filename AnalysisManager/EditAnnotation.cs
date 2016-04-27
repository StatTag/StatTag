using AnalysisManager.Controls;
using AnalysisManager.Core;
using AnalysisManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AnalysisManager.Core.Utility;
using AnalysisManager.Models;
using ScintillaNET;
using Annotation = AnalysisManager.Core.Models.Annotation;

namespace AnalysisManager
{
    public sealed partial class EditAnnotation : Form
    {
        private const int AnnotationMargin = 1;
        private const int AnnotationMarker = 1;
        private const uint AnnotationMask = (1 << AnnotationMarker);

        public const int SelectedButtonWidth = 83;
        public const int UnselectedButtonWidth = 70;
        public readonly Font SelectedButtonFont = DefaultFont;
        public readonly Font UnselectedButtonFont = DefaultFont;

        public DocumentManager Manager { get; set; }
        protected Annotation OriginalAnnotation { get; set; }
        public Annotation Annotation { get; set; }
        public string CodeText { get; set; }

        private string AnnotationType { get; set; }
        private bool ReprocessCodeReview { get; set; }

        public EditAnnotation(DocumentManager manager = null)
        {
            try
            {
                Manager = manager;

                InitializeComponent();
                Font = UIUtility.CreateScaledFont(Font, CreateGraphics());
                SelectedButtonFont = Font;
                UnselectedButtonFont = new Font(Font.FontFamily, 8.25f);
            }
            catch (Exception exc)
            {
                const string CannotLoadDialogMessage =
                    "There was an error trying to load the Annotation dialog.";
                if (Manager != null && Manager.Logger != null)
                {
                    Manager.Logger.WriteMessage(
                        "An exception was caught while trying to construct the EditAnnotation dialog.  Will set the auto-close event");
                    UIUtility.ReportException(exc, CannotLoadDialogMessage, Manager.Logger);
                }
                else
                {
                    MessageBox.Show(CannotLoadDialogMessage, UIUtility.GetAddInName(), MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                this.Shown += new EventHandler(EditAnnotation_CloseOnStart);
            }
        }

        /// <summary>
        /// This is a special-purpose event handler to be used when the form should automatically close, such as
        /// when there is an error trying to construct the dialog because of missing dependencies (e.g. Scintilla).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditAnnotation_CloseOnStart(object sender, EventArgs e)
        {
            Close();
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
            MinimumSize = Size;

            UpdateForTypeClick(cmdValue);

            cboRunFrequency.Items.AddRange(GeneralUtil.StringArrayToObjectArray(Constants.RunFrequency.GetList()));
            cboCodeFiles.DisplayMember = "FilePath";
            if (Manager != null && Manager.Files != null)
            {
                cboCodeFiles.Items.AddRange(Manager.Files.Select(x => x as object).ToArray());
            }

            scintilla1.Margins[0].Width = 40;
            var margin = scintilla1.Margins[AnnotationMargin];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            margin.Mask = Marker.MaskAll;
            margin.Cursor = MarginCursor.Arrow;
            var marker = scintilla1.Markers[AnnotationMarker];
            marker.SetBackColor(Color.DarkSeaGreen);
            marker.Symbol = MarkerSymbol.Background;

            if (Annotation != null)
            {
                OriginalAnnotation = Annotation;
                cboCodeFiles.SelectedItem = Annotation.CodeFile;
                ScintillaManager.ConfigureEditor(scintilla1, Annotation.CodeFile);
                cboCodeFiles.Enabled = false;  // We don't allow switching code files
                cboRunFrequency.SelectedItem = Annotation.RunFrequency;
                txtOutputLabel.Text = Annotation.OutputLabel;
                AnnotationType = Annotation.Type;
                LoadCodeFile(Annotation.CodeFile);
                if (Annotation.LineStart.HasValue && Annotation.LineEnd.HasValue)
                {
                    int maxIndex = scintilla1.Lines.Count - 1;
                    int startIndex = Math.Max(0, Annotation.LineStart.Value);
                    startIndex = Math.Min(startIndex, maxIndex);
                    int endIndex = Math.Min(Annotation.LineEnd.Value, maxIndex);
                    for (int index = startIndex; index <= endIndex; index++)
                    {
                        SetLineMarker(scintilla1.Lines[index], true);
                    }
                    scintilla1.LineScroll(startIndex, 0);
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
                OriginalAnnotation = null;

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

            CodeText = scintilla1.Text;
            Annotation.Type = AnnotationType;
            Annotation.OutputLabel = Annotation.NormalizeOutputLabel(txtOutputLabel.Text);
            Annotation.RunFrequency = cboRunFrequency.SelectedItem as string;
            Annotation.CodeFile = cboCodeFiles.SelectedItem as CodeFile;
            var selectedIndices = GetSelectedIndices();
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
            scintilla1.Text = string.Empty;
            scintilla1.EmptyUndoBuffer();
            if (file != null)
            {
                scintilla1.Text = string.Join("\r\n", file.Content);
                ScintillaManager.ConfigureEditor(scintilla1, file);
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
                if (!AnnotationUtil.ShouldCheckForDuplicateLabel(OriginalAnnotation, Annotation))
                {
                    return;
                }

                var result = AnnotationUtil.CheckForDuplicateLabels(Annotation, Manager.Files);
                if (result != null && result.Count > 0)
                {
                    if (AnnotationUtil.IsDuplicateLabelInSameFile(Annotation, result))
                    {
                        UIUtility.WarningMessageBox(
                            string.Format("The output label you have entered ('{0}') already appears in this file.\r\nPlease give this annotation a unique name before proceeding.", Annotation.OutputLabel), 
                            Manager.Logger);
                        this.DialogResult = DialogResult.None;
                        e.Cancel = true;
                    }
                    else if (DialogResult.Yes != MessageBox.Show(
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

        private void codeCheckWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            using (var automation = new Stata.Automation())
            {
                var commands = e.Argument as string[];
                if (commands.Any(command => automation.IsReturnable(command)))
                {
                    e.Result = false;
                    return;
                }

                e.Result = true;
            }
        }

        private void codeCheckWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (ReprocessCodeReview)
            {
                ReprocessCodeReview = false;
                var selectedText = GetSelectedText();
                codeCheckWorker.RunWorkerAsync(selectedText);
            }
            else
            {
                lblNoOutputWarning.Visible = (bool)e.Result;
            }
        }

        private void scintilla1_MarginClick(object sender, MarginClickEventArgs e)
        {
            if (e.Margin == AnnotationMargin)
            {
                var lineIndex = scintilla1.LineFromPosition(e.Position);
                var line = scintilla1.Lines[lineIndex];

                // Check to see if there are any existing selections.  If so, we need to determine if the newly selected
                // row is a neighbor to the existing selection since we only allow continuous ranges.
                var previousLineIndex = scintilla1.Lines[lineIndex - 1].MarkerPrevious(1 << AnnotationMarker);
                if (previousLineIndex != -1)
                {
                    if (Math.Abs(lineIndex - previousLineIndex) > 1)
                    {
                        if ((e.Modifiers & Keys.Shift) == Keys.Shift)
                        {
                            for (int index = previousLineIndex; index < lineIndex; index++)
                            {
                                SetLineMarker(scintilla1.Lines[index], true);
                            }
                        }
                        else
                        {
                            // Deselect everything
                            while (previousLineIndex > -1)
                            {
                                SetLineMarker(scintilla1.Lines[previousLineIndex], false);
                                previousLineIndex =
                                    scintilla1.Lines[previousLineIndex].MarkerPrevious(1 << AnnotationMarker);
                            }
                        }
                    }
                }
                else
                {
                    var nextLineIndex = scintilla1.Lines[lineIndex + 1].MarkerNext(1 << AnnotationMarker);
                    if (Math.Abs(lineIndex - nextLineIndex) > 1)
                    {
                        if ((e.Modifiers & Keys.Shift) == Keys.Shift)
                        {
                            for (int index = nextLineIndex; index > lineIndex; index--)
                            {
                                SetLineMarker(scintilla1.Lines[index], true);
                            }
                        }
                        else
                        {
                            // Deselect everything
                            while (nextLineIndex > -1)
                            {
                                SetLineMarker(scintilla1.Lines[nextLineIndex], false);
                                nextLineIndex =
                                    scintilla1.Lines[nextLineIndex].MarkerNext(1 << AnnotationMarker);
                            }
                        }
                    }
                }

                // Toggle based on the line's current marker status.
                SetLineMarker(line, (line.MarkerGet() & AnnotationMask) <= 0);

                if (codeCheckWorker.IsBusy)
                {
                    ReprocessCodeReview = true;
                    return;
                }

                var selectedText = GetSelectedText();
                if (selectedText.Length == 0)
                {
                    lblNoOutputWarning.Visible = false;
                }
                else
                {
                    codeCheckWorker.RunWorkerAsync(selectedText);
                }
            }
        }

        private string[] GetSelectedText()
        {
            return GetSelectedLines().Select(x => x.Text).ToArray();
        }

        private int[] GetSelectedIndices()
        {
            return GetSelectedLines().Select(x => x.Index).ToArray();
        }

        private Line[] GetSelectedLines()
        {
            var lines = new List<Line>();
            var nextLineIndex = scintilla1.Lines[0].MarkerNext(1 << AnnotationMarker);
            while (nextLineIndex > -1 && nextLineIndex < scintilla1.Lines.Count)
            {
                lines.Add(scintilla1.Lines[nextLineIndex]);
                if (nextLineIndex == 0 || nextLineIndex == scintilla1.Lines.Count - 1)
                {
                    break;
                }
                nextLineIndex =
                    scintilla1.Lines[nextLineIndex + 1].MarkerNext(1 << AnnotationMarker);
            }
            return lines.ToArray();
        }

        private void SetLineMarker(Line line, bool mark)
        {
            if (mark)
            {
                line.MarkerAdd(AnnotationMarker);
            }
            else
            {
                line.MarkerDelete(AnnotationMarker);
            }
        }

        private void cboCodeFiles_DropDown(object sender, System.EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null)
            {
                return;
            }

            int width = comboBox.DropDownWidth;
            using (Graphics graphics = comboBox.CreateGraphics())
            {
                Font font = comboBox.Font;
                int vertScrollBarWidth = (comboBox.Items.Count > comboBox.MaxDropDownItems) ? SystemInformation.VerticalScrollBarWidth : 0;
                foreach (CodeFile file in comboBox.Items)
                {
                    width = Math.Max(width, (int)graphics.MeasureString(file.FilePath, font).Width + vertScrollBarWidth);
                }
                comboBox.DropDownWidth = width;
            }
        }
    }
}
