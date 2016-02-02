using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using AnalysisManager.Core.Models;
using AnalysisManager.Models;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using System.Windows.Forms;

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
            var filesNotFound = new List<CodeFile>();
            foreach (var file in Manager.Files)
            {
                if (!File.Exists(file.FilePath))
                {
                    filesNotFound.Add(file);
                }
                else
                {
                    file.LoadAnnotationsFromContent();
                }
            }

            if (filesNotFound.Any())
            {
                MessageBox.Show(
                    string.Format("The following source code files were referenced by this document, but could not be found on this device:\r\n\r\n{0}", string.Join("\r\n", filesNotFound.Select(x => x.FilePath))),
                    UIUtility.GetAddInName(),
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);   
            }
        }

        void Application_BeforeDoubleClick(Word.Selection selection, ref bool cancel)
        {
            var fields = selection.Fields;
            if (fields.Count > 0)
            {
                // We will only handle the first, if there are multiple
                var field = selection.Fields[1];
                if (Manager.IsAnalysisManagerField(field))
                {
                    var annotation = Manager.GetFieldAnnotation(field);
                    // A field annotation may have a cell index associated with it if it's a table
                    // annotation.  We never want to carry that forward as an annotation parameter
                    // that gets edited, so we explicitly clear it out each time.
                    annotation.CellIndex = null;
                    Manager.EditAnnotation(annotation);
                }
                Marshal.ReleaseComObject(field);
            }

            Marshal.ReleaseComObject(fields);
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
            this.Application.WindowBeforeDoubleClick +=
                new Word.ApplicationEvents4_WindowBeforeDoubleClickEventHandler(Application_BeforeDoubleClick);
        }
        
        #endregion
    }
}
