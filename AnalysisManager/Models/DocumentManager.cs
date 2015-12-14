using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using AnalysisManager.Core.Models;
using Microsoft.Office.Interop.Word;

namespace AnalysisManager.Models
{
    public class DocumentManager
    {
        public List<CodeFile> Files { get; set; }
        public FieldCreator FieldManager { get; set; }

        public const string ConfigurationAttribute = "Analysis Manager Configuration";

        public DocumentManager()
        {
            Files = new List<CodeFile>();
            FieldManager = new FieldCreator();
        }

        /// <summary>
        /// Provider a wrapper to check if a variable exists in the document.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        protected bool DocumentVariableExists(Variable variable)
        {
            // The Word interop doesn't provide a nice check mechanism, and uses exceptions instead.
            try
            {

                var value = variable.Value;
                return true;
            }
            catch (COMException exc)
            {
                return false;
            }
        }

        public void SaveFilesToDocument(Document document)
        {
            var variables = document.Variables;
            var variable = variables[ConfigurationAttribute];
            try
            {
                var attribute = CodeFile.SerializeList(Files);
                if (!DocumentVariableExists(variable))
                {
                    variables.Add(ConfigurationAttribute, attribute);
                }
                else
                {
                    variable.Value = attribute;
                }
            }
            finally
            {
                Marshal.ReleaseComObject(variable);
                Marshal.ReleaseComObject(variables);
            }
        }

        public void LoadFilesFromDocument(Document document)
        {
            var variables = document.Variables;
            var variable = variables[ConfigurationAttribute];
            try
            {
                Files = DocumentVariableExists(variable) ? CodeFile.DeserializeList(variable.Value) : new List<CodeFile>();
            }
            finally
            {
                Marshal.ReleaseComObject(variable);
                Marshal.ReleaseComObject(variables);
            }
        }

        public void InsertImage(Annotation annotation)
        {
            if (annotation == null)
            {
                return;
            }

            var application = Globals.ThisAddIn.Application; // Doesn't need to be cleaned up

            string fileName = annotation.CachedResult[0];
            if (fileName.EndsWith(".pdf", StringComparison.CurrentCultureIgnoreCase))
            {
                object fileNameObject = fileName;
                object classType = "AcroExch.Document.DC";
                object oFalse = false;
                object oTrue = true;
                object missing = System.Reflection.Missing.Value;
                // For PDFs, we can ONLY insert them as a link to the file.  Trying it any other way will cause
                // an error during the insert.
                application.Selection.InlineShapes.AddOLEObject(ref classType, ref fileNameObject, 
                    ref oTrue, ref oFalse, ref missing, ref missing, ref missing, ref missing);
            }
            else
            {
                application.Selection.InlineShapes.AddPicture(fileName);
            }
        }

        public void InsertField(Annotation annotation)
        {
            if (annotation == null)
            {
                return;
            }

            if (annotation.Type == Constants.AnnotationType.Figure)
            {
                InsertImage(annotation);
                return;
            }

            var application = Globals.ThisAddIn.Application; // Doesn't need to be cleaned up
            var document = application.ActiveDocument;
            try
            {
                var selection = application.Selection;
                if (selection == null)
                {
                    return;
                }

                var range = selection.Range;

                var fields = FieldManager.InsertField(range, string.Format("{{{{MacroButton AnalysisManager {0}{{{{ADDIN {1}}}}}}}}}",
                    annotation.FormattedResult, annotation.OutputLabel));
                var dataField = fields[0];
                dataField.Data = annotation.Serialize();

                #region Nothing to see here
                // Awful little hack... something with the way the InsertField method works returns fields
                // with special characters in the embedded fields.  A workaround is toggling the fields
                // to show and hide codes.
                document.Fields.ToggleShowCodes();
                document.Fields.ToggleShowCodes();
                #endregion

                Marshal.ReleaseComObject(range);
                Marshal.ReleaseComObject(selection);
            }
            finally
            {
                Marshal.ReleaseComObject(document);
            }
        }

        public Annotation FindAnnotation(string name, string type)
        {
            if (Files == null)
            {
                return null;
            }

            foreach (var file in Files)
            {
                foreach (var annotation in file.Annotations)
                {
                    if (annotation.OutputLabel.Equals(name) && annotation.Type.Equals(type))
                    {
                        return annotation;
                    }
                }
            }

            return null;
        }
    }
}
