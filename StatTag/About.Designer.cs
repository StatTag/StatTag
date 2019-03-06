namespace StatTag
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.txtCitation = new System.Windows.Forms.TextBox();
            this.cmdOK = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.tabSystemDetails = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.cmdCopyToClipboard = new System.Windows.Forms.Button();
            this.rtbSystemDetails = new System.Windows.Forms.RichTextBox();
            this.tabAcknowledgements = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdCopyCitation = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabAbout.SuspendLayout();
            this.tabSystemDetails.SuspendLayout();
            this.tabAcknowledgements.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtCitation
            // 
            this.txtCitation.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCitation.CausesValidation = false;
            this.txtCitation.Location = new System.Drawing.Point(25, 86);
            this.txtCitation.Multiline = true;
            this.txtCitation.Name = "txtCitation";
            this.txtCitation.ReadOnly = true;
            this.txtCitation.Size = new System.Drawing.Size(623, 41);
            this.txtCitation.TabIndex = 26;
            this.txtCitation.Text = "Welty, L.J., Rasmussen, L.V., Baldridge, A.S., & Whitley E. (2016). StatTag. Chic" +
    "ago, Illinois, United States: Galter Health Sciences Library. doi:10.18131/G3K76" +
    "";
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(341, 406);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(87, 25);
            this.cmdOK.TabIndex = 19;
            this.cmdOK.Text = "Close";
            this.cmdOK.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabAbout);
            this.tabControl1.Controls.Add(this.tabAcknowledgements);
            this.tabControl1.Controls.Add(this.tabSystemDetails);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(745, 374);
            this.tabControl1.TabIndex = 20;
            // 
            // tabAbout
            // 
            this.tabAbout.BackColor = System.Drawing.SystemColors.Control;
            this.tabAbout.Controls.Add(this.label8);
            this.tabAbout.Controls.Add(this.cmdCopyCitation);
            this.tabAbout.Controls.Add(this.txtCitation);
            this.tabAbout.Controls.Add(this.label6);
            this.tabAbout.Controls.Add(this.lblCopyright);
            this.tabAbout.Controls.Add(this.lblVersion);
            this.tabAbout.Location = new System.Drawing.Point(4, 26);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tabAbout.Size = new System.Drawing.Size(737, 344);
            this.tabAbout.TabIndex = 0;
            this.tabAbout.Text = "About";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(137, 17);
            this.label6.TabIndex = 25;
            this.label6.Text = "Please cite StatTag as:";
            // 
            // lblCopyright
            // 
            this.lblCopyright.AutoSize = true;
            this.lblCopyright.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCopyright.Location = new System.Drawing.Point(9, 26);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(62, 20);
            this.lblCopyright.TabIndex = 24;
            this.lblCopyright.Text = "(c) 2016";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.Location = new System.Drawing.Point(9, 6);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(90, 20);
            this.lblVersion.TabIndex = 23;
            this.lblVersion.Text = "StatTag v1.0";
            // 
            // tabSystemDetails
            // 
            this.tabSystemDetails.BackColor = System.Drawing.SystemColors.Control;
            this.tabSystemDetails.Controls.Add(this.label7);
            this.tabSystemDetails.Controls.Add(this.cmdCopyToClipboard);
            this.tabSystemDetails.Controls.Add(this.rtbSystemDetails);
            this.tabSystemDetails.Location = new System.Drawing.Point(4, 26);
            this.tabSystemDetails.Name = "tabSystemDetails";
            this.tabSystemDetails.Padding = new System.Windows.Forms.Padding(3);
            this.tabSystemDetails.Size = new System.Drawing.Size(737, 344);
            this.tabSystemDetails.TabIndex = 1;
            this.tabSystemDetails.Text = "System Info";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(3, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(702, 40);
            this.label7.TabIndex = 3;
            this.label7.Text = "If you would like to provide this information to the StatTag team for support, pl" +
    "ease copy the following text to your clipboard.  This information is never autom" +
    "atically shared.";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmdCopyToClipboard
            // 
            this.cmdCopyToClipboard.AccessibleDescription = "Copy the system details to the clipboard";
            this.cmdCopyToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCopyToClipboard.Image = global::StatTag.Properties.Resources.clipboard;
            this.cmdCopyToClipboard.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.cmdCopyToClipboard.Location = new System.Drawing.Point(711, 3);
            this.cmdCopyToClipboard.Name = "cmdCopyToClipboard";
            this.cmdCopyToClipboard.Size = new System.Drawing.Size(23, 30);
            this.cmdCopyToClipboard.TabIndex = 2;
            this.cmdCopyToClipboard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdCopyToClipboard.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdCopyToClipboard.UseVisualStyleBackColor = true;
            this.cmdCopyToClipboard.Click += new System.EventHandler(this.cmdCopyToClipboard_Click);
            // 
            // rtbSystemDetails
            // 
            this.rtbSystemDetails.Location = new System.Drawing.Point(3, 46);
            this.rtbSystemDetails.Name = "rtbSystemDetails";
            this.rtbSystemDetails.Size = new System.Drawing.Size(731, 295);
            this.rtbSystemDetails.TabIndex = 0;
            this.rtbSystemDetails.Text = "";
            // 
            // tabAcknowledgements
            // 
            this.tabAcknowledgements.BackColor = System.Drawing.SystemColors.Control;
            this.tabAcknowledgements.Controls.Add(this.label5);
            this.tabAcknowledgements.Controls.Add(this.label4);
            this.tabAcknowledgements.Controls.Add(this.label3);
            this.tabAcknowledgements.Controls.Add(this.label2);
            this.tabAcknowledgements.Controls.Add(this.label1);
            this.tabAcknowledgements.Location = new System.Drawing.Point(4, 26);
            this.tabAcknowledgements.Name = "tabAcknowledgements";
            this.tabAcknowledgements.Size = new System.Drawing.Size(737, 344);
            this.tabAcknowledgements.TabIndex = 2;
            this.tabAcknowledgements.Text = "Acknowledgements";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(8, 289);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(709, 34);
            this.label5.TabIndex = 9;
            this.label5.Text = "* Use of these projects does not imply endorsement of StatTag by the respective p" +
    "roject owners, or endorsement of the use of these projects by Northwestern Unive" +
    "rsity.\r\n";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(20, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(483, 120);
            this.label4.TabIndex = 8;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(4, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(515, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "StatTag makes use of the following open source projects (licenses included in the" +
    " User\'s Guide)*:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(713, 48);
            this.label2.TabIndex = 6;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(713, 64);
            this.label1.TabIndex = 5;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // cmdCopyCitation
            // 
            this.cmdCopyCitation.AccessibleDescription = "Copy the system details to the clipboard";
            this.cmdCopyCitation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCopyCitation.Image = global::StatTag.Properties.Resources.clipboard;
            this.cmdCopyCitation.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.cmdCopyCitation.Location = new System.Drawing.Point(707, 86);
            this.cmdCopyCitation.Name = "cmdCopyCitation";
            this.cmdCopyCitation.Size = new System.Drawing.Size(24, 30);
            this.cmdCopyCitation.TabIndex = 27;
            this.cmdCopyCitation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdCopyCitation.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdCopyCitation.UseVisualStyleBackColor = true;
            this.cmdCopyCitation.Click += new System.EventHandler(this.cmdCopyCitation_Click);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(10, 151);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(713, 64);
            this.label8.TabIndex = 28;
            this.label8.Text = resources.GetString("label8.Text");
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdOK;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(769, 439);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.cmdOK);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "About";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.Load += new System.EventHandler(this.About_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabAbout.ResumeLayout(false);
            this.tabAbout.PerformLayout();
            this.tabSystemDetails.ResumeLayout(false);
            this.tabAcknowledgements.ResumeLayout(false);
            this.tabAcknowledgements.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.TabPage tabSystemDetails;
        private System.Windows.Forms.RichTextBox rtbSystemDetails;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button cmdCopyToClipboard;
        private System.Windows.Forms.TabPage tabAcknowledgements;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button cmdCopyCitation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCitation;
    }
}