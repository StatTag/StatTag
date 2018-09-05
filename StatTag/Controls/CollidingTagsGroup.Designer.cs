namespace StatTag.Controls
{
    partial class CollidingTagsGroup
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Tag 1",
            "2-12"}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "Tag 2",
            "5-10"}, -1);
            StatTag.Controls.HorizontalLine horizontalLine1;
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.lvwTags = new System.Windows.Forms.ListView();
            this.colTag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLines = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.cboKeepTag = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            horizontalLine1 = new StatTag.Controls.HorizontalLine();
            this.layout.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(0, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(152, 17);
            label1.TabIndex = 3;
            label1.Text = "Select which tag to keep:";
            // 
            // layout
            // 
            this.layout.AutoSize = true;
            this.layout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.layout.ColumnCount = 2;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.layout.Controls.Add(horizontalLine1, 0, 1);
            this.layout.Controls.Add(this.lvwTags, 0, 0);
            this.layout.Controls.Add(this.panel1, 1, 0);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(0, 0);
            this.layout.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.layout.Name = "layout";
            this.layout.RowCount = 2;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 7F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.layout.Size = new System.Drawing.Size(700, 67);
            this.layout.TabIndex = 0;
            // 
            // lvwTags
            // 
            this.lvwTags.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvwTags.CausesValidation = false;
            this.lvwTags.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTag,
            this.colLines});
            this.lvwTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwTags.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvwTags.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.lvwTags.LabelWrap = false;
            this.lvwTags.Location = new System.Drawing.Point(3, 4);
            this.lvwTags.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lvwTags.MultiSelect = false;
            this.lvwTags.Name = "lvwTags";
            this.lvwTags.Scrollable = false;
            this.lvwTags.Size = new System.Drawing.Size(414, 52);
            this.lvwTags.TabIndex = 0;
            this.lvwTags.UseCompatibleStateImageBehavior = false;
            this.lvwTags.View = System.Windows.Forms.View.Details;
            // 
            // colTag
            // 
            this.colTag.Text = "Tag";
            // 
            // colLines
            // 
            this.colLines.Text = "Lines";
            // 
            // panel1
            // 
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(label1);
            this.panel1.Controls.Add(this.cboKeepTag);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(423, 4);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(274, 52);
            this.panel1.TabIndex = 1;
            // 
            // cboKeepTag
            // 
            this.cboKeepTag.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboKeepTag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboKeepTag.FormattingEnabled = true;
            this.cboKeepTag.Location = new System.Drawing.Point(0, 22);
            this.cboKeepTag.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cboKeepTag.Name = "cboKeepTag";
            this.cboKeepTag.Size = new System.Drawing.Size(270, 25);
            this.cboKeepTag.TabIndex = 4;
            // 
            // horizontalLine1
            // 
            horizontalLine1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            horizontalLine1.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.layout.SetColumnSpan(horizontalLine1, 2);
            horizontalLine1.Location = new System.Drawing.Point(3, 63);
            horizontalLine1.Name = "horizontalLine1";
            horizontalLine1.Size = new System.Drawing.Size(694, 1);
            horizontalLine1.TabIndex = 0;
            // 
            // CollidingTagsGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CausesValidation = false;
            this.Controls.Add(this.layout);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "CollidingTagsGroup";
            this.Size = new System.Drawing.Size(700, 67);
            this.layout.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.ListView lvwTags;
        private System.Windows.Forms.ColumnHeader colTag;
        private System.Windows.Forms.ColumnHeader colLines;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cboKeepTag;
    }
}
