namespace StatTag
{
    partial class CheckDocument
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
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.tabCollision = new System.Windows.Forms.TabPage();
            this.dgvOverlappingTags = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.KeepRight = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.KeepLeft = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.label5 = new System.Windows.Forms.Label();
            this.tabUnlinked = new System.Windows.Forms.TabPage();
            this.dgvUnlinkedTags = new System.Windows.Forms.DataGridView();
            this.colActionToTake = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colCodeFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTagName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.tabDuplicate = new System.Windows.Forms.TabPage();
            this.dgvDuplicateTags = new System.Windows.Forms.DataGridView();
            this.colDuplicateLines = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDuplicateLabel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTagLines = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.tabResults = new System.Windows.Forms.TabControl();
            this.tabCollision.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOverlappingTags)).BeginInit();
            this.tabUnlinked.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnlinkedTags)).BeginInit();
            this.tabDuplicate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDuplicateTags)).BeginInit();
            this.tabResults.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(364, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Your document has been reviewed, with the following results:";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(437, 357);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(87, 25);
            this.cmdCancel.TabIndex = 9;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(236, 357);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(87, 25);
            this.cmdOK.TabIndex = 8;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // tabCollision
            // 
            this.tabCollision.Controls.Add(this.label5);
            this.tabCollision.Controls.Add(this.dgvOverlappingTags);
            this.tabCollision.Location = new System.Drawing.Point(4, 26);
            this.tabCollision.Name = "tabCollision";
            this.tabCollision.Size = new System.Drawing.Size(728, 267);
            this.tabCollision.TabIndex = 3;
            this.tabCollision.Text = "Overlapping Tags";
            this.tabCollision.UseVisualStyleBackColor = true;
            // 
            // dgvOverlappingTags
            // 
            this.dgvOverlappingTags.AllowUserToAddRows = false;
            this.dgvOverlappingTags.AllowUserToDeleteRows = false;
            this.dgvOverlappingTags.AllowUserToResizeRows = false;
            this.dgvOverlappingTags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvOverlappingTags.CausesValidation = false;
            this.dgvOverlappingTags.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOverlappingTags.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.KeepLeft,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.KeepRight,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7});
            this.dgvOverlappingTags.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvOverlappingTags.Location = new System.Drawing.Point(5, 48);
            this.dgvOverlappingTags.MultiSelect = false;
            this.dgvOverlappingTags.Name = "dgvOverlappingTags";
            this.dgvOverlappingTags.RowHeadersVisible = false;
            this.dgvOverlappingTags.ShowCellErrors = false;
            this.dgvOverlappingTags.ShowEditingIcon = false;
            this.dgvOverlappingTags.ShowRowErrors = false;
            this.dgvOverlappingTags.Size = new System.Drawing.Size(719, 212);
            this.dgvOverlappingTags.TabIndex = 6;
            this.dgvOverlappingTags.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvOverlappingTags_CellContentClick);
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn7.FillWeight = 10F;
            this.dataGridViewTextBoxColumn7.HeaderText = "Lines";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn6.FillWeight = 40F;
            this.dataGridViewTextBoxColumn6.HeaderText = "Inner (Nested) Tag";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // KeepRight
            // 
            this.KeepRight.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.KeepRight.HeaderText = "Keep";
            this.KeepRight.Name = "KeepRight";
            this.KeepRight.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.KeepRight.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.KeepRight.Width = 63;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn5.DividerWidth = 3;
            this.dataGridViewTextBoxColumn5.FillWeight = 10F;
            this.dataGridViewTextBoxColumn5.HeaderText = "Lines";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn4.FillWeight = 40F;
            this.dataGridViewTextBoxColumn4.HeaderText = "Outer Tag";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // KeepLeft
            // 
            this.KeepLeft.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.KeepLeft.HeaderText = "Keep";
            this.KeepLeft.Name = "KeepLeft";
            this.KeepLeft.Width = 44;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.Location = new System.Drawing.Point(5, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(716, 38);
            this.label5.TabIndex = 7;
            this.label5.Text = "The following tags are overlapping.  This means a tag definition is inside anothe" +
    "r tag definition, which StatTag does not support.  Please select which of the ov" +
    "erlapping tags to preserve.\r\n";
            // 
            // tabUnlinked
            // 
            this.tabUnlinked.Controls.Add(this.label2);
            this.tabUnlinked.Controls.Add(this.dgvUnlinkedTags);
            this.tabUnlinked.Location = new System.Drawing.Point(4, 26);
            this.tabUnlinked.Name = "tabUnlinked";
            this.tabUnlinked.Padding = new System.Windows.Forms.Padding(3);
            this.tabUnlinked.Size = new System.Drawing.Size(728, 267);
            this.tabUnlinked.TabIndex = 0;
            this.tabUnlinked.Text = "Unlinked Tags";
            this.tabUnlinked.UseVisualStyleBackColor = true;
            // 
            // dgvUnlinkedTags
            // 
            this.dgvUnlinkedTags.AllowUserToAddRows = false;
            this.dgvUnlinkedTags.AllowUserToDeleteRows = false;
            this.dgvUnlinkedTags.AllowUserToResizeRows = false;
            this.dgvUnlinkedTags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvUnlinkedTags.CausesValidation = false;
            this.dgvUnlinkedTags.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUnlinkedTags.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTagName,
            this.colCodeFile,
            this.colActionToTake});
            this.dgvUnlinkedTags.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvUnlinkedTags.Location = new System.Drawing.Point(6, 49);
            this.dgvUnlinkedTags.MultiSelect = false;
            this.dgvUnlinkedTags.Name = "dgvUnlinkedTags";
            this.dgvUnlinkedTags.RowHeadersVisible = false;
            this.dgvUnlinkedTags.ShowCellErrors = false;
            this.dgvUnlinkedTags.ShowEditingIcon = false;
            this.dgvUnlinkedTags.ShowRowErrors = false;
            this.dgvUnlinkedTags.Size = new System.Drawing.Size(719, 212);
            this.dgvUnlinkedTags.TabIndex = 2;
            this.dgvUnlinkedTags.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvUnlinkedTags_CellValueChanged);
            this.dgvUnlinkedTags.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvUnlinkedTags_CurrentCellDirtyStateChanged);
            // 
            // colActionToTake
            // 
            this.colActionToTake.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colActionToTake.FillWeight = 40F;
            this.colActionToTake.HeaderText = "Action to Take";
            this.colActionToTake.Name = "colActionToTake";
            // 
            // colCodeFile
            // 
            this.colCodeFile.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCodeFile.FillWeight = 35F;
            this.colCodeFile.HeaderText = "Code File";
            this.colCodeFile.Name = "colCodeFile";
            this.colCodeFile.ReadOnly = true;
            // 
            // colTagName
            // 
            this.colTagName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTagName.FillWeight = 25F;
            this.colTagName.HeaderText = "Tag Name";
            this.colTagName.Name = "colTagName";
            this.colTagName.ReadOnly = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(6, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(716, 38);
            this.label2.TabIndex = 3;
            this.label2.Text = "There are tags in the current Word document that reference code files that are no" +
    "t linked.  Please select the appropriate action to take.";
            // 
            // tabDuplicate
            // 
            this.tabDuplicate.Controls.Add(this.label3);
            this.tabDuplicate.Controls.Add(this.dgvDuplicateTags);
            this.tabDuplicate.Location = new System.Drawing.Point(4, 26);
            this.tabDuplicate.Name = "tabDuplicate";
            this.tabDuplicate.Size = new System.Drawing.Size(728, 267);
            this.tabDuplicate.TabIndex = 1;
            this.tabDuplicate.Text = "Duplicate Tags";
            this.tabDuplicate.UseVisualStyleBackColor = true;
            // 
            // dgvDuplicateTags
            // 
            this.dgvDuplicateTags.AllowUserToAddRows = false;
            this.dgvDuplicateTags.AllowUserToDeleteRows = false;
            this.dgvDuplicateTags.AllowUserToResizeRows = false;
            this.dgvDuplicateTags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDuplicateTags.CausesValidation = false;
            this.dgvDuplicateTags.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDuplicateTags.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2,
            this.colTagLines,
            this.colDuplicateLabel,
            this.colDuplicateLines});
            this.dgvDuplicateTags.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvDuplicateTags.Location = new System.Drawing.Point(6, 49);
            this.dgvDuplicateTags.MultiSelect = false;
            this.dgvDuplicateTags.Name = "dgvDuplicateTags";
            this.dgvDuplicateTags.RowHeadersVisible = false;
            this.dgvDuplicateTags.ShowCellErrors = false;
            this.dgvDuplicateTags.ShowEditingIcon = false;
            this.dgvDuplicateTags.ShowRowErrors = false;
            this.dgvDuplicateTags.Size = new System.Drawing.Size(719, 212);
            this.dgvDuplicateTags.TabIndex = 4;
            // 
            // colDuplicateLines
            // 
            this.colDuplicateLines.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDuplicateLines.FillWeight = 10F;
            this.colDuplicateLines.HeaderText = "Lines";
            this.colDuplicateLines.Name = "colDuplicateLines";
            this.colDuplicateLines.ReadOnly = true;
            // 
            // colDuplicateLabel
            // 
            this.colDuplicateLabel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDuplicateLabel.FillWeight = 40F;
            this.colDuplicateLabel.HeaderText = "Duplicate Tag Name";
            this.colDuplicateLabel.Name = "colDuplicateLabel";
            // 
            // colTagLines
            // 
            this.colTagLines.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTagLines.DividerWidth = 3;
            this.colTagLines.FillWeight = 10F;
            this.colTagLines.HeaderText = "Lines";
            this.colTagLines.Name = "colTagLines";
            this.colTagLines.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.FillWeight = 40F;
            this.dataGridViewTextBoxColumn2.HeaderText = "Tag Name";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(6, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(716, 38);
            this.label3.TabIndex = 5;
            this.label3.Text = "The following tag names appear multiple times in the same code file. A tag name c" +
    "an only be used once in a code file.\r\nPlease change at least one of the tag name" +
    "s to make them unique.\r\n";
            // 
            // tabResults
            // 
            this.tabResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabResults.Controls.Add(this.tabUnlinked);
            this.tabResults.Controls.Add(this.tabDuplicate);
            this.tabResults.Controls.Add(this.tabCollision);
            this.tabResults.Location = new System.Drawing.Point(12, 39);
            this.tabResults.Name = "tabResults";
            this.tabResults.SelectedIndex = 0;
            this.tabResults.Size = new System.Drawing.Size(736, 297);
            this.tabResults.TabIndex = 0;
            // 
            // CheckDocument
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(760, 394);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabResults);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "CheckDocument";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Check Tags";
            this.Load += new System.EventHandler(this.CheckDocument_Load);
            this.tabCollision.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOverlappingTags)).EndInit();
            this.tabUnlinked.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnlinkedTags)).EndInit();
            this.tabDuplicate.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDuplicateTags)).EndInit();
            this.tabResults.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.TabPage tabCollision;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dgvOverlappingTags;
        private System.Windows.Forms.DataGridViewCheckBoxColumn KeepLeft;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewCheckBoxColumn KeepRight;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.TabPage tabUnlinked;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dgvUnlinkedTags;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTagName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCodeFile;
        private System.Windows.Forms.DataGridViewComboBoxColumn colActionToTake;
        private System.Windows.Forms.TabPage tabDuplicate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgvDuplicateTags;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTagLines;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDuplicateLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDuplicateLines;
        private System.Windows.Forms.TabControl tabResults;
    }
}