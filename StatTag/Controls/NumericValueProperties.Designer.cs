namespace StatTag.Controls
{
    partial class NumericValueProperties
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
            this.chkThousandSeparator = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudDecimalPlaces = new System.Windows.Forms.NumericUpDown();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDecimalPlaces)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            tableLayoutPanel1.Controls.Add(this.chkThousandSeparator, 0, 1);
            tableLayoutPanel1.Controls.Add(this.nudDecimalPlaces, 1, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new System.Drawing.Size(259, 82);
            tableLayoutPanel1.TabIndex = 3;
            // 
            // chkThousandSeparator
            // 
            this.chkThousandSeparator.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(this.chkThousandSeparator, 2);
            this.chkThousandSeparator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkThousandSeparator.Location = new System.Drawing.Point(3, 44);
            this.chkThousandSeparator.Name = "chkThousandSeparator";
            this.chkThousandSeparator.Size = new System.Drawing.Size(253, 35);
            this.chkThousandSeparator.TabIndex = 2;
            this.chkThousandSeparator.Text = "Use thousands separator";
            this.chkThousandSeparator.UseVisualStyleBackColor = true;
            this.chkThousandSeparator.CheckedChanged += new System.EventHandler(this.chkThousandSeparator_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 41);
            this.label1.TabIndex = 1;
            this.label1.Text = "Decimal places:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nudDecimalPlaces
            // 
            this.nudDecimalPlaces.AutoSize = true;
            this.nudDecimalPlaces.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nudDecimalPlaces.Location = new System.Drawing.Point(187, 4);
            this.nudDecimalPlaces.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nudDecimalPlaces.Name = "nudDecimalPlaces";
            this.nudDecimalPlaces.Size = new System.Drawing.Size(69, 33);
            this.nudDecimalPlaces.TabIndex = 0;
            this.nudDecimalPlaces.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudDecimalPlaces.ValueChanged += new System.EventHandler(this.nudDecimalPlaces_ValueChanged);
            // 
            // NumericValueProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "NumericValueProperties";
            this.Size = new System.Drawing.Size(259, 82);
            this.Load += new System.EventHandler(this.NumericValueProperties_Load);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDecimalPlaces)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudDecimalPlaces;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkThousandSeparator;
    }
}
