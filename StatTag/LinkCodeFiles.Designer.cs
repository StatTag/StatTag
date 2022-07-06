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
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinkCodeFiles));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvCodeFiles = new System.Windows.Forms.DataGridView();
            this.colMissingCodeFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActionToTake = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCodeFiles)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            tableLayoutPanel1.Controls.Add(this.cmdCancel, 1, 2);
            tableLayoutPanel1.Controls.Add(this.dgvCodeFiles, 0, 1);
            tableLayoutPanel1.Controls.Add(this.cmdOK, 0, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(3);
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(776, 383);
            tableLayoutPanel1.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(764, 68);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // dgvCodeFiles
            // 
            this.dgvCodeFiles.AllowUserToAddRows = false;
            this.dgvCodeFiles.AllowUserToDeleteRows = false;
            this.dgvCodeFiles.AllowUserToResizeRows = false;
            this.dgvCodeFiles.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.dgvCodeFiles.CausesValidation = false;
            this.dgvCodeFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCodeFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMissingCodeFile,
            this.colActionToTake});
            tableLayoutPanel1.SetColumnSpan(this.dgvCodeFiles, 2);
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCodeFiles.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCodeFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCodeFiles.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvCodeFiles.Location = new System.Drawing.Point(6, 74);
            this.dgvCodeFiles.MultiSelect = false;
            this.dgvCodeFiles.Name = "dgvCodeFiles";
            this.dgvCodeFiles.RowHeadersVisible = false;
            this.dgvCodeFiles.ShowCellErrors = false;
            this.dgvCodeFiles.ShowEditingIcon = false;
            this.dgvCodeFiles.ShowRowErrors = false;
            this.dgvCodeFiles.Size = new System.Drawing.Size(764, 270);
            this.dgvCodeFiles.TabIndex = 1;
            this.dgvCodeFiles.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCodeFiles_CellValueChanged);
            this.dgvCodeFiles.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvCodeFiles_CurrentCellDirtyStateChanged);
            this.dgvCodeFiles.Leave += new System.EventHandler(this.dgvCodeFiles_Leave);
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
            this.cmdCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cmdCancel.AutoSize = true;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(408, 350);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(87, 27);
            this.cmdCancel.TabIndex = 7;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cmdOK.AutoSize = true;
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(281, 350);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(87, 27);
            this.cmdOK.TabIndex = 6;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // LinkCodeFiles
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(776, 383);
            this.Controls.Add(tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "LinkCodeFiles";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Link Code Files";
            this.Load += new System.EventHandler(this.LinkCodeFiles_Load);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCodeFiles)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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