namespace AnalysisManager
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
            this.tabUnlinked = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvUnlinkedAnnotations = new System.Windows.Forms.DataGridView();
            this.colCodeFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAnnotationLabel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActionToTake = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tabResults = new System.Windows.Forms.TabControl();
            this.tabDuplicate = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.dgvDuplicateAnnotations = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAnnotationLines = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDuplicateLabel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDuplicateLines = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.tabUnlinked.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnlinkedAnnotations)).BeginInit();
            this.tabResults.SuspendLayout();
            this.tabDuplicate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDuplicateAnnotations)).BeginInit();
            this.SuspendLayout();
            // 
            // tabUnlinked
            // 
            this.tabUnlinked.Controls.Add(this.label2);
            this.tabUnlinked.Controls.Add(this.dgvUnlinkedAnnotations);
            this.tabUnlinked.Location = new System.Drawing.Point(4, 26);
            this.tabUnlinked.Name = "tabUnlinked";
            this.tabUnlinked.Padding = new System.Windows.Forms.Padding(3);
            this.tabUnlinked.Size = new System.Drawing.Size(728, 267);
            this.tabUnlinked.TabIndex = 0;
            this.tabUnlinked.Text = "Unlinked Annotations";
            this.tabUnlinked.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(6, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(716, 38);
            this.label2.TabIndex = 3;
            this.label2.Text = "These annotations have been inserted into your document text, but the code file a" +
    "ssociated with them is not currently linked to the document:";
            // 
            // dgvUnlinkedAnnotations
            // 
            this.dgvUnlinkedAnnotations.AllowUserToAddRows = false;
            this.dgvUnlinkedAnnotations.AllowUserToDeleteRows = false;
            this.dgvUnlinkedAnnotations.AllowUserToResizeRows = false;
            this.dgvUnlinkedAnnotations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvUnlinkedAnnotations.CausesValidation = false;
            this.dgvUnlinkedAnnotations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUnlinkedAnnotations.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCodeFile,
            this.colAnnotationLabel,
            this.colActionToTake});
            this.dgvUnlinkedAnnotations.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvUnlinkedAnnotations.Location = new System.Drawing.Point(6, 49);
            this.dgvUnlinkedAnnotations.MultiSelect = false;
            this.dgvUnlinkedAnnotations.Name = "dgvUnlinkedAnnotations";
            this.dgvUnlinkedAnnotations.RowHeadersVisible = false;
            this.dgvUnlinkedAnnotations.ShowCellErrors = false;
            this.dgvUnlinkedAnnotations.ShowEditingIcon = false;
            this.dgvUnlinkedAnnotations.ShowRowErrors = false;
            this.dgvUnlinkedAnnotations.Size = new System.Drawing.Size(719, 212);
            this.dgvUnlinkedAnnotations.TabIndex = 2;
            // 
            // colCodeFile
            // 
            this.colCodeFile.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCodeFile.FillWeight = 35F;
            this.colCodeFile.HeaderText = "Code File";
            this.colCodeFile.Name = "colCodeFile";
            this.colCodeFile.ReadOnly = true;
            // 
            // colAnnotationLabel
            // 
            this.colAnnotationLabel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colAnnotationLabel.FillWeight = 25F;
            this.colAnnotationLabel.HeaderText = "Annotation Label";
            this.colAnnotationLabel.Name = "colAnnotationLabel";
            this.colAnnotationLabel.ReadOnly = true;
            // 
            // colActionToTake
            // 
            this.colActionToTake.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colActionToTake.FillWeight = 40F;
            this.colActionToTake.HeaderText = "Action to Take";
            this.colActionToTake.Name = "colActionToTake";
            // 
            // tabResults
            // 
            this.tabResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabResults.Controls.Add(this.tabUnlinked);
            this.tabResults.Controls.Add(this.tabDuplicate);
            this.tabResults.Location = new System.Drawing.Point(12, 39);
            this.tabResults.Name = "tabResults";
            this.tabResults.SelectedIndex = 0;
            this.tabResults.Size = new System.Drawing.Size(736, 297);
            this.tabResults.TabIndex = 0;
            // 
            // tabDuplicate
            // 
            this.tabDuplicate.Controls.Add(this.label3);
            this.tabDuplicate.Controls.Add(this.dgvDuplicateAnnotations);
            this.tabDuplicate.Location = new System.Drawing.Point(4, 26);
            this.tabDuplicate.Name = "tabDuplicate";
            this.tabDuplicate.Size = new System.Drawing.Size(728, 267);
            this.tabDuplicate.TabIndex = 1;
            this.tabDuplicate.Text = "Duplicate Annotations";
            this.tabDuplicate.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(6, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(716, 38);
            this.label3.TabIndex = 5;
            this.label3.Text = "The following annotation names appear multiple times in the same code file.  An o" +
    "utput label should only appear once in each code file.";
            // 
            // dgvDuplicateAnnotations
            // 
            this.dgvDuplicateAnnotations.AllowUserToAddRows = false;
            this.dgvDuplicateAnnotations.AllowUserToDeleteRows = false;
            this.dgvDuplicateAnnotations.AllowUserToResizeRows = false;
            this.dgvDuplicateAnnotations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDuplicateAnnotations.CausesValidation = false;
            this.dgvDuplicateAnnotations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDuplicateAnnotations.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2,
            this.colAnnotationLines,
            this.colDuplicateLabel,
            this.colDuplicateLines});
            this.dgvDuplicateAnnotations.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvDuplicateAnnotations.Location = new System.Drawing.Point(6, 49);
            this.dgvDuplicateAnnotations.MultiSelect = false;
            this.dgvDuplicateAnnotations.Name = "dgvDuplicateAnnotations";
            this.dgvDuplicateAnnotations.RowHeadersVisible = false;
            this.dgvDuplicateAnnotations.ShowCellErrors = false;
            this.dgvDuplicateAnnotations.ShowEditingIcon = false;
            this.dgvDuplicateAnnotations.ShowRowErrors = false;
            this.dgvDuplicateAnnotations.Size = new System.Drawing.Size(719, 212);
            this.dgvDuplicateAnnotations.TabIndex = 4;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.FillWeight = 40F;
            this.dataGridViewTextBoxColumn2.HeaderText = "Annotation Label";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // colAnnotationLines
            // 
            this.colAnnotationLines.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colAnnotationLines.DividerWidth = 3;
            this.colAnnotationLines.FillWeight = 10F;
            this.colAnnotationLines.HeaderText = "Lines";
            this.colAnnotationLines.Name = "colAnnotationLines";
            this.colAnnotationLines.ReadOnly = true;
            // 
            // colDuplicateLabel
            // 
            this.colDuplicateLabel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDuplicateLabel.FillWeight = 40F;
            this.colDuplicateLabel.HeaderText = "Duplicate Label";
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
            this.Text = "Analysis Manager - Check Annotations";
            this.Load += new System.EventHandler(this.CheckDocument_Load);
            this.tabUnlinked.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnlinkedAnnotations)).EndInit();
            this.tabResults.ResumeLayout(false);
            this.tabDuplicate.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDuplicateAnnotations)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabPage tabUnlinked;
        private System.Windows.Forms.TabControl tabResults;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dgvUnlinkedAnnotations;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCodeFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAnnotationLabel;
        private System.Windows.Forms.DataGridViewComboBoxColumn colActionToTake;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.TabPage tabDuplicate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgvDuplicateAnnotations;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAnnotationLines;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDuplicateLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDuplicateLines;
    }
}