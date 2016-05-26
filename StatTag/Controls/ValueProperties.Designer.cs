namespace StatTag.Controls
{
    partial class ValueProperties
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
            this.radDefault = new System.Windows.Forms.RadioButton();
            this.radNumeric = new System.Windows.Forms.RadioButton();
            this.radDateTime = new System.Windows.Forms.RadioButton();
            this.radPercentage = new System.Windows.Forms.RadioButton();
            this.pnlDetails = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // radDefault
            // 
            this.radDefault.AutoSize = true;
            this.radDefault.Checked = true;
            this.radDefault.Location = new System.Drawing.Point(4, 4);
            this.radDefault.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.radDefault.Name = "radDefault";
            this.radDefault.Size = new System.Drawing.Size(293, 21);
            this.radDefault.TabIndex = 0;
            this.radDefault.TabStop = true;
            this.radDefault.Text = "Default (exact output from statistical package)";
            this.radDefault.UseVisualStyleBackColor = true;
            this.radDefault.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // radNumeric
            // 
            this.radNumeric.AutoSize = true;
            this.radNumeric.Location = new System.Drawing.Point(4, 28);
            this.radNumeric.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.radNumeric.Name = "radNumeric";
            this.radNumeric.Size = new System.Drawing.Size(75, 21);
            this.radNumeric.TabIndex = 1;
            this.radNumeric.Text = "Numeric";
            this.radNumeric.UseVisualStyleBackColor = true;
            this.radNumeric.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // radDateTime
            // 
            this.radDateTime.AutoSize = true;
            this.radDateTime.Location = new System.Drawing.Point(4, 52);
            this.radDateTime.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.radDateTime.Name = "radDateTime";
            this.radDateTime.Size = new System.Drawing.Size(86, 21);
            this.radDateTime.TabIndex = 2;
            this.radDateTime.Text = "Date/Time";
            this.radDateTime.UseVisualStyleBackColor = true;
            this.radDateTime.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // radPercentage
            // 
            this.radPercentage.AutoSize = true;
            this.radPercentage.Location = new System.Drawing.Point(4, 76);
            this.radPercentage.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.radPercentage.Name = "radPercentage";
            this.radPercentage.Size = new System.Drawing.Size(91, 21);
            this.radPercentage.TabIndex = 3;
            this.radPercentage.Text = "Percentage";
            this.radPercentage.UseVisualStyleBackColor = true;
            this.radPercentage.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // pnlDetails
            // 
            this.pnlDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDetails.Location = new System.Drawing.Point(111, 32);
            this.pnlDetails.Name = "pnlDetails";
            this.pnlDetails.Size = new System.Drawing.Size(231, 70);
            this.pnlDetails.TabIndex = 4;
            // 
            // ValueProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.pnlDetails);
            this.Controls.Add(this.radPercentage);
            this.Controls.Add(this.radDateTime);
            this.Controls.Add(this.radNumeric);
            this.Controls.Add(this.radDefault);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ValueProperties";
            this.Size = new System.Drawing.Size(342, 102);
            this.Load += new System.EventHandler(this.ValueProperties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radDefault;
        private System.Windows.Forms.RadioButton radNumeric;
        private System.Windows.Forms.RadioButton radDateTime;
        private System.Windows.Forms.RadioButton radPercentage;
        private System.Windows.Forms.Panel pnlDetails;
    }
}
