namespace AnalysisManager
{
    partial class MainRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public MainRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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
            this.tab1 = this.Factory.CreateRibbonTab();
            this.codeGroup = this.Factory.CreateRibbonGroup();
            this.cmdLoadCode = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.codeGroup.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.codeGroup);
            this.tab1.Label = "TabAddIns";
            this.tab1.Name = "tab1";
            // 
            // codeGroup
            // 
            this.codeGroup.Items.Add(this.cmdLoadCode);
            this.codeGroup.Label = "Manage Code";
            this.codeGroup.Name = "codeGroup";
            // 
            // cmdLoadCode
            // 
            this.cmdLoadCode.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdLoadCode.Image = global::AnalysisManager.Properties.Resources._1446845069_Copy;
            this.cmdLoadCode.Label = "Load Analysis Code";
            this.cmdLoadCode.Name = "cmdLoadCode";
            this.cmdLoadCode.ShowImage = true;
            this.cmdLoadCode.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdLoadCode_Click);
            // 
            // MainRibbon
            // 
            this.Name = "MainRibbon";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.MainRibbon_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.codeGroup.ResumeLayout(false);
            this.codeGroup.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup codeGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdLoadCode;
    }

    partial class ThisRibbonCollection
    {
        internal MainRibbon MainRibbon
        {
            get { return this.GetRibbon<MainRibbon>(); }
        }
    }
}
