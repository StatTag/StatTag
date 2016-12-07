﻿namespace StatTag.Controls
{
    partial class TableProperties
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
            this.chkExcludeColumns = new System.Windows.Forms.CheckBox();
            this.chkExcludeRows = new System.Windows.Forms.CheckBox();
            this.txtRows = new System.Windows.Forms.TextBox();
            this.txtColumns = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericValueProperties = new StatTag.Controls.NumericValueProperties();
            this.label2 = new System.Windows.Forms.Label();
            this.horizontalLine1 = new StatTag.Controls.HorizontalLine();
            this.SuspendLayout();
            // 
            // chkExcludeColumns
            // 
            this.chkExcludeColumns.AutoSize = true;
            this.chkExcludeColumns.Location = new System.Drawing.Point(14, 57);
            this.chkExcludeColumns.Margin = new System.Windows.Forms.Padding(3, 4, 0, 4);
            this.chkExcludeColumns.Name = "chkExcludeColumns";
            this.chkExcludeColumns.Size = new System.Drawing.Size(134, 21);
            this.chkExcludeColumns.TabIndex = 0;
            this.chkExcludeColumns.Text = "Exclude column(s):";
            this.chkExcludeColumns.UseVisualStyleBackColor = true;
            this.chkExcludeColumns.CheckedChanged += new System.EventHandler(this.chkExcludeColumns_CheckedChanged);
            // 
            // chkExcludeRows
            // 
            this.chkExcludeRows.AutoSize = true;
            this.chkExcludeRows.Location = new System.Drawing.Point(14, 26);
            this.chkExcludeRows.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.chkExcludeRows.Name = "chkExcludeRows";
            this.chkExcludeRows.Size = new System.Drawing.Size(114, 21);
            this.chkExcludeRows.TabIndex = 1;
            this.chkExcludeRows.Text = "Exclude row(s):";
            this.chkExcludeRows.UseVisualStyleBackColor = true;
            this.chkExcludeRows.CheckedChanged += new System.EventHandler(this.chkExcludeRows_CheckedChanged);
            // 
            // txtRows
            // 
            this.txtRows.Enabled = false;
            this.txtRows.Location = new System.Drawing.Point(148, 24);
            this.txtRows.Name = "txtRows";
            this.txtRows.Size = new System.Drawing.Size(119, 25);
            this.txtRows.TabIndex = 3;
            // 
            // txtColumns
            // 
            this.txtColumns.Enabled = false;
            this.txtColumns.Location = new System.Drawing.Point(148, 55);
            this.txtColumns.Name = "txtColumns";
            this.txtColumns.Size = new System.Drawing.Size(119, 25);
            this.txtColumns.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 83);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(256, 32);
            this.label1.TabIndex = 5;
            this.label1.Text = "Enter the values or ranges to exclude, separated by commas: (e.g. 1, 3, 8-10)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // numericValueProperties
            // 
            this.numericValueProperties.DecimalPlaces = 0;
            this.numericValueProperties.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericValueProperties.Location = new System.Drawing.Point(10, 127);
            this.numericValueProperties.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numericValueProperties.Name = "numericValueProperties";
            this.numericValueProperties.Size = new System.Drawing.Size(188, 63);
            this.numericValueProperties.TabIndex = 2;
            this.numericValueProperties.UseThousands = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Table Options";
            // 
            // horizontalLine1
            // 
            this.horizontalLine1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.horizontalLine1.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.horizontalLine1.Location = new System.Drawing.Point(0, 115);
            this.horizontalLine1.Margin = new System.Windows.Forms.Padding(0);
            this.horizontalLine1.Name = "horizontalLine1";
            this.horizontalLine1.Size = new System.Drawing.Size(279, 10);
            this.horizontalLine1.TabIndex = 9;
            // 
            // TableProperties
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.horizontalLine1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtColumns);
            this.Controls.Add(this.txtRows);
            this.Controls.Add(this.numericValueProperties);
            this.Controls.Add(this.chkExcludeRows);
            this.Controls.Add(this.chkExcludeColumns);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TableProperties";
            this.Size = new System.Drawing.Size(279, 201);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkExcludeColumns;
        private System.Windows.Forms.CheckBox chkExcludeRows;
        private NumericValueProperties numericValueProperties;
        private System.Windows.Forms.TextBox txtRows;
        private System.Windows.Forms.TextBox txtColumns;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private HorizontalLine horizontalLine1;
    }
}
