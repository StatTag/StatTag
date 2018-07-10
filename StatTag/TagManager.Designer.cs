namespace StatTag
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
            this.colTag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colProgram = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.lvwTags.Location = new System.Drawing.Point(12, 13);
            this.lvwTags.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lvwTags.Name = "lvwTags";
            this.lvwTags.OwnerDraw = true;
            this.lvwTags.Size = new System.Drawing.Size(672, 518);
            this.lvwTags.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvwTags.TabIndex = 3;
            this.lvwTags.UseCompatibleStateImageBehavior = false;
            this.lvwTags.View = System.Windows.Forms.View.Details;
            this.lvwTags.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView1_DrawItem);
            this.lvwTags.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView1_DrawSubItem);
            // 
            // colTag
            // 
            this.colTag.Text = "Tag";
            this.colTag.Width = 300;
            // 
            // colType
            // 
            this.colType.Text = "Type";
            this.colType.Width = 300;
            // 
            // colProgram
            // 
            this.colProgram.Text = "";
            this.colProgram.Width = 40;
            // 
            // TagManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 597);
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
    }
}