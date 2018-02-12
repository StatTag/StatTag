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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.chkRunCodeOnOpen = new System.Windows.Forms.CheckBox();
            this.tabSettings = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.lblEmptyValueWarning = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.missingValueSettings1 = new StatTag.Controls.MissingValueSettings();
            this.tabLogging = new System.Windows.Forms.TabPage();
            this.txtMaxLogFiles = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMaxLogSize = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblLogWarning = new System.Windows.Forms.Label();
            this.chkEnableLogging = new System.Windows.Forms.CheckBox();
            this.cmdLogLocation = new System.Windows.Forms.Button();
            this.txtLogLocation = new System.Windows.Forms.TextBox();
            this.tabStata = new System.Windows.Forms.TabPage();
            this.cmdDisableStataAutomation = new System.Windows.Forms.Button();
            this.cmdRegisterStataAutomation = new System.Windows.Forms.Button();
            this.cmdStataLocation = new System.Windows.Forms.Button();
            this.txtStataLocation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabSettings.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabLogging.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxLogFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxLogSize)).BeginInit();
            this.tabStata.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(378, 251);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(87, 25);
            this.cmdCancel.TabIndex = 9;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(228, 251);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(87, 25);
            this.cmdOK.TabIndex = 8;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // chkRunCodeOnOpen
            // 
            this.chkRunCodeOnOpen.AutoSize = true;
            this.chkRunCodeOnOpen.Location = new System.Drawing.Point(8, 8);
            this.chkRunCodeOnOpen.Name = "chkRunCodeOnOpen";
            this.chkRunCodeOnOpen.Size = new System.Drawing.Size(412, 21);
            this.chkRunCodeOnOpen.TabIndex = 17;
            this.chkRunCodeOnOpen.Text = "Automatically run statistical code when a StatTag document opens";
            this.chkRunCodeOnOpen.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            this.tabSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabSettings.Controls.Add(this.tabGeneral);
            this.tabSettings.Controls.Add(this.tabLogging);
            this.tabSettings.Controls.Add(this.tabStata);
            this.tabSettings.Location = new System.Drawing.Point(12, 12);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            this.tabSettings.Size = new System.Drawing.Size(673, 227);
            this.tabSettings.TabIndex = 16;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.lblEmptyValueWarning);
            this.tabGeneral.Controls.Add(this.label5);
            this.tabGeneral.Controls.Add(this.missingValueSettings1);
            this.tabGeneral.Controls.Add(this.chkRunCodeOnOpen);
            this.tabGeneral.Location = new System.Drawing.Point(4, 26);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(665, 197);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // lblEmptyValueWarning
            // 
            this.lblEmptyValueWarning.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmptyValueWarning.Location = new System.Drawing.Point(281, 102);
            this.lblEmptyValueWarning.Name = "lblEmptyValueWarning";
            this.lblEmptyValueWarning.Size = new System.Drawing.Size(378, 70);
            this.lblEmptyValueWarning.TabIndex = 20;
            this.lblEmptyValueWarning.Text = resources.GetString("lblEmptyValueWarning.Text");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(623, 17);
            this.label5.TabIndex = 19;
            this.label5.Text = "For a new document, if a table has a missing value (empty cell), default to displ" +
    "ay the missing value using:";
            // 
            // missingValueSettings1
            // 
            this.missingValueSettings1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.missingValueSettings1.Location = new System.Drawing.Point(20, 66);
            this.missingValueSettings1.Margin = new System.Windows.Forms.Padding(0);
            this.missingValueSettings1.Name = "missingValueSettings1";
            this.missingValueSettings1.Size = new System.Drawing.Size(391, 74);
            this.missingValueSettings1.TabIndex = 18;
            // 
            // tabLogging
            // 
            this.tabLogging.Controls.Add(this.txtMaxLogFiles);
            this.tabLogging.Controls.Add(this.label4);
            this.tabLogging.Controls.Add(this.txtMaxLogSize);
            this.tabLogging.Controls.Add(this.label3);
            this.tabLogging.Controls.Add(this.label2);
            this.tabLogging.Controls.Add(this.lblLogWarning);
            this.tabLogging.Controls.Add(this.chkEnableLogging);
            this.tabLogging.Controls.Add(this.cmdLogLocation);
            this.tabLogging.Controls.Add(this.txtLogLocation);
            this.tabLogging.Location = new System.Drawing.Point(4, 26);
            this.tabLogging.Name = "tabLogging";
            this.tabLogging.Size = new System.Drawing.Size(665, 197);
            this.tabLogging.TabIndex = 2;
            this.tabLogging.Text = "Logging";
            this.tabLogging.UseVisualStyleBackColor = true;
            // 
            // txtMaxLogFiles
            // 
            this.txtMaxLogFiles.Location = new System.Drawing.Point(276, 103);
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.CausesValidation = false;
            this.label4.Location = new System.Drawing.Point(328, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 17);
            this.label4.TabIndex = 24;
            this.label4.Text = "MB in size.";
            // 
            // txtMaxLogSize
            // 
            this.txtMaxLogSize.Location = new System.Drawing.Point(276, 72);
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.CausesValidation = false;
            this.label3.Location = new System.Drawing.Point(23, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(247, 17);
            this.label3.TabIndex = 22;
            this.label3.Text = "Total number of debug text files to keep:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.CausesValidation = false;
            this.label2.Location = new System.Drawing.Point(23, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(254, 17);
            this.label2.TabIndex = 21;
            this.label2.Text = "Allow debug text files to grow as large as:";
            // 
            // lblLogWarning
            // 
            this.lblLogWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLogWarning.ForeColor = System.Drawing.Color.Red;
            this.lblLogWarning.Location = new System.Drawing.Point(368, 13);
            this.lblLogWarning.Name = "lblLogWarning";
            this.lblLogWarning.Size = new System.Drawing.Size(249, 19);
            this.lblLogWarning.TabIndex = 20;
            this.lblLogWarning.Text = "Please enter a file path";
            this.lblLogWarning.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblLogWarning.Visible = false;
            // 
            // chkEnableLogging
            // 
            this.chkEnableLogging.AutoSize = true;
            this.chkEnableLogging.Location = new System.Drawing.Point(8, 8);
            this.chkEnableLogging.Name = "chkEnableLogging";
            this.chkEnableLogging.Size = new System.Drawing.Size(179, 21);
            this.chkEnableLogging.TabIndex = 19;
            this.chkEnableLogging.Text = "Enable the debug text file:";
            this.chkEnableLogging.UseVisualStyleBackColor = true;
            this.chkEnableLogging.Click += new System.EventHandler(this.chkEnableLogging_CheckedChanged);
            // 
            // cmdLogLocation
            // 
            this.cmdLogLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdLogLocation.Enabled = false;
            this.cmdLogLocation.Location = new System.Drawing.Point(623, 35);
            this.cmdLogLocation.Name = "cmdLogLocation";
            this.cmdLogLocation.Size = new System.Drawing.Size(28, 23);
            this.cmdLogLocation.TabIndex = 18;
            this.cmdLogLocation.Text = "...";
            this.cmdLogLocation.UseVisualStyleBackColor = true;
            this.cmdLogLocation.Click += new System.EventHandler(this.cmdLogLocation_Click);
            // 
            // txtLogLocation
            // 
            this.txtLogLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogLocation.Enabled = false;
            this.txtLogLocation.Location = new System.Drawing.Point(23, 35);
            this.txtLogLocation.Name = "txtLogLocation";
            this.txtLogLocation.Size = new System.Drawing.Size(594, 25);
            this.txtLogLocation.TabIndex = 17;
            this.txtLogLocation.TextChanged += new System.EventHandler(this.txtLogLocation_TextChanged);
            // 
            // tabStata
            // 
            this.tabStata.Controls.Add(this.cmdDisableStataAutomation);
            this.tabStata.Controls.Add(this.cmdRegisterStataAutomation);
            this.tabStata.Controls.Add(this.cmdStataLocation);
            this.tabStata.Controls.Add(this.txtStataLocation);
            this.tabStata.Controls.Add(this.label1);
            this.tabStata.Location = new System.Drawing.Point(4, 26);
            this.tabStata.Name = "tabStata";
            this.tabStata.Padding = new System.Windows.Forms.Padding(3);
            this.tabStata.Size = new System.Drawing.Size(665, 197);
            this.tabStata.TabIndex = 1;
            this.tabStata.Text = "Stata";
            this.tabStata.UseVisualStyleBackColor = true;
            // 
            // cmdDisableStataAutomation
            // 
            this.cmdDisableStataAutomation.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cmdDisableStataAutomation.Location = new System.Drawing.Point(335, 66);
            this.cmdDisableStataAutomation.Name = "cmdDisableStataAutomation";
            this.cmdDisableStataAutomation.Size = new System.Drawing.Size(174, 27);
            this.cmdDisableStataAutomation.TabIndex = 19;
            this.cmdDisableStataAutomation.Text = "Disable Automation";
            this.cmdDisableStataAutomation.UseVisualStyleBackColor = true;
            this.cmdDisableStataAutomation.Click += new System.EventHandler(this.cmdDisableStataAutomation_Click);
            // 
            // cmdRegisterStataAutomation
            // 
            this.cmdRegisterStataAutomation.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cmdRegisterStataAutomation.Location = new System.Drawing.Point(155, 66);
            this.cmdRegisterStataAutomation.Name = "cmdRegisterStataAutomation";
            this.cmdRegisterStataAutomation.Size = new System.Drawing.Size(174, 27);
            this.cmdRegisterStataAutomation.TabIndex = 18;
            this.cmdRegisterStataAutomation.Text = "Enable Automation";
            this.cmdRegisterStataAutomation.UseVisualStyleBackColor = true;
            this.cmdRegisterStataAutomation.Click += new System.EventHandler(this.cmdRegisterStataAutomation_Click);
            // 
            // cmdStataLocation
            // 
            this.cmdStataLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdStataLocation.Location = new System.Drawing.Point(623, 35);
            this.cmdStataLocation.Name = "cmdStataLocation";
            this.cmdStataLocation.Size = new System.Drawing.Size(28, 23);
            this.cmdStataLocation.TabIndex = 17;
            this.cmdStataLocation.Text = "...";
            this.cmdStataLocation.UseVisualStyleBackColor = true;
            this.cmdStataLocation.Click += new System.EventHandler(this.cmdStataLocation_Click);
            // 
            // txtStataLocation
            // 
            this.txtStataLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStataLocation.Location = new System.Drawing.Point(23, 35);
            this.txtStataLocation.Name = "txtStataLocation";
            this.txtStataLocation.Size = new System.Drawing.Size(594, 25);
            this.txtStataLocation.TabIndex = 16;
            this.txtStataLocation.TextChanged += new System.EventHandler(this.txtStataLocation_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 17);
            this.label1.TabIndex = 15;
            this.label1.Text = "Stata executable location:";
            // 
            // Settings
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(697, 289);
            this.Controls.Add(this.tabSettings);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "User Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.tabSettings.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.tabLogging.ResumeLayout(false);
            this.tabLogging.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxLogFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxLogSize)).EndInit();
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
        private System.Windows.Forms.Button cmdDisableStataAutomation;
        private System.Windows.Forms.Button cmdRegisterStataAutomation;
        private System.Windows.Forms.Button cmdStataLocation;
        private System.Windows.Forms.TextBox txtStataLocation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown txtMaxLogFiles;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown txtMaxLogSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private Controls.MissingValueSettings missingValueSettings1;
        private System.Windows.Forms.Label lblEmptyValueWarning;
    }
}