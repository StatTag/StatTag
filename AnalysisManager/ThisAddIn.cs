﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using AnalysisManager.Models;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;

namespace AnalysisManager
{
    public partial class ThisAddIn
    {
        public DocumentManager Manager = new DocumentManager();
        public PropertiesManager PropertiesManager = new PropertiesManager();

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // We'll load at Startup but won't save on Shutdown.  We only save when the user makes
            // a change and then confirms it through the Settings dialog.
            PropertiesManager.Load();
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        void Application_DocumentBeforeSave(Word.Document doc, ref bool saveAsUI, ref bool cancel)
        {
            Manager.SaveFilesToDocument(doc);
        }

        void Application_DocumentOpen(Word.Document doc)
        {
            Manager.LoadFilesFromDocument(doc);
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
            this.Application.DocumentBeforeSave += new Word.ApplicationEvents4_DocumentBeforeSaveEventHandler(Application_DocumentBeforeSave);
            this.Application.DocumentOpen += new Word.ApplicationEvents4_DocumentOpenEventHandler(Application_DocumentOpen);
        }
        
        #endregion
    }
}
