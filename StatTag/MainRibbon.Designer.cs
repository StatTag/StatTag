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
            this.group1 = this.Factory.CreateRibbonGroup();
            this.cmdLoadCode = this.Factory.CreateRibbonButton();
            this.cmdManageTags = this.Factory.CreateRibbonButton();
            this.cmdDocumentProperties = this.Factory.CreateRibbonButton();
            this.group2 = this.Factory.CreateRibbonGroup();
            this.cmdSettings = this.Factory.CreateRibbonButton();
            this.cmdAbout = this.Factory.CreateRibbonButton();
            this.cmdHelp = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.group1.SuspendLayout();
            this.group2.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.group1);
            this.tab1.Groups.Add(this.group2);
            this.tab1.Label = "StatTag";
            this.tab1.Name = "tab1";
            // 
            // group1
            // 
            this.group1.Items.Add(this.cmdLoadCode);
            this.group1.Items.Add(this.cmdManageTags);
            this.group1.Items.Add(this.cmdDocumentProperties);
            this.group1.Label = "Manage";
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
            // cmdManageTags
            // 
            this.cmdManageTags.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdManageTags.Image = ((System.Drawing.Image)(resources.GetObject("cmdManageTags.Image")));
            this.cmdManageTags.Label = "Tags";
            this.cmdManageTags.Name = "cmdManageTags";
            this.cmdManageTags.ShowImage = true;
            this.cmdManageTags.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdManageTags_Click);
            // 
            // cmdDocumentProperties
            // 
            this.cmdDocumentProperties.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdDocumentProperties.Image = ((System.Drawing.Image)(resources.GetObject("cmdDocumentProperties.Image")));
            this.cmdDocumentProperties.Label = "Document Properties";
            this.cmdDocumentProperties.Name = "cmdDocumentProperties";
            this.cmdDocumentProperties.ShowImage = true;
            this.cmdDocumentProperties.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdDocumentProperties_Click);
            // 
            // group2
            // 
            this.group2.Items.Add(this.cmdSettings);
            this.group2.Items.Add(this.cmdAbout);
            this.group2.Items.Add(this.cmdHelp);
            this.group2.Label = "Support";
            this.group2.Name = "group2";
            // 
            // cmdSettings
            // 
            this.cmdSettings.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdSettings.Image = ((System.Drawing.Image)(resources.GetObject("cmdSettings.Image")));
            this.cmdSettings.Label = "User Settings";
            this.cmdSettings.Name = "cmdSettings";
            this.cmdSettings.ShowImage = true;
            this.cmdSettings.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdSettings_Click);
            // 
            // cmdAbout
            // 
            this.cmdAbout.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.cmdAbout.Description = "Find out more information about StatTag";
            this.cmdAbout.Image = ((System.Drawing.Image)(resources.GetObject("cmdAbout.Image")));
            this.cmdAbout.Label = "About";
            this.cmdAbout.Name = "cmdAbout";
            this.cmdAbout.ShowImage = true;
            this.cmdAbout.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cmdAbout_Click);
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
            // MainRibbon
            // 
            this.Name = "MainRibbon";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.MainRibbon_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.group2.ResumeLayout(false);
            this.group2.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdLoadCode;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdSettings;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdManageTags;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdHelp;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdAbout;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton cmdDocumentProperties;
    }

    partial class ThisRibbonCollection
    {
        internal MainRibbon MainRibbon
        {
            get { return this.GetRibbon<MainRibbon>(); }
        }
    }
}
