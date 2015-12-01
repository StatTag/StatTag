namespace AnalysisManager
{
    sealed partial class ManageAnnotation
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
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboRunFrequency = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtOutputLabel = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pnlType = new System.Windows.Forms.Panel();
            this.cmdValue = new System.Windows.Forms.Button();
            this.cmdFigure = new System.Windows.Forms.Button();
            this.cmdTable = new System.Windows.Forms.Button();
            this.cboCodeFiles = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdCancel.Location = new System.Drawing.Point(377, 520);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 25);
            this.cmdCancel.TabIndex = 7;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOK.Location = new System.Drawing.Point(143, 520);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 25);
            this.cmdOK.TabIndex = 6;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Code file:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(167, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "When should this code run:";
            // 
            // cboRunFrequency
            // 
            this.cboRunFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRunFrequency.FormattingEnabled = true;
            this.cboRunFrequency.Location = new System.Drawing.Point(203, 39);
            this.cboRunFrequency.Name = "cboRunFrequency";
            this.cboRunFrequency.Size = new System.Drawing.Size(209, 25);
            this.cboRunFrequency.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(182, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Select the code region to use:";
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(16, 125);
            this.txtCode.Multiline = true;
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(566, 180);
            this.txtCode.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 17);
            this.label4.TabIndex = 14;
            this.label4.Text = "Output label:";
            // 
            // txtOutputLabel
            // 
            this.txtOutputLabel.Location = new System.Drawing.Point(203, 73);
            this.txtOutputLabel.Name = "txtOutputLabel";
            this.txtOutputLabel.Size = new System.Drawing.Size(379, 25);
            this.txtOutputLabel.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 332);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(355, 17);
            this.label5.TabIndex = 16;
            this.label5.Text = "How should these results be accessible from the document:";
            // 
            // pnlType
            // 
            this.pnlType.BackColor = System.Drawing.Color.White;
            this.pnlType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlType.Location = new System.Drawing.Point(98, 353);
            this.pnlType.Name = "pnlType";
            this.pnlType.Size = new System.Drawing.Size(484, 160);
            this.pnlType.TabIndex = 17;
            // 
            // cmdValue
            // 
            this.cmdValue.BackColor = System.Drawing.Color.White;
            this.cmdValue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdValue.Location = new System.Drawing.Point(16, 353);
            this.cmdValue.Name = "cmdValue";
            this.cmdValue.Size = new System.Drawing.Size(83, 30);
            this.cmdValue.TabIndex = 18;
            this.cmdValue.Text = "Value";
            this.cmdValue.UseVisualStyleBackColor = false;
            this.cmdValue.Click += new System.EventHandler(this.cmdValue_Click);
            // 
            // cmdFigure
            // 
            this.cmdFigure.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdFigure.Location = new System.Drawing.Point(24, 382);
            this.cmdFigure.Name = "cmdFigure";
            this.cmdFigure.Size = new System.Drawing.Size(75, 30);
            this.cmdFigure.TabIndex = 19;
            this.cmdFigure.Text = "Figure";
            this.cmdFigure.UseVisualStyleBackColor = true;
            this.cmdFigure.Click += new System.EventHandler(this.cmdFigure_Click);
            // 
            // cmdTable
            // 
            this.cmdTable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdTable.Location = new System.Drawing.Point(24, 411);
            this.cmdTable.Name = "cmdTable";
            this.cmdTable.Size = new System.Drawing.Size(75, 30);
            this.cmdTable.TabIndex = 20;
            this.cmdTable.Text = "Table";
            this.cmdTable.UseVisualStyleBackColor = true;
            this.cmdTable.Click += new System.EventHandler(this.cmdTable_Click);
            // 
            // cboCodeFiles
            // 
            this.cboCodeFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCodeFiles.FormattingEnabled = true;
            this.cboCodeFiles.Location = new System.Drawing.Point(203, 8);
            this.cboCodeFiles.Name = "cboCodeFiles";
            this.cboCodeFiles.Size = new System.Drawing.Size(379, 25);
            this.cboCodeFiles.TabIndex = 21;
            // 
            // ManageAnnotation
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(594, 558);
            this.Controls.Add(this.pnlType);
            this.Controls.Add(this.cboCodeFiles);
            this.Controls.Add(this.cmdTable);
            this.Controls.Add(this.cmdFigure);
            this.Controls.Add(this.cmdValue);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtOutputLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cboRunFrequency);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ManageAnnotation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Annotation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboRunFrequency;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtOutputLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel pnlType;
        private System.Windows.Forms.Button cmdValue;
        private System.Windows.Forms.Button cmdFigure;
        private System.Windows.Forms.Button cmdTable;
        private System.Windows.Forms.ComboBox cboCodeFiles;
    }
}