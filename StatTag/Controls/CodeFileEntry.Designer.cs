namespace StatTag.Controls
{
    partial class CodeFileEntry
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CodeFileEntry));
            StatTag.Controls.HorizontalLine horizontalLine1;
            this.lblFileName = new System.Windows.Forms.Label();
            this.lblFilePath = new System.Windows.Forms.Label();
            this.imgR = new System.Windows.Forms.PictureBox();
            this.imgSAS = new System.Windows.Forms.PictureBox();
            this.imgStata = new System.Windows.Forms.PictureBox();
            this.imgWarning = new System.Windows.Forms.PictureBox();
            horizontalLine1 = new StatTag.Controls.HorizontalLine();
            ((System.ComponentModel.ISupportInitialize)(this.imgR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgSAS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgStata)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgWarning)).BeginInit();
            this.SuspendLayout();
            // 
            // lblFileName
            // 
            this.lblFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFileName.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileName.Location = new System.Drawing.Point(47, 0);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(361, 23);
            this.lblFileName.TabIndex = 1;
            this.lblFileName.Text = "[File Name]";
            this.lblFileName.Click += new System.EventHandler(this.CodeFileEntry_Click);
            this.lblFileName.DoubleClick += new System.EventHandler(this.CodeFileEntry_DoubleClick);
            // 
            // lblFilePath
            // 
            this.lblFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFilePath.Font = new System.Drawing.Font("Segoe UI Light", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilePath.Location = new System.Drawing.Point(48, 23);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new System.Drawing.Size(386, 17);
            this.lblFilePath.TabIndex = 2;
            this.lblFilePath.Text = "[File Directory]";
            this.lblFilePath.Click += new System.EventHandler(this.CodeFileEntry_Click);
            this.lblFilePath.DoubleClick += new System.EventHandler(this.CodeFileEntry_DoubleClick);
            // 
            // imgR
            // 
            this.imgR.BackColor = System.Drawing.Color.Transparent;
            this.imgR.Image = ((System.Drawing.Image)(resources.GetObject("imgR.Image")));
            this.imgR.Location = new System.Drawing.Point(0, 4);
            this.imgR.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.imgR.Name = "imgR";
            this.imgR.Size = new System.Drawing.Size(32, 32);
            this.imgR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.imgR.TabIndex = 0;
            this.imgR.TabStop = false;
            this.imgR.Click += new System.EventHandler(this.CodeFileEntry_Click);
            this.imgR.DoubleClick += new System.EventHandler(this.CodeFileEntry_DoubleClick);
            // 
            // imgSAS
            // 
            this.imgSAS.BackColor = System.Drawing.Color.Transparent;
            this.imgSAS.Image = ((System.Drawing.Image)(resources.GetObject("imgSAS.Image")));
            this.imgSAS.Location = new System.Drawing.Point(0, 4);
            this.imgSAS.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.imgSAS.Name = "imgSAS";
            this.imgSAS.Size = new System.Drawing.Size(32, 32);
            this.imgSAS.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.imgSAS.TabIndex = 3;
            this.imgSAS.TabStop = false;
            this.imgSAS.Click += new System.EventHandler(this.CodeFileEntry_Click);
            this.imgSAS.DoubleClick += new System.EventHandler(this.CodeFileEntry_DoubleClick);
            // 
            // imgStata
            // 
            this.imgStata.BackColor = System.Drawing.Color.Transparent;
            this.imgStata.Image = ((System.Drawing.Image)(resources.GetObject("imgStata.Image")));
            this.imgStata.Location = new System.Drawing.Point(0, 4);
            this.imgStata.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.imgStata.Name = "imgStata";
            this.imgStata.Size = new System.Drawing.Size(32, 32);
            this.imgStata.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.imgStata.TabIndex = 4;
            this.imgStata.TabStop = false;
            this.imgStata.Click += new System.EventHandler(this.CodeFileEntry_Click);
            this.imgStata.DoubleClick += new System.EventHandler(this.CodeFileEntry_DoubleClick);
            // 
            // imgWarning
            // 
            this.imgWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.imgWarning.BackColor = System.Drawing.Color.Transparent;
            this.imgWarning.Image = ((System.Drawing.Image)(resources.GetObject("imgWarning.Image")));
            this.imgWarning.Location = new System.Drawing.Point(415, 2);
            this.imgWarning.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.imgWarning.Name = "imgWarning";
            this.imgWarning.Size = new System.Drawing.Size(16, 16);
            this.imgWarning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgWarning.TabIndex = 5;
            this.imgWarning.TabStop = false;
            this.imgWarning.Visible = false;
            this.imgWarning.Click += new System.EventHandler(this.CodeFileEntry_Click);
            this.imgWarning.DoubleClick += new System.EventHandler(this.CodeFileEntry_DoubleClick);
            // 
            // horizontalLine1
            // 
            horizontalLine1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            horizontalLine1.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            horizontalLine1.Location = new System.Drawing.Point(-2, 43);
            horizontalLine1.Margin = new System.Windows.Forms.Padding(0);
            horizontalLine1.Name = "horizontalLine1";
            horizontalLine1.Size = new System.Drawing.Size(437, 3);
            horizontalLine1.TabIndex = 6;
            // 
            // CodeFileEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(horizontalLine1);
            this.Controls.Add(this.imgWarning);
            this.Controls.Add(this.imgStata);
            this.Controls.Add(this.imgSAS);
            this.Controls.Add(this.lblFilePath);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.imgR);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "CodeFileEntry";
            this.Size = new System.Drawing.Size(434, 45);
            this.Click += new System.EventHandler(this.CodeFileEntry_Click);
            ((System.ComponentModel.ISupportInitialize)(this.imgR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgSAS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgStata)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgWarning)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox imgR;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Label lblFilePath;
        private System.Windows.Forms.PictureBox imgSAS;
        private System.Windows.Forms.PictureBox imgStata;
        private System.Windows.Forms.PictureBox imgWarning;
    }
}
