namespace StatTag.Controls
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
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.horizontalLine1 = new StatTag.Controls.HorizontalLine();
            this.valueProperties1 = new StatTag.Controls.ValueProperties();
            this.chkExcludeRows = new System.Windows.Forms.CheckBox();
            this.chkExcludeColumns = new System.Windows.Forms.CheckBox();
            this.txtRows = new System.Windows.Forms.TextBox();
            this.txtColumns = new System.Windows.Forms.TextBox();
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
            tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            tableLayoutPanel1.Controls.Add(this.valueProperties1, 0, 5);
            tableLayoutPanel1.Controls.Add(this.chkExcludeRows, 0, 1);
            tableLayoutPanel1.Controls.Add(this.horizontalLine1, 0, 4);
            tableLayoutPanel1.Controls.Add(this.chkExcludeColumns, 0, 2);
            tableLayoutPanel1.Controls.Add(this.label1, 0, 3);
            tableLayoutPanel1.Controls.Add(this.txtRows, 1, 1);
            tableLayoutPanel1.Controls.Add(this.txtColumns, 1, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 6;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(258, 302);
            tableLayoutPanel1.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 108);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(258, 69);
            this.label1.TabIndex = 5;
            this.label1.Text = "Enter the values or ranges to exclude, separated by commas: (e.g. 1, 3, 8-10)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 28);
            this.label2.TabIndex = 8;
            this.label2.Text = "Table Options";
            // 
            // horizontalLine1
            // 
            this.horizontalLine1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.horizontalLine1.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            tableLayoutPanel1.SetColumnSpan(this.horizontalLine1, 2);
            this.horizontalLine1.Location = new System.Drawing.Point(0, 177);
            this.horizontalLine1.Margin = new System.Windows.Forms.Padding(0);
            this.horizontalLine1.Name = "horizontalLine1";
            this.horizontalLine1.Size = new System.Drawing.Size(258, 10);
            this.horizontalLine1.TabIndex = 9;
            // 
            // valueProperties1
            // 
            this.valueProperties1.AutoSize = true;
            this.valueProperties1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.SetColumnSpan(this.valueProperties1, 2);
            this.valueProperties1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.valueProperties1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valueProperties1.Location = new System.Drawing.Point(3, 191);
            this.valueProperties1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.valueProperties1.Name = "valueProperties1";
            this.valueProperties1.Size = new System.Drawing.Size(252, 107);
            this.valueProperties1.TabIndex = 10;
            // 
            // chkExcludeRows
            // 
            this.chkExcludeRows.AutoSize = true;
            this.chkExcludeRows.Location = new System.Drawing.Point(3, 32);
            this.chkExcludeRows.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkExcludeRows.Name = "chkExcludeRows";
            this.chkExcludeRows.Size = new System.Drawing.Size(166, 32);
            this.chkExcludeRows.TabIndex = 1;
            this.chkExcludeRows.Text = "Exclude row(s):";
            this.chkExcludeRows.UseVisualStyleBackColor = true;
            this.chkExcludeRows.CheckedChanged += new System.EventHandler(this.chkExcludeRows_CheckedChanged);
            // 
            // chkExcludeColumns
            // 
            this.chkExcludeColumns.AutoSize = true;
            this.chkExcludeColumns.Location = new System.Drawing.Point(3, 72);
            this.chkExcludeColumns.Margin = new System.Windows.Forms.Padding(3, 4, 0, 4);
            this.chkExcludeColumns.Name = "chkExcludeColumns";
            this.chkExcludeColumns.Size = new System.Drawing.Size(198, 32);
            this.chkExcludeColumns.TabIndex = 0;
            this.chkExcludeColumns.Text = "Exclude column(s):";
            this.chkExcludeColumns.UseVisualStyleBackColor = true;
            this.chkExcludeColumns.CheckedChanged += new System.EventHandler(this.chkExcludeColumns_CheckedChanged);
            // 
            // txtRows
            // 
            this.txtRows.Enabled = false;
            this.txtRows.Location = new System.Drawing.Point(204, 31);
            this.txtRows.Name = "txtRows";
            this.txtRows.Size = new System.Drawing.Size(51, 33);
            this.txtRows.TabIndex = 3;
            // 
            // txtColumns
            // 
            this.txtColumns.Enabled = false;
            this.txtColumns.Location = new System.Drawing.Point(204, 71);
            this.txtColumns.Name = "txtColumns";
            this.txtColumns.Size = new System.Drawing.Size(51, 33);
            this.txtColumns.TabIndex = 4;
            // 
            // TableProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TableProperties";
            this.Size = new System.Drawing.Size(258, 302);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkExcludeColumns;
        private System.Windows.Forms.CheckBox chkExcludeRows;
        private System.Windows.Forms.TextBox txtRows;
        private System.Windows.Forms.TextBox txtColumns;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private HorizontalLine horizontalLine1;
        private ValueProperties valueProperties1;
    }
}
