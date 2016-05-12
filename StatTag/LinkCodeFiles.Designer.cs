namespace StatTag
{
    partial class LinkCodeFiles
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinkCodeFiles));
            this.label1 = new System.Windows.Forms.Label();
            this.dgvCodeFiles = new System.Windows.Forms.DataGridView();
            this.colMissingCodeFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActionToTake = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCodeFiles)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(751, 77);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // dgvCodeFiles
            // 
            this.dgvCodeFiles.AllowUserToAddRows = false;
            this.dgvCodeFiles.AllowUserToDeleteRows = false;
            this.dgvCodeFiles.AllowUserToResizeRows = false;
            this.dgvCodeFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCodeFiles.CausesValidation = false;
            this.dgvCodeFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCodeFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMissingCodeFile,
            this.colActionToTake});
            this.dgvCodeFiles.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvCodeFiles.Location = new System.Drawing.Point(16, 94);
            this.dgvCodeFiles.MultiSelect = false;
            this.dgvCodeFiles.Name = "dgvCodeFiles";
            this.dgvCodeFiles.RowHeadersVisible = false;
            this.dgvCodeFiles.ShowCellErrors = false;
            this.dgvCodeFiles.ShowEditingIcon = false;
            this.dgvCodeFiles.ShowRowErrors = false;
            this.dgvCodeFiles.Size = new System.Drawing.Size(748, 229);
            this.dgvCodeFiles.TabIndex = 1;
            // 
            // colMissingCodeFile
            // 
            this.colMissingCodeFile.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colMissingCodeFile.FillWeight = 40F;
            this.colMissingCodeFile.HeaderText = "Missing Code File";
            this.colMissingCodeFile.Name = "colMissingCodeFile";
            this.colMissingCodeFile.ReadOnly = true;
            // 
            // colActionToTake
            // 
            this.colActionToTake.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colActionToTake.FillWeight = 60F;
            this.colActionToTake.HeaderText = "Action to Take";
            this.colActionToTake.Name = "colActionToTake";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(445, 346);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(87, 25);
            this.cmdCancel.TabIndex = 7;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(244, 346);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(87, 25);
            this.cmdOK.TabIndex = 6;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // LinkCodeFiles
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(776, 383);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.dgvCodeFiles);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "LinkCodeFiles";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "StatTag - Link Code Files";
            this.Load += new System.EventHandler(this.LinkCodeFiles_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCodeFiles)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvCodeFiles;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMissingCodeFile;
        private System.Windows.Forms.DataGridViewComboBoxColumn colActionToTake;
    }
}