namespace StatTag
{
    sealed partial class TagManagerForm
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
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TagManagerForm));
            this.cboCodeFiles = new System.Windows.Forms.ComboBox();
            this.cmdInsert = new System.Windows.Forms.Button();
            this.cmdUpdate = new System.Windows.Forms.Button();
            this.lvwTags = new System.Windows.Forms.ListView();
            this.colProgram = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDuplicate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.cmdDefineTag = new System.Windows.Forms.Button();
            this.cmdRemoveTags = new System.Windows.Forms.Button();
            this.cmdCheckUnlinkedTags = new System.Windows.Forms.Button();
            this.updateBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.insertBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.txtFilter = new StatTag.Controls.PlaceholderTextBox();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel4, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 3);
            tableLayoutPanel1.Controls.Add(this.lvwTags, 0, 1);
            tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(701, 623);
            tableLayoutPanel1.TabIndex = 15;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.AutoSize = true;
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.62003F));
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.37997F));
            tableLayoutPanel4.Controls.Add(this.cboCodeFiles, 0, 0);
            tableLayoutPanel4.Controls.Add(this.txtFilter, 1, 0);
            tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel4.Location = new System.Drawing.Point(8, 8);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel4.Size = new System.Drawing.Size(685, 31);
            tableLayoutPanel4.TabIndex = 16;
            // 
            // cboCodeFiles
            // 
            this.cboCodeFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboCodeFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCodeFiles.FormattingEnabled = true;
            this.cboCodeFiles.Location = new System.Drawing.Point(3, 3);
            this.cboCodeFiles.Name = "cboCodeFiles";
            this.cboCodeFiles.Size = new System.Drawing.Size(450, 25);
            this.cboCodeFiles.Sorted = true;
            this.cboCodeFiles.TabIndex = 11;
            this.cboCodeFiles.SelectedIndexChanged += new System.EventHandler(this.cboCodeFiles_SelectedIndexChanged);
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.AutoSize = true;
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(this.cmdInsert, 0, 0);
            tableLayoutPanel2.Controls.Add(this.cmdUpdate, 1, 0);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(8, 577);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new System.Drawing.Size(685, 38);
            tableLayoutPanel2.TabIndex = 16;
            // 
            // cmdInsert
            // 
            this.cmdInsert.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cmdInsert.AutoSize = true;
            this.cmdInsert.Location = new System.Drawing.Point(105, 4);
            this.cmdInsert.Margin = new System.Windows.Forms.Padding(3, 4, 15, 4);
            this.cmdInsert.Name = "cmdInsert";
            this.cmdInsert.Size = new System.Drawing.Size(222, 30);
            this.cmdInsert.TabIndex = 5;
            this.cmdInsert.Text = "Insert Tag Placeholders";
            this.cmdInsert.UseVisualStyleBackColor = true;
            this.cmdInsert.Click += new System.EventHandler(this.cmdInsert_Click);
            // 
            // cmdUpdate
            // 
            this.cmdUpdate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cmdUpdate.AutoSize = true;
            this.cmdUpdate.Location = new System.Drawing.Point(357, 4);
            this.cmdUpdate.Margin = new System.Windows.Forms.Padding(15, 4, 3, 4);
            this.cmdUpdate.Name = "cmdUpdate";
            this.cmdUpdate.Size = new System.Drawing.Size(190, 30);
            this.cmdUpdate.TabIndex = 4;
            this.cmdUpdate.Text = "Update Tag Results";
            this.cmdUpdate.UseVisualStyleBackColor = true;
            this.cmdUpdate.Click += new System.EventHandler(this.cmdUpdate_Click);
            // 
            // lvwTags
            // 
            this.lvwTags.AutoArrange = false;
            this.lvwTags.CheckBoxes = true;
            this.lvwTags.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colProgram,
            this.colTag,
            this.colType,
            this.colDuplicate});
            this.lvwTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwTags.FullRowSelect = true;
            this.lvwTags.HideSelection = false;
            this.lvwTags.Location = new System.Drawing.Point(8, 46);
            this.lvwTags.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lvwTags.Name = "lvwTags";
            this.lvwTags.OwnerDraw = true;
            this.lvwTags.Size = new System.Drawing.Size(685, 482);
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
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.cmdDefineTag, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.cmdRemoveTags, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.cmdCheckUnlinkedTags, 3, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(8, 535);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(685, 36);
            this.tableLayoutPanel3.TabIndex = 16;
            // 
            // cmdDefineTag
            // 
            this.cmdDefineTag.AutoSize = true;
            this.cmdDefineTag.Image = global::StatTag.Properties.Resources.plus;
            this.cmdDefineTag.Location = new System.Drawing.Point(3, 4);
            this.cmdDefineTag.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdDefineTag.Name = "cmdDefineTag";
            this.cmdDefineTag.Size = new System.Drawing.Size(146, 28);
            this.cmdDefineTag.TabIndex = 12;
            this.cmdDefineTag.Text = "   Define Tag";
            this.cmdDefineTag.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdDefineTag.UseVisualStyleBackColor = true;
            this.cmdDefineTag.Click += new System.EventHandler(this.cmdDefineTag_Click);
            // 
            // cmdRemoveTags
            // 
            this.cmdRemoveTags.AutoSize = true;
            this.cmdRemoveTags.Image = global::StatTag.Properties.Resources.minus;
            this.cmdRemoveTags.Location = new System.Drawing.Point(155, 4);
            this.cmdRemoveTags.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdRemoveTags.Name = "cmdRemoveTags";
            this.cmdRemoveTags.Size = new System.Drawing.Size(169, 28);
            this.cmdRemoveTags.TabIndex = 13;
            this.cmdRemoveTags.Text = "   Remove Tags";
            this.cmdRemoveTags.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdRemoveTags.UseVisualStyleBackColor = true;
            this.cmdRemoveTags.Click += new System.EventHandler(this.cmdRemoveTags_Click);
            // 
            // cmdCheckUnlinkedTags
            // 
            this.cmdCheckUnlinkedTags.AutoSize = true;
            this.cmdCheckUnlinkedTags.Enabled = false;
            this.cmdCheckUnlinkedTags.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.cmdCheckUnlinkedTags.Location = new System.Drawing.Point(483, 3);
            this.cmdCheckUnlinkedTags.Name = "cmdCheckUnlinkedTags";
            this.cmdCheckUnlinkedTags.Size = new System.Drawing.Size(199, 30);
            this.cmdCheckUnlinkedTags.TabIndex = 14;
            this.cmdCheckUnlinkedTags.Text = "   Troubleshoot Tags";
            this.cmdCheckUnlinkedTags.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdCheckUnlinkedTags.UseVisualStyleBackColor = true;
            this.cmdCheckUnlinkedTags.Click += new System.EventHandler(this.cmdCheckUnlinkedTags_Click);
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
            this.txtFilter.AutoSize = true;
            this.txtFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFilter.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilter.Location = new System.Drawing.Point(471, 0);
            this.txtFilter.Margin = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.txtFilter.MinimumSize = new System.Drawing.Size(100, 25);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.PlaceholderText = "Search";
            this.txtFilter.Size = new System.Drawing.Size(214, 31);
            this.txtFilter.TabIndex = 10;
            this.txtFilter.FilterChanged += new System.EventHandler(this.txtFilter_FilterChanged);
            // 
            // TagManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 623);
            this.Controls.Add(tableLayoutPanel1);
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
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

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
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button cmdInsert;
        private System.Windows.Forms.Button cmdUpdate;
    }
}