namespace StatTag
{
    partial class DocumentProperties
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
            this.lblMissingValueInformation = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.missingValueSettings1 = new StatTag.Controls.MissingValueSettings();
            this.SuspendLayout();
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(344, 137);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(87, 25);
            this.cmdCancel.TabIndex = 11;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(143, 137);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(87, 25);
            this.cmdOK.TabIndex = 10;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // lblMissingValueInformation
            // 
            this.lblMissingValueInformation.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMissingValueInformation.Location = new System.Drawing.Point(281, 55);
            this.lblMissingValueInformation.Name = "lblMissingValueInformation";
            this.lblMissingValueInformation.Size = new System.Drawing.Size(280, 66);
            this.lblMissingValueInformation.TabIndex = 14;
            this.lblMissingValueInformation.Text = "[Placeholder for informational messages]";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(549, 17);
            this.label5.TabIndex = 17;
            this.label5.Text = "For this document, if a table has a missing value (empty cell), display the missi" +
    "ng value using:";
            // 
            // missingValueSettings1
            // 
            this.missingValueSettings1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.missingValueSettings1.Location = new System.Drawing.Point(27, 29);
            this.missingValueSettings1.Margin = new System.Windows.Forms.Padding(0);
            this.missingValueSettings1.Name = "missingValueSettings1";
            this.missingValueSettings1.Size = new System.Drawing.Size(391, 74);
            this.missingValueSettings1.TabIndex = 18;
            // 
            // DocumentProperties
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(575, 174);
            this.Controls.Add(this.lblMissingValueInformation);
            this.Controls.Add(this.missingValueSettings1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "DocumentProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Document Properties";
            this.Shown += new System.EventHandler(this.DocumentProperties_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Label lblMissingValueInformation;
        private Controls.MissingValueSettings missingValueSettings1;
        private System.Windows.Forms.Label label5;
    }
}