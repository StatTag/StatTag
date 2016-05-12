namespace StatTag
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
            this.separator1 = this.Factory.CreateRibbonSeparator();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.cmdLoadCode = this.Factory.CreateRibbonButton();
            this.cmdManageAnnotations = this.Factory.CreateRibbonButton();
            this.cmdInsertOutput = this.Factory.CreateRibbonButton();
            this.cmdUpdateOutput = this.Factory.CreateRibbonButton();
            this.cmdValidateDocument = this.Factory.CreateRibbonButton();
            this.cmdSettings = this.Factory.CreateRibbonButton();
            this.cmdManageTags = this.Factory.CreateRibbonButton();
            this.cmdHelp = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.codeGroup.SuspendLayout();
            this.group1.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.codeGroup);
            this.tab1.Groups.Add(this.group1);
            this.tab1.Label = "StatTag";
            this.tab1.Name = "tab1";
            // 
            // codeGroup
            // 
            this.codeGroup.Items.Add(this.cmdLoadCode);
            this.codeGroup.Items.Add(this.cmdManageAnnotations);
            this.codeGroup.Items.Add(this.cmdInsertOutput);
            this.codeGroup.Items.Add(this.separator1);
            this.codeGroup.Items.Add(this.cmdUpdateOutput);
            this.codeGroup.Items.Add(this.cmdManageTags);
            this.codeGroup.Items.Add(this.cmdValidateDocument);
            this.codeGroup.Label = "StatTag";
            this.codeGroup.Name = "codeGroup";
            // 
            // separator1
            // 
            this.separator1.Name = "separator1";
            // 
            // group1
            // 
            this.group1.Items.Add(this.cmdSettings);
            this.group1.Items.Add(this.cmdHelp);
            this.group1.Name = "group1";
            // 
            // cmdLoadCode
            // 
            this.cmdLoadCode.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdLoadCode.Image = ((System.Drawing.Image)(resources.GetObject("cmdLoadCode.Image")));
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
            this.cmdManageAnnotations.Label = "Define Tag";
            this.cmdManageAnnotations.Name = "cmdManageAnnotations";
            this.cmdManageAnnotations.ShowImage = true;
            this.cmdManageAnnotations.SuperTip = "Manage the list of annotations that are used and referenced within this document." +
    "";
            this.cmdManageAnnotations.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdManageAnnotations_Click);
            // 
            // cmdInsertOutput
            // 
            this.cmdInsertOutput.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
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
            this.cmdUpdateOutput.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdUpdateOutput.Image = ((System.Drawing.Image)(resources.GetObject("cmdUpdateOutput.Image")));
            this.cmdUpdateOutput.Label = "Update Output";
            this.cmdUpdateOutput.Name = "cmdUpdateOutput";
            this.cmdUpdateOutput.ShowImage = true;
            this.cmdUpdateOutput.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdUpdateOutput_Click);
            // 
            // cmdValidateDocument
            // 
            this.cmdValidateDocument.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdValidateDocument.Image = ((System.Drawing.Image)(resources.GetObject("cmdValidateDocument.Image")));
            this.cmdValidateDocument.Label = "Check Annotations";
            this.cmdValidateDocument.Name = "cmdValidateDocument";
            this.cmdValidateDocument.ScreenTip = "Perform a set of validations on the current document to ensure it is properly con" +
    "figured for use with StatTag.";
            this.cmdValidateDocument.ShowImage = true;
            this.cmdValidateDocument.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdValidateDocument_Click);
            // 
            // cmdSettings
            // 
            this.cmdSettings.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdSettings.Image = ((System.Drawing.Image)(resources.GetObject("cmdSettings.Image")));
            this.cmdSettings.Label = "Settings";
            this.cmdSettings.Name = "cmdSettings";
            this.cmdSettings.ShowImage = true;
            this.cmdSettings.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdSettings_Click);
            // 
            // cmdManageTags
            // 
            this.cmdManageTags.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdManageTags.Image = ((System.Drawing.Image)(resources.GetObject("cmdManageTags.Image")));
            this.cmdManageTags.Label = "Manage Tags";
            this.cmdManageTags.Name = "cmdManageTags";
            this.cmdManageTags.ShowImage = true;
            // 
            // cmdHelp
            // 
            this.cmdHelp.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdHelp.Image = ((System.Drawing.Image)(resources.GetObject("cmdHelp.Image")));
            this.cmdHelp.Label = "Help";
            this.cmdHelp.Name = "cmdHelp";
            this.cmdHelp.ShowImage = true;
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
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup codeGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdLoadCode;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdManageAnnotations;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdInsertOutput;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdSettings;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdUpdateOutput;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdValidateDocument;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdManageTags;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdHelp;
    }

    partial class ThisRibbonCollection
    {
        internal MainRibbon MainRibbon
        {
            get { return this.GetRibbon<MainRibbon>(); }
        }
    }
}
