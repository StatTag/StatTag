using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using StatTag.Core.Models;
using StatTag.Models;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using System.Windows.Forms;

namespace StatTag
{
    public partial class ThisAddIn
    {
        public static event EventHandler<EventArgs> AfterDoubleClickErrorCallback;

        public LogManager LogManager = new LogManager();
        public DocumentManager DocumentManager = new DocumentManager();
        public PropertiesManager PropertiesManager = new PropertiesManager();
        public StatsManager StatsManager = null;

        /// <summary>
        /// Perform a safe get of the active document.  There is no other way to safely
        /// check for the active document, since if it is not set it throws an exception.
        /// </summary>
        /// <returns></returns>
        public Word.Document SafeGetActiveDocument()
        {
            try
            {
                return Application.ActiveDocument;
            }
            catch (Exception exc)
            {
                LogManager.WriteMessage("Getting ActiveDocument threw an exception");
            }

            return null;
        }

        /// <summary>
        /// Called when the add-in is started up.  This performs basic initialization and one-time setup for running
        /// in a Word session.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            StatsManager = new StatsManager(DocumentManager);

            // We'll load at Startup but won't save on Shutdown.  We only save when the user makes
            // a change and then confirms it through the Settings dialog.
            PropertiesManager.Load();
            LogManager.UpdateSettings(PropertiesManager.Properties.EnableLogging, PropertiesManager.Properties.LogLocation,
                PropertiesManager.Properties.MaxLogFileSize, PropertiesManager.Properties.MaxLogFiles);
            LogManager.WriteMessage(GetUserEnvironmentDetails());
            LogManager.WriteMessage("Startup completed");
            DocumentManager.Logger = LogManager;
            DocumentManager.TagManager.Logger = LogManager;
            AfterDoubleClickErrorCallback += OnAfterDoubleClickErrorCallback;

            try
            {
                // When you double-click on a document to open it (and Word is close), the DocumentOpen event isn't called.
                // We will process the DocumentOpen event when the add-in is initialized, if there is an active document
                var document = SafeGetActiveDocument();
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
            catch (Exception exc)
            {
                UIUtility.ReportException(exc, "There was an unexpected error when trying to initialize StatTag.  Not all functionality may be available.", LogManager);
            }
        }
        
        /// <summary>
        /// Called when the add-in is being unloaded and shut down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            LogManager.WriteMessage("Shutdown completed");
        }

        /// <summary>
        /// Perform some customization to document metadata before the document is actually saved.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="saveAsUI"></param>
        /// <param name="cancel"></param>
        void Application_DocumentBeforeSave(Word.Document doc, ref bool saveAsUI, ref bool cancel)
        {
            LogManager.WriteMessage("DocumentBeforeSave - preparing to save document properties");

            try
            {
                DocumentManager.SaveMetadataToDocument(doc);
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc, "There was an error while trying to save the document.  Your StatTag data may not be saved.", LogManager);
            }

            LogManager.WriteMessage("DocumentBeforeSave - properties saved");
        }

        void Application_NewDocument(Word.Document doc)
        {
            LogManager.WriteMessage("NewDocument - Started");
        }

        // Hande initailization when a document is opened.  This may be called multiple times in a single Word session.
        void Application_DocumentOpen(Word.Document doc)
        {
            LogManager.WriteMessage("DocumentOpen - Started");
            DocumentManager.LoadMetadataFromDocument(doc);
            var files = DocumentManager.GetCodeFileList(doc);
            LogManager.WriteMessage(string.Format("Loaded {0} code files from document", files.Count));

            var filesNotFound = new List<CodeFile>();
            foreach (var file in files)
            {
                if (!File.Exists(file.FilePath))
                {
                    filesNotFound.Add(file);
                    LogManager.WriteMessage(string.Format("Code file: {0} not found", file.FilePath));
                }
                else
                {
                    file.LoadTagsFromContent(false);  // Skip saving the cache, since this is the first load

                    if (PropertiesManager.Properties.RunCodeOnOpen)
                    {
                        try
                        {
                            Globals.ThisAddIn.Application.ScreenUpdating = false;
                            LogManager.WriteMessage(string.Format("Code file: {0} found and {1} tags loaded",
                                file.FilePath, file.Tags.Count));
                            var results = StatsManager.ExecuteStatPackage(file);
                            LogManager.WriteMessage(
                                string.Format("Executed the statistical code for file, with success = {0}",
                                    results.Success));
                        }
                        finally
                        {
                            Globals.ThisAddIn.Application.ScreenUpdating = true;
                        }
                    }
                    else
                    {
                        LogManager.WriteMessage(string.Format("Per user preferences, skipping auto-run of {0}", file.FilePath));
                    }
                }
            }

            if (filesNotFound.Any())
            {
                MessageBox.Show(
                    string.Format(
                        "The following source code files were referenced by this document, but could not be found on this device:\r\n\r\n{0}",
                        string.Join("\r\n", filesNotFound.Select(x => x.FilePath))),
                    UIUtility.GetAddInName(),
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                LogManager.WriteMessage("Performing the document validation check");
                DocumentManager.PerformDocumentCheck(doc, true);
            }

            LogManager.WriteMessage("DocumentOpen - Completed");
        }

        /// <summary>
        /// Respond to an error after double-clicking on a field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnAfterDoubleClickErrorCallback(object sender, EventArgs eventArgs)
        {
            var exception = sender as Exception;
            if (exception != null)
            {
                UIUtility.ReportException(exception,
                    "There was an error attempting to load the details of this tag.  If this problem persists, you may want to remove and insert the tag again.",
                    LogManager);
            }
        }

        /// <summary>
        /// The official event handler for add-ins in response to double-click.
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="cancel"></param>
        void Application_BeforeDoubleClick(Word.Selection selection, ref bool cancel)
        {
            // Workaround for Word add-in API - there is no AfterDoubleClick event, so we will set a new
            // thread with a timer in it to process the double-click after the message is fully processed.
            var thread = new System.Threading.Thread(Application_AfterDoubleClick) {IsBackground = true};
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private Word.ShapeRange SafeGetShapeRange(Word.Selection selection)
        {
            Word.ShapeRange shape = null;
            try
            {
                shape = selection.ShapeRange;
            }
            catch
            {
                // This is a safe wrapper function, so we are eating the exception
                shape = null;
            }

            return shape;
        }

        private Word.Fields SafeGetFields(Word.Selection selection)
        {
            Word.Fields fields = null;
            try
            {
                fields = selection.Fields;
            }
            catch
            {
                // This is a safe wrapper function, so we are eating the exception
                fields = null;
            }

            return fields;
        }

        /// <summary>
        /// Special handler called by us to allow the event queue to be processed before we try responding to
        /// a double-click message.  This allows us to simulate an AfterDoubleClick event.
        /// </summary>
        void Application_AfterDoubleClick()
        {
            // There is a UI delay that is noticeable, but is required as a workaround to get the double-click to
            // be processed after the actual selection has changed.
            Thread.Sleep(100);

            var selection = Application.Selection;
            var fields = SafeGetFields(selection);
            var shape = SafeGetShapeRange(selection);

            try
            {
                // We require more than one field, since our AM fields show up as 2 fields.
                if (fields.Count > 1)
                {
                    // If there are multiple items selected, we will grab the first field in the selection.
                    var field = selection.Fields[1];
                    if (field != null)
                    {
                        DocumentManager.EditTagField(field);
                        Marshal.ReleaseComObject(field);
                    }
                }
                else if (shape != null && shape.Count > 0)
                {
                    DocumentManager.EditTagShape(shape[1]);
                }
            }
            catch (Exception exc)
            {
                // This may also seem kludgy to use a callback, but we want to display the dialog in
                // the main UI thread.  So we use the callback to transition control back over there
                // when an error is detected.
                if (AfterDoubleClickErrorCallback != null)
                {
                    AfterDoubleClickErrorCallback(exc, EventArgs.Empty);
                }
            }
            finally
            {
                if (fields != null)
                {
                    Marshal.ReleaseComObject(fields);
                }
                if (shape != null)
                {
                    Marshal.ReleaseComObject(shape);
                }
                Marshal.ReleaseComObject(selection);
            }
        }

        void Application_WindowActivate(Word.Document doc, Word.Window window)
        {
            Globals.Ribbons.MainRibbon.UIStatusAfterFileLoad();
        }

        /// <summary>
        /// Produce a logging string that contains information about the user's environment (StatTag, Word and OS)
        /// </summary>
        /// <returns></returns>
        public string GetUserEnvironmentDetails()
        {
            string value = String.Empty;
            try
            {
                var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                value = string.Format(
                    "\r\n***********************\r\n* {0}\r\n*    Full name: {1}\r\n*    Code base: {2}\r\n*    CLR: {3}\r\n* MS Word {4}\r\n*    Build: {5}\r\n*    Is sandboxed: {6}\r\n*    Path: {7}\r\n* Process environment\r\n*    Is 64-bit: {8}\r\n*    Current directory: {9}\r\n* OS\r\n*    Version: {10}\r\n*    Is 64-bit: {11}\r\n***********************",
                    UIUtility.GetVersionLabel(), executingAssembly.FullName, executingAssembly.CodeBase,
                    executingAssembly.ImageRuntimeVersion,
                    Application.Version, Application.Build, Application.IsSandboxed, Application.Path,
                    Environment.Is64BitProcess, Environment.CurrentDirectory,
                    Environment.OSVersion.ToString(), Environment.Is64BitOperatingSystem);
            }
            catch (Exception exc)
            {
                value = string.Format("Error in GetUserEnvironment: {0}", exc.Message);
            }
            return value;
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
            // Have to cast the application object to avoid ambiguity on the reference to NewDocument
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/34c6abe2-2544-4f47-aff7-74ec5e08b814/ambiguity-between-microsoftofficeinteropwordapplicationnewdocument-and?forum=vsto
            ((Microsoft.Office.Interop.Word.ApplicationEvents4_Event)this.Application).NewDocument += new Word.ApplicationEvents4_NewDocumentEventHandler(Application_NewDocument);
            this.Application.WindowActivate += new Word.ApplicationEvents4_WindowActivateEventHandler(Application_WindowActivate);
        }

        #endregion
    }
}
