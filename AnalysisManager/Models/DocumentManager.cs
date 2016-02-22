using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Log("InsertImage - Started");
            if (annotation == null)
            {
                Log("The annotation is null, no action will be taken");
                return;
            }

            var application = Globals.ThisAddIn.Application; // Doesn't need to be cleaned up

            string fileName = annotation.CachedResult[0].FigureResult;
            if (fileName.EndsWith(".pdf", StringComparison.CurrentCultureIgnoreCase))
            {
                Log(string.Format("Inserting a PDF image - {0}", fileName));
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
                Log(string.Format("Inserting a non-PDF image - {0}", fileName));
                application.Selection.InlineShapes.AddPicture(fileName);
            }

            Log("InsertImage - Finished");
        }

        /// <summary>
        /// Determine if an updated annotation pair resulted in a table having different dimensions.  This purely
        /// looks at structure of the table with headers - it does not (currently) factor in data changes.
        /// </summary>
        /// <param name="annotationUpdatePair"></param>
        /// <returns></returns>
        private bool IsTableAnnotationChangingDimensions(UpdatePair<Annotation> annotationUpdatePair)
        {
            if (annotationUpdatePair == null || annotationUpdatePair.New == null || annotationUpdatePair.Old == null)
            {
                return false;
            }

            if (!annotationUpdatePair.Old.IsTableAnnotation() || !annotationUpdatePair.New.IsTableAnnotation())
            {
                return false;
            }

            // Are we changing the display of headers?
            if (annotationUpdatePair.Old.TableFormat.IncludeColumnNames != annotationUpdatePair.New.TableFormat.IncludeColumnNames
                || annotationUpdatePair.Old.TableFormat.IncludeRowNames != annotationUpdatePair.New.TableFormat.IncludeRowNames)
            {
                Log("Table dimensions have changed based on header settings");
                return true;
            }

            return false;
        }

        /// <summary>
        /// For a given Word document, remove all of the field annotations for a single table.  This
        /// is in preparation to then re-insert the table in response to a dimension change.
        /// </summary>
        /// <param name="annotation"></param>
        /// <param name="document"></param>
        private bool RefreshTableAnnotationFields(Annotation annotation, Document document)
        {
            Log("RefreshTableAnnotationFields - Started");
            var fields = document.Fields;
            int fieldsCount = fields.Count;
            bool tableRefreshed = false;

            // Fields is a 1-based index
            Log(string.Format("Preparing to process {0} fields", fieldsCount));
            for (int index = fieldsCount; index >= 1; index--)
            {
                var field = fields[index];
                if (field == null)
                {
                    Log(string.Format("Null field detected at index", index));
                    continue;
                }

                if (!IsAnalysisManagerField(field))
                {
                    Marshal.ReleaseComObject(field);
                    continue;
                }

                Log("Processing Analysis Manager field");
                var fieldAnnotation = GetFieldAnnotation(field);
                if (fieldAnnotation == null)
                {
                    Log("The field annotation is null or could not be found");
                    Marshal.ReleaseComObject(field);
                    continue;
                }

                if (annotation.Equals(fieldAnnotation))
                {
                    bool isFirstCell = (fieldAnnotation.TableCellIndex.HasValue &&
                                        fieldAnnotation.TableCellIndex.Value == 0);
                    int firstFieldLocation = -1;
                    if (isFirstCell)
                    {
                        field.Select();
                        var selection = document.Application.Selection;
                        firstFieldLocation = selection.Range.Start;
                        Marshal.ReleaseComObject(selection);

                        Log(string.Format("First table cell found at position {0}", firstFieldLocation));
                    }
                    
                    field.Delete();

                    if (isFirstCell)
                    {
                        document.Application.Selection.Start = firstFieldLocation;
                        document.Application.Selection.End = firstFieldLocation;
                        Log("Set position, attempting to insert table");
                        InsertField(annotation);
                        tableRefreshed = true;
                    }
                }

                Marshal.ReleaseComObject(field);
            }

            Log(string.Format("RefreshTableAnnotationFields - Finished, Returning {0}", tableRefreshed));
            return tableRefreshed;
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
            Log("UpdateFields - Started");

            var application = Globals.ThisAddIn.Application; // Doesn't need to be cleaned up
            var document = application.ActiveDocument;

            try
            {
                var tableDimensionChange = IsTableAnnotationChangingDimensions(annotationUpdatePair);
                if (tableDimensionChange)
                {
                    Log(string.Format("Attempting to refresh table with annotation label: {0}", annotationUpdatePair.New.OutputLabel));
                    if (RefreshTableAnnotationFields(annotationUpdatePair.New, document))
                    {
                        Log("Completed refreshing table - leaving UpdateFields");
                        return;
                    }
                }

                var fields = document.Fields;
                int fieldsCount = fields.Count;
                // Fields is a 1-based index
                Log(string.Format("Preparing to process {0} fields", fieldsCount));
                for (int index = 1; index <= fieldsCount; index++)
                {
                    var field = fields[index];
                    if (field == null)
                    {
                        Log(string.Format("Null field detected at index", index));
                        continue;
                    }

                    if (!IsAnalysisManagerField(field))
                    {
                        Marshal.ReleaseComObject(field);
                        continue;
                    }

                    Log("Processing Analysis Manager field");
                    var annotation = GetFieldAnnotation(field);
                    if (annotation == null)
                    {
                        Log("The field annotation is null or could not be found");
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

                        Log(string.Format("Processing only a specific annotation with label: {0}", annotationUpdatePair.New.OutputLabel));
                        annotation = new FieldAnnotation(annotationUpdatePair.New, annotation.TableCellIndex);
                        UpdateAnnotationFieldData(field, annotation);
                    }

                    Log(string.Format("Inserting field for annotation: {0}", annotation.OutputLabel));
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

            Log("UpdateFields - Finished");
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
        /// Utility method that assumes the cursor is in a single cell of an existing table.  It then finds the maximum number
        /// of cells that it can fill in that fit within the dimensions of that table, and that use the available data for
        /// the resulting table.
        /// </summary>
        /// <param name="selectedCell"></param>
        /// <param name="table"></param>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        private Cells SelectExistingTableRange(Cell selectedCell, Microsoft.Office.Interop.Word.Table table, int[] dimensions)
        {
            var columns = table.Columns;
            var rows = table.Rows;
            int endColumn = Math.Min(columns.Count, selectedCell.ColumnIndex + dimensions[Constants.DimensionIndex.Columns] - 1);
            int endRow = Math.Min(rows.Count, selectedCell.RowIndex + dimensions[Constants.DimensionIndex.Rows] - 1);

            Log(string.Format("Selecting in existing to row {0}, column {1}", endRow, endColumn));
            Log(string.Format("Selected table has {0} rows and {1} columns", rows.Count, columns.Count));
            Log(string.Format("Table to insert has dimensions {0} by {1}", dimensions[0], dimensions[1]));

            var endCell = table.Cell(endRow, endColumn);

            var application = Globals.ThisAddIn.Application; // Doesn't need to be cleaned up
            var document = application.ActiveDocument;
            document.Range(selectedCell.Range.Start, endCell.Range.End).Select();

            var cells = GetCells(application.Selection);

            Marshal.ReleaseComObject(endCell);
            Marshal.ReleaseComObject(columns);
            Marshal.ReleaseComObject(rows);
            Marshal.ReleaseComObject(document);
            return cells;
        }

        /// <summary>
        /// Insert a table annotation into the current selection.
        /// </summary>
        /// <remarks>This assumes that the annotation is known to be a table result.</remarks>
        /// <param name="selection"></param>
        /// <param name="annotation"></param>
        public void InsertTable(Selection selection, Annotation annotation)
        {
            Log("InsertTable - Started");

            if (annotation == null)
            {
                Log("Unable to insert the table because the annotation is null");
                return;
            }

            if (!annotation.HasTableData())
            {
                var selectionRange = selection.Range;
                CreateAnnotationField(selectionRange, annotation.Id, Constants.Placeholders.EmptyField, annotation);
                Log("Unable to insert the table because there are no cached results for the annotation");
                return;
            }

            var cells = GetCells(selection);
            annotation.UpdateFormattedTableData();
            var table = annotation.CachedResult[0].TableResult;

            var dimensions = annotation.GetTableDisplayDimensions();

            var cellsCount = cells == null ? 0 : cells.Count;  // Because of the issue we mention below, pull the cell count right away

            // Insert a new table if there is none selected.
            if (cellsCount == 0)
            {
                Log("No cells selected, creating a new table");
                CreateWordTableForTableResult(selection, table, annotation.TableFormat);
                // The table will be the size we need.  Update these tracking variables with the cells and
                // total size so that we can begin inserting data.
                cells = GetCells(selection);
                cellsCount = table.FormattedCells.Length;
            }
            // Our heuristic is that a single cell selected with the selection being the same position most
            // likely means the user has their cursor in a table.  We are going to assume they want us to
            // fill in that table.
            else if (cellsCount == 1 && selection.Start == selection.End)
            {
                Log("Cursor is in a single table cell, selecting table");
                cells = SelectExistingTableRange(cells.OfType<Cell>().First(), selection.Tables[1], dimensions);
                cellsCount = cells.Count;
            }

            if (table.FormattedCells == null || table.FormattedCells.Length == 0)
            {
                UIUtility.WarningMessageBox("There are no table results to insert.", Logger);
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
                    Log(string.Format("Index {0} is beyond result cell length of {1}", index, table.FormattedCells.Length));
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
                index++;
                Marshal.ReleaseComObject(range);
            }

            WarnOnMismatchedCellCount(cellsCount, table.FormattedCells.Length);

            Marshal.ReleaseComObject(cells);

            Log("InsertTable - Finished");
        }

        /// <summary>
        /// Create a new table in the Word document at the current selection point.  This assumes we have a
        /// statistical result containing a table that needs to be inserted.
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="table"></param>
        /// <param name="format"></param>
        public void CreateWordTableForTableResult(Selection selection, Core.Models.Table table, TableFormat format)
        {
            Log("CreateWordTableForTableResult - Started");

            var application = Globals.ThisAddIn.Application; // Doesn't need to be cleaned up
            var document = application.ActiveDocument;
            try
            {
                int rowCount = (format.IncludeColumnNames) ? (table.RowSize + 1) : (table.RowSize);
                int columnCount = (format.IncludeRowNames) ? (table.ColumnSize + 1) : (table.ColumnSize);

                Log(string.Format("Table dimensions r={0}, c={1}", rowCount, columnCount));

                var wordTable = document.Tables.Add(selection.Range, rowCount, columnCount);
                wordTable.Select();
                var borders = wordTable.Borders;
                borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                Marshal.ReleaseComObject(borders);
                Marshal.ReleaseComObject(wordTable);
            }
            finally
            {
                Marshal.ReleaseComObject(document);
            }

            Log("CreateWordTableForTableResult - Finished");
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
                    selectedCellCount, dataLength), Logger);
            }
            else if (selectedCellCount < dataLength)
            {
                UIUtility.WarningMessageBox(
                    string.Format("The number of cells you have selected ({0}) is smaller than the number of cells in your results ({1}).\r\n\r\nOnly the first {0} cells from your results have been used.",
                    selectedCellCount, dataLength), Logger);
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
            Log("InsertField for Annotation");
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
            Log("InsertField - Started");

            if (annotation == null)
            {
                Log("The annotation is null");
                return;
            }

            if (annotation.Type == Constants.AnnotationType.Figure)
            {
                Log("Detected a Figure annotation");
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
                    Log("There is no active selection");
                    return;
                }

                // If the annotation is a table, and the cell index is not set, it means we are inserting the entire
                // table into the document.  Otherwise, we are able to just insert a single table cell.
                if (annotation.IsTableAnnotation() && !annotation.TableCellIndex.HasValue)
                {
                    Log("Inserting a new table annotation");
                    InsertTable(selection, annotation);
                }
                else
                {
                    Log("Inserting a single annotation field");
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

            Log("InsertField - Finished");
        }

        protected void CreateAnnotationField(Range range, string annotationIdentifier, string displayValue, FieldAnnotation annotation)
        {
            Log("CreateAnnotationField - Started");
            var fields = FieldManager.InsertField(range, string.Format("{3}MacroButton {0} {1}{3}ADDIN {2}{4}{4}",
                MacroButtonName, displayValue, annotationIdentifier, FieldCreator.FieldOpen, FieldCreator.FieldClose));
            Log(string.Format("Inserted field with identifier {0} and display value {1}", annotationIdentifier, displayValue));

            var dataField = fields[0];
            dataField.Data = annotation.Serialize();
            Log("CreateAnnotationField - Finished");
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
            return FindAnnotation(annotation.Id);
        }

        /// <summary>
        /// Find the master reference of an annotation, which is contained in the code files
        /// associated with the current document
        /// </summary>
        /// <param name="id">The annotation identifier to search for</param>
        /// <returns></returns>
        public Annotation FindAnnotation(string id)
        {
            if (Files == null)
            {
                Log("Unable to find an annotation because the Files collection is null");
                return null;
            }

            return Files.SelectMany(file => file.Annotations).FirstOrDefault(annotation => annotation.Id.Equals(id));
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
        /// Deserialize a field to extract the FieldAnnotation data
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public FieldAnnotation DeserializeFieldAnnotation(Field field)
        {
            var code = field.Code;
            var nestedField = code.Fields[1];
            var fieldAnnotation = FieldAnnotation.Deserialize(nestedField.Data.ToString(CultureInfo.InvariantCulture));
            Marshal.ReleaseComObject(nestedField);
            Marshal.ReleaseComObject(code);

            return fieldAnnotation;
        }

        /// <summary>
        /// Given a Word document Field, extracts the embedded Analysis Manager annotation
        /// associated with it.
        /// </summary>
        /// <param name="field">The Word field object to investigate</param>
        /// <returns></returns>
        public FieldAnnotation GetFieldAnnotation(Field field)
        {
            var fieldAnnotation = DeserializeFieldAnnotation(field);
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
            Log("EditAnnotation - Started");

            var dialog = new EditAnnotation(this);
            IntPtr hwnd = Process.GetCurrentProcess().MainWindowHandle;

            dialog.Annotation = new Annotation(annotation);
            if (DialogResult.OK == dialog.ShowDialog(new WindowWrapper(hwnd)))
            {
                // If the value format has changed, refresh the values in the document with the
                // new formatting of the results.
                // TODO: Sometimes date/time format are null in one and blank strings in the other.  This is causing extra update cycles that aren't needed.
                if (dialog.Annotation.ValueFormat != annotation.ValueFormat)
                {
                    Log("Updating fields after annotation value format changed");
                    if (dialog.Annotation.TableFormat != annotation.TableFormat)
                    {
                        Log("Updating formatted table data");
                        dialog.Annotation.UpdateFormattedTableData();
                    }
                    UpdateFields(new UpdatePair<Annotation>(annotation, dialog.Annotation));
                }
                else if (dialog.Annotation.TableFormat != annotation.TableFormat)
                {
                    Log("Updating fields after annotation table format changed");
                    dialog.Annotation.UpdateFormattedTableData();
                    UpdateFields(new UpdatePair<Annotation>(annotation, dialog.Annotation));
                }

                SaveEditedAnnotation(dialog, annotation);
                Log("EditAnnotation - Finished (action)");
                return true;
            }

            Log("EditAnnotation - Finished (no action)");
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
