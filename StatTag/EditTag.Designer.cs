namespace StatTag
{
    sealed partial class EditTag
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboRunFrequency = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.cboCodeFiles = new System.Windows.Forms.ComboBox();
            this.lblNoOutputWarning = new System.Windows.Forms.Label();
            this.codeCheckWorker = new System.ComponentModel.BackgroundWorker();
            this.cmdSaveAndInsert = new System.Windows.Forms.Button();
            this.pnlType = new System.Windows.Forms.Panel();
            this.lblAllowedCommands = new System.Windows.Forms.Label();
            this.lblInstructionTitle = new System.Windows.Forms.Label();
            this.tableProperties = new StatTag.Controls.TableProperties();
            this.figureProperties = new StatTag.Controls.FigureProperties();
            this.valueProperties = new StatTag.Controls.ValueProperties();
            this.cmdTable = new System.Windows.Forms.Button();
            this.cmdFigure = new System.Windows.Forms.Button();
            this.cmdValue = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.scintilla1 = new ScintillaNET.Scintilla();
            this.pnlType.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(501, 696);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(164, 25);
            this.cmdCancel.TabIndex = 7;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(113, 696);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(164, 25);
            this.cmdOK.TabIndex = 6;
            this.cmdOK.Text = "Save";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Code file:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(167, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "When should this code run:";
            // 
            // cboRunFrequency
            // 
            this.cboRunFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboRunFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRunFrequency.FormattingEnabled = true;
            this.cboRunFrequency.Location = new System.Drawing.Point(203, 39);
            this.cboRunFrequency.Name = "cboRunFrequency";
            this.cboRunFrequency.Size = new System.Drawing.Size(242, 25);
            this.cboRunFrequency.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 245);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(203, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Click in the margin to define tags:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 17);
            this.label4.TabIndex = 14;
            this.label4.Text = "Tag name:";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(203, 73);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(563, 25);
            this.txtName.TabIndex = 15;
            this.txtName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtName_KeyPress);
            // 
            // cboCodeFiles
            // 
            this.cboCodeFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCodeFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCodeFiles.FormattingEnabled = true;
            this.cboCodeFiles.Location = new System.Drawing.Point(203, 8);
            this.cboCodeFiles.Name = "cboCodeFiles";
            this.cboCodeFiles.Size = new System.Drawing.Size(563, 25);
            this.cboCodeFiles.TabIndex = 21;
            this.cboCodeFiles.DropDown += new System.EventHandler(this.cboCodeFiles_DropDown);
            this.cboCodeFiles.SelectedIndexChanged += new System.EventHandler(this.cboCodeFiles_SelectedIndexChanged);
            // 
            // lblNoOutputWarning
            // 
            this.lblNoOutputWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNoOutputWarning.AutoSize = true;
            this.lblNoOutputWarning.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoOutputWarning.ForeColor = System.Drawing.Color.Red;
            this.lblNoOutputWarning.Location = new System.Drawing.Point(353, 244);
            this.lblNoOutputWarning.Name = "lblNoOutputWarning";
            this.lblNoOutputWarning.Size = new System.Drawing.Size(414, 17);
            this.lblNoOutputWarning.TabIndex = 23;
            this.lblNoOutputWarning.Text = "WARNING: The selected region of code cannot output to StatTag.";
            this.lblNoOutputWarning.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblNoOutputWarning.Visible = false;
            // 
            // codeCheckWorker
            // 
            this.codeCheckWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.codeCheckWorker_DoWork);
            this.codeCheckWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.codeCheckWorker_RunWorkerCompleted);
            // 
            // cmdSaveAndInsert
            // 
            this.cmdSaveAndInsert.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdSaveAndInsert.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdSaveAndInsert.Location = new System.Drawing.Point(307, 696);
            this.cmdSaveAndInsert.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdSaveAndInsert.Name = "cmdSaveAndInsert";
            this.cmdSaveAndInsert.Size = new System.Drawing.Size(164, 25);
            this.cmdSaveAndInsert.TabIndex = 26;
            this.cmdSaveAndInsert.Text = "Save and Insert in Word";
            this.cmdSaveAndInsert.UseVisualStyleBackColor = true;
            this.cmdSaveAndInsert.Click += new System.EventHandler(this.cmdSaveAndInsert_Click);
            // 
            // pnlType
            // 
            this.pnlType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlType.BackColor = System.Drawing.Color.White;
            this.pnlType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlType.Controls.Add(this.lblAllowedCommands);
            this.pnlType.Controls.Add(this.lblInstructionTitle);
            this.pnlType.Controls.Add(this.tableProperties);
            this.pnlType.Controls.Add(this.figureProperties);
            this.pnlType.Controls.Add(this.valueProperties);
            this.pnlType.Location = new System.Drawing.Point(95, 122);
            this.pnlType.Name = "pnlType";
            this.pnlType.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.pnlType.Size = new System.Drawing.Size(669, 110);
            this.pnlType.TabIndex = 28;
            // 
            // lblAllowedCommands
            // 
            this.lblAllowedCommands.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAllowedCommands.Location = new System.Drawing.Point(397, 44);
            this.lblAllowedCommands.Name = "lblAllowedCommands";
            this.lblAllowedCommands.Size = new System.Drawing.Size(267, 60);
            this.lblAllowedCommands.TabIndex = 4;
            this.lblAllowedCommands.Text = "(None specified)";
            // 
            // lblInstructionTitle
            // 
            this.lblInstructionTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInstructionTitle.Location = new System.Drawing.Point(381, 6);
            this.lblInstructionTitle.Name = "lblInstructionTitle";
            this.lblInstructionTitle.Size = new System.Drawing.Size(283, 34);
            this.lblInstructionTitle.TabIndex = 3;
            this.lblInstructionTitle.Text = "The following Stata commands can be used for Value results:\r\n";
            // 
            // tableProperties
            // 
            this.tableProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableProperties.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableProperties.Location = new System.Drawing.Point(6, 4);
            this.tableProperties.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableProperties.Name = "tableProperties";
            this.tableProperties.Size = new System.Drawing.Size(369, 100);
            this.tableProperties.TabIndex = 2;
            // 
            // figureProperties
            // 
            this.figureProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.figureProperties.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.figureProperties.Location = new System.Drawing.Point(6, 4);
            this.figureProperties.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.figureProperties.Name = "figureProperties";
            this.figureProperties.Size = new System.Drawing.Size(369, 100);
            this.figureProperties.TabIndex = 1;
            // 
            // valueProperties
            // 
            this.valueProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueProperties.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valueProperties.Location = new System.Drawing.Point(6, 2);
            this.valueProperties.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.valueProperties.Name = "valueProperties";
            this.valueProperties.Size = new System.Drawing.Size(369, 100);
            this.valueProperties.TabIndex = 0;
            // 
            // cmdTable
            // 
            this.cmdTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdTable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdTable.Location = new System.Drawing.Point(21, 180);
            this.cmdTable.Name = "cmdTable";
            this.cmdTable.Size = new System.Drawing.Size(75, 30);
            this.cmdTable.TabIndex = 31;
            this.cmdTable.Text = "Table";
            this.cmdTable.UseVisualStyleBackColor = true;
            this.cmdTable.Click += new System.EventHandler(this.cmdTable_Click);
            // 
            // cmdFigure
            // 
            this.cmdFigure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdFigure.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdFigure.Location = new System.Drawing.Point(21, 151);
            this.cmdFigure.Name = "cmdFigure";
            this.cmdFigure.Size = new System.Drawing.Size(75, 30);
            this.cmdFigure.TabIndex = 30;
            this.cmdFigure.Text = "Figure";
            this.cmdFigure.UseVisualStyleBackColor = true;
            this.cmdFigure.Click += new System.EventHandler(this.cmdFigure_Click);
            // 
            // cmdValue
            // 
            this.cmdValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdValue.BackColor = System.Drawing.Color.White;
            this.cmdValue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdValue.Location = new System.Drawing.Point(13, 122);
            this.cmdValue.Name = "cmdValue";
            this.cmdValue.Size = new System.Drawing.Size(83, 30);
            this.cmdValue.TabIndex = 29;
            this.cmdValue.Text = "Value";
            this.cmdValue.UseVisualStyleBackColor = false;
            this.cmdValue.Click += new System.EventHandler(this.cmdValue_Click);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(355, 17);
            this.label5.TabIndex = 27;
            this.label5.Text = "How should these results be accessible from the document:";
            // 
            // scintilla1
            // 
            this.scintilla1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scintilla1.AnnotationVisible = ScintillaNET.Annotation.Standard;
            this.scintilla1.Lexer = ScintillaNET.Lexer.R;
            this.scintilla1.Location = new System.Drawing.Point(16, 265);
            this.scintilla1.Name = "scintilla1";
            this.scintilla1.Size = new System.Drawing.Size(751, 408);
            this.scintilla1.TabIndex = 24;
            this.scintilla1.MarginClick += new System.EventHandler<ScintillaNET.MarginClickEventArgs>(this.scintilla1_MarginClick);
            // 
            // EditTag
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(779, 734);
            this.Controls.Add(this.pnlType);
            this.Controls.Add(this.cmdTable);
            this.Controls.Add(this.cmdFigure);
            this.Controls.Add(this.cmdValue);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmdSaveAndInsert);
            this.Controls.Add(this.scintilla1);
            this.Controls.Add(this.lblNoOutputWarning);
            this.Controls.Add(this.cboCodeFiles);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cboRunFrequency);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "EditTag";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tag";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditTag_FormClosing);
            this.Load += new System.EventHandler(this.ManageTag_Load);
            this.pnlType.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboRunFrequency;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.ComboBox cboCodeFiles;
        private System.Windows.Forms.Label lblNoOutputWarning;
        private System.ComponentModel.BackgroundWorker codeCheckWorker;
        private ScintillaNET.Scintilla scintilla1;
        private System.Windows.Forms.Button cmdSaveAndInsert;
        private System.Windows.Forms.Panel pnlType;
        private Controls.TableProperties tableProperties;
        private Controls.FigureProperties figureProperties;
        private Controls.ValueProperties valueProperties;
        private System.Windows.Forms.Button cmdTable;
        private System.Windows.Forms.Button cmdFigure;
        private System.Windows.Forms.Button cmdValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblAllowedCommands;
        private System.Windows.Forms.Label lblInstructionTitle;
    }
}