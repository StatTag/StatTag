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
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
            this.label1 = new System.Windows.Forms.Label();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.tabCollision = new System.Windows.Forms.TabPage();
            this.pnlOverlappingTags = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.tabUnlinked = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvUnlinkedTags = new System.Windows.Forms.DataGridView();
            this.colTagName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCodeFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActionToTake = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tabDuplicate = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.dgvDuplicateTags = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTagLines = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDuplicateLabel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDuplicateLines = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabResults = new System.Windows.Forms.TabControl();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tabCollision.SuspendLayout();
            this.tabUnlinked.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnlinkedTags)).BeginInit();
            this.tabDuplicate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDuplicateTags)).BeginInit();
            this.tabResults.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(8, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(744, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Your document has been reviewed, with the following results:";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cmdCancel.AutoSize = true;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(524, 359);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(87, 27);
            this.cmdCancel.TabIndex = 9;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cmdOK.AutoSize = true;
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(149, 359);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(87, 27);
            this.cmdOK.TabIndex = 8;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // tabCollision
            // 
            this.tabCollision.Controls.Add(this.tableLayoutPanel4);
            this.tabCollision.Location = new System.Drawing.Point(4, 26);
            this.tabCollision.Name = "tabCollision";
            this.tabCollision.Size = new System.Drawing.Size(736, 298);
            this.tabCollision.TabIndex = 3;
            this.tabCollision.Text = "Overlapping Tags";
            this.tabCollision.UseVisualStyleBackColor = true;
            // 
            // pnlOverlappingTags
            // 
            this.pnlOverlappingTags.AutoScroll = true;
            this.pnlOverlappingTags.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlOverlappingTags.CausesValidation = false;
            this.pnlOverlappingTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOverlappingTags.Location = new System.Drawing.Point(3, 37);
            this.pnlOverlappingTags.Name = "pnlOverlappingTags";
            this.pnlOverlappingTags.Size = new System.Drawing.Size(730, 258);
            this.pnlOverlappingTags.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(730, 34);
            this.label5.TabIndex = 7;
            this.label5.Text = "Overlapping tags are not permitted.  Please select the preferred tag (unselected " +
    "tags will be removed), or manually edit the tags in the code file.";
            // 
            // tabUnlinked
            // 
            this.tabUnlinked.Controls.Add(tableLayoutPanel2);
            this.tabUnlinked.Location = new System.Drawing.Point(4, 26);
            this.tabUnlinked.Name = "tabUnlinked";
            this.tabUnlinked.Padding = new System.Windows.Forms.Padding(3);
            this.tabUnlinked.Size = new System.Drawing.Size(736, 298);
            this.tabUnlinked.TabIndex = 0;
            this.tabUnlinked.Text = "Unlinked Tags";
            this.tabUnlinked.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(5, 2);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(720, 34);
            this.label2.TabIndex = 3;
            this.label2.Text = "There are tags in the current Word document that reference code files that are no" +
    "t linked.  Please select the appropriate action to take.";
            // 
            // dgvUnlinkedTags
            // 
            this.dgvUnlinkedTags.AllowUserToAddRows = false;
            this.dgvUnlinkedTags.AllowUserToDeleteRows = false;
            this.dgvUnlinkedTags.AllowUserToResizeRows = false;
            this.dgvUnlinkedTags.CausesValidation = false;
            this.dgvUnlinkedTags.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUnlinkedTags.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTagName,
            this.colCodeFile,
            this.colActionToTake});
            this.dgvUnlinkedTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUnlinkedTags.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvUnlinkedTags.Location = new System.Drawing.Point(5, 42);
            this.dgvUnlinkedTags.MultiSelect = false;
            this.dgvUnlinkedTags.Name = "dgvUnlinkedTags";
            this.dgvUnlinkedTags.RowHeadersVisible = false;
            this.dgvUnlinkedTags.ShowCellErrors = false;
            this.dgvUnlinkedTags.ShowEditingIcon = false;
            this.dgvUnlinkedTags.ShowRowErrors = false;
            this.dgvUnlinkedTags.Size = new System.Drawing.Size(720, 245);
            this.dgvUnlinkedTags.TabIndex = 2;
            this.dgvUnlinkedTags.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvUnlinkedTags_CellValueChanged);
            this.dgvUnlinkedTags.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvUnlinkedTags_CurrentCellDirtyStateChanged);
            // 
            // colTagName
            // 
            this.colTagName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTagName.FillWeight = 25F;
            this.colTagName.HeaderText = "Tag Name";
            this.colTagName.Name = "colTagName";
            this.colTagName.ReadOnly = true;
            // 
            // colCodeFile
            // 
            this.colCodeFile.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCodeFile.FillWeight = 35F;
            this.colCodeFile.HeaderText = "Code File";
            this.colCodeFile.Name = "colCodeFile";
            this.colCodeFile.ReadOnly = true;
            // 
            // colActionToTake
            // 
            this.colActionToTake.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colActionToTake.FillWeight = 40F;
            this.colActionToTake.HeaderText = "Action to Take";
            this.colActionToTake.Name = "colActionToTake";
            // 
            // tabDuplicate
            // 
            this.tabDuplicate.Controls.Add(this.tableLayoutPanel3);
            this.tabDuplicate.Location = new System.Drawing.Point(4, 26);
            this.tabDuplicate.Name = "tabDuplicate";
            this.tabDuplicate.Size = new System.Drawing.Size(736, 298);
            this.tabDuplicate.TabIndex = 1;
            this.tabDuplicate.Text = "Duplicate Tags";
            this.tabDuplicate.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(6, 3);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(724, 34);
            this.label3.TabIndex = 5;
            this.label3.Text = "The following tag names appear multiple times in the same code file. A tag name c" +
    "an only be used once in a code file.\r\nPlease change at least one of the tag name" +
    "s to make them unique.\r\n";
            // 
            // dgvDuplicateTags
            // 
            this.dgvDuplicateTags.AllowUserToAddRows = false;
            this.dgvDuplicateTags.AllowUserToDeleteRows = false;
            this.dgvDuplicateTags.AllowUserToResizeRows = false;
            this.dgvDuplicateTags.CausesValidation = false;
            this.dgvDuplicateTags.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDuplicateTags.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2,
            this.colTagLines,
            this.colDuplicateLabel,
            this.colDuplicateLines});
            this.dgvDuplicateTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDuplicateTags.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvDuplicateTags.Location = new System.Drawing.Point(6, 43);
            this.dgvDuplicateTags.MultiSelect = false;
            this.dgvDuplicateTags.Name = "dgvDuplicateTags";
            this.dgvDuplicateTags.RowHeadersVisible = false;
            this.dgvDuplicateTags.ShowCellErrors = false;
            this.dgvDuplicateTags.ShowEditingIcon = false;
            this.dgvDuplicateTags.ShowRowErrors = false;
            this.dgvDuplicateTags.Size = new System.Drawing.Size(724, 249);
            this.dgvDuplicateTags.TabIndex = 4;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.FillWeight = 40F;
            this.dataGridViewTextBoxColumn2.HeaderText = "Tag Name";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
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
            // colDuplicateLabel
            // 
            this.colDuplicateLabel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDuplicateLabel.FillWeight = 40F;
            this.colDuplicateLabel.HeaderText = "Duplicate Tag Name";
            this.colDuplicateLabel.Name = "colDuplicateLabel";
            // 
            // colDuplicateLines
            // 
            this.colDuplicateLines.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDuplicateLines.FillWeight = 10F;
            this.colDuplicateLines.HeaderText = "Lines";
            this.colDuplicateLines.Name = "colDuplicateLines";
            this.colDuplicateLines.ReadOnly = true;
            // 
            // tabResults
            // 
            tableLayoutPanel1.SetColumnSpan(this.tabResults, 2);
            this.tabResults.Controls.Add(this.tabUnlinked);
            this.tabResults.Controls.Add(this.tabDuplicate);
            this.tabResults.Controls.Add(this.tabCollision);
            this.tabResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabResults.Location = new System.Drawing.Point(8, 25);
            this.tabResults.Name = "tabResults";
            this.tabResults.SelectedIndex = 0;
            this.tabResults.Size = new System.Drawing.Size(744, 328);
            this.tabResults.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            tableLayoutPanel1.Controls.Add(this.cmdCancel, 1, 2);
            tableLayoutPanel1.Controls.Add(this.tabResults, 0, 1);
            tableLayoutPanel1.Controls.Add(this.cmdOK, 0, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(760, 394);
            tableLayoutPanel1.TabIndex = 10;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.AutoSize = true;
            tableLayoutPanel2.CausesValidation = false;
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            tableLayoutPanel2.Controls.Add(this.dgvUnlinkedTags, 0, 1);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(2);
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new System.Drawing.Size(730, 292);
            tableLayoutPanel2.TabIndex = 4;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.dgvDuplicateTags, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(736, 298);
            this.tableLayoutPanel3.TabIndex = 6;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.pnlOverlappingTags, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(736, 298);
            this.tableLayoutPanel4.TabIndex = 9;
            // 
            // CheckDocument
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(760, 394);
            this.Controls.Add(tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "CheckDocument";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Check Tags";
            this.Load += new System.EventHandler(this.CheckDocument_Load);
            this.tabCollision.ResumeLayout(false);
            this.tabUnlinked.ResumeLayout(false);
            this.tabUnlinked.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnlinkedTags)).EndInit();
            this.tabDuplicate.ResumeLayout(false);
            this.tabDuplicate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDuplicateTags)).EndInit();
            this.tabResults.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.TabPage tabCollision;
        private System.Windows.Forms.Label label5;
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
        private System.Windows.Forms.FlowLayoutPanel pnlOverlappingTags;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    }
}