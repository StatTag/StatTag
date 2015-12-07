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
            this.cmdManageOutput = this.Factory.CreateRibbonButton();
            this.cmdInsertOutput = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.codeGroup.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.codeGroup);
            this.tab1.Label = "Analysis Manager";
            this.tab1.Name = "tab1";
            // 
            // codeGroup
            // 
            this.codeGroup.Items.Add(this.cmdLoadCode);
            this.codeGroup.Items.Add(this.cmdManageOutput);
            this.codeGroup.Items.Add(this.cmdInsertOutput);
            this.codeGroup.Label = "Analysis Manager";
            this.codeGroup.Name = "codeGroup";
            // 
            // cmdLoadCode
            // 
            this.cmdLoadCode.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdLoadCode.Image = global::AnalysisManager.Properties.Resources._1446845069_Copy;
            this.cmdLoadCode.Label = "Code Files";
            this.cmdLoadCode.Name = "cmdLoadCode";
            this.cmdLoadCode.ShowImage = true;
            this.cmdLoadCode.SuperTip = "Manage the list of analysis source code files used within this document.";
            this.cmdLoadCode.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdLoadCode_Click);
            // 
            // cmdManageOutput
            // 
            this.cmdManageOutput.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdManageOutput.Image = global::AnalysisManager.Properties.Resources._1447149791_file_manager;
            this.cmdManageOutput.Label = "Annotations";
            this.cmdManageOutput.Name = "cmdManageOutput";
            this.cmdManageOutput.ShowImage = true;
            this.cmdManageOutput.SuperTip = "Manage the list of annotations that are used and referenced within this document." +
    "";
            this.cmdManageOutput.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdManageOutput_Click);
            // 
            // cmdInsertOutput
            // 
            this.cmdInsertOutput.Label = "Insert Output";
            this.cmdInsertOutput.Name = "cmdInsertOutput";
            this.cmdInsertOutput.SuperTip = "Insert output from your statistical analysis into the document at the current cur" +
    "sor location.";
            this.cmdInsertOutput.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdInsertOutput_Click);
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
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdManageOutput;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdInsertOutput;
    }

    partial class ThisRibbonCollection
    {
        internal MainRibbon MainRibbon
        {
            get { return this.GetRibbon<MainRibbon>(); }
        }
    }
}
