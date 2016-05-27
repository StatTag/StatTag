using StatTag.Controls;
using StatTag.Core;
using StatTag.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StatTag.Core.Utility;
using StatTag.Models;
using ScintillaNET;
using Tag = StatTag.Core.Models.Tag;

namespace StatTag
{
    public sealed partial class EditTag : Form
    {
        private const int TagMargin = 1;
        private const int TagMarker = 1;
        private const uint TagMask = (1 << TagMarker);
        private const string CannotLoadDialogMessage =
            "There was an error trying to load the Tag dialog.";

        public const int SelectedButtonWidth = 83;
        public const int UnselectedButtonWidth = 70;
        public readonly Font SelectedButtonFont = DefaultFont;
        public readonly Font UnselectedButtonFont = DefaultFont;

        public DocumentManager Manager { get; set; }
        private Tag OriginalTag { get; set; }
        public Tag Tag { get; set; }
        public string CodeText { get; set; }
        public bool InsertInDocument { get; private set; }

        private string TagType { get; set; }
        private bool ReprocessCodeReview { get; set; }

        public EditTag(bool allowInsertInDocument, DocumentManager manager = null)
        {
            try
            {
                Manager = manager;

                InitializeComponent();
                Font = UIUtility.CreateScaledFont(Font, CreateGraphics());
                SelectedButtonFont = Font;
                UnselectedButtonFont = new Font(Font.FontFamily, 8.25f);
                UIUtility.SetDialogTitle(this);
                cmdSaveAndInsert.Enabled = allowInsertInDocument;
            }
            catch (Exception exc)
            {
                if (Manager != null && Manager.Logger != null)
                {
                    Manager.Logger.WriteMessage(
                        "An exception was caught while trying to construct the EditTag dialog.  Will set the auto-close event");
                    UIUtility.ReportException(exc, CannotLoadDialogMessage, Manager.Logger);
                }
                else
                {
                    MessageBox.Show(CannotLoadDialogMessage, UIUtility.GetAddInName(), MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                this.Shown += new EventHandler(EditTag_CloseOnStart);
            }
        }

        /// <summary>
        /// This is a special-purpose event handler to be used when the form should automatically close, such as
        /// when there is an error trying to construct the dialog because of missing dependencies (e.g. Scintilla).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTag_CloseOnStart(object sender, EventArgs e)
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
                TagType = Constants.TagType.Value;
            }
            else if (button == cmdFigure)
            {
                valueProperties.Visible = false;
                figureProperties.Visible = true;
                tableProperties.Visible = false;
                UnselectTypeButton(cmdValue);
                UnselectTypeButton(cmdTable);
                TagType = Constants.TagType.Figure;
            }
            else if (button == cmdTable)
            {
                valueProperties.Visible = false;
                figureProperties.Visible = false;
                tableProperties.Visible = true;
                UnselectTypeButton(cmdValue);
                UnselectTypeButton(cmdFigure);
                TagType = Constants.TagType.Table;
            }

            SetInstructionText();
        }

        private void SetInstructionText()
        {
            var selectedCodeFile = cboCodeFiles.SelectedItem as CodeFile;
            var statPackage = (selectedCodeFile == null) ? "tag" : selectedCodeFile.StatisticalPackage;
            lblInstructionTitle.Text = string.Format("The following {0} commands may be used for {1} output:",
                statPackage, TagType);
            var commandList = UIUtility.GetResultCommandList(selectedCodeFile, TagType);
            lblAllowedCommands.Text = commandList == null ? "(None specified)" : string.Join("\r\n", commandList.GetCommands());
        }

        private void cmdFigure_Click(object sender, EventArgs e)
        {
            UpdateForTypeClick(sender as Button);
        }

        private void cmdTable_Click(object sender, EventArgs e)
        {
            UpdateForTypeClick(sender as Button);
        }

        private void ManageTag_Load(object sender, EventArgs e)
        {
            OverrideCenterToScreen();
            MinimumSize = Size;

            UpdateForTypeClick(cmdValue);

            cboRunFrequency.Items.AddRange(GeneralUtil.StringArrayToObjectArray(Constants.RunFrequency.GetList()));
            cboCodeFiles.DisplayMember = "FilePath";
            if (Manager != null)
            {
                var files = Manager.GetCodeFileList();
                if (files != null)
                {
                    cboCodeFiles.Items.AddRange(files.Select(x => x as object).ToArray());
                }
            }

            scintilla1.Margins[0].Width = 40;
            var margin = scintilla1.Margins[TagMargin];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            margin.Mask = Marker.MaskAll;
            margin.Cursor = MarginCursor.Arrow;
            var marker = scintilla1.Markers[TagMarker];
            marker.SetBackColor(Color.FromArgb(0, 204, 196, 223));
            //marker.SetBackColor(Color.DarkSeaGreen);
            marker.Symbol = MarkerSymbol.Background;

            if (Tag != null)
            {
                OriginalTag = new Tag(Tag);
                cboCodeFiles.SelectedItem = Tag.CodeFile;
                ScintillaManager.ConfigureEditor(scintilla1, Tag.CodeFile);
                cboCodeFiles.Enabled = false;  // We don't allow switching code files
                cboRunFrequency.SelectedItem = Tag.RunFrequency;
                txtName.Text = Tag.Name;
                TagType = Tag.Type;
                LoadCodeFile(Tag.CodeFile);
                if (Tag.LineStart.HasValue && Tag.LineEnd.HasValue)
                {
                    int maxIndex = scintilla1.Lines.Count - 1;
                    int startIndex = Math.Max(0, Tag.LineStart.Value);
                    startIndex = Math.Min(startIndex, maxIndex);
                    int endIndex = Math.Min(Tag.LineEnd.Value, maxIndex);
                    for (int index = startIndex; index <= endIndex; index++)
                    {
                        SetLineMarker(scintilla1.Lines[index], true);
                    }
                    scintilla1.LineScroll(startIndex, 0);
                }

                switch (TagType)
                {
                    case Constants.TagType.Value:
                        UpdateForTypeClick(cmdValue);
                        valueProperties.SetValueFormat(Tag.ValueFormat);
                        break;
                    case Constants.TagType.Figure:
                        UpdateForTypeClick(cmdFigure);
                        figureProperties.SetFigureFormat(Tag.FigureFormat);
                        break;
                    case Constants.TagType.Table:
                        UpdateForTypeClick(cmdTable);
                        tableProperties.SetTableFormat(Tag.TableFormat);
                        tableProperties.SetValueFormat(Tag.ValueFormat);
                        break;
                }
            }
            else
            {
                OriginalTag = null;

                // If there is only one file available, select it by default
                if (Manager != null)
                {
                    var files = Manager.GetCodeFileList();
                    if (files != null && files.Count == 1)
                    {
                        cboCodeFiles.SelectedIndex = 0;
                    }
                }

                // Default the default run frequency to "Default" (by default)
                cboRunFrequency.SelectedItem = Constants.RunFrequency.Always;
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            HandleOkClick(false);
        }

        private void cboCodeFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCodeFile(cboCodeFiles.SelectedItem as CodeFile);
        }

        private void LoadCodeFile(CodeFile file)
        {
            scintilla1.Text = string.Empty;
            if (file != null)
            {
                scintilla1.Text = string.Join("\r\n", file.Content);
                ScintillaManager.ConfigureEditor(scintilla1, file);
            }
            scintilla1.EmptyUndoBuffer();

            SetInstructionText();
        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ignore reserved characters
            if (e.KeyChar == Constants.ReservedCharacters.TagTableCellDelimiter)
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

        private void EditTag_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                if (!TagUtil.ShouldCheckForDuplicateLabel(OriginalTag, Tag))
                {
                    return;
                }

                var files = Manager.GetCodeFileList();
                var result = TagUtil.CheckForDuplicateLabels(Tag, files);
                if (result != null && result.Count > 0)
                {
                    if (TagUtil.IsDuplicateLabelInSameFile(Tag, result))
                    {
                        UIUtility.WarningMessageBox(
                            string.Format("The tag name you have entered ('{0}') already appears in this file.\r\nPlease give this tag a unique name before proceeding.", Tag.Name), 
                            Manager.Logger);
                        this.DialogResult = DialogResult.None;
                        e.Cancel = true;
                    }
                    else if (DialogResult.Yes != MessageBox.Show(
                        string.Format(
                            "The tag name you have entered ('{0}') appears in {1} other {2}.  Are you sure you want to use the same label?",
                            Tag.Name, result.Count, "file".Pluralize(result.Count)),
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
                if (commands != null && commands.Any(command => automation.IsReturnable(command)))
                {
                    e.Result = false;
                    return;
                }

                e.Result = true;
            }
        }

        private void SetWarningDisplay(bool warning)
        {
            lblNoOutputWarning.Visible = warning;
            lblInstructionTitle.Font = UIUtility.ToggleBoldFont(lblInstructionTitle, warning);
            lblAllowedCommands.Font = lblInstructionTitle.Font;
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
                bool warning = (bool) e.Result;
                SetWarningDisplay(warning);
            }
        }

        private void scintilla1_MarginClick(object sender, MarginClickEventArgs e)
        {
            if (e.Margin == TagMargin)
            {
                var lineIndex = scintilla1.LineFromPosition(e.Position);
                var line = scintilla1.Lines[lineIndex];

                // Check to see if there are any existing selections.  If so, we need to determine if the newly selected
                // row is a neighbor to the existing selection since we only allow continuous ranges.
                var previousLineIndex = scintilla1.Lines[lineIndex - 1].MarkerPrevious(1 << TagMarker);
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
                                    scintilla1.Lines[previousLineIndex].MarkerPrevious(1 << TagMarker);
                            }
                        }
                    }
                }
                else
                {
                    var nextLineIndex = scintilla1.Lines[lineIndex + 1].MarkerNext(1 << TagMarker);
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
                                    scintilla1.Lines[nextLineIndex].MarkerNext(1 << TagMarker);
                            }
                        }
                    }
                }

                // Toggle based on the line's current marker status.
                SetLineMarker(line, (line.MarkerGet() & TagMask) <= 0);

                if (codeCheckWorker.IsBusy)
                {
                    ReprocessCodeReview = true;
                    return;
                }

                var selectedText = GetSelectedText();
                if (selectedText.Length == 0)
                {
                    SetWarningDisplay(false);
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
            var nextLineIndex = scintilla1.Lines[0].MarkerNext(1 << TagMarker);
            while (nextLineIndex > -1 && nextLineIndex < scintilla1.Lines.Count)
            {
                lines.Add(scintilla1.Lines[nextLineIndex]);
                if (nextLineIndex == 0 || nextLineIndex == scintilla1.Lines.Count - 1)
                {
                    break;
                }
                nextLineIndex =
                    scintilla1.Lines[nextLineIndex + 1].MarkerNext(1 << TagMarker);
            }
            return lines.ToArray();
        }

        private void SetLineMarker(Line line, bool mark)
        {
            if (mark)
            {
                line.MarkerAdd(TagMarker);
            }
            else
            {
                line.MarkerDelete(TagMarker);
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

        private void cmdSaveAndInsert_Click(object sender, EventArgs e)
        {
            HandleOkClick(true);
        }

        private void HandleOkClick(bool insertInDocument)
        {
            InsertInDocument = insertInDocument;

            if (Tag == null)
            {
                Tag = new Tag();
            }

            CodeText = scintilla1.Text;
            Tag.Type = TagType;
            Tag.Name = Tag.NormalizeName(txtName.Text);
            Tag.RunFrequency = cboRunFrequency.SelectedItem as string;
            Tag.CodeFile = cboCodeFiles.SelectedItem as CodeFile;
            var selectedIndices = GetSelectedIndices();
            if (!selectedIndices.Any())
            {
                Tag.LineStart = null;
                Tag.LineEnd = null;
            }
            else if (selectedIndices.Length == 1)
            {
                Tag.LineStart = selectedIndices[0];
                Tag.LineEnd = Tag.LineStart;
            }
            else
            {
                Tag.LineStart = selectedIndices.Min();
                Tag.LineEnd = selectedIndices.Max();
            }

            switch (TagType)
            {
                case Constants.TagType.Value:
                    Tag.ValueFormat = valueProperties.GetValueFormat();
                    break;
                case Constants.TagType.Figure:
                    Tag.FigureFormat = figureProperties.GetFigureFormat();
                    break;
                case Constants.TagType.Table:
                    Tag.TableFormat = tableProperties.GetTableFormat();
                    Tag.ValueFormat = tableProperties.GetValueFormat();
                    break;
                default:
                    throw new NotSupportedException("This tag type is not yet supported");
            }
        }
    }
}
