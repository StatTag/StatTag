namespace AnalysisManager.Controls
{
    partial class TableProperties
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
            this.chkIncludeColumnNames = new System.Windows.Forms.CheckBox();
            this.chkIncludeRowNames = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkIncludeColumnNames
            // 
            this.chkIncludeColumnNames.AutoSize = true;
            this.chkIncludeColumnNames.Location = new System.Drawing.Point(5, 5);
            this.chkIncludeColumnNames.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkIncludeColumnNames.Name = "chkIncludeColumnNames";
            this.chkIncludeColumnNames.Size = new System.Drawing.Size(156, 21);
            this.chkIncludeColumnNames.TabIndex = 0;
            this.chkIncludeColumnNames.Text = "Include column names";
            this.chkIncludeColumnNames.UseVisualStyleBackColor = true;
            // 
            // chkIncludeRowNames
            // 
            this.chkIncludeRowNames.AutoSize = true;
            this.chkIncludeRowNames.Location = new System.Drawing.Point(5, 34);
            this.chkIncludeRowNames.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkIncludeRowNames.Name = "chkIncludeRowNames";
            this.chkIncludeRowNames.Size = new System.Drawing.Size(136, 21);
            this.chkIncludeRowNames.TabIndex = 1;
            this.chkIncludeRowNames.Text = "Include row names";
            this.chkIncludeRowNames.UseVisualStyleBackColor = true;
            // 
            // TableProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkIncludeRowNames);
            this.Controls.Add(this.chkIncludeColumnNames);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TableProperties";
            this.Size = new System.Drawing.Size(222, 94);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkIncludeColumnNames;
        private System.Windows.Forms.CheckBox chkIncludeRowNames;
    }
}
