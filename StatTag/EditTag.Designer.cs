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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Panel panel1;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditTag));
            this.valueProperties = new StatTag.Controls.ValueProperties();
            this.tableProperties = new StatTag.Controls.TableProperties();
            this.cmdSave = new Sce.Atf.Controls.SplitButton();
            this.cmsSave = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmSaveAndDefine = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.cboResultType = new System.Windows.Forms.ComboBox();
            this.horizontalLine1 = new StatTag.Controls.HorizontalLine();
            this.lblAllowedCommands = new System.Windows.Forms.Label();
            this.lblInstructionTitle = new System.Windows.Forms.Label();
            this.lblMarginClick = new System.Windows.Forms.Label();
            this.incrementalSearcher1 = new ScintillaNET_FindReplaceDialog.IncrementalSearcher();
            this.label1 = new System.Windows.Forms.Label();
            this.cboCodeFiles = new System.Windows.Forms.ComboBox();
            this.scintilla1 = new ScintillaNET.Scintilla();
            this.verticalLine1 = new StatTag.Controls.VerticalLine();
            this.lblNoOutputWarning = new System.Windows.Forms.Label();
            this.codeCheckWorker = new System.ComponentModel.BackgroundWorker();
            panel1 = new System.Windows.Forms.Panel();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            panel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            this.cmsSave.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(this.valueProperties);
            panel1.Controls.Add(this.tableProperties);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(5, 99);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(304, 414);
            panel1.TabIndex = 41;
            // 
            // valueProperties
            // 
            this.valueProperties.AutoSize = true;
            this.valueProperties.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.valueProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.valueProperties.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valueProperties.Location = new System.Drawing.Point(0, 0);
            this.valueProperties.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.valueProperties.Name = "valueProperties";
            this.valueProperties.Size = new System.Drawing.Size(304, 414);
            this.valueProperties.TabIndex = 0;
            // 
            // tableProperties
            // 
            this.tableProperties.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableProperties.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableProperties.Location = new System.Drawing.Point(0, 0);
            this.tableProperties.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableProperties.Name = "tableProperties";
            this.tableProperties.Size = new System.Drawing.Size(304, 414);
            this.tableProperties.TabIndex = 40;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.Controls.Add(tableLayoutPanel4, 0, 4);
            tableLayoutPanel1.Controls.Add(this.tableLayoutPanel6, 1, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Controls.Add(this.scintilla1, 0, 2);
            tableLayoutPanel1.Controls.Add(this.verticalLine1, 1, 0);
            tableLayoutPanel1.Controls.Add(this.lblNoOutputWarning, 0, 3);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(1091, 647);
            tableLayoutPanel1.TabIndex = 45;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.AutoSize = true;
            tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel4.CausesValidation = false;
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel1.SetColumnSpan(tableLayoutPanel4, 3);
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel4.Controls.Add(this.cmdSave, 0, 0);
            tableLayoutPanel4.Controls.Add(this.cmdCancel, 1, 0);
            tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel4.Location = new System.Drawing.Point(8, 598);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel4.Size = new System.Drawing.Size(1075, 41);
            tableLayoutPanel4.TabIndex = 46;
            // 
            // cmdSave
            // 
            this.cmdSave.AutoSize = true;
            this.cmdSave.ContextMenuStrip = this.cmsSave;
            this.cmdSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.cmdSave.Location = new System.Drawing.Point(344, 3);
            this.cmdSave.Margin = new System.Windows.Forms.Padding(3, 3, 20, 10);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(173, 28);
            this.cmdSave.TabIndex = 44;
            this.cmdSave.Text = "Save";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // cmsSave
            // 
            this.cmsSave.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.cmsSave.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSaveAndDefine});
            this.cmsSave.Name = "cmsSave";
            this.cmsSave.ShowImageMargin = false;
            this.cmsSave.ShowItemToolTips = false;
            this.cmsSave.Size = new System.Drawing.Size(180, 26);
            // 
            // tsmSaveAndDefine
            // 
            this.tsmSaveAndDefine.Name = "tsmSaveAndDefine";
            this.tsmSaveAndDefine.Size = new System.Drawing.Size(179, 22);
            this.tsmSaveAndDefine.Text = "Save and Define Another";
            this.tsmSaveAndDefine.Click += new System.EventHandler(this.tsmSaveAndDefine_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.AutoSize = true;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Dock = System.Windows.Forms.DockStyle.Left;
            this.cmdCancel.Location = new System.Drawing.Point(557, 4);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(20, 4, 3, 10);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(164, 27);
            this.cmdCancel.TabIndex = 7;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.horizontalLine1, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.lblAllowedCommands, 0, 4);
            this.tableLayoutPanel6.Controls.Add(panel1, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.lblInstructionTitle, 0, 3);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(769, 8);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.Padding = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel6.RowCount = 5;
            tableLayoutPanel1.SetRowSpan(this.tableLayoutPanel6, 4);
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.Size = new System.Drawing.Size(314, 584);
            this.tableLayoutPanel6.TabIndex = 47;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.AutoSize = true;
            tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel5.Controls.Add(this.label5, 1, 0);
            tableLayoutPanel5.Controls.Add(this.label6, 0, 0);
            tableLayoutPanel5.Controls.Add(this.lblName, 0, 1);
            tableLayoutPanel5.Controls.Add(this.label7, 0, 2);
            tableLayoutPanel5.Controls.Add(this.txtName, 1, 1);
            tableLayoutPanel5.Controls.Add(this.cboResultType, 1, 2);
            tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel5.Location = new System.Drawing.Point(5, 5);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            tableLayoutPanel5.RowCount = 3;
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel5.Size = new System.Drawing.Size(304, 78);
            tableLayoutPanel5.TabIndex = 46;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(94, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(207, 17);
            this.label5.TabIndex = 44;
            this.label5.Text = "(Required)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 17);
            this.label6.TabIndex = 36;
            this.label6.Text = "Tag Settings";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblName.Location = new System.Drawing.Point(3, 17);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(85, 31);
            this.lblName.TabIndex = 34;
            this.lblName.Text = "Name";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(3, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 27);
            this.label7.TabIndex = 37;
            this.label7.Text = "Result Type";
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(94, 20);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(207, 25);
            this.txtName.TabIndex = 35;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            this.txtName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtName_KeyPress);
            // 
            // cboResultType
            // 
            this.cboResultType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboResultType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboResultType.FormattingEnabled = true;
            this.cboResultType.Location = new System.Drawing.Point(94, 51);
            this.cboResultType.Name = "cboResultType";
            this.cboResultType.Size = new System.Drawing.Size(207, 25);
            this.cboResultType.TabIndex = 38;
            this.cboResultType.SelectedIndexChanged += new System.EventHandler(this.cboResultType_SelectedIndexChanged);
            // 
            // horizontalLine1
            // 
            this.horizontalLine1.AutoSize = true;
            this.horizontalLine1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.horizontalLine1.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.horizontalLine1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.horizontalLine1.Location = new System.Drawing.Point(2, 86);
            this.horizontalLine1.Margin = new System.Windows.Forms.Padding(0);
            this.horizontalLine1.Name = "horizontalLine1";
            this.horizontalLine1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.horizontalLine1.Size = new System.Drawing.Size(310, 10);
            this.horizontalLine1.TabIndex = 39;
            // 
            // lblAllowedCommands
            // 
            this.lblAllowedCommands.AutoSize = true;
            this.lblAllowedCommands.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAllowedCommands.Location = new System.Drawing.Point(5, 550);
            this.lblAllowedCommands.Margin = new System.Windows.Forms.Padding(3, 0, 3, 15);
            this.lblAllowedCommands.Name = "lblAllowedCommands";
            this.lblAllowedCommands.Size = new System.Drawing.Size(304, 17);
            this.lblAllowedCommands.TabIndex = 4;
            this.lblAllowedCommands.Text = "(None specified)";
            // 
            // lblInstructionTitle
            // 
            this.lblInstructionTitle.AutoSize = true;
            this.lblInstructionTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInstructionTitle.Location = new System.Drawing.Point(5, 516);
            this.lblInstructionTitle.Name = "lblInstructionTitle";
            this.lblInstructionTitle.Size = new System.Drawing.Size(304, 34);
            this.lblInstructionTitle.TabIndex = 3;
            this.lblInstructionTitle.Text = "The following Stata commands can be used for Value results:\r\n";
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.AutoSize = true;
            tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel3.Controls.Add(this.lblMarginClick, 0, 0);
            tableLayoutPanel3.Controls.Add(this.incrementalSearcher1, 1, 0);
            tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel3.Location = new System.Drawing.Point(8, 43);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel3.Size = new System.Drawing.Size(739, 26);
            tableLayoutPanel3.TabIndex = 46;
            // 
            // lblMarginClick
            // 
            this.lblMarginClick.AutoSize = true;
            this.lblMarginClick.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMarginClick.Location = new System.Drawing.Point(3, 0);
            this.lblMarginClick.Name = "lblMarginClick";
            this.lblMarginClick.Size = new System.Drawing.Size(363, 26);
            this.lblMarginClick.TabIndex = 12;
            this.lblMarginClick.Text = "Click in the margin to define a tag:";
            // 
            // incrementalSearcher1
            // 
            this.incrementalSearcher1.AutoPosition = false;
            this.incrementalSearcher1.AutoSize = true;
            this.incrementalSearcher1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.incrementalSearcher1.BackColor = System.Drawing.Color.Transparent;
            this.incrementalSearcher1.Dock = System.Windows.Forms.DockStyle.Right;
            this.incrementalSearcher1.FindReplace = null;
            this.incrementalSearcher1.Location = new System.Drawing.Point(453, 0);
            this.incrementalSearcher1.Margin = new System.Windows.Forms.Padding(0);
            this.incrementalSearcher1.Name = "incrementalSearcher1";
            this.incrementalSearcher1.Scintilla = null;
            this.incrementalSearcher1.Size = new System.Drawing.Size(286, 26);
            this.incrementalSearcher1.TabIndex = 43;
            this.incrementalSearcher1.ToolItem = true;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.AutoSize = true;
            tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            tableLayoutPanel2.Controls.Add(this.cboCodeFiles, 1, 0);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(8, 8);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            tableLayoutPanel2.Size = new System.Drawing.Size(739, 29);
            tableLayoutPanel2.TabIndex = 46;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 27);
            this.label1.TabIndex = 8;
            this.label1.Text = "Code file:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboCodeFiles
            // 
            this.cboCodeFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCodeFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCodeFiles.FormattingEnabled = true;
            this.cboCodeFiles.Location = new System.Drawing.Point(72, 3);
            this.cboCodeFiles.Name = "cboCodeFiles";
            this.cboCodeFiles.Size = new System.Drawing.Size(664, 25);
            this.cboCodeFiles.TabIndex = 21;
            this.cboCodeFiles.DropDown += new System.EventHandler(this.cboCodeFiles_DropDown);
            this.cboCodeFiles.SelectedIndexChanged += new System.EventHandler(this.cboCodeFiles_SelectedIndexChanged);
            // 
            // scintilla1
            // 
            this.scintilla1.AnnotationVisible = ScintillaNET.Annotation.Standard;
            this.scintilla1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scintilla1.Lexer = ScintillaNET.Lexer.R;
            this.scintilla1.Location = new System.Drawing.Point(8, 75);
            this.scintilla1.MarginOptions = ScintillaNET.MarginOptions.None;
            this.scintilla1.Name = "scintilla1";
            this.scintilla1.Size = new System.Drawing.Size(739, 485);
            this.scintilla1.TabIndex = 24;
            this.scintilla1.MarginClick += new System.EventHandler<ScintillaNET.MarginClickEventArgs>(this.scintilla1_MarginClick);
            this.scintilla1.LineSelectClick += new System.EventHandler<ScintillaNET.MarginClickEventArgs>(this.scintilla1_LineSelectClick);
            // 
            // verticalLine1
            // 
            this.verticalLine1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.verticalLine1.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.verticalLine1.Location = new System.Drawing.Point(753, 8);
            this.verticalLine1.Name = "verticalLine1";
            tableLayoutPanel1.SetRowSpan(this.verticalLine1, 4);
            this.verticalLine1.Size = new System.Drawing.Size(10, 584);
            this.verticalLine1.TabIndex = 42;
            // 
            // lblNoOutputWarning
            // 
            this.lblNoOutputWarning.AutoSize = true;
            this.lblNoOutputWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNoOutputWarning.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoOutputWarning.ForeColor = System.Drawing.Color.Red;
            this.lblNoOutputWarning.Location = new System.Drawing.Point(8, 563);
            this.lblNoOutputWarning.Name = "lblNoOutputWarning";
            this.lblNoOutputWarning.Padding = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this.lblNoOutputWarning.Size = new System.Drawing.Size(739, 32);
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
            // EditTag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(1091, 647);
            this.Controls.Add(tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "EditTag";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tag";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditTag_FormClosing);
            this.Load += new System.EventHandler(this.EditTag_Load);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            this.cmsSave.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblMarginClick;
        private System.Windows.Forms.ComboBox cboCodeFiles;
        private System.Windows.Forms.Label lblNoOutputWarning;
        private System.ComponentModel.BackgroundWorker codeCheckWorker;
        private ScintillaNET.Scintilla scintilla1;
        private System.Windows.Forms.Label lblAllowedCommands;
        private System.Windows.Forms.Label lblInstructionTitle;
        private Controls.ValueProperties valueProperties;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboResultType;
        private System.Windows.Forms.Label label7;
        private Controls.HorizontalLine horizontalLine1;
        private Controls.VerticalLine verticalLine1;
        private Controls.TableProperties tableProperties;
        private System.Windows.Forms.Label label5;
        private ScintillaNET_FindReplaceDialog.IncrementalSearcher incrementalSearcher1;
        private Sce.Atf.Controls.SplitButton cmdSave;
        private System.Windows.Forms.ContextMenuStrip cmsSave;
        private System.Windows.Forms.ToolStripMenuItem tsmSaveAndDefine;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
    }
}