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
            UnselectedButtonFont.Dispose();
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
            this.label3 = new System.Windows.Forms.Label();
            this.cboCodeFiles = new System.Windows.Forms.ComboBox();
            this.lblNoOutputWarning = new System.Windows.Forms.Label();
            this.codeCheckWorker = new System.ComponentModel.BackgroundWorker();
            this.cmdSaveAndInsert = new System.Windows.Forms.Button();
            this.lblAllowedCommands = new System.Windows.Forms.Label();
            this.lblInstructionTitle = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboRunFrequency = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.figureProperties = new StatTag.Controls.FigureProperties();
            this.tableProperties = new StatTag.Controls.TableProperties();
            this.horizontalLine1 = new StatTag.Controls.HorizontalLine();
            this.valueProperties = new StatTag.Controls.ValueProperties();
            this.cboResultType = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.scintilla1 = new ScintillaNET.Scintilla();
            this.verticalLine1 = new StatTag.Controls.VerticalLine();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(657, 603);
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
            this.cmdOK.Location = new System.Drawing.Point(269, 603);
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(203, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Click in the margin to define tags:";
            // 
            // cboCodeFiles
            // 
            this.cboCodeFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCodeFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCodeFiles.FormattingEnabled = true;
            this.cboCodeFiles.Location = new System.Drawing.Point(82, 10);
            this.cboCodeFiles.Name = "cboCodeFiles";
            this.cboCodeFiles.Size = new System.Drawing.Size(666, 25);
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
            this.lblNoOutputWarning.Location = new System.Drawing.Point(334, 38);
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
            this.cmdSaveAndInsert.Location = new System.Drawing.Point(463, 603);
            this.cmdSaveAndInsert.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdSaveAndInsert.Name = "cmdSaveAndInsert";
            this.cmdSaveAndInsert.Size = new System.Drawing.Size(164, 25);
            this.cmdSaveAndInsert.TabIndex = 26;
            this.cmdSaveAndInsert.Text = "Save and Insert in Word";
            this.cmdSaveAndInsert.UseVisualStyleBackColor = true;
            this.cmdSaveAndInsert.Click += new System.EventHandler(this.cmdSaveAndInsert_Click);
            // 
            // lblAllowedCommands
            // 
            this.lblAllowedCommands.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAllowedCommands.Location = new System.Drawing.Point(16, 499);
            this.lblAllowedCommands.Name = "lblAllowedCommands";
            this.lblAllowedCommands.Size = new System.Drawing.Size(280, 62);
            this.lblAllowedCommands.TabIndex = 4;
            this.lblAllowedCommands.Text = "(None specified)";
            // 
            // lblInstructionTitle
            // 
            this.lblInstructionTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInstructionTitle.Location = new System.Drawing.Point(13, 459);
            this.lblInstructionTitle.Name = "lblInstructionTitle";
            this.lblInstructionTitle.Size = new System.Drawing.Size(283, 40);
            this.lblInstructionTitle.TabIndex = 3;
            this.lblInstructionTitle.Text = "The following Stata commands can be used for Value results:\r\n";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(94, 28);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(205, 25);
            this.txtName.TabIndex = 35;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 17);
            this.label4.TabIndex = 34;
            this.label4.Text = "Name";
            // 
            // cboRunFrequency
            // 
            this.cboRunFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboRunFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRunFrequency.FormattingEnabled = true;
            this.cboRunFrequency.Location = new System.Drawing.Point(94, 59);
            this.cboRunFrequency.Name = "cboRunFrequency";
            this.cboRunFrequency.Size = new System.Drawing.Size(205, 25);
            this.cboRunFrequency.TabIndex = 33;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 17);
            this.label2.TabIndex = 32;
            this.label2.Text = "Refresh";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.figureProperties);
            this.panel1.Controls.Add(this.tableProperties);
            this.panel1.Controls.Add(this.horizontalLine1);
            this.panel1.Controls.Add(this.lblAllowedCommands);
            this.panel1.Controls.Add(this.valueProperties);
            this.panel1.Controls.Add(this.cboResultType);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.lblInstructionTitle);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txtName);
            this.panel1.Controls.Add(this.cboRunFrequency);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(767, 10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(312, 570);
            this.panel1.TabIndex = 41;
            // 
            // figureProperties
            // 
            this.figureProperties.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.figureProperties.Location = new System.Drawing.Point(6, 134);
            this.figureProperties.Name = "figureProperties";
            this.figureProperties.Size = new System.Drawing.Size(293, 20);
            this.figureProperties.TabIndex = 43;
            // 
            // tableProperties
            // 
            this.tableProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableProperties.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableProperties.Location = new System.Drawing.Point(6, 134);
            this.tableProperties.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableProperties.Name = "tableProperties";
            this.tableProperties.Size = new System.Drawing.Size(293, 309);
            this.tableProperties.TabIndex = 40;
            // 
            // horizontalLine1
            // 
            this.horizontalLine1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.horizontalLine1.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.horizontalLine1.Location = new System.Drawing.Point(1, 120);
            this.horizontalLine1.Margin = new System.Windows.Forms.Padding(0);
            this.horizontalLine1.Name = "horizontalLine1";
            this.horizontalLine1.Size = new System.Drawing.Size(308, 10);
            this.horizontalLine1.TabIndex = 39;
            // 
            // valueProperties
            // 
            this.valueProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueProperties.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valueProperties.Location = new System.Drawing.Point(6, 134);
            this.valueProperties.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.valueProperties.Name = "valueProperties";
            this.valueProperties.Size = new System.Drawing.Size(293, 135);
            this.valueProperties.TabIndex = 0;
            // 
            // cboResultType
            // 
            this.cboResultType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboResultType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboResultType.FormattingEnabled = true;
            this.cboResultType.Location = new System.Drawing.Point(94, 90);
            this.cboResultType.Name = "cboResultType";
            this.cboResultType.Size = new System.Drawing.Size(205, 25);
            this.cboResultType.TabIndex = 38;
            this.cboResultType.SelectedIndexChanged += new System.EventHandler(this.cboResultType_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 93);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 17);
            this.label7.TabIndex = 37;
            this.label7.Text = "Result Type";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 17);
            this.label6.TabIndex = 36;
            this.label6.Text = "Tag Settings";
            // 
            // scintilla1
            // 
            this.scintilla1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scintilla1.AnnotationVisible = ScintillaNET.Annotation.Standard;
            this.scintilla1.Lexer = ScintillaNET.Lexer.R;
            this.scintilla1.Location = new System.Drawing.Point(16, 58);
            this.scintilla1.MarginOptions = ScintillaNET.MarginOptions.None;
            this.scintilla1.Name = "scintilla1";
            this.scintilla1.Size = new System.Drawing.Size(732, 522);
            this.scintilla1.TabIndex = 24;
            this.scintilla1.MarginClick += new System.EventHandler<ScintillaNET.MarginClickEventArgs>(this.scintilla1_MarginClick);
            this.scintilla1.LineSelectClick += new System.EventHandler<ScintillaNET.MarginClickEventArgs>(this.scintilla1_LineSelectClick);
            // 
            // verticalLine1
            // 
            this.verticalLine1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.verticalLine1.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.verticalLine1.Location = new System.Drawing.Point(754, 10);
            this.verticalLine1.Name = "verticalLine1";
            this.verticalLine1.Size = new System.Drawing.Size(10, 570);
            this.verticalLine1.TabIndex = 42;
            // 
            // EditTag
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(1091, 641);
            this.Controls.Add(this.verticalLine1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cmdSaveAndInsert);
            this.Controls.Add(this.scintilla1);
            this.Controls.Add(this.lblNoOutputWarning);
            this.Controls.Add(this.cboCodeFiles);
            this.Controls.Add(this.label3);
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
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboCodeFiles;
        private System.Windows.Forms.Label lblNoOutputWarning;
        private System.ComponentModel.BackgroundWorker codeCheckWorker;
        private ScintillaNET.Scintilla scintilla1;
        private System.Windows.Forms.Button cmdSaveAndInsert;
        private System.Windows.Forms.Label lblAllowedCommands;
        private System.Windows.Forms.Label lblInstructionTitle;
        private Controls.ValueProperties valueProperties;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboRunFrequency;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboResultType;
        private System.Windows.Forms.Label label7;
        private Controls.HorizontalLine horizontalLine1;
        private Controls.VerticalLine verticalLine1;
        private Controls.TableProperties tableProperties;
        private Controls.FigureProperties figureProperties;
    }
}