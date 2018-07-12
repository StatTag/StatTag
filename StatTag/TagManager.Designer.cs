﻿namespace StatTag
{
    partial class TagManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TagManager));
            this.cmdInsert = new System.Windows.Forms.Button();
            this.cmdUpdate = new System.Windows.Forms.Button();
            this.lvwTags = new System.Windows.Forms.ListView();
            this.colProgram = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cboCodeFiles = new System.Windows.Forms.ComboBox();
            this.txtFilter = new StatTag.Controls.PlaceholderTextBox();
            this.SuspendLayout();
            // 
            // cmdInsert
            // 
            this.cmdInsert.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdInsert.Location = new System.Drawing.Point(124, 554);
            this.cmdInsert.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdInsert.Name = "cmdInsert";
            this.cmdInsert.Size = new System.Drawing.Size(213, 30);
            this.cmdInsert.TabIndex = 1;
            this.cmdInsert.Text = "Insert Selected Tags";
            this.cmdInsert.UseVisualStyleBackColor = true;
            this.cmdInsert.Click += new System.EventHandler(this.cmdInsert_Click);
            // 
            // cmdUpdate
            // 
            this.cmdUpdate.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdUpdate.Location = new System.Drawing.Point(364, 554);
            this.cmdUpdate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdUpdate.Name = "cmdUpdate";
            this.cmdUpdate.Size = new System.Drawing.Size(213, 30);
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
            this.colType});
            this.lvwTags.FullRowSelect = true;
            this.lvwTags.HideSelection = false;
            this.lvwTags.Location = new System.Drawing.Point(12, 38);
            this.lvwTags.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lvwTags.Name = "lvwTags";
            this.lvwTags.OwnerDraw = true;
            this.lvwTags.Size = new System.Drawing.Size(672, 493);
            this.lvwTags.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvwTags.TabIndex = 3;
            this.lvwTags.UseCompatibleStateImageBehavior = false;
            this.lvwTags.View = System.Windows.Forms.View.Details;
            this.lvwTags.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwTags_ColumnClick);
            this.lvwTags.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.lvwTags_DrawColumnHeader);
            this.lvwTags.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.lvwTags_DrawSubItem);
            this.lvwTags.SelectedIndexChanged += new System.EventHandler(this.lvwTags_SelectedIndexChanged);
            this.lvwTags.DoubleClick += new System.EventHandler(this.lvwTags_DoubleClick);
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
            // TagManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 597);
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
            this.Name = "TagManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "StatTag - Tag Manager";
            this.TopMost = true;
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
    }
}