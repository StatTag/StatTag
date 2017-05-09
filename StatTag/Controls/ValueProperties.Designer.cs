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
            this.cboType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlNumeric = new System.Windows.Forms.Panel();
            this.pnlPercentage = new System.Windows.Forms.Panel();
            this.pnlDateTime = new System.Windows.Forms.Panel();
            this.pnlDefault = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.horizontalLine4 = new StatTag.Controls.HorizontalLine();
            this.dateTimeValueProperties1 = new StatTag.Controls.DateTimeValueProperties();
            this.horizontalLine3 = new StatTag.Controls.HorizontalLine();
            this.percentageValueProperties1 = new StatTag.Controls.PercentageValueProperties();
            this.horizontalLine2 = new StatTag.Controls.HorizontalLine();
            this.numericValueProperties1 = new StatTag.Controls.NumericValueProperties();
            this.horizontalLine1 = new StatTag.Controls.HorizontalLine();
            this.pnlNumeric.SuspendLayout();
            this.pnlPercentage.SuspendLayout();
            this.pnlDateTime.SuspendLayout();
            this.pnlDefault.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboType
            // 
            this.cboType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.FormattingEnabled = true;
            this.cboType.Location = new System.Drawing.Point(69, 23);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(185, 25);
            this.cboType.TabIndex = 5;
            this.cboType.SelectedIndexChanged += new System.EventHandler(this.cboType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Format";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Value Options";
            // 
            // pnlNumeric
            // 
            this.pnlNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNumeric.Controls.Add(this.numericValueProperties1);
            this.pnlNumeric.Controls.Add(this.horizontalLine1);
            this.pnlNumeric.Location = new System.Drawing.Point(0, 51);
            this.pnlNumeric.Margin = new System.Windows.Forms.Padding(0);
            this.pnlNumeric.Name = "pnlNumeric";
            this.pnlNumeric.Size = new System.Drawing.Size(268, 94);
            this.pnlNumeric.TabIndex = 8;
            // 
            // pnlPercentage
            // 
            this.pnlPercentage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPercentage.Controls.Add(this.percentageValueProperties1);
            this.pnlPercentage.Controls.Add(this.horizontalLine2);
            this.pnlPercentage.Location = new System.Drawing.Point(0, 145);
            this.pnlPercentage.Margin = new System.Windows.Forms.Padding(0);
            this.pnlPercentage.Name = "pnlPercentage";
            this.pnlPercentage.Size = new System.Drawing.Size(268, 62);
            this.pnlPercentage.TabIndex = 9;
            // 
            // pnlDateTime
            // 
            this.pnlDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDateTime.Controls.Add(this.dateTimeValueProperties1);
            this.pnlDateTime.Controls.Add(this.horizontalLine3);
            this.pnlDateTime.Location = new System.Drawing.Point(0, 213);
            this.pnlDateTime.Margin = new System.Windows.Forms.Padding(0);
            this.pnlDateTime.Name = "pnlDateTime";
            this.pnlDateTime.Size = new System.Drawing.Size(268, 88);
            this.pnlDateTime.TabIndex = 10;
            // 
            // pnlDefault
            // 
            this.pnlDefault.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDefault.Controls.Add(this.label3);
            this.pnlDefault.Controls.Add(this.horizontalLine4);
            this.pnlDefault.Location = new System.Drawing.Point(0, 314);
            this.pnlDefault.Margin = new System.Windows.Forms.Padding(0);
            this.pnlDefault.Name = "pnlDefault";
            this.pnlDefault.Size = new System.Drawing.Size(268, 57);
            this.pnlDefault.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(11, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(243, 41);
            this.label3.TabIndex = 2;
            this.label3.Text = "Default is the exact same output from the statistical package.";
            // 
            // horizontalLine4
            // 
            this.horizontalLine4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.horizontalLine4.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.horizontalLine4.Location = new System.Drawing.Point(0, 0);
            this.horizontalLine4.Name = "horizontalLine4";
            this.horizontalLine4.Size = new System.Drawing.Size(268, 11);
            this.horizontalLine4.TabIndex = 1;
            // 
            // dateTimeValueProperties1
            // 
            this.dateTimeValueProperties1.DateFormat = "";
            this.dateTimeValueProperties1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimeValueProperties1.Location = new System.Drawing.Point(11, 11);
            this.dateTimeValueProperties1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dateTimeValueProperties1.Name = "dateTimeValueProperties1";
            this.dateTimeValueProperties1.Size = new System.Drawing.Size(254, 76);
            this.dateTimeValueProperties1.TabIndex = 1;
            this.dateTimeValueProperties1.TimeFormat = "";
            // 
            // horizontalLine3
            // 
            this.horizontalLine3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.horizontalLine3.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.horizontalLine3.Location = new System.Drawing.Point(0, 0);
            this.horizontalLine3.Name = "horizontalLine3";
            this.horizontalLine3.Size = new System.Drawing.Size(268, 11);
            this.horizontalLine3.TabIndex = 0;
            // 
            // percentageValueProperties1
            // 
            this.percentageValueProperties1.DecimalPlaces = 0;
            this.percentageValueProperties1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.percentageValueProperties1.Location = new System.Drawing.Point(11, 13);
            this.percentageValueProperties1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.percentageValueProperties1.Name = "percentageValueProperties1";
            this.percentageValueProperties1.Size = new System.Drawing.Size(175, 37);
            this.percentageValueProperties1.TabIndex = 1;
            // 
            // horizontalLine2
            // 
            this.horizontalLine2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.horizontalLine2.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.horizontalLine2.Location = new System.Drawing.Point(0, 0);
            this.horizontalLine2.Name = "horizontalLine2";
            this.horizontalLine2.Size = new System.Drawing.Size(268, 11);
            this.horizontalLine2.TabIndex = 0;
            // 
            // numericValueProperties1
            // 
            this.numericValueProperties1.DecimalPlaces = 0;
            this.numericValueProperties1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericValueProperties1.Location = new System.Drawing.Point(11, 13);
            this.numericValueProperties1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numericValueProperties1.Name = "numericValueProperties1";
            this.numericValueProperties1.Size = new System.Drawing.Size(188, 63);
            this.numericValueProperties1.TabIndex = 1;
            this.numericValueProperties1.UseThousands = false;
            // 
            // horizontalLine1
            // 
            this.horizontalLine1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.horizontalLine1.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.horizontalLine1.Location = new System.Drawing.Point(0, 0);
            this.horizontalLine1.Name = "horizontalLine1";
            this.horizontalLine1.Size = new System.Drawing.Size(268, 11);
            this.horizontalLine1.TabIndex = 0;
            // 
            // ValueProperties
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.pnlDefault);
            this.Controls.Add(this.pnlDateTime);
            this.Controls.Add(this.pnlPercentage);
            this.Controls.Add(this.pnlNumeric);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboType);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ValueProperties";
            this.Size = new System.Drawing.Size(268, 387);
            this.Load += new System.EventHandler(this.ValueProperties_Load);
            this.pnlNumeric.ResumeLayout(false);
            this.pnlPercentage.ResumeLayout(false);
            this.pnlDateTime.ResumeLayout(false);
            this.pnlDefault.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlNumeric;
        private HorizontalLine horizontalLine1;
        private System.Windows.Forms.Panel pnlPercentage;
        private HorizontalLine horizontalLine2;
        private System.Windows.Forms.Panel pnlDateTime;
        private HorizontalLine horizontalLine3;
        private NumericValueProperties numericValueProperties1;
        private PercentageValueProperties percentageValueProperties1;
        private DateTimeValueProperties dateTimeValueProperties1;
        private System.Windows.Forms.Panel pnlDefault;
        private System.Windows.Forms.Label label3;
        private HorizontalLine horizontalLine4;

    }
}
