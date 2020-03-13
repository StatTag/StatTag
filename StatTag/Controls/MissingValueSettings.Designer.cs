namespace StatTag.Controls
{
    partial class MissingValueSettings
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
            System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
            this.radMissingValueBlankString = new System.Windows.Forms.RadioButton();
            this.radMissingValueStatDefault = new System.Windows.Forms.RadioButton();
            this.radMissingValueCustomString = new System.Windows.Forms.RadioButton();
            this.txtMissingValueString = new System.Windows.Forms.TextBox();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // radMissingValueBlankString
            // 
            this.radMissingValueBlankString.AutoSize = true;
            this.radMissingValueBlankString.Checked = true;
            this.radMissingValueBlankString.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radMissingValueBlankString.Location = new System.Drawing.Point(1, 24);
            this.radMissingValueBlankString.Margin = new System.Windows.Forms.Padding(1);
            this.radMissingValueBlankString.Name = "radMissingValueBlankString";
            this.radMissingValueBlankString.Size = new System.Drawing.Size(391, 21);
            this.radMissingValueBlankString.TabIndex = 13;
            this.radMissingValueBlankString.TabStop = true;
            this.radMissingValueBlankString.Text = "An empty (blank) string";
            this.radMissingValueBlankString.UseVisualStyleBackColor = true;
            this.radMissingValueBlankString.CheckedChanged += new System.EventHandler(this.MissingValueRadio_Changed);
            // 
            // radMissingValueStatDefault
            // 
            this.radMissingValueStatDefault.AutoSize = true;
            this.radMissingValueStatDefault.Location = new System.Drawing.Point(1, 1);
            this.radMissingValueStatDefault.Margin = new System.Windows.Forms.Padding(1);
            this.radMissingValueStatDefault.Name = "radMissingValueStatDefault";
            this.radMissingValueStatDefault.Size = new System.Drawing.Size(391, 21);
            this.radMissingValueStatDefault.TabIndex = 11;
            this.radMissingValueStatDefault.Text = "The statistical program\'s default (R = \'\', SAS = \'.\', Stata = \'.\')";
            this.radMissingValueStatDefault.UseVisualStyleBackColor = true;
            this.radMissingValueStatDefault.CheckedChanged += new System.EventHandler(this.MissingValueRadio_Changed);
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(this.radMissingValueStatDefault, 0, 0);
            tableLayoutPanel1.Controls.Add(this.radMissingValueBlankString, 0, 1);
            tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(393, 77);
            tableLayoutPanel1.TabIndex = 15;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.Controls.Add(this.radMissingValueCustomString);
            flowLayoutPanel1.Controls.Add(this.txtMissingValueString);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanel1.Location = new System.Drawing.Point(0, 46);
            flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(393, 31);
            flowLayoutPanel1.TabIndex = 14;
            // 
            // radMissingValueCustomString
            // 
            this.radMissingValueCustomString.AutoSize = true;
            this.radMissingValueCustomString.Location = new System.Drawing.Point(1, 1);
            this.radMissingValueCustomString.Margin = new System.Windows.Forms.Padding(1);
            this.radMissingValueCustomString.Name = "radMissingValueCustomString";
            this.radMissingValueCustomString.Size = new System.Drawing.Size(138, 21);
            this.radMissingValueCustomString.TabIndex = 15;
            this.radMissingValueCustomString.Text = "The following value";
            this.radMissingValueCustomString.UseVisualStyleBackColor = true;
            this.radMissingValueCustomString.CheckedChanged += new System.EventHandler(this.MissingValueRadio_Changed);
            // 
            // txtMissingValueString
            // 
            this.txtMissingValueString.Location = new System.Drawing.Point(143, 3);
            this.txtMissingValueString.Name = "txtMissingValueString";
            this.txtMissingValueString.Size = new System.Drawing.Size(80, 25);
            this.txtMissingValueString.TabIndex = 16;
            // 
            // MissingValueSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.Controls.Add(tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MissingValueSettings";
            this.Size = new System.Drawing.Size(393, 77);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radMissingValueBlankString;
        private System.Windows.Forms.RadioButton radMissingValueStatDefault;
        private System.Windows.Forms.RadioButton radMissingValueCustomString;
        private System.Windows.Forms.TextBox txtMissingValueString;
    }
}
