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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainRibbon));
            this.tab1 = this.Factory.CreateRibbonTab();
            this.codeGroup = this.Factory.CreateRibbonGroup();
            this.cmdLoadCode = this.Factory.CreateRibbonButton();
            this.cmdManageAnnotations = this.Factory.CreateRibbonButton();
            this.cmdInsertOutput = this.Factory.CreateRibbonButton();
            this.cmdUpdateOutput = this.Factory.CreateRibbonButton();
            this.cmdValidateDocument = this.Factory.CreateRibbonButton();
            this.cmdSettings = this.Factory.CreateRibbonButton();
            this.button1 = this.Factory.CreateRibbonButton();
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
            this.codeGroup.Items.Add(this.cmdManageAnnotations);
            this.codeGroup.Items.Add(this.cmdInsertOutput);
            this.codeGroup.Items.Add(this.cmdUpdateOutput);
            this.codeGroup.Items.Add(this.cmdValidateDocument);
            this.codeGroup.Items.Add(this.cmdSettings);
            this.codeGroup.Items.Add(this.button1);
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
            // cmdManageAnnotations
            // 
            this.cmdManageAnnotations.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdManageAnnotations.Image = ((System.Drawing.Image)(resources.GetObject("cmdManageAnnotations.Image")));
            this.cmdManageAnnotations.Label = "Annotations";
            this.cmdManageAnnotations.Name = "cmdManageAnnotations";
            this.cmdManageAnnotations.ShowImage = true;
            this.cmdManageAnnotations.SuperTip = "Manage the list of annotations that are used and referenced within this document." +
    "";
            this.cmdManageAnnotations.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdManageAnnotations_Click);
            // 
            // cmdInsertOutput
            // 
            this.cmdInsertOutput.Image = ((System.Drawing.Image)(resources.GetObject("cmdInsertOutput.Image")));
            this.cmdInsertOutput.Label = "Insert Output";
            this.cmdInsertOutput.Name = "cmdInsertOutput";
            this.cmdInsertOutput.ShowImage = true;
            this.cmdInsertOutput.SuperTip = "Insert output from your statistical analysis into the document at the current cur" +
    "sor location.";
            this.cmdInsertOutput.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdInsertOutput_Click);
            // 
            // cmdUpdateOutput
            // 
            this.cmdUpdateOutput.Image = ((System.Drawing.Image)(resources.GetObject("cmdUpdateOutput.Image")));
            this.cmdUpdateOutput.Label = "Update Output";
            this.cmdUpdateOutput.Name = "cmdUpdateOutput";
            this.cmdUpdateOutput.ShowImage = true;
            this.cmdUpdateOutput.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdUpdateOutput_Click);
            // 
            // cmdValidateDocument
            // 
            this.cmdValidateDocument.Image = ((System.Drawing.Image)(resources.GetObject("cmdValidateDocument.Image")));
            this.cmdValidateDocument.Label = "Check Annotations";
            this.cmdValidateDocument.Name = "cmdValidateDocument";
            this.cmdValidateDocument.ScreenTip = "Perform a set of validations on the current document to ensure it is properly con" +
    "figured for use with Analysis Manager.";
            this.cmdValidateDocument.ShowImage = true;
            this.cmdValidateDocument.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdValidateDocument_Click);
            // 
            // cmdSettings
            // 
            this.cmdSettings.Image = ((System.Drawing.Image)(resources.GetObject("cmdSettings.Image")));
            this.cmdSettings.Label = "Settings";
            this.cmdSettings.Name = "cmdSettings";
            this.cmdSettings.ShowImage = true;
            this.cmdSettings.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdSettings_Click);
            // 
            // button1
            // 
            this.button1.Label = "";
            this.button1.Name = "button1";
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
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdManageAnnotations;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdInsertOutput;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdSettings;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdUpdateOutput;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdValidateDocument;
    }

    partial class ThisRibbonCollection
    {
        internal MainRibbon MainRibbon
        {
            get { return this.GetRibbon<MainRibbon>(); }
        }
    }
}
