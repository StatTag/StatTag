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
            this.cmdLoadCode = this.Factory.CreateRibbonButton();
            this.cmdDefineTag = this.Factory.CreateRibbonButton();
            this.cmdInsertOutput = this.Factory.CreateRibbonButton();
            this.cmdUpdateOutput = this.Factory.CreateRibbonButton();
            this.cmdManageTags = this.Factory.CreateRibbonButton();
            this.cmdValidateDocument = this.Factory.CreateRibbonButton();
            this.cmdSettings = this.Factory.CreateRibbonButton();
            this.cmdHelp = this.Factory.CreateRibbonButton();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.group2 = this.Factory.CreateRibbonGroup();
            this.tab1.SuspendLayout();
            this.codeGroup.SuspendLayout();
            this.group1.SuspendLayout();
            this.group2.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.codeGroup);
            this.tab1.Groups.Add(this.group1);
            this.tab1.Groups.Add(this.group2);
            this.tab1.Label = "StatTag";
            this.tab1.Name = "tab1";
            // 
            // codeGroup
            // 
            this.codeGroup.Items.Add(this.cmdLoadCode);
            this.codeGroup.Items.Add(this.cmdDefineTag);
            this.codeGroup.Items.Add(this.cmdInsertOutput);
            this.codeGroup.Label = "Define";
            this.codeGroup.Name = "codeGroup";
            // 
            // cmdLoadCode
            // 
            this.cmdLoadCode.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdLoadCode.Image = global::StatTag.Properties.Resources._1446845069_Copy;
            this.cmdLoadCode.Label = "Code Files";
            this.cmdLoadCode.Name = "cmdLoadCode";
            this.cmdLoadCode.ShowImage = true;
            this.cmdLoadCode.SuperTip = "Manage the list of analysis source code files used within this document.";
            this.cmdLoadCode.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdLoadCode_Click);
            // 
            // cmdDefineTag
            // 
            this.cmdDefineTag.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdDefineTag.Image = ((System.Drawing.Image)(resources.GetObject("cmdDefineTag.Image")));
            this.cmdDefineTag.Label = "Define Tag";
            this.cmdDefineTag.Name = "cmdDefineTag";
            this.cmdDefineTag.ShowImage = true;
            this.cmdDefineTag.SuperTip = "Manage the list of tags that are used and referenced within this document.";
            this.cmdDefineTag.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdDefineTag_Click);
            // 
            // cmdInsertOutput
            // 
            this.cmdInsertOutput.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdInsertOutput.Image = ((System.Drawing.Image)(resources.GetObject("cmdInsertOutput.Image")));
            this.cmdInsertOutput.Label = "Insert Tag Output";
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
            this.cmdUpdateOutput.Label = "Update Tag Output";
            this.cmdUpdateOutput.Name = "cmdUpdateOutput";
            this.cmdUpdateOutput.ShowImage = true;
            this.cmdUpdateOutput.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdUpdateOutput_Click);
            // 
            // cmdManageTags
            // 
            this.cmdManageTags.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdManageTags.Image = ((System.Drawing.Image)(resources.GetObject("cmdManageTags.Image")));
            this.cmdManageTags.Label = "Manage Tags";
            this.cmdManageTags.Name = "cmdManageTags";
            this.cmdManageTags.ShowImage = true;
            this.cmdManageTags.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdManageTags_Click);
            // 
            // cmdValidateDocument
            // 
            this.cmdValidateDocument.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdValidateDocument.Image = ((System.Drawing.Image)(resources.GetObject("cmdValidateDocument.Image")));
            this.cmdValidateDocument.Label = "Troubleshoot Tags";
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
            // cmdHelp
            // 
            this.cmdHelp.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdHelp.Image = ((System.Drawing.Image)(resources.GetObject("cmdHelp.Image")));
            this.cmdHelp.Label = "Help";
            this.cmdHelp.Name = "cmdHelp";
            this.cmdHelp.ShowImage = true;
            this.cmdHelp.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdHelp_Click);
            // 
            // group1
            // 
            this.group1.Items.Add(this.cmdUpdateOutput);
            this.group1.Items.Add(this.cmdManageTags);
            this.group1.Items.Add(this.cmdValidateDocument);
            this.group1.Label = "Manage";
            this.group1.Name = "group1";
            // 
            // group2
            // 
            this.group2.Items.Add(this.cmdSettings);
            this.group2.Items.Add(this.cmdHelp);
            this.group2.Label = "Support";
            this.group2.Name = "group2";
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
            this.group2.ResumeLayout(false);
            this.group2.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup codeGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdLoadCode;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdDefineTag;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdInsertOutput;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdSettings;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdUpdateOutput;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdValidateDocument;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdManageTags;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdHelp;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group2;
    }

    partial class ThisRibbonCollection
    {
        internal MainRibbon MainRibbon
        {
            get { return this.GetRibbon<MainRibbon>(); }
        }
    }
}
