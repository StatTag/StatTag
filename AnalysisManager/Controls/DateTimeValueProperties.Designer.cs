namespace AnalysisManager.Controls
{
    partial class DateTimeValueProperties
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkShowDate = new System.Windows.Forms.CheckBox();
            this.chkShowTime = new System.Windows.Forms.CheckBox();
            this.cboDate = new System.Windows.Forms.ComboBox();
            this.cboTime = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // chkShowDate
            // 
            this.chkShowDate.AutoSize = true;
            this.chkShowDate.Location = new System.Drawing.Point(5, 5);
            this.chkShowDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkShowDate.Name = "chkShowDate";
            this.chkShowDate.Size = new System.Drawing.Size(88, 21);
            this.chkShowDate.TabIndex = 0;
            this.chkShowDate.Text = "Show date";
            this.chkShowDate.UseVisualStyleBackColor = true;
            this.chkShowDate.CheckedChanged += new System.EventHandler(this.chkShowDate_CheckedChanged);
            // 
            // chkShowTime
            // 
            this.chkShowTime.AutoSize = true;
            this.chkShowTime.Location = new System.Drawing.Point(5, 38);
            this.chkShowTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkShowTime.Name = "chkShowTime";
            this.chkShowTime.Size = new System.Drawing.Size(87, 21);
            this.chkShowTime.TabIndex = 1;
            this.chkShowTime.Text = "Show time";
            this.chkShowTime.UseVisualStyleBackColor = true;
            this.chkShowTime.CheckedChanged += new System.EventHandler(this.chkShowTime_CheckedChanged);
            // 
            // cboDate
            // 
            this.cboDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDate.Enabled = false;
            this.cboDate.FormattingEnabled = true;
            this.cboDate.Location = new System.Drawing.Point(103, 5);
            this.cboDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cboDate.Name = "cboDate";
            this.cboDate.Size = new System.Drawing.Size(140, 25);
            this.cboDate.TabIndex = 2;
            this.cboDate.SelectedIndexChanged += new System.EventHandler(this.cboDate_SelectedIndexChanged);
            // 
            // cboTime
            // 
            this.cboTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTime.Enabled = false;
            this.cboTime.FormattingEnabled = true;
            this.cboTime.Location = new System.Drawing.Point(103, 38);
            this.cboTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cboTime.Name = "cboTime";
            this.cboTime.Size = new System.Drawing.Size(140, 25);
            this.cboTime.TabIndex = 3;
            this.cboTime.SelectedIndexChanged += new System.EventHandler(this.cboTime_SelectedIndexChanged);
            // 
            // DateTimeValueProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboTime);
            this.Controls.Add(this.cboDate);
            this.Controls.Add(this.chkShowTime);
            this.Controls.Add(this.chkShowDate);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "DateTimeValueProperties";
            this.Size = new System.Drawing.Size(259, 76);
            this.Load += new System.EventHandler(this.DateTimeValueProperties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkShowDate;
        private System.Windows.Forms.CheckBox chkShowTime;
        private System.Windows.Forms.ComboBox cboDate;
        private System.Windows.Forms.ComboBox cboTime;
    }
}
