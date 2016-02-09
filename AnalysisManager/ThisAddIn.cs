﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
        public LogManager LogManager = new LogManager();
        public DocumentManager Manager = new DocumentManager();
        public PropertiesManager PropertiesManager = new PropertiesManager();

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // We'll load at Startup but won't save on Shutdown.  We only save when the user makes
            // a change and then confirms it through the Settings dialog.
            PropertiesManager.Load();
            LogManager.UpdateSettings(PropertiesManager.Properties.EnableLogging, PropertiesManager.Properties.LogLocation);
            LogManager.WriteMessage("Startup completed");
            Manager.Logger = LogManager;

            // When you double-click on a document to open it (and Word is close), the DocumentOpen event isn't called.
            // We will process the DocumentOpen event when the add-in is initialized, if there is an active document
            var document = Application.ActiveDocument;
            if (document == null)
            {
                LogManager.WriteMessage("Active document not accessible");
            }
            else
            {
                LogManager.WriteMessage("Active document is " + document.Name);
                Application_DocumentOpen(document);
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            LogManager.WriteMessage("Shutdown completed");
        }

        void Application_DocumentBeforeSave(Word.Document doc, ref bool saveAsUI, ref bool cancel)
        {
            LogManager.WriteMessage("DocumentBeforeSave - preparing to save code files to document");
            Manager.SaveFilesToDocument(doc);
            LogManager.WriteMessage("DocumentBeforeSave - code files saved");
        }

        void Application_DocumentOpen(Word.Document doc)
        {
            LogManager.WriteMessage("DocumentOpen - Started");
            Manager.LoadFilesFromDocument(doc);
            LogManager.WriteMessage(string.Format("Loaded {0} code files from document", Manager.Files.Count));

            var filesNotFound = new List<CodeFile>();
            foreach (var file in Manager.Files)
            {
                if (!File.Exists(file.FilePath))
                {
                    filesNotFound.Add(file);
                    LogManager.WriteMessage(string.Format("Code file: {0} not found", file.FilePath));
                }
                else
                {
                    file.LoadAnnotationsFromContent();
                    LogManager.WriteMessage(string.Format("Code file: {0} found and {1} annotations loaded", file.FilePath, file.Annotations.Count));
                }
            }

            if (filesNotFound.Any())
            {
                MessageBox.Show(
                    string.Format("The following source code files were referenced by this document, but could not be found on this device:\r\n\r\n{0}", string.Join("\r\n", filesNotFound.Select(x => x.FilePath))),
                    UIUtility.GetAddInName(),
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);   
            }

            LogManager.WriteMessage("DocumentOpen - Completed");
        }

        void Application_BeforeDoubleClick(Word.Selection selection, ref bool cancel)
        {
            // Workaround for Word add-in API - there is no AfterDoubleClick event, so we will set a new
            // thread with a timer in it to process the double-click after the message is fully processed.
            var thread = new System.Threading.Thread(Application_AfterDoubleClick);
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        void Application_AfterDoubleClick()
        {
            // There is a UI delay that is noticeable, but is required as a workaround to get the double-click to
            // be processed after the actual selection has changed.
            Thread.Sleep(100);

            var selection = Application.Selection;
            var fields = selection.Fields;

            try
            {
                // We require more than one field, since our AM fields show up as 2 fields.
                if (fields.Count > 1)
                {
                    // If there are multiple items selected, we will grab the first field in the selection.
                    var field = selection.Fields[1];
                    if (field != null)
                    {
                        if (Manager.IsAnalysisManagerField(field))
                        {
                            var fieldAnnotation = Manager.GetFieldAnnotation(field);
                            var annotation = Manager.FindAnnotation(fieldAnnotation);
                            Manager.EditAnnotation(annotation);
                        }
                        Marshal.ReleaseComObject(field);
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("There was an error attempting to load the details of this annotation.  If this problem persists, you may want to remove and insert the annotation again.", UIUtility.GetAddInName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogManager.WriteMessage(exc.Message);
                LogManager.WriteMessage(exc.StackTrace);
            }
            finally
            {
                Marshal.ReleaseComObject(fields);
                Marshal.ReleaseComObject(selection);
            }
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
