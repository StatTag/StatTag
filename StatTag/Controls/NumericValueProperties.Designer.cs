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
            this.nudDecimalPlaces = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.chkThousandSeparator = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudDecimalPlaces)).BeginInit();
            this.SuspendLayout();
            // 
            // nudDecimalPlaces
            // 
            this.nudDecimalPlaces.Location = new System.Drawing.Point(107, 4);
            this.nudDecimalPlaces.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nudDecimalPlaces.Name = "nudDecimalPlaces";
            this.nudDecimalPlaces.Size = new System.Drawing.Size(73, 25);
            this.nudDecimalPlaces.TabIndex = 0;
            this.nudDecimalPlaces.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudDecimalPlaces.ValueChanged += new System.EventHandler(this.nudDecimalPlaces_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Decimal places:";
            // 
            // chkThousandSeparator
            // 
            this.chkThousandSeparator.AutoSize = true;
            this.chkThousandSeparator.Location = new System.Drawing.Point(6, 36);
            this.chkThousandSeparator.Name = "chkThousandSeparator";
            this.chkThousandSeparator.Size = new System.Drawing.Size(174, 21);
            this.chkThousandSeparator.TabIndex = 2;
            this.chkThousandSeparator.Text = "Use thousands separator";
            this.chkThousandSeparator.UseVisualStyleBackColor = true;
            this.chkThousandSeparator.CheckedChanged += new System.EventHandler(this.chkThousandSeparator_CheckedChanged);
            // 
            // NumericValueProperties
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.chkThousandSeparator);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudDecimalPlaces);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "NumericValueProperties";
            this.Size = new System.Drawing.Size(188, 63);
            this.Load += new System.EventHandler(this.NumericValueProperties_Load);
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
