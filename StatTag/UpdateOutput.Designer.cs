﻿namespace StatTag
{
    partial class UpdateOutput
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
            this.cmdDefaultSelectAll = new System.Windows.Forms.Button();
            this.cmdDefaultSelectNone = new System.Windows.Forms.Button();
            this.cmdOnDemandSelectAll = new System.Windows.Forms.Button();
            this.cmdOnDemandSelectNone = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lvwOnDemand = new System.Windows.Forms.ListView();
            this.colTagName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvwDefault = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtOnDemandFilter = new StatTag.Controls.PlaceholderTextBox();
            this.txtDefaultFilter = new StatTag.Controls.PlaceholderTextBox();
            this.SuspendLayout();
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(360, 436);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
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
            this.cmdOK.Location = new System.Drawing.Point(185, 436);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(87, 25);
            this.cmdOK.TabIndex = 8;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            // 
            // cmdDefaultSelectAll
            // 
            this.cmdDefaultSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdDefaultSelectAll.Location = new System.Drawing.Point(12, 373);
            this.cmdDefaultSelectAll.Name = "cmdDefaultSelectAll";
            this.cmdDefaultSelectAll.Size = new System.Drawing.Size(87, 25);
            this.cmdDefaultSelectAll.TabIndex = 12;
            this.cmdDefaultSelectAll.Text = "Select All";
            this.cmdDefaultSelectAll.UseVisualStyleBackColor = true;
            this.cmdDefaultSelectAll.Click += new System.EventHandler(this.cmdDefaultSelectAll_Click);
            // 
            // cmdDefaultSelectNone
            // 
            this.cmdDefaultSelectNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdDefaultSelectNone.Location = new System.Drawing.Point(111, 373);
            this.cmdDefaultSelectNone.Name = "cmdDefaultSelectNone";
            this.cmdDefaultSelectNone.Size = new System.Drawing.Size(87, 25);
            this.cmdDefaultSelectNone.TabIndex = 13;
            this.cmdDefaultSelectNone.Text = "Select None";
            this.cmdDefaultSelectNone.UseVisualStyleBackColor = true;
            this.cmdDefaultSelectNone.Click += new System.EventHandler(this.cmdDefaultSelectNone_Click);
            // 
            // cmdOnDemandSelectAll
            // 
            this.cmdOnDemandSelectAll.Location = new System.Drawing.Point(12, 143);
            this.cmdOnDemandSelectAll.Name = "cmdOnDemandSelectAll";
            this.cmdOnDemandSelectAll.Size = new System.Drawing.Size(87, 25);
            this.cmdOnDemandSelectAll.TabIndex = 14;
            this.cmdOnDemandSelectAll.Text = "Select All";
            this.cmdOnDemandSelectAll.UseVisualStyleBackColor = true;
            this.cmdOnDemandSelectAll.Click += new System.EventHandler(this.cmdOnDemandSelectAll_Click);
            // 
            // cmdOnDemandSelectNone
            // 
            this.cmdOnDemandSelectNone.Location = new System.Drawing.Point(110, 143);
            this.cmdOnDemandSelectNone.Name = "cmdOnDemandSelectNone";
            this.cmdOnDemandSelectNone.Size = new System.Drawing.Size(87, 25);
            this.cmdOnDemandSelectNone.TabIndex = 15;
            this.cmdOnDemandSelectNone.Text = "Select None";
            this.cmdOnDemandSelectNone.UseVisualStyleBackColor = true;
            this.cmdOnDemandSelectNone.Click += new System.EventHandler(this.cmdOnDemandSelectNone_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(171, 17);
            this.label1.TabIndex = 16;
            this.label1.Text = "On Demand tags to update:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 198);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 17);
            this.label2.TabIndex = 17;
            this.label2.Text = "Tags to always update:";
            // 
            // lvwOnDemand
            // 
            this.lvwOnDemand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwOnDemand.CheckBoxes = true;
            this.lvwOnDemand.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTagName,
            this.colFileName});
            this.lvwOnDemand.FullRowSelect = true;
            this.lvwOnDemand.GridLines = true;
            this.lvwOnDemand.Location = new System.Drawing.Point(12, 31);
            this.lvwOnDemand.Name = "lvwOnDemand";
            this.lvwOnDemand.ShowGroups = false;
            this.lvwOnDemand.Size = new System.Drawing.Size(609, 106);
            this.lvwOnDemand.TabIndex = 18;
            this.lvwOnDemand.UseCompatibleStateImageBehavior = false;
            this.lvwOnDemand.View = System.Windows.Forms.View.Details;
            this.lvwOnDemand.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwOnDemand_ColumnClick);
            // 
            // colTagName
            // 
            this.colTagName.Text = "Output";
            this.colTagName.Width = 250;
            // 
            // colFileName
            // 
            this.colFileName.Text = "Code File";
            this.colFileName.Width = 300;
            // 
            // lvwDefault
            // 
            this.lvwDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwDefault.CheckBoxes = true;
            this.lvwDefault.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvwDefault.FullRowSelect = true;
            this.lvwDefault.GridLines = true;
            this.lvwDefault.Location = new System.Drawing.Point(12, 223);
            this.lvwDefault.Name = "lvwDefault";
            this.lvwDefault.ShowGroups = false;
            this.lvwDefault.Size = new System.Drawing.Size(609, 144);
            this.lvwDefault.TabIndex = 19;
            this.lvwDefault.UseCompatibleStateImageBehavior = false;
            this.lvwDefault.View = System.Windows.Forms.View.Details;
            this.lvwDefault.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwDefault_ColumnClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Output";
            this.columnHeader1.Width = 250;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Code File";
            this.columnHeader2.Width = 300;
            // 
            // txtOnDemandFilter
            // 
            this.txtOnDemandFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOnDemandFilter.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOnDemandFilter.Location = new System.Drawing.Point(226, 3);
            this.txtOnDemandFilter.Margin = new System.Windows.Forms.Padding(0);
            this.txtOnDemandFilter.Name = "txtOnDemandFilter";
            this.txtOnDemandFilter.PlaceholderText = "Search";
            this.txtOnDemandFilter.Size = new System.Drawing.Size(395, 25);
            this.txtOnDemandFilter.TabIndex = 20;
            this.txtOnDemandFilter.FilterChanged += new System.EventHandler(this.txtOnDemandFilter_FilterChanged);
            // 
            // txtDefaultFilter
            // 
            this.txtDefaultFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDefaultFilter.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDefaultFilter.Location = new System.Drawing.Point(226, 195);
            this.txtDefaultFilter.Margin = new System.Windows.Forms.Padding(0);
            this.txtDefaultFilter.Name = "txtDefaultFilter";
            this.txtDefaultFilter.PlaceholderText = "Search";
            this.txtDefaultFilter.Size = new System.Drawing.Size(395, 25);
            this.txtDefaultFilter.TabIndex = 21;
            this.txtDefaultFilter.FilterChanged += new System.EventHandler(this.txtDefaultFilter_FilterChanged);
            // 
            // UpdateOutput
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(633, 475);
            this.Controls.Add(this.txtDefaultFilter);
            this.Controls.Add(this.txtOnDemandFilter);
            this.Controls.Add(this.lvwDefault);
            this.Controls.Add(this.lvwOnDemand);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdOnDemandSelectNone);
            this.Controls.Add(this.cmdOnDemandSelectAll);
            this.Controls.Add(this.cmdDefaultSelectNone);
            this.Controls.Add(this.cmdDefaultSelectAll);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "UpdateOutput";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Update Statistical Output";
            this.Load += new System.EventHandler(this.UpdateOutput_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdDefaultSelectAll;
        private System.Windows.Forms.Button cmdDefaultSelectNone;
        private System.Windows.Forms.Button cmdOnDemandSelectAll;
        private System.Windows.Forms.Button cmdOnDemandSelectNone;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView lvwOnDemand;
        private System.Windows.Forms.ColumnHeader colTagName;
        private System.Windows.Forms.ColumnHeader colFileName;
        private System.Windows.Forms.ListView lvwDefault;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private Controls.PlaceholderTextBox txtOnDemandFilter;
        private Controls.PlaceholderTextBox txtDefaultFilter;
    }
}