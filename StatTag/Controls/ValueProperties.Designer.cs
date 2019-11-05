namespace StatTag.Controls
{
    partial class ValueProperties
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
            System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
            this.lblDefault = new System.Windows.Forms.Label();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.numericValueProperties1 = new StatTag.Controls.NumericValueProperties();
            this.percentageValueProperties1 = new StatTag.Controls.PercentageValueProperties();
            this.dateTimeValueProperties1 = new StatTag.Controls.DateTimeValueProperties();
            this.horizontalLine1 = new StatTag.Controls.HorizontalLine();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(flowLayoutPanel1, 2);
            flowLayoutPanel1.Controls.Add(this.lblDefault);
            flowLayoutPanel1.Controls.Add(this.numericValueProperties1);
            flowLayoutPanel1.Controls.Add(this.percentageValueProperties1);
            flowLayoutPanel1.Controls.Add(this.dateTimeValueProperties1);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanel1.Location = new System.Drawing.Point(3, 93);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(547, 190);
            flowLayoutPanel1.TabIndex = 13;
            // 
            // lblDefault
            // 
            this.lblDefault.AutoSize = true;
            this.lblDefault.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDefault.Location = new System.Drawing.Point(3, 0);
            this.lblDefault.Name = "lblDefault";
            this.lblDefault.Size = new System.Drawing.Size(541, 28);
            this.lblDefault.TabIndex = 2;
            this.lblDefault.Text = "Default is the exact same output from the statistical package.";
            // 
            // cboType
            // 
            this.cboType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.FormattingEnabled = true;
            this.cboType.Location = new System.Drawing.Point(84, 31);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(466, 36);
            this.cboType.TabIndex = 5;
            this.cboType.SelectedIndexChanged += new System.EventHandler(this.cboType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 42);
            this.label1.TabIndex = 6;
            this.label1.Text = "Format";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(553, 28);
            this.label2.TabIndex = 7;
            this.label2.Text = "Value Options";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.horizontalLine1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cboType, 1, 1);
            this.tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(558, 286);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // numericValueProperties1
            // 
            this.numericValueProperties1.AutoSize = true;
            this.numericValueProperties1.DecimalPlaces = 0;
            this.numericValueProperties1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericValueProperties1.Location = new System.Drawing.Point(3, 28);
            this.numericValueProperties1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.numericValueProperties1.Name = "numericValueProperties1";
            this.numericValueProperties1.Size = new System.Drawing.Size(259, 82);
            this.numericValueProperties1.TabIndex = 1;
            this.numericValueProperties1.UseThousands = false;
            // 
            // percentageValueProperties1
            // 
            this.percentageValueProperties1.AutoSize = true;
            this.percentageValueProperties1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.percentageValueProperties1.DecimalPlaces = 0;
            this.percentageValueProperties1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.percentageValueProperties1.Location = new System.Drawing.Point(268, 28);
            this.percentageValueProperties1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.percentageValueProperties1.Name = "percentageValueProperties1";
            this.percentageValueProperties1.Size = new System.Drawing.Size(209, 39);
            this.percentageValueProperties1.TabIndex = 1;
            // 
            // dateTimeValueProperties1
            // 
            this.dateTimeValueProperties1.AutoSize = true;
            this.dateTimeValueProperties1.DateFormat = "";
            this.dateTimeValueProperties1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimeValueProperties1.Location = new System.Drawing.Point(3, 110);
            this.dateTimeValueProperties1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.dateTimeValueProperties1.Name = "dateTimeValueProperties1";
            this.dateTimeValueProperties1.Size = new System.Drawing.Size(259, 80);
            this.dateTimeValueProperties1.TabIndex = 1;
            this.dateTimeValueProperties1.TimeFormat = "";
            // 
            // horizontalLine1
            // 
            this.horizontalLine1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.horizontalLine1.AutoSize = true;
            this.horizontalLine1.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.tableLayoutPanel1.SetColumnSpan(this.horizontalLine1, 2);
            this.horizontalLine1.Location = new System.Drawing.Point(3, 73);
            this.horizontalLine1.Name = "horizontalLine1";
            this.horizontalLine1.Padding = new System.Windows.Forms.Padding(3);
            this.horizontalLine1.Size = new System.Drawing.Size(547, 6);
            this.horizontalLine1.TabIndex = 0;
            // 
            // ValueProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ValueProperties";
            this.Size = new System.Drawing.Size(558, 286);
            this.Load += new System.EventHandler(this.ValueProperties_Load);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private HorizontalLine horizontalLine1;
        private NumericValueProperties numericValueProperties1;
        private PercentageValueProperties percentageValueProperties1;
        private DateTimeValueProperties dateTimeValueProperties1;
        private System.Windows.Forms.Label lblDefault;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

    }
}
