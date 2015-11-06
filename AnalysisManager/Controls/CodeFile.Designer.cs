namespace AnalysisManager.Controls
{
    partial class CodeFile
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
            this.cboStatPackage = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.cmdLoadFile = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlMetadata = new System.Windows.Forms.Panel();
            this.cmdMetadataToggle = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lblChecksum = new System.Windows.Forms.Label();
            this.lblLastCached = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmdDelete = new System.Windows.Forms.Button();
            this.pnlMetadata.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboStatPackage
            // 
            this.cboStatPackage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatPackage.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboStatPackage.FormattingEnabled = true;
            this.cboStatPackage.Items.AddRange(new object[] {
            "Stata",
            "R",
            "SAS"});
            this.cboStatPackage.Location = new System.Drawing.Point(72, 0);
            this.cboStatPackage.Name = "cboStatPackage";
            this.cboStatPackage.Size = new System.Drawing.Size(121, 25);
            this.cboStatPackage.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Run in:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Code file:";
            // 
            // txtFilePath
            // 
            this.txtFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilePath.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilePath.Location = new System.Drawing.Point(72, 29);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(363, 25);
            this.txtFilePath.TabIndex = 3;
            // 
            // cmdLoadFile
            // 
            this.cmdLoadFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdLoadFile.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdLoadFile.Location = new System.Drawing.Point(436, 29);
            this.cmdLoadFile.Name = "cmdLoadFile";
            this.cmdLoadFile.Size = new System.Drawing.Size(28, 23);
            this.cmdLoadFile.TabIndex = 4;
            this.cmdLoadFile.Text = "...";
            this.cmdLoadFile.UseVisualStyleBackColor = true;
            this.cmdLoadFile.Click += new System.EventHandler(this.cmdLoadFile_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Details:";
            // 
            // pnlMetadata
            // 
            this.pnlMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMetadata.Controls.Add(this.lblLastCached);
            this.pnlMetadata.Controls.Add(this.label6);
            this.pnlMetadata.Controls.Add(this.lblChecksum);
            this.pnlMetadata.Controls.Add(this.label4);
            this.pnlMetadata.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.pnlMetadata.Location = new System.Drawing.Point(0, 74);
            this.pnlMetadata.Name = "pnlMetadata";
            this.pnlMetadata.Size = new System.Drawing.Size(463, 65);
            this.pnlMetadata.TabIndex = 7;
            // 
            // cmdMetadataToggle
            // 
            this.cmdMetadataToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdMetadataToggle.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdMetadataToggle.Location = new System.Drawing.Point(51, 58);
            this.cmdMetadataToggle.Margin = new System.Windows.Forms.Padding(0);
            this.cmdMetadataToggle.Name = "cmdMetadataToggle";
            this.cmdMetadataToggle.Size = new System.Drawing.Size(30, 15);
            this.cmdMetadataToggle.TabIndex = 8;
            this.cmdMetadataToggle.Text = ">>";
            this.cmdMetadataToggle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.cmdMetadataToggle.UseVisualStyleBackColor = true;
            this.cmdMetadataToggle.Click += new System.EventHandler(this.cmdMetadataToggle_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.label4.Location = new System.Drawing.Point(3, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Checksum:";
            // 
            // lblChecksum
            // 
            this.lblChecksum.AutoSize = true;
            this.lblChecksum.Location = new System.Drawing.Point(79, 30);
            this.lblChecksum.Name = "lblChecksum";
            this.lblChecksum.Size = new System.Drawing.Size(80, 13);
            this.lblChecksum.TabIndex = 1;
            this.lblChecksum.Text = "(Not available)";
            // 
            // lblLastCached
            // 
            this.lblLastCached.AutoSize = true;
            this.lblLastCached.Location = new System.Drawing.Point(79, 7);
            this.lblLastCached.Name = "lblLastCached";
            this.lblLastCached.Size = new System.Drawing.Size(80, 13);
            this.lblLastCached.TabIndex = 3;
            this.lblLastCached.Text = "(Not available)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.label6.Location = new System.Drawing.Point(4, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Last cached:";
            // 
            // cmdDelete
            // 
            this.cmdDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdDelete.BackgroundImage = global::AnalysisManager.Properties.Resources._1446843246_delete;
            this.cmdDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.cmdDelete.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdDelete.Location = new System.Drawing.Point(444, 0);
            this.cmdDelete.Name = "cmdDelete";
            this.cmdDelete.Size = new System.Drawing.Size(20, 20);
            this.cmdDelete.TabIndex = 9;
            this.cmdDelete.UseVisualStyleBackColor = true;
            // 
            // CodeFile
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Controls.Add(this.cmdDelete);
            this.Controls.Add(this.cmdMetadataToggle);
            this.Controls.Add(this.pnlMetadata);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmdLoadFile);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboStatPackage);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "CodeFile";
            this.Size = new System.Drawing.Size(466, 142);
            this.Load += new System.EventHandler(this.CodeFile_Load);
            this.pnlMetadata.ResumeLayout(false);
            this.pnlMetadata.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboStatPackage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button cmdLoadFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlMetadata;
        private System.Windows.Forms.Button cmdMetadataToggle;
        private System.Windows.Forms.Button cmdDelete;
        private System.Windows.Forms.Label lblLastCached;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblChecksum;
        private System.Windows.Forms.Label label4;
    }
}
