namespace AnalysisManager
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
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmdDisableStataAutomation = new System.Windows.Forms.Button();
            this.cmdRegisterStataAutomation = new System.Windows.Forms.Button();
            this.cmdStataLocation = new System.Windows.Forms.Button();
            this.txtStataLocation = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdCancel.Location = new System.Drawing.Point(262, 237);
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
            this.cmdOK.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOK.Location = new System.Drawing.Point(112, 237);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(87, 25);
            this.cmdOK.TabIndex = 8;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 17);
            this.label1.TabIndex = 10;
            this.label1.Text = "Stata executable location:";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(441, 87);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmdDisableStataAutomation);
            this.groupBox2.Controls.Add(this.cmdRegisterStataAutomation);
            this.groupBox2.Controls.Add(this.cmdStataLocation);
            this.groupBox2.Controls.Add(this.txtStataLocation);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 105);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(441, 113);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Stata";
            // 
            // cmdDisableStataAutomation
            // 
            this.cmdDisableStataAutomation.Location = new System.Drawing.Point(230, 76);
            this.cmdDisableStataAutomation.Name = "cmdDisableStataAutomation";
            this.cmdDisableStataAutomation.Size = new System.Drawing.Size(174, 27);
            this.cmdDisableStataAutomation.TabIndex = 14;
            this.cmdDisableStataAutomation.Text = "Disable Automation";
            this.cmdDisableStataAutomation.UseVisualStyleBackColor = true;
            this.cmdDisableStataAutomation.Click += new System.EventHandler(this.cmdDisableStataAutomation_Click);
            // 
            // cmdRegisterStataAutomation
            // 
            this.cmdRegisterStataAutomation.Location = new System.Drawing.Point(37, 76);
            this.cmdRegisterStataAutomation.Name = "cmdRegisterStataAutomation";
            this.cmdRegisterStataAutomation.Size = new System.Drawing.Size(174, 27);
            this.cmdRegisterStataAutomation.TabIndex = 13;
            this.cmdRegisterStataAutomation.Text = "Enable Automation";
            this.cmdRegisterStataAutomation.UseVisualStyleBackColor = true;
            this.cmdRegisterStataAutomation.Click += new System.EventHandler(this.cmdRegisterStataAutomation_Click);
            // 
            // cmdStataLocation
            // 
            this.cmdStataLocation.Location = new System.Drawing.Point(407, 42);
            this.cmdStataLocation.Name = "cmdStataLocation";
            this.cmdStataLocation.Size = new System.Drawing.Size(28, 23);
            this.cmdStataLocation.TabIndex = 12;
            this.cmdStataLocation.Text = "...";
            this.cmdStataLocation.UseVisualStyleBackColor = true;
            this.cmdStataLocation.Click += new System.EventHandler(this.cmdStataLocation_Click);
            // 
            // txtStataLocation
            // 
            this.txtStataLocation.Location = new System.Drawing.Point(9, 42);
            this.txtStataLocation.Name = "txtStataLocation";
            this.txtStataLocation.Size = new System.Drawing.Size(395, 25);
            this.txtStataLocation.TabIndex = 11;
            this.txtStataLocation.TextChanged += new System.EventHandler(this.txtStataLocation_TextChanged);
            // 
            // Settings
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(465, 275);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Analysis Manager - Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmdDisableStataAutomation;
        private System.Windows.Forms.Button cmdRegisterStataAutomation;
        private System.Windows.Forms.Button cmdStataLocation;
        private System.Windows.Forms.TextBox txtStataLocation;
    }
}