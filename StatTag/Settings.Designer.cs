namespace StatTag
{
    partial class Settings
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
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
            this.lblEmptyValueWarning = new System.Windows.Forms.Label();
            this.chkRunCodeOnOpen = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMaxLogSize = new System.Windows.Forms.NumericUpDown();
            this.txtMaxLogFiles = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdStataLocation = new System.Windows.Forms.Button();
            this.tabSettings = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.tabLogging = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.lblLogWarning = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.txtLogLocation = new System.Windows.Forms.TextBox();
            this.cmdLogLocation = new System.Windows.Forms.Button();
            this.chkEnableLogging = new System.Windows.Forms.CheckBox();
            this.tabStata = new System.Windows.Forms.TabPage();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.chkStataAutomation = new System.Windows.Forms.CheckBox();
            this.missingValueSettings1 = new StatTag.Controls.MissingValueSettings();
            this.txtStataLocation = new StatTag.Controls.PlaceholderTextBox();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxLogSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxLogFiles)).BeginInit();
            tableLayoutPanel6.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            tableLayoutPanel8.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabLogging.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tabStata.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.CausesValidation = false;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(this.lblEmptyValueWarning, 0, 3);
            tableLayoutPanel1.Controls.Add(this.chkRunCodeOnOpen, 0, 0);
            tableLayoutPanel1.Controls.Add(this.label5, 0, 1);
            tableLayoutPanel1.Controls.Add(this.missingValueSettings1, 0, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(3);
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(727, 229);
            tableLayoutPanel1.TabIndex = 17;
            // 
            // lblEmptyValueWarning
            // 
            this.lblEmptyValueWarning.AutoSize = true;
            this.lblEmptyValueWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEmptyValueWarning.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmptyValueWarning.Location = new System.Drawing.Point(23, 156);
            this.lblEmptyValueWarning.Margin = new System.Windows.Forms.Padding(20, 5, 5, 5);
            this.lblEmptyValueWarning.Name = "lblEmptyValueWarning";
            this.lblEmptyValueWarning.Size = new System.Drawing.Size(696, 65);
            this.lblEmptyValueWarning.TabIndex = 20;
            this.lblEmptyValueWarning.Text = resources.GetString("lblEmptyValueWarning.Text");
            // 
            // chkRunCodeOnOpen
            // 
            this.chkRunCodeOnOpen.AutoSize = true;
            this.chkRunCodeOnOpen.Location = new System.Drawing.Point(6, 6);
            this.chkRunCodeOnOpen.Margin = new System.Windows.Forms.Padding(3, 3, 3, 20);
            this.chkRunCodeOnOpen.Name = "chkRunCodeOnOpen";
            this.chkRunCodeOnOpen.Size = new System.Drawing.Size(412, 21);
            this.chkRunCodeOnOpen.TabIndex = 17;
            this.chkRunCodeOnOpen.Text = "Automatically run statistical code when a StatTag document opens";
            this.chkRunCodeOnOpen.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(6, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(715, 17);
            this.label5.TabIndex = 19;
            this.label5.Text = "For a new document, if a table has a missing value (empty cell), default to displ" +
    "ay the missing value using:";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.AutoSize = true;
            tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel2.CausesValidation = false;
            tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel5.SetColumnSpan(tableLayoutPanel2, 2);
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 1, 0);
            tableLayoutPanel2.Controls.Add(this.txtMaxLogFiles, 1, 1);
            tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
            tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            tableLayoutPanel2.Location = new System.Drawing.Point(23, 85);
            tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(20, 15, 3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel2.Size = new System.Drawing.Size(394, 68);
            tableLayoutPanel2.TabIndex = 17;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.label4, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtMaxLogSize, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(263, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(128, 31);
            this.tableLayoutPanel3.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.CausesValidation = false;
            this.label4.Location = new System.Drawing.Point(55, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 17);
            this.label4.TabIndex = 24;
            this.label4.Text = "MB in size.";
            // 
            // txtMaxLogSize
            // 
            this.txtMaxLogSize.Location = new System.Drawing.Point(3, 3);
            this.txtMaxLogSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtMaxLogSize.Name = "txtMaxLogSize";
            this.txtMaxLogSize.Size = new System.Drawing.Size(46, 25);
            this.txtMaxLogSize.TabIndex = 23;
            this.txtMaxLogSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // txtMaxLogFiles
            // 
            this.txtMaxLogFiles.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txtMaxLogFiles.Location = new System.Drawing.Point(266, 40);
            this.txtMaxLogFiles.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.txtMaxLogFiles.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtMaxLogFiles.Name = "txtMaxLogFiles";
            this.txtMaxLogFiles.Size = new System.Drawing.Size(46, 25);
            this.txtMaxLogFiles.TabIndex = 25;
            this.txtMaxLogFiles.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.CausesValidation = false;
            this.label3.Location = new System.Drawing.Point(3, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(247, 17);
            this.label3.TabIndex = 22;
            this.label3.Text = "Total number of debug text files to keep:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.CausesValidation = false;
            this.label2.Location = new System.Drawing.Point(3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(254, 17);
            this.label2.TabIndex = 21;
            this.label2.Text = "Allow debug text files to grow as large as:";
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.AutoSize = true;
            tableLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel6.CausesValidation = false;
            tableLayoutPanel6.ColumnCount = 2;
            tableLayoutPanel7.SetColumnSpan(tableLayoutPanel6, 2);
            tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel6.Controls.Add(this.cmdStataLocation, 1, 0);
            tableLayoutPanel6.Controls.Add(this.txtStataLocation, 0, 0);
            tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel6.Location = new System.Drawing.Point(23, 33);
            tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(20, 3, 3, 7);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 1;
            tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel6.Size = new System.Drawing.Size(698, 29);
            tableLayoutPanel6.TabIndex = 17;
            // 
            // cmdStataLocation
            // 
            this.cmdStataLocation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cmdStataLocation.Location = new System.Drawing.Point(667, 3);
            this.cmdStataLocation.Name = "cmdStataLocation";
            this.cmdStataLocation.Size = new System.Drawing.Size(28, 23);
            this.cmdStataLocation.TabIndex = 17;
            this.cmdStataLocation.Text = "...";
            this.cmdStataLocation.UseVisualStyleBackColor = true;
            this.cmdStataLocation.Click += new System.EventHandler(this.cmdStataLocation_Click);
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.AutoSize = true;
            tableLayoutPanel7.CausesValidation = false;
            tableLayoutPanel7.ColumnCount = 2;
            tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel7.Controls.Add(tableLayoutPanel6, 0, 1);
            tableLayoutPanel7.Controls.Add(this.chkStataAutomation, 0, 0);
            tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel7.Location = new System.Drawing.Point(3, 3);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.Padding = new System.Windows.Forms.Padding(3);
            tableLayoutPanel7.RowCount = 3;
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel7.Size = new System.Drawing.Size(727, 229);
            tableLayoutPanel7.TabIndex = 18;
            // 
            // tableLayoutPanel8
            // 
            tableLayoutPanel8.ColumnCount = 2;
            tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel8.Controls.Add(this.tabSettings, 0, 0);
            tableLayoutPanel8.Controls.Add(this.cmdCancel, 1, 1);
            tableLayoutPanel8.Controls.Add(this.cmdOK, 0, 1);
            tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel8.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel8.Name = "tableLayoutPanel8";
            tableLayoutPanel8.Padding = new System.Windows.Forms.Padding(5);
            tableLayoutPanel8.RowCount = 2;
            tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel8.Size = new System.Drawing.Size(757, 316);
            tableLayoutPanel8.TabIndex = 17;
            // 
            // tabSettings
            // 
            tableLayoutPanel8.SetColumnSpan(this.tabSettings, 2);
            this.tabSettings.Controls.Add(this.tabGeneral);
            this.tabSettings.Controls.Add(this.tabLogging);
            this.tabSettings.Controls.Add(this.tabStata);
            this.tabSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSettings.Location = new System.Drawing.Point(8, 8);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            this.tabSettings.Size = new System.Drawing.Size(741, 265);
            this.tabSettings.TabIndex = 16;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(tableLayoutPanel1);
            this.tabGeneral.Location = new System.Drawing.Point(4, 26);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(733, 235);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // tabLogging
            // 
            this.tabLogging.Controls.Add(this.tableLayoutPanel5);
            this.tabLogging.Location = new System.Drawing.Point(4, 26);
            this.tabLogging.Name = "tabLogging";
            this.tabLogging.Size = new System.Drawing.Size(733, 235);
            this.tabLogging.TabIndex = 2;
            this.tabLogging.Text = "Logging";
            this.tabLogging.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.lblLogWarning, 1, 0);
            this.tableLayoutPanel5.Controls.Add(tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.chkEnableLogging, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel5.RowCount = 3;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(733, 235);
            this.tableLayoutPanel5.TabIndex = 19;
            // 
            // lblLogWarning
            // 
            this.lblLogWarning.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblLogWarning.AutoSize = true;
            this.lblLogWarning.ForeColor = System.Drawing.Color.Red;
            this.lblLogWarning.Location = new System.Drawing.Point(586, 9);
            this.lblLogWarning.Name = "lblLogWarning";
            this.lblLogWarning.Size = new System.Drawing.Size(141, 17);
            this.lblLogWarning.TabIndex = 20;
            this.lblLogWarning.Text = "Please enter a file path";
            this.lblLogWarning.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblLogWarning.Visible = false;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.CausesValidation = false;
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel5.SetColumnSpan(this.tableLayoutPanel4, 2);
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.txtLogLocation, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.cmdLogLocation, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(23, 36);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(704, 31);
            this.tableLayoutPanel4.TabIndex = 18;
            // 
            // txtLogLocation
            // 
            this.txtLogLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogLocation.Enabled = false;
            this.txtLogLocation.Location = new System.Drawing.Point(3, 3);
            this.txtLogLocation.Name = "txtLogLocation";
            this.txtLogLocation.Size = new System.Drawing.Size(664, 25);
            this.txtLogLocation.TabIndex = 17;
            this.txtLogLocation.TextChanged += new System.EventHandler(this.txtLogLocation_TextChanged);
            // 
            // cmdLogLocation
            // 
            this.cmdLogLocation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cmdLogLocation.Enabled = false;
            this.cmdLogLocation.Location = new System.Drawing.Point(673, 4);
            this.cmdLogLocation.Name = "cmdLogLocation";
            this.cmdLogLocation.Size = new System.Drawing.Size(28, 23);
            this.cmdLogLocation.TabIndex = 18;
            this.cmdLogLocation.Text = "...";
            this.cmdLogLocation.UseVisualStyleBackColor = true;
            this.cmdLogLocation.Click += new System.EventHandler(this.cmdLogLocation_Click);
            // 
            // chkEnableLogging
            // 
            this.chkEnableLogging.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkEnableLogging.AutoSize = true;
            this.chkEnableLogging.Location = new System.Drawing.Point(9, 9);
            this.chkEnableLogging.Margin = new System.Windows.Forms.Padding(6, 6, 3, 3);
            this.chkEnableLogging.Name = "chkEnableLogging";
            this.chkEnableLogging.Size = new System.Drawing.Size(179, 21);
            this.chkEnableLogging.TabIndex = 19;
            this.chkEnableLogging.Text = "Enable the debug text file:";
            this.chkEnableLogging.UseVisualStyleBackColor = true;
            this.chkEnableLogging.Click += new System.EventHandler(this.chkEnableLogging_CheckedChanged);
            // 
            // tabStata
            // 
            this.tabStata.Controls.Add(tableLayoutPanel7);
            this.tabStata.Location = new System.Drawing.Point(4, 26);
            this.tabStata.Name = "tabStata";
            this.tabStata.Padding = new System.Windows.Forms.Padding(3);
            this.tabStata.Size = new System.Drawing.Size(733, 235);
            this.tabStata.TabIndex = 1;
            this.tabStata.Text = "Stata";
            this.tabStata.UseVisualStyleBackColor = true;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cmdCancel.AutoSize = true;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(393, 280);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(15, 4, 3, 4);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(87, 27);
            this.cmdCancel.TabIndex = 9;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cmdOK.AutoSize = true;
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(276, 280);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(3, 4, 15, 4);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(87, 27);
            this.cmdOK.TabIndex = 8;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // chkStataAutomation
            // 
            this.chkStataAutomation.AutoSize = true;
            this.chkStataAutomation.Location = new System.Drawing.Point(6, 6);
            this.chkStataAutomation.Name = "chkStataAutomation";
            this.chkStataAutomation.Size = new System.Drawing.Size(191, 21);
            this.chkStataAutomation.TabIndex = 20;
            this.chkStataAutomation.Text = "Enable Stata automation API";
            this.chkStataAutomation.UseVisualStyleBackColor = true;
            this.chkStataAutomation.CheckedChanged += new System.EventHandler(this.chkStataAutomation_CheckedChanged);
            // 
            // missingValueSettings1
            // 
            this.missingValueSettings1.AutoSize = true;
            this.missingValueSettings1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.missingValueSettings1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.missingValueSettings1.Location = new System.Drawing.Point(23, 69);
            this.missingValueSettings1.Margin = new System.Windows.Forms.Padding(20, 5, 0, 5);
            this.missingValueSettings1.Name = "missingValueSettings1";
            this.missingValueSettings1.Size = new System.Drawing.Size(701, 77);
            this.missingValueSettings1.TabIndex = 18;
            // 
            // txtStataLocation
            // 
            this.txtStataLocation.AutoSize = true;
            this.txtStataLocation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtStataLocation.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStataLocation.Location = new System.Drawing.Point(0, 0);
            this.txtStataLocation.Margin = new System.Windows.Forms.Padding(0);
            this.txtStataLocation.MinimumSize = new System.Drawing.Size(100, 25);
            this.txtStataLocation.Name = "txtStataLocation";
            this.txtStataLocation.PlaceholderText = "Select or enter the Stata executable location";
            this.txtStataLocation.Size = new System.Drawing.Size(664, 29);
            this.txtStataLocation.TabIndex = 18;
            // 
            // Settings
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(757, 316);
            this.Controls.Add(tableLayoutPanel8);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "User Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxLogSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxLogFiles)).EndInit();
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            tableLayoutPanel7.ResumeLayout(false);
            tableLayoutPanel7.PerformLayout();
            tableLayoutPanel8.ResumeLayout(false);
            tableLayoutPanel8.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.tabLogging.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tabStata.ResumeLayout(false);
            this.tabStata.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.CheckBox chkRunCodeOnOpen;
        private System.Windows.Forms.TabControl tabSettings;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabLogging;
        private System.Windows.Forms.Label lblLogWarning;
        private System.Windows.Forms.CheckBox chkEnableLogging;
        private System.Windows.Forms.Button cmdLogLocation;
        private System.Windows.Forms.TextBox txtLogLocation;
        private System.Windows.Forms.TabPage tabStata;
        private System.Windows.Forms.Button cmdStataLocation;
        private System.Windows.Forms.NumericUpDown txtMaxLogFiles;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown txtMaxLogSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private Controls.MissingValueSettings missingValueSettings1;
        private System.Windows.Forms.Label lblEmptyValueWarning;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.CheckBox chkStataAutomation;
        private Controls.PlaceholderTextBox txtStataLocation;
    }
}