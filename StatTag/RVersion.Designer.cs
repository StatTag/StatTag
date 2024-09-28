
namespace StatTag
{
    partial class RVersion
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
            this.radCustomR = new System.Windows.Forms.RadioButton();
            this.radDefaultR = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.cmdCustomRPath = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lstRVersions = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // radCustomR
            // 
            this.radCustomR.AutoSize = true;
            this.radCustomR.Location = new System.Drawing.Point(8, 52);
            this.radCustomR.Name = "radCustomR";
            this.radCustomR.Size = new System.Drawing.Size(205, 21);
            this.radCustomR.TabIndex = 5;
            this.radCustomR.Text = "Choose a specific version of R:";
            this.radCustomR.UseVisualStyleBackColor = true;
            this.radCustomR.CheckedChanged += new System.EventHandler(this.radCustomR_CheckedChanged);
            // 
            // radDefaultR
            // 
            this.radDefaultR.AutoSize = true;
            this.radDefaultR.Checked = true;
            this.radDefaultR.Location = new System.Drawing.Point(8, 25);
            this.radDefaultR.Name = "radDefaultR";
            this.radDefaultR.Size = new System.Drawing.Size(257, 21);
            this.radDefaultR.TabIndex = 4;
            this.radDefaultR.TabStop = true;
            this.radDefaultR.Text = "Use your machine\'s default version of R";
            this.radDefaultR.UseVisualStyleBackColor = true;
            this.radDefaultR.CheckedChanged += new System.EventHandler(this.radDefaultR_CheckedChanged);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.cmdCustomRPath, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.cmdOK, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.cmdCancel, 2, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(5, 205);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(570, 35);
            this.tableLayoutPanel3.TabIndex = 3;
            // 
            // cmdCustomRPath
            // 
            this.cmdCustomRPath.AutoSize = true;
            this.cmdCustomRPath.Location = new System.Drawing.Point(3, 3);
            this.cmdCustomRPath.Name = "cmdCustomRPath";
            this.cmdCustomRPath.Size = new System.Drawing.Size(69, 27);
            this.cmdCustomRPath.TabIndex = 0;
            this.cmdCustomRPath.Text = "Browse...";
            this.cmdCustomRPath.UseVisualStyleBackColor = true;
            this.cmdCustomRPath.Click += new System.EventHandler(this.cmdCustomRPath_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(232, 4);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(3, 4, 15, 4);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 27);
            this.cmdOK.TabIndex = 0;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(337, 4);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(15, 4, 3, 4);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 27);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(229, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please select the version of R to use:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.radDefaultR, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.radCustomR, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lstRVersions, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(580, 245);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lstRVersions
            // 
            this.lstRVersions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstRVersions.Enabled = false;
            this.lstRVersions.FormattingEnabled = true;
            this.lstRVersions.ItemHeight = 17;
            this.lstRVersions.Location = new System.Drawing.Point(8, 79);
            this.lstRVersions.Name = "lstRVersions";
            this.lstRVersions.Size = new System.Drawing.Size(564, 123);
            this.lstRVersions.TabIndex = 6;
            // 
            // RVersion
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(580, 245);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(596, 284);
            this.Name = "RVersion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure R Installation";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.RVersion_Load);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radCustomR;
        private System.Windows.Forms.RadioButton radDefaultR;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button cmdCustomRPath;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListBox lstRVersions;
    }
}