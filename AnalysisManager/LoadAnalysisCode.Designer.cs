namespace AnalysisManager
{
    partial class LoadAnalysisCode
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.pnlFiles = new System.Windows.Forms.FlowLayoutPanel();
            this.cmdAdd = new System.Windows.Forms.Button();
            this.lblEmpty = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(312, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Code files that should be used within this document:";
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdOK.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOK.Location = new System.Drawing.Point(224, 307);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 25);
            this.cmdOK.TabIndex = 2;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdCancel.Location = new System.Drawing.Point(425, 307);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 25);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // pnlFiles
            // 
            this.pnlFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFiles.AutoScroll = true;
            this.pnlFiles.BackColor = System.Drawing.Color.White;
            this.pnlFiles.CausesValidation = false;
            this.pnlFiles.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlFiles.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlFiles.Location = new System.Drawing.Point(16, 33);
            this.pnlFiles.Name = "pnlFiles";
            this.pnlFiles.Size = new System.Drawing.Size(695, 250);
            this.pnlFiles.TabIndex = 4;
            this.pnlFiles.WrapContents = false;
            this.pnlFiles.SizeChanged += new System.EventHandler(this.pnlFiles_SizeChanged);
            this.pnlFiles.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.pnlFiles_ControlAdded);
            this.pnlFiles.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.pnlFiles_ControlRemoved);
            // 
            // cmdAdd
            // 
            this.cmdAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAdd.BackgroundImage = global::AnalysisManager.Properties.Resources._1446843266_plus;
            this.cmdAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.cmdAdd.Location = new System.Drawing.Point(686, 5);
            this.cmdAdd.Name = "cmdAdd";
            this.cmdAdd.Size = new System.Drawing.Size(25, 25);
            this.cmdAdd.TabIndex = 0;
            this.cmdAdd.UseVisualStyleBackColor = true;
            this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
            // 
            // lblEmpty
            // 
            this.lblEmpty.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmpty.ForeColor = System.Drawing.Color.Gray;
            this.lblEmpty.Location = new System.Drawing.Point(355, 280);
            this.lblEmpty.Name = "lblEmpty";
            this.lblEmpty.Size = new System.Drawing.Size(356, 24);
            this.lblEmpty.TabIndex = 5;
            this.lblEmpty.Text = "There are no files currently loaded.  Please click the \"+\" icon to add a new file" +
    ".";
            this.lblEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEmpty.Visible = false;
            // 
            // LoadAnalysisCode
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(723, 347);
            this.Controls.Add(this.lblEmpty);
            this.Controls.Add(this.cmdAdd);
            this.Controls.Add(this.pnlFiles);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.HelpButton = true;
            this.Name = "LoadAnalysisCode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Analysis Manager - Load Code Files";
            this.Load += new System.EventHandler(this.LoadAnalysisCode_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.FlowLayoutPanel pnlFiles;
        private System.Windows.Forms.Button cmdAdd;
        private System.Windows.Forms.Label lblEmpty;
    }
}