﻿namespace StatTag.Controls
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
            this.txtMissingValueString = new System.Windows.Forms.TextBox();
            this.radMissingValueBlankString = new System.Windows.Forms.RadioButton();
            this.radMissingValueCustomString = new System.Windows.Forms.RadioButton();
            this.radMissingValueStatDefault = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // txtMissingValueString
            // 
            this.txtMissingValueString.Location = new System.Drawing.Point(146, 45);
            this.txtMissingValueString.Name = "txtMissingValueString";
            this.txtMissingValueString.Size = new System.Drawing.Size(80, 25);
            this.txtMissingValueString.TabIndex = 14;
            // 
            // radMissingValueBlankString
            // 
            this.radMissingValueBlankString.AutoSize = true;
            this.radMissingValueBlankString.Location = new System.Drawing.Point(1, 23);
            this.radMissingValueBlankString.Margin = new System.Windows.Forms.Padding(1);
            this.radMissingValueBlankString.Name = "radMissingValueBlankString";
            this.radMissingValueBlankString.Size = new System.Drawing.Size(161, 21);
            this.radMissingValueBlankString.TabIndex = 13;
            this.radMissingValueBlankString.TabStop = true;
            this.radMissingValueBlankString.Text = "An empty (blank) string";
            this.radMissingValueBlankString.UseVisualStyleBackColor = true;
            this.radMissingValueBlankString.CheckedChanged += new System.EventHandler(this.MissingValueRadio_Changed);
            // 
            // radMissingValueCustomString
            // 
            this.radMissingValueCustomString.AutoSize = true;
            this.radMissingValueCustomString.Location = new System.Drawing.Point(1, 46);
            this.radMissingValueCustomString.Margin = new System.Windows.Forms.Padding(1);
            this.radMissingValueCustomString.Name = "radMissingValueCustomString";
            this.radMissingValueCustomString.Size = new System.Drawing.Size(138, 21);
            this.radMissingValueCustomString.TabIndex = 12;
            this.radMissingValueCustomString.TabStop = true;
            this.radMissingValueCustomString.Text = "The following value";
            this.radMissingValueCustomString.UseVisualStyleBackColor = true;
            this.radMissingValueCustomString.CheckedChanged += new System.EventHandler(this.MissingValueRadio_Changed);
            // 
            // radMissingValueStatDefault
            // 
            this.radMissingValueStatDefault.AutoSize = true;
            this.radMissingValueStatDefault.Location = new System.Drawing.Point(1, 0);
            this.radMissingValueStatDefault.Margin = new System.Windows.Forms.Padding(1);
            this.radMissingValueStatDefault.Name = "radMissingValueStatDefault";
            this.radMissingValueStatDefault.Size = new System.Drawing.Size(391, 21);
            this.radMissingValueStatDefault.TabIndex = 11;
            this.radMissingValueStatDefault.TabStop = true;
            this.radMissingValueStatDefault.Text = "The statistical program\'s default (R = \'NA\', SAS = \'.\', Stata = \'.\')";
            this.radMissingValueStatDefault.UseVisualStyleBackColor = true;
            this.radMissingValueStatDefault.CheckedChanged += new System.EventHandler(this.MissingValueRadio_Changed);
            // 
            // MissingValueSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.txtMissingValueString);
            this.Controls.Add(this.radMissingValueBlankString);
            this.Controls.Add(this.radMissingValueCustomString);
            this.Controls.Add(this.radMissingValueStatDefault);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MissingValueSettings";
            this.Size = new System.Drawing.Size(391, 74);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMissingValueString;
        private System.Windows.Forms.RadioButton radMissingValueBlankString;
        private System.Windows.Forms.RadioButton radMissingValueCustomString;
        private System.Windows.Forms.RadioButton radMissingValueStatDefault;
    }
}