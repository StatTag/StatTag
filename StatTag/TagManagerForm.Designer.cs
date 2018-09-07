namespace StatTag
{
    partial class TagManagerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TagManagerForm));
            this.cmdInsert = new System.Windows.Forms.Button();
            this.cmdUpdate = new System.Windows.Forms.Button();
            this.lvwTags = new System.Windows.Forms.ListView();
            this.colProgram = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDuplicate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cboCodeFiles = new System.Windows.Forms.ComboBox();
            this.cmdCheckUnlinkedTags = new System.Windows.Forms.Button();
            this.cmdRemoveTags = new System.Windows.Forms.Button();
            this.cmdDefineTag = new System.Windows.Forms.Button();
            this.updateBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.insertBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.txtFilter = new StatTag.Controls.PlaceholderTextBox();
            this.SuspendLayout();
            // 
            // cmdInsert
            // 
            this.cmdInsert.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdInsert.Location = new System.Drawing.Point(159, 580);
            this.cmdInsert.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdInsert.Name = "cmdInsert";
            this.cmdInsert.Size = new System.Drawing.Size(185, 30);
            this.cmdInsert.TabIndex = 1;
            this.cmdInsert.Text = "Insert Selected Tags";
            this.cmdInsert.UseVisualStyleBackColor = true;
            this.cmdInsert.Click += new System.EventHandler(this.cmdInsert_Click);
            // 
            // cmdUpdate
            // 
            this.cmdUpdate.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdUpdate.Location = new System.Drawing.Point(350, 580);
            this.cmdUpdate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdUpdate.Name = "cmdUpdate";
            this.cmdUpdate.Size = new System.Drawing.Size(185, 30);
            this.cmdUpdate.TabIndex = 2;
            this.cmdUpdate.Text = "Update Selected Tags";
            this.cmdUpdate.UseVisualStyleBackColor = true;
            this.cmdUpdate.Click += new System.EventHandler(this.cmdUpdate_Click);
            // 
            // lvwTags
            // 
            this.lvwTags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwTags.AutoArrange = false;
            this.lvwTags.CheckBoxes = true;
            this.lvwTags.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colProgram,
            this.colTag,
            this.colType,
            this.colDuplicate});
            this.lvwTags.FullRowSelect = true;
            this.lvwTags.HideSelection = false;
            this.lvwTags.Location = new System.Drawing.Point(12, 38);
            this.lvwTags.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lvwTags.Name = "lvwTags";
            this.lvwTags.OwnerDraw = true;
            this.lvwTags.Size = new System.Drawing.Size(672, 487);
            this.lvwTags.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvwTags.TabIndex = 3;
            this.lvwTags.UseCompatibleStateImageBehavior = false;
            this.lvwTags.View = System.Windows.Forms.View.Details;
            this.lvwTags.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwTags_ColumnClick);
            this.lvwTags.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.lvwTags_DrawColumnHeader);
            this.lvwTags.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.lvwTags_DrawSubItem);
            this.lvwTags.SelectedIndexChanged += new System.EventHandler(this.lvwTags_SelectedIndexChanged);
            this.lvwTags.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvwTags_KeyDown);
            this.lvwTags.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvwTags_MouseClick);
            this.lvwTags.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvwTags_MouseDoubleClick);
            // 
            // colProgram
            // 
            this.colProgram.Text = "";
            this.colProgram.Width = 50;
            // 
            // colTag
            // 
            this.colTag.Text = "Tag";
            this.colTag.Width = 250;
            // 
            // colType
            // 
            this.colType.Text = "Type";
            this.colType.Width = 250;
            // 
            // colDuplicate
            // 
            this.colDuplicate.Text = "";
            this.colDuplicate.Width = 30;
            // 
            // cboCodeFiles
            // 
            this.cboCodeFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCodeFiles.FormattingEnabled = true;
            this.cboCodeFiles.Location = new System.Drawing.Point(12, 9);
            this.cboCodeFiles.Name = "cboCodeFiles";
            this.cboCodeFiles.Size = new System.Drawing.Size(425, 25);
            this.cboCodeFiles.Sorted = true;
            this.cboCodeFiles.TabIndex = 11;
            this.cboCodeFiles.SelectedIndexChanged += new System.EventHandler(this.cboCodeFiles_SelectedIndexChanged);
            // 
            // cmdCheckUnlinkedTags
            // 
            this.cmdCheckUnlinkedTags.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCheckUnlinkedTags.Enabled = false;
            this.cmdCheckUnlinkedTags.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.cmdCheckUnlinkedTags.Location = new System.Drawing.Point(491, 527);
            this.cmdCheckUnlinkedTags.Name = "cmdCheckUnlinkedTags";
            this.cmdCheckUnlinkedTags.Size = new System.Drawing.Size(193, 30);
            this.cmdCheckUnlinkedTags.TabIndex = 14;
            this.cmdCheckUnlinkedTags.Text = "   Troubleshoot Tags";
            this.cmdCheckUnlinkedTags.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdCheckUnlinkedTags.UseVisualStyleBackColor = true;
            this.cmdCheckUnlinkedTags.Click += new System.EventHandler(this.cmdCheckUnlinkedTags_Click);
            // 
            // cmdRemoveTags
            // 
            this.cmdRemoveTags.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdRemoveTags.Image = global::StatTag.Properties.Resources.minus;
            this.cmdRemoveTags.Location = new System.Drawing.Point(153, 527);
            this.cmdRemoveTags.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdRemoveTags.Name = "cmdRemoveTags";
            this.cmdRemoveTags.Size = new System.Drawing.Size(135, 30);
            this.cmdRemoveTags.TabIndex = 13;
            this.cmdRemoveTags.Text = "   Remove Tags";
            this.cmdRemoveTags.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdRemoveTags.UseVisualStyleBackColor = true;
            this.cmdRemoveTags.Click += new System.EventHandler(this.cmdRemoveTags_Click);
            // 
            // cmdDefineTag
            // 
            this.cmdDefineTag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdDefineTag.Image = global::StatTag.Properties.Resources.plus;
            this.cmdDefineTag.Location = new System.Drawing.Point(12, 527);
            this.cmdDefineTag.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdDefineTag.Name = "cmdDefineTag";
            this.cmdDefineTag.Size = new System.Drawing.Size(135, 30);
            this.cmdDefineTag.TabIndex = 12;
            this.cmdDefineTag.Text = "   Define Tag";
            this.cmdDefineTag.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdDefineTag.UseVisualStyleBackColor = true;
            this.cmdDefineTag.Click += new System.EventHandler(this.cmdDefineTag_Click);
            // 
            // updateBackgroundWorker
            // 
            this.updateBackgroundWorker.WorkerReportsProgress = true;
            this.updateBackgroundWorker.WorkerSupportsCancellation = true;
            this.updateBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.updateBackgroundWorker_DoWork);
            this.updateBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.updateBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // insertBackgroundWorker
            // 
            this.insertBackgroundWorker.WorkerReportsProgress = true;
            this.insertBackgroundWorker.WorkerSupportsCancellation = true;
            this.insertBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.insertBackgroundWorker_DoWork);
            this.insertBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.insertBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // txtFilter
            // 
            this.txtFilter.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilter.Location = new System.Drawing.Point(440, 9);
            this.txtFilter.Margin = new System.Windows.Forms.Padding(0);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.PlaceholderText = "Search";
            this.txtFilter.Size = new System.Drawing.Size(244, 25);
            this.txtFilter.TabIndex = 10;
            this.txtFilter.FilterChanged += new System.EventHandler(this.txtFilter_FilterChanged);
            // 
            // TagManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 623);
            this.Controls.Add(this.cmdCheckUnlinkedTags);
            this.Controls.Add(this.cmdRemoveTags);
            this.Controls.Add(this.cmdDefineTag);
            this.Controls.Add(this.cboCodeFiles);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.lvwTags);
            this.Controls.Add(this.cmdUpdate);
            this.Controls.Add(this.cmdInsert);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TagManagerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "StatTag - Tag Manager";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TagManager_FormClosed);
            this.Load += new System.EventHandler(this.TagManager_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdInsert;
        private System.Windows.Forms.Button cmdUpdate;
        private System.Windows.Forms.ListView lvwTags;
        private System.Windows.Forms.ColumnHeader colTag;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colProgram;
        private Controls.PlaceholderTextBox txtFilter;
        private System.Windows.Forms.ComboBox cboCodeFiles;
        private System.Windows.Forms.Button cmdRemoveTags;
        private System.Windows.Forms.Button cmdDefineTag;
        private System.Windows.Forms.Button cmdCheckUnlinkedTags;
        private System.Windows.Forms.ColumnHeader colDuplicate;
        private System.ComponentModel.BackgroundWorker updateBackgroundWorker;
        private System.ComponentModel.BackgroundWorker insertBackgroundWorker;
    }
}