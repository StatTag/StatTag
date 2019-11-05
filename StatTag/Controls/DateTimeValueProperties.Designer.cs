namespace StatTag.Controls
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
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
            this.chkShowTime = new System.Windows.Forms.CheckBox();
            this.chkShowDate = new System.Windows.Forms.CheckBox();
            this.cboDate = new System.Windows.Forms.ComboBox();
            this.cboTime = new System.Windows.Forms.ComboBox();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(this.chkShowTime, 0, 1);
            tableLayoutPanel1.Controls.Add(this.chkShowDate, 0, 0);
            tableLayoutPanel1.Controls.Add(this.cboDate, 1, 0);
            tableLayoutPanel1.Controls.Add(this.cboTime, 1, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new System.Drawing.Size(259, 80);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // chkShowTime
            // 
            this.chkShowTime.AutoSize = true;
            this.chkShowTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkShowTime.Location = new System.Drawing.Point(3, 44);
            this.chkShowTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkShowTime.Name = "chkShowTime";
            this.chkShowTime.Size = new System.Drawing.Size(130, 32);
            this.chkShowTime.TabIndex = 1;
            this.chkShowTime.Text = "Show time";
            this.chkShowTime.UseVisualStyleBackColor = true;
            this.chkShowTime.CheckedChanged += new System.EventHandler(this.chkShowTime_CheckedChanged);
            // 
            // chkShowDate
            // 
            this.chkShowDate.AutoSize = true;
            this.chkShowDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkShowDate.Location = new System.Drawing.Point(3, 4);
            this.chkShowDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkShowDate.Name = "chkShowDate";
            this.chkShowDate.Size = new System.Drawing.Size(130, 32);
            this.chkShowDate.TabIndex = 0;
            this.chkShowDate.Text = "Show date";
            this.chkShowDate.UseVisualStyleBackColor = true;
            this.chkShowDate.CheckedChanged += new System.EventHandler(this.chkShowDate_CheckedChanged);
            // 
            // cboDate
            // 
            this.cboDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDate.Enabled = false;
            this.cboDate.FormattingEnabled = true;
            this.cboDate.Location = new System.Drawing.Point(139, 4);
            this.cboDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cboDate.Name = "cboDate";
            this.cboDate.Size = new System.Drawing.Size(114, 36);
            this.cboDate.TabIndex = 2;
            this.cboDate.SelectedIndexChanged += new System.EventHandler(this.cboDate_SelectedIndexChanged);
            // 
            // cboTime
            // 
            this.cboTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTime.Enabled = false;
            this.cboTime.FormattingEnabled = true;
            this.cboTime.Location = new System.Drawing.Point(139, 44);
            this.cboTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cboTime.Name = "cboTime";
            this.cboTime.Size = new System.Drawing.Size(114, 36);
            this.cboTime.TabIndex = 3;
            this.cboTime.SelectedIndexChanged += new System.EventHandler(this.cboTime_SelectedIndexChanged);
            // 
            // DateTimeValueProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "DateTimeValueProperties";
            this.Size = new System.Drawing.Size(259, 80);
            this.Load += new System.EventHandler(this.DateTimeValueProperties_Load);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
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
