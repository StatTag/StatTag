using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AnalysisManager.Core.Models;
using Microsoft.Office.Interop.Word;

namespace AnalysisManager.Models
{
    /// <summary>
    /// Manages interactions with the Word document, including managing attributes and annotations.
    /// </summary>
    public class DocumentManager
    {
        public List<CodeFile> Files { get; set; }
        public FieldCreator FieldManager { get; set; }

        public const string ConfigurationAttribute = "Analysis Manager Configuration";
        public const string MacroButtonName = "AnalysisManager";

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

        /// <summary>
        /// Save the referenced code files to the current Word document.
        /// </summary>
        /// <param name="document">The Word document of interest</param>
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

        /// <summary>
        /// Load the list of associated Code Files from a Word document.
        /// </summary>
        /// <param name="document">The Word document of interest</param>
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

        /// <summary>
        /// Insert an image (given a definition from an annotation) into the current Word
        /// document at the current cursor location.
        /// </summary>
        /// <param name="annotation"></param>
        public void InsertImage(Annotation annotation)
        {
            if (annotation == null)
            {
                return;
            }

            var application = Globals.ThisAddIn.Application; // Doesn't need to be cleaned up

            string fileName = annotation.CachedResult[0].FigureResult;
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

        /// <summary>
        /// Finds the first annotation that matches a given label.
        /// </summary>
        /// <param name="annotationLabel"></param>
        /// <returns></returns>
        private Annotation FindAnnotation(string annotationLabel)
        {
            return Files.Select(codeFile => codeFile.Annotations.Find(x => x.OutputLabel.Equals(annotationLabel))).FirstOrDefault();
        }

        /// <summary>
        /// Update all of the field values in the current document.
        /// <remarks>This does not invoke a statistical package to recalculate values, it assumes
        /// that has already been done.  Instead it just updates the displayed text of a field
        /// with whatever is set as the current cached value.</remarks>
        /// <param name="annotationUpdatePair">An optional annotation to update.  If specified, the contents of the annotation (including its underlying data) will be refreshed.  If not specified, all annotation fields will be updated</param>
        /// </summary>
        public void UpdateFields(UpdatePair<Annotation> annotationUpdatePair = null)
        {
            var application = Globals.ThisAddIn.Application; // Doesn't need to be cleaned up
            var document = application.ActiveDocument;

            try
            {
                var fields = document.Fields;
                // Fields is a 1-based index
                for (int index = 1; index <= fields.Count; index++)
                {
                    var field = fields[index];
                    if (field == null)
                    {
                        continue;
                    }

                    if (IsAnalysisManagerField(field))
                    {
                        var annotation = GetFieldAnnotation(field);
                        
                        if (annotation != null)
                        {
                            // If we are asked to update an annotation, we are only going to update that
                            // annotation specifically.  Otherwise, we will process all annotation fields.
                            if (annotationUpdatePair != null)
                            {
                                if (!annotation.Equals(annotationUpdatePair.Old))
                                {
                                    continue;
                                }

                                annotation = annotationUpdatePair.New;
                                UpdateAnnotationFieldData(field, annotation);
                            }

                            field.Select();
                            InsertField(annotation);
                        }
                    }
                    Marshal.ReleaseComObject(field);
                }
                Marshal.ReleaseComObject(fields);
            }
            finally
            {
                Marshal.ReleaseComObject(document);
            }
        }

        public Columns GetSelectedColumns(Selection selection)
        {
            try
            {
                return selection.Columns;
            }
            catch (COMException exc)
            {}

            return null;
        }

        public Rows GetSelectedRows(Selection selection)
        {
            try
            {
                return selection.Rows;
            }
            catch (COMException exc)
            { }

            return null;
        }

        public Cells GetCells(Selection selection)
        {
            try
            {
                return selection.Cells;
            }
            catch (COMException exc)
            {}

            return null;
        }

        /// <summary>
        /// Insert a table annotation into the current selection.
        /// </summary>
        /// <remarks>This assumes that the annotation is known to be a table result.</remarks>
        /// <param name="selection"></param>
        /// <param name="annotation"></param>
        public void InsertTable(Selection selection, Annotation annotation)
        {
            var cells = GetCells(selection);
            var table = annotation.CachedResult[0].TableResult;
            var data = annotation.TableFormat.Format(table);

            // TODO: Insert a new table if there is none selected.
            var cellsCount = cells == null ? 0 : cells.Count;  // Because of the issue we mention below, pull the cell count right away
            if (cellsCount == 0)
            {
                UIUtility.WarningMessageBox("Please select the cells in an existing table that you would like to fill in, and then insert the result again.");
                return;
            }

            if (data == null || data.Length == 0)
            {
                UIUtility.WarningMessageBox("There are no table results to insert.");
                return;
            }

            // Wait, why aren't I using a for (int index = 0...) loop instead of this foreach?
            // There is some weird issue with the Cells collection that was crashing when I used
            // a for loop and index.  After a few iterations it was chopping out a few of the
            // cells, which caused a crash.  No idea why, and moved to this approach in the interest
            // of time.  Long-term it'd be nice to figure out what was causing the crash.
            int index = 0;
            foreach (var cell in cells.OfType<Cell>())
            {
                if (index >= data.Length)
                {
                    break;
                }

                var range = cell.Range;
                range.Text = data[index];
                index++;
                Marshal.ReleaseComObject(range);
            }

            WarnOnMismatchedCellCount(cellsCount, data.Length);

            Marshal.ReleaseComObject(cells);
        }

        /// <summary>
        /// Provide a warning to the user if the number of data cells available doesn't match
        /// the number of table cells they selected in the document.
        /// </summary>
        /// <param name="selectedCellCount"></param>
        /// <param name="dataLength"></param>
        protected void WarnOnMismatchedCellCount(int selectedCellCount, int dataLength)
        {
            if (selectedCellCount > dataLength)
            {
                UIUtility.WarningMessageBox(
                    string.Format("The number of cells you have selected ({0}) is larger than the number of cells in your results ({1}).\r\n\r\nOnly the first {1} cells have been filled in with results.",
                    selectedCellCount, dataLength));
            }
            else if (selectedCellCount < dataLength)
            {
                UIUtility.WarningMessageBox(
                    string.Format("The number of cells you have selected ({0}) is smaller than the number of cells in your results ({1}).\r\n\r\nOnly the first {0} cells from your results have been used.",
                    selectedCellCount, dataLength));
            }
        }

        /// <summary>
        /// Given an annotation, insert the result into the document at the current cursor position.
        /// <remarks>This method assumes the annotation result is already refreshed.  It does not
        /// attempt to refresh or recalculate it.</remarks>
        /// </summary>
        /// <param name="annotation"></param>
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

                if (annotation.Type == Constants.AnnotationType.Table)
                {
                    InsertTable(selection, annotation);
                    return;
                }
                else
                {
                    var range = selection.Range;

                    var fields = FieldManager.InsertField(range, string.Format("{{{{MacroButton {0} {1}{{{{ADDIN {2}}}}}}}}}",
                        MacroButtonName, annotation.FormattedResult, annotation.OutputLabel));
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
                }

                Marshal.ReleaseComObject(selection);
            }
            finally
            {
                Marshal.ReleaseComObject(document);
            }
        }

        /// <summary>
        /// Find the master reference of an annotation, which is contained in the code files
        /// associated with the current document
        /// </summary>
        /// <param name="annotation">The annotation to find</param>
        /// <returns></returns>
        public Annotation FindAnnotation(Annotation annotation)
        {
            return FindAnnotation(annotation.OutputLabel, annotation.Type);
        }

        /// <summary>
        /// Find the master reference of an annotation, which is contained in the code files
        /// associated with the current document
        /// </summary>
        /// <param name="name">The annotation name to search for</param>
        /// <param name="type">The annotation type to search for</param>
        /// <returns></returns>
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

        /// <summary>
        /// For all of the code files associated with the current document, get all of the
        /// annotations as a single list.
        /// </summary>
        /// <returns></returns>
        public List<Annotation> GetAnnotations()
        {
            var annotations = new List<Annotation>();
            Files.ForEach(file => annotations.AddRange(file.Annotations));
            return annotations;
        }

        /// <summary>
        /// Given an old and a new annotation, update all of the Fields in the document to refer
        /// to the new annotation's name (label).
        /// </summary>
        /// <param name="annotations">The pair of old and new annotations</param>
        public void UpdateAnnotationLabel(UpdatePair<Annotation> annotations)
        {
            var application = Globals.ThisAddIn.Application; // Doesn't need to be cleaned up
            var document = application.ActiveDocument;

            try
            {
                var fields = document.Fields;
                // Fields is a 1-based index
                for (int index = 1; index <= fields.Count; index++)
                {
                    var field = fields[index];
                    if (field == null)
                    {
                        continue;
                    }

                    if (IsAnalysisManagerField(field))
                    {
                        var annotation = GetFieldAnnotation(field);
                        if (annotation != null && annotation.Equals(annotations.Old))
                        {
                            UpdateAnnotationFieldData(field, annotations.New);
                        }
                    }
                    Marshal.ReleaseComObject(field);
                }
                Marshal.ReleaseComObject(fields);
            }
            finally
            {
                Marshal.ReleaseComObject(document);
            }
        }

        /// <summary>
        /// Update the annotation data in a field.
        /// </summary>
        /// <remarks>Assumes that the field parameter is known to be an annotation field</remarks>
        /// <param name="field">The field to update.  This is the outermost layer of the annotation field.</param>
        /// <param name="annotation">The annotation to be updated.</param>
        private void UpdateAnnotationFieldData(Field field, Annotation annotation)
        {
            var code = field.Code;
            var nestedField = code.Fields[1];
            nestedField.Data = annotation.Serialize();

            Marshal.ReleaseComObject(nestedField);
            Marshal.ReleaseComObject(code);
        }

        /// <summary>
        /// Given a Word field, determine if it is our specialized Analysis Manager field type given
        /// its composition.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool IsAnalysisManagerField(Field field)
        {
            return (field != null
                && field.Type == WdFieldType.wdFieldMacroButton
                && field.Code != null && field.Code.Text.Contains(MacroButtonName)
                && field.Code.Fields.Count > 0);
        }

        /// <summary>
        /// Given a Word document Field, extracts the embedded Analysis Manager annotation
        /// associated with it.
        /// </summary>
        /// <param name="field">The Word field object to investigate</param>
        /// <returns></returns>
        public Annotation GetFieldAnnotation(Field field)
        {
            var code = field.Code;
            var nestedField = code.Fields[1];
            var fieldAnnotation = Annotation.Deserialize(nestedField.Data.ToString(CultureInfo.InvariantCulture));
            Marshal.ReleaseComObject(nestedField);
            Marshal.ReleaseComObject(code);

            return FindAnnotation(fieldAnnotation);
        }

        /// <summary>
        /// Manage the process of editing an annotation via a dialog, and processing any
        /// changes within the document.
        /// </summary>
        /// <remarks>This does not call the statistical software to update values.  It assumes that the annotation
        /// contains the most up-to-date cached value and that it may be used for display if needed.</remarks>
        /// <param name="annotation"></param>
        /// <returns></returns>
        public bool EditAnnotation(Annotation annotation)
        {
            var dialog = new EditAnnotation(Files);
            dialog.Annotation = new Annotation(annotation);
            if (DialogResult.OK == dialog.ShowDialog())
            {
                // If the value format has changed, refresh the values in the document with the
                // new formatting of the results.
                if (dialog.Annotation.ValueFormat != annotation.ValueFormat)
                {
                    UpdateFields(new UpdatePair<Annotation>(annotation, dialog.Annotation));
                }

                // Perform label changes AFTER any other updates.  This way we don't have to worry about
                // managing name changes.
                bool labelChanged = !annotation.OutputLabel.Equals(dialog.Annotation.OutputLabel);
                if (labelChanged)
                {
                    UpdateAnnotationLabel(new UpdatePair<Annotation>(annotation, dialog.Annotation));
                }

                SaveEditedAnnotation(dialog, annotation);
                return true;
            }

            return false;
        }

        /// <summary>
        /// After an annotation has been edited in a dialog, handle all reference updates and saving
        /// that annotation in its source file.
        /// </summary>
        /// <param name="dialog"></param>
        /// <param name="existingAnnotation"></param>
        public void SaveEditedAnnotation(EditAnnotation dialog, Annotation existingAnnotation = null)
        {
            if (dialog.Annotation != null && dialog.Annotation.CodeFile != null)
            {
                var codeFile = dialog.Annotation.CodeFile;
                dialog.Annotation.CodeFile.AddAnnotation(dialog.Annotation, existingAnnotation);
                codeFile.Save();
            }
        }

        /// <summary>
        /// Save all changes to all code files referenced by the current document.
        /// </summary>
        public void SaveAllCodeFiles()
        {
            // Update the code files with their annotations
            foreach (var file in Files)
            {
                file.Save();
            }
        }
    }
}
