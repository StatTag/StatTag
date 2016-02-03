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
        public LogManager Logger { get; set; }

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
            Log("SaveFilesToDocument - Started");

            var variables = document.Variables;
            var variable = variables[ConfigurationAttribute];
            try
            {
                var attribute = CodeFile.SerializeList(Files);
                if (!DocumentVariableExists(variable))
                {
                    Log(string.Format("Document variable does not exist.  Adding attribute value of {0}", attribute));
                    variables.Add(ConfigurationAttribute, attribute);
                }
                else
                {
                    Log(string.Format("Document variable already exists.  Updating attribute value to {0}", attribute));
                    variable.Value = attribute;
                }
            }
            finally
            {
                Marshal.ReleaseComObject(variable);
                Marshal.ReleaseComObject(variables);
            }

            Log("SaveFilesToDocument - Finished");
        }

        /// <summary>
        /// Load the list of associated Code Files from a Word document.
        /// </summary>
        /// <param name="document">The Word document of interest</param>
        public void LoadFilesFromDocument(Document document)
        {
            Log("LoadFilesFromDocument - Started");

            var variables = document.Variables;
            var variable = variables[ConfigurationAttribute];
            try
            {
                if (DocumentVariableExists(variable))
                {
                    Files = CodeFile.DeserializeList(variable.Value);
                    Log(string.Format("Document variable existed, loaded {0} code files", Files.Count));
                }
                else
                {
                    Files = new List<CodeFile>();
                    Log(string.Format("Document variable does not exist, no code files loaded"));
                }
            }
            finally
            {
                Marshal.ReleaseComObject(variable);
                Marshal.ReleaseComObject(variables);
            }

            Log("LoadFilesFromDocument - Finished");
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
        public Annotation FindAnnotation(string annotationLabel)
        {
            return Files.Select(codeFile => codeFile.Annotations.Find(x => x.OutputLabel.Equals(annotationLabel))).FirstOrDefault();
        }

        /// <summary>
        /// Update all of the field values in the current document.
        /// <remarks>This does not invoke a statistical package to recalculate values, it assumes
        /// that has already been done.  Instead it just updates the displayed text of a field
        /// with whatever is set as the current cached value.</remarks>
        /// <param name="annotationUpdatePair">An optional annotation to update.  If specified, the contents of the annotation (including its underlying data) will be refreshed.
        /// The reaason this is an Annotation and not a FieldAnnotation is that the function is only called after a change to the main annotation reference.
        /// If not specified, all annotation fields will be updated</param>
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

                    if (!IsAnalysisManagerField(field))
                    {
                        Marshal.ReleaseComObject(field);
                        continue;
                    }

                    var annotation = GetFieldAnnotation(field);
                    if (annotation == null)
                    {
                        Marshal.ReleaseComObject(field);
                        continue;
                    }

                    // If we are asked to update an annotation, we are only going to update that
                    // annotation specifically.  Otherwise, we will process all annotation fields.
                    if (annotationUpdatePair != null)
                    {
                        if (!annotation.Equals(annotationUpdatePair.Old))
                        {
                            continue;
                        }

                        annotation = new FieldAnnotation(annotationUpdatePair.New, annotation.TableCellIndex);
                        UpdateAnnotationFieldData(field, annotation);
                    }

                    field.Select();
                    InsertField(annotation);

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
        /// Helper function to retrieve Cells from a Selection.  This guards against exceptions
        /// and just returns null when thrown (indicating no Cells found).
        /// </summary>
        /// <param name="selection"></param>
        /// <returns></returns>
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
            table.FormattedCells = annotation.TableFormat.Format(table);

            // TODO: Insert a new table if there is none selected.
            var cellsCount = cells == null ? 0 : cells.Count;  // Because of the issue we mention below, pull the cell count right away
            if (cellsCount == 0)
            {
                UIUtility.WarningMessageBox("Please select the cells in an existing table that you would like to fill in, and then insert the result again.");
                return;
            }

            if (table.FormattedCells == null || table.FormattedCells.Length == 0)
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
                if (index >= table.FormattedCells.Length)
                {
                    break;
                }

                var range = cell.Range;

                // Make a copy of the annotation and set the cell index.  This will let us discriminate which cell an annotation
                // value is related with, since we have multiple fields (and therefore multiple copies of the annotation) in the
                // document.  Note that we are wiping out the cached value to just have the individual cell value present.
                var innerAnnotation = new FieldAnnotation(annotation, index);
                innerAnnotation.CachedResult = new List<CommandResult>() { new CommandResult() { ValueResult = table.FormattedCells[index] } };
                CreateAnnotationField(range,
                    string.Format("{0}{1}{2}", annotation.OutputLabel, Constants.ReservedCharacters.AnnotationTableCellDelimiter, index),
                    innerAnnotation.FormattedResult, innerAnnotation);
                    //data[index], innerAnnotation);
                index++;
                Marshal.ReleaseComObject(range);
            }

            WarnOnMismatchedCellCount(cellsCount, table.FormattedCells.Length);

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

        public void InsertField(Annotation annotation)
        {
            InsertField(new FieldAnnotation(annotation));
        }

        /// <summary>
        /// Given an annotation, insert the result into the document at the current cursor position.
        /// <remarks>This method assumes the annotation result is already refreshed.  It does not
        /// attempt to refresh or recalculate it.</remarks>
        /// </summary>
        /// <param name="annotation"></param>
        public void InsertField(FieldAnnotation annotation)
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

                // If the annotation is a table, and the cell index is not set, it means we are inserting the entire
                // table into the document.  Otherwise, we are able to just insert a single table cell.
                if (annotation.IsTableAnnotation() && !annotation.TableCellIndex.HasValue)
                {
                    InsertTable(selection, annotation);
                }
                else
                {
                    var range = selection.Range;
                    CreateAnnotationField(range, annotation.OutputLabel, annotation.FormattedResult, annotation);
                    Marshal.ReleaseComObject(range);
                }

                #region Nothing to see here
                // Awful little hack... something with the way the InsertField method works returns fields
                // with special characters in the embedded fields.  A workaround is toggling the fields
                // to show and hide codes.
                document.Fields.ToggleShowCodes();
                document.Fields.ToggleShowCodes();
                #endregion

                Marshal.ReleaseComObject(selection);
            }
            finally
            {
                Marshal.ReleaseComObject(document);
            }
        }

        protected void CreateAnnotationField(Range range, string annotationIdentifier, string displayValue, FieldAnnotation annotation)
        {
            var fields = FieldManager.InsertField(range, string.Format("{{{{MacroButton {0} {1}{{{{ADDIN {2}}}}}}}}}",
                MacroButtonName, displayValue, annotationIdentifier));
            var dataField = fields[0];
            dataField.Data = annotation.Serialize();
        }

        protected void CreateAnnotationField(Range range, string annotationIdentifier, string displayValue, Annotation annotation)
        {
            CreateAnnotationField(range, annotationIdentifier, displayValue, new FieldAnnotation(annotation));
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
                            UpdateAnnotationFieldData(field, new FieldAnnotation(annotations.New));
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
        private void UpdateAnnotationFieldData(Field field, FieldAnnotation annotation)
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
        public FieldAnnotation GetFieldAnnotation(Field field)
        {
            var code = field.Code;
            var nestedField = code.Fields[1];
            var fieldAnnotation = FieldAnnotation.Deserialize(nestedField.Data.ToString(CultureInfo.InvariantCulture));
            Marshal.ReleaseComObject(nestedField);
            Marshal.ReleaseComObject(code);

            var annotation = FindAnnotation(fieldAnnotation);

            // The result of FindAnnotation is going to be a document-level annotation, not a
            // cell specific one that exists as a field.  We need to re-set the cell index
            // from the annotation we found, to ensure it's available for later use.
            return new FieldAnnotation(annotation, fieldAnnotation.TableCellIndex);
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
                // TODO: Sometimes date/time format are null in one and blank strings in the other.  This is causing extra update cycles that aren't needed.
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

        /// <summary>
        /// Wrapper around a LogManager instance.  Since logging is not always enabled/available for this object
        /// the wrapper only writes if a logger is accessible.
        /// </summary>
        /// <param name="text"></param>
        protected void Log(string text)
        {
            if (Logger != null)
            {
                Logger.WriteMessage(text);
            }
        }
    }
}
