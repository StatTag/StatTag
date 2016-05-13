namespace StatTag
{
    sealed partial class ManageTags
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.colCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colStatPackage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOutputLabel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRunFrequency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEdit = new System.Windows.Forms.DataGridViewButtonColumn();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.cmdRemove = new System.Windows.Forms.Button();
            this.cmdAdd = new System.Windows.Forms.Button();
            this.txtFilter = new StatTag.Controls.PlaceholderTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(266, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "List of tags defined within linked code file(s):";
            // 
            // dgvItems
            // 
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToDeleteRows = false;
            this.dgvItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvItems.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvItems.CausesValidation = false;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCheck,
            this.colStatPackage,
            this.colType,
            this.colOutputLabel,
            this.colRunFrequency,
            this.colEdit});
            this.dgvItems.Location = new System.Drawing.Point(15, 39);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.RowHeadersVisible = false;
            this.dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvItems.Size = new System.Drawing.Size(589, 255);
            this.dgvItems.TabIndex = 1;
            this.dgvItems.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellContentClick);
            this.dgvItems.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellContentDoubleClick);
            // 
            // colCheck
            // 
            this.colCheck.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colCheck.HeaderText = "";
            this.colCheck.MinimumWidth = 20;
            this.colCheck.Name = "colCheck";
            this.colCheck.Width = 20;
            // 
            // colStatPackage
            // 
            this.colStatPackage.HeaderText = "Run In";
            this.colStatPackage.MinimumWidth = 70;
            this.colStatPackage.Name = "colStatPackage";
            this.colStatPackage.ReadOnly = true;
            this.colStatPackage.Width = 70;
            // 
            // colType
            // 
            this.colType.HeaderText = "Type";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            this.colType.Width = 61;
            // 
            // colOutputLabel
            // 
            this.colOutputLabel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colOutputLabel.HeaderText = "Output Label";
            this.colOutputLabel.MinimumWidth = 150;
            this.colOutputLabel.Name = "colOutputLabel";
            this.colOutputLabel.ReadOnly = true;
            // 
            // colRunFrequency
            // 
            this.colRunFrequency.HeaderText = "When to Run";
            this.colRunFrequency.MinimumWidth = 100;
            this.colRunFrequency.Name = "colRunFrequency";
            this.colRunFrequency.Width = 108;
            // 
            // colEdit
            // 
            this.colEdit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.colEdit.DefaultCellStyle = dataGridViewCellStyle1;
            this.colEdit.HeaderText = "";
            this.colEdit.MinimumWidth = 40;
            this.colEdit.Name = "colEdit";
            this.colEdit.ReadOnly = true;
            this.colEdit.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colEdit.Text = "";
            this.colEdit.ToolTipText = "Edit this item";
            this.colEdit.Width = 40;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(365, 347);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(87, 25);
            this.cmdCancel.TabIndex = 5;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(164, 347);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(87, 25);
            this.cmdOK.TabIndex = 4;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            // 
            // cmdRemove
            // 
            this.cmdRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.cmdRemove.Location = new System.Drawing.Point(311, 301);
            this.cmdRemove.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdRemove.Name = "cmdRemove";
            this.cmdRemove.Size = new System.Drawing.Size(168, 25);
            this.cmdRemove.TabIndex = 8;
            this.cmdRemove.Text = "&Remove Tag(s)";
            this.cmdRemove.UseVisualStyleBackColor = true;
            this.cmdRemove.Click += new System.EventHandler(this.cmdRemove_Click);
            // 
            // cmdAdd
            // 
            this.cmdAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.cmdAdd.Location = new System.Drawing.Point(137, 301);
            this.cmdAdd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdAdd.Name = "cmdAdd";
            this.cmdAdd.Size = new System.Drawing.Size(168, 25);
            this.cmdAdd.TabIndex = 7;
            this.cmdAdd.Text = "&Add Tag";
            this.cmdAdd.UseVisualStyleBackColor = true;
            this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
            // 
            // txtFilter
            // 
            this.txtFilter.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilter.Location = new System.Drawing.Point(403, 9);
            this.txtFilter.Margin = new System.Windows.Forms.Padding(0);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.PlaceholderText = "Search";
            this.txtFilter.Size = new System.Drawing.Size(201, 25);
            this.txtFilter.TabIndex = 9;
            this.txtFilter.FilterChanged += new System.EventHandler(this.txtFilter_FilterChanged);
            // 
            // ManageTags
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(616, 384);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.cmdRemove);
            this.Controls.Add(this.cmdAdd);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ManageTags";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "StatTag - Available Tags";
            this.Load += new System.EventHandler(this.ManageCodeBlocks_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.Button cmdRemove;
        private System.Windows.Forms.Button cmdAdd;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatPackage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOutputLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRunFrequency;
        private System.Windows.Forms.DataGridViewButtonColumn colEdit;
        private Controls.PlaceholderTextBox txtFilter;
    }
}