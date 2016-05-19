using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using StatTag.Core;
using StatTag.Core.Models;
using Microsoft.Office.Interop.Word;

namespace StatTag.Models
{
    /// <summary>
    /// Manages interactions with the Word document, including managing attributes and tags.
    /// </summary>
    public class DocumentManager : BaseManager
    {
        public List<CodeFile> Files { get; set; }
        public FieldCreator FieldManager { get; set; }
        public TagManager TagManager { get; set; }
        public StatsManager StatsManager { get; set; }

        public const string ConfigurationAttribute = "StatTag Configuration";

        public DocumentManager()
        {
            Files = new List<CodeFile>();
            FieldManager = new FieldCreator();
            TagManager = new TagManager(this);
            StatsManager = new StatsManager(this);
        }

        /// <summary>
        /// Provider a wrapper to check if a variable exists in the document.
        /// </summary>
        /// <remarks>Needed because Word interop doesn't provide a nice check mechanism, and uses exceptions instead.</remarks>
        /// <param name="variable">The variable to check</param>
        /// <returns>True if a variable exists and has a value, false otherwise</returns>
        protected bool DocumentVariableExists(Variable variable)
        {
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
        /// Insert an image (given a definition from an tag) into the current Word
        /// document at the current cursor location.
        /// </summary>
        /// <param name="tag"></param>
        public void InsertImage(Tag tag)
        {
            Log("InsertImage - Started");
            if (tag == null)
            {
                Log("The tag is null, no action will be taken");
                return;
            }

            if (tag.CachedResult == null || tag.CachedResult.Count == 0)
            {
                Log("The tag has no cached results - unable to insert image");
                return;
            }

            var application = Globals.ThisAddIn.Application; // Doesn't need to be cleaned up

            string fileName = tag.CachedResult.First().FigureResult;
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
                object linkToFile = true;
                object saveWithDocument = true;
                application.Selection.InlineShapes.AddPicture(fileName, linkToFile, saveWithDocument);
            }

            Log("InsertImage - Finished");
        }

        /// <summary>
        /// Determine if an updated tag pair resulted in a table having different dimensions.  This purely
        /// looks at structure of the table with headers - it does not (currently) factor in data changes.
        /// </summary>
        /// <param name="tagUpdatePair"></param>
        /// <returns></returns>
        /// TODO: Move to utility class and write tests
        private bool IsTableTagChangingDimensions(UpdatePair<Tag> tagUpdatePair)
        {
            if (tagUpdatePair == null || tagUpdatePair.New == null || tagUpdatePair.Old == null)
            {
                return false;
            }

            if (!tagUpdatePair.Old.IsTableTag() || !tagUpdatePair.New.IsTableTag())
            {
                return false;
            }

            // Are we changing the display of headers?
            if (tagUpdatePair.Old.TableFormat.IncludeColumnNames != tagUpdatePair.New.TableFormat.IncludeColumnNames
                || tagUpdatePair.Old.TableFormat.IncludeRowNames != tagUpdatePair.New.TableFormat.IncludeRowNames)
            {
                Log("Table dimensions have changed based on header settings");
                return true;
            }

            return false;
        }

        /// <summary>
        /// For a given Word document, remove all of the field tags for a single table.  This
        /// is in preparation to then re-insert the table in response to a dimension change.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="document"></param>
        private bool RefreshTableTagFields(Tag tag, Document document)
        {
            Log("RefreshTableTagFields - Started");
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

                if (!TagManager.IsStatTagField(field))
                {
                    Marshal.ReleaseComObject(field);
                    continue;
                }

                Log("Processing StatTag field");
                var fieldTag = TagManager.GetFieldTag(field);
                if (fieldTag == null)
                {
                    Log("The field tag is null or could not be found");
                    Marshal.ReleaseComObject(field);
                    continue;
                }

                if (tag.Equals(fieldTag))
                {
                    bool isFirstCell = (fieldTag.TableCellIndex.HasValue &&
                                        fieldTag.TableCellIndex.Value == 0);
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
                        InsertField(tag);
                        tableRefreshed = true;
                    }
                }

                Marshal.ReleaseComObject(field);
            }

            Log(string.Format("RefreshTableTagFields - Finished, Returning {0}", tableRefreshed));
            return tableRefreshed;
        }

        /// <summary>
        /// Processes all inline shapes within the document, which will include our inserted figures.
        /// If the shape can be updated, we will process the update.
        /// </summary>
        /// <param name="document"></param>
        private void UpdateInlineShapes(Document document)
        {
            var shapes = document.InlineShapes;
            if (shapes == null)
            {
                return;
            }

            int shapesCount = shapes.Count;
            for (int shapeIndex = 1; shapeIndex <= shapesCount; shapeIndex++)
            {
                var shape = shapes[shapeIndex];
                if (shape != null)
                {
                    var linkFormat = shape.LinkFormat;
                    if (linkFormat != null)
                    {
                        linkFormat.Update();
                        Marshal.ReleaseComObject(linkFormat);
                    }

                    Marshal.ReleaseComObject(shape);
                }
            }

            Marshal.ReleaseComObject(shapes);
        }

        /// <summary>
        /// Update all of the field values in the current document.
        /// <remarks>This does not invoke a statistical package to recalculate values, it assumes
        /// that has already been done.  Instead it just updates the displayed text of a field
        /// with whatever is set as the current cached value.</remarks>
        /// <param name="tagUpdatePair">An optional tag to update.  If specified, the contents of the tag (including its underlying data) will be refreshed.
        /// The reaason this is an Tag and not a FieldTag is that the function is only called after a change to the main tag reference.
        /// If not specified, all tag fields will be updated</param>
        /// <param name="matchOnPosition">If set to true, an tag will only be matched if its line numbers (in the code file) are a match.  This is used when updating
        /// after disambiguating two tags with the same name, but isn't needed otherwise.</param>
        /// </summary>
        public void UpdateFields(UpdatePair<Tag> tagUpdatePair = null, bool matchOnPosition = false)
        {
            Log("UpdateFields - Started");

            var application = Globals.ThisAddIn.Application; // Doesn't need to be cleaned up
            var document = application.ActiveDocument;

            try
            {
                var tableDimensionChange = IsTableTagChangingDimensions(tagUpdatePair);
                if (tableDimensionChange)
                {
                    Log(string.Format("Attempting to refresh table with tag name: {0}", tagUpdatePair.New.Name));
                    if (RefreshTableTagFields(tagUpdatePair.New, document))
                    {
                        Log("Completed refreshing table - leaving UpdateFields");
                        return;
                    }
                }

                UpdateInlineShapes(document);
                
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

                    if (!TagManager.IsStatTagField(field))
                    {
                        Marshal.ReleaseComObject(field);
                        continue;
                    }

                    Log("Processing StatTag field");
                    var tag = TagManager.GetFieldTag(field);
                    if (tag == null)
                    {
                        Log("The field tag is null or could not be found");
                        Marshal.ReleaseComObject(field);
                        continue;
                    }

                    // If we are asked to update an tag, we are only going to update that
                    // tag specifically.  Otherwise, we will process all tag fields.
                    if (tagUpdatePair != null)
                    {
                        // Determine if this is a match, factoring in if we should be doing a more exact match on the tag.
                        if ((!matchOnPosition && !tag.Equals(tagUpdatePair.Old))
                            || matchOnPosition && !tag.EqualsWithPosition(tagUpdatePair.Old))
                        {
                            continue;
                        }

                        Log(string.Format("Processing only a specific tag with label: {0}", tagUpdatePair.New.Name));
                        tag = new FieldTag(tagUpdatePair.New, tag);
                        TagManager.UpdateTagFieldData(field, tag);
                    }

                    Log(string.Format("Inserting field for tag: {0}", tag.Name));
                    field.Select();
                    InsertField(tag);

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
        /// Insert a table tag into the current selection.
        /// </summary>
        /// <remarks>This assumes that the tag is known to be a table result.</remarks>
        /// <param name="selection"></param>
        /// <param name="tag"></param>
        public void InsertTable(Selection selection, Tag tag)
        {
            Log("InsertTable - Started");

            if (tag == null)
            {
                Log("Unable to insert the table because the tag is null");
                return;
            }

            if (!tag.HasTableData())
            {
                var selectionRange = selection.Range;
                CreateTagField(selectionRange, tag.Id, Constants.Placeholders.EmptyField, tag);
                Log("Unable to insert the table because there are no cached results for the tag");
                return;
            }

            var cells = GetCells(selection);
            tag.UpdateFormattedTableData();
            var table = tag.CachedResult.First().TableResult;

            var dimensions = tag.GetTableDisplayDimensions();

            var cellsCount = cells == null ? 0 : cells.Count;  // Because of the issue we mention below, pull the cell count right away

            // Insert a new table if there is none selected.
            if (cellsCount == 0)
            {
                Log("No cells selected, creating a new table");
                CreateWordTableForTableResult(selection, table, tag.TableFormat);
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

                // Make a copy of the tag and set the cell index.  This will let us discriminate which cell an tag
                // value is related with, since we have multiple fields (and therefore multiple copies of the tag) in the
                // document.  Note that we are wiping out the cached value to just have the individual cell value present.
                var innerTag = new FieldTag(tag, index);
                innerTag.CachedResult = new List<CommandResult>() { new CommandResult() { ValueResult = table.FormattedCells[index] } };
                CreateTagField(range,
                    string.Format("{0}{1}{2}", tag.Name, Constants.ReservedCharacters.TagTableCellDelimiter, index),
                    innerTag.FormattedResult, innerTag);
                index++;
                Marshal.ReleaseComObject(range);
            }

            WarnOnMismatchedCellCount(cellsCount, table.FormattedCells.Length);

            Marshal.ReleaseComObject(cells);

            InsertNewLineAndMoveDown(selection);

            Log("InsertTable - Finished");
        }

        /// <summary>
        /// Helper method to insert a new line in the document at the current selection,
        /// and then move the cursor down.  This gives us a way to insert extra space
        /// after a table is inserted.
        /// </summary>
        /// <param name="selection">The selection to insert the new line after.</param>
        protected void InsertNewLineAndMoveDown(Selection selection)
        {
            selection.Collapse(WdCollapseDirection.wdCollapseEnd);
            var range = selection.Range;
            range.InsertParagraphAfter();
            selection.MoveDown(WdUnits.wdLine, 1);
            Marshal.ReleaseComObject(range);
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
        /// Given an tag, insert the result into the document at the current cursor position.
        /// <remarks>This method assumes the tag result is already refreshed.  It does not
        /// attempt to refresh or recalculate it.</remarks>
        /// </summary>
        /// <param name="tag"></param>

        public void InsertField(Tag tag)
        {
            Log("InsertField for Tag");
            InsertField(new FieldTag(tag));
        }

        /// <summary>
        /// Given an tag, insert the result into the document at the current cursor position.
        /// <remarks>This method assumes the tag result is already refreshed.  It does not
        /// attempt to refresh or recalculate it.</remarks>
        /// </summary>
        /// <param name="tag"></param>
        public void InsertField(FieldTag tag)
        {
            Log("InsertField - Started");

            if (tag == null)
            {
                Log("The tag is null");
                return;
            }

            if (tag.Type == Constants.TagType.Figure)
            {
                Log("Detected a Figure tag");
                InsertImage(tag);
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

                // If the tag is a table, and the cell index is not set, it means we are inserting the entire
                // table into the document.  Otherwise, we are able to just insert a single table cell.
                if (tag.IsTableTag() && !tag.TableCellIndex.HasValue)
                {
                    Log("Inserting a new table tag");
                    InsertTable(selection, tag);
                }
                else
                {
                    Log("Inserting a single tag field");
                    var range = selection.Range;
                    CreateTagField(range, tag.Name, tag.FormattedResult, tag);
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

        /// <summary>
        /// Insert an StatTag field at the currently specified document range.
        /// </summary>
        /// <param name="range">The range to insert the field at</param>
        /// <param name="tagIdentifier">The visible identifier of the tag (does not need to be globablly unique)</param>
        /// <param name="displayValue">The value that should display when the field is shown.</param>
        /// <param name="tag">The tag to be inserted</param>
        protected void CreateTagField(Range range, string tagIdentifier, string displayValue, FieldTag tag)
        {
            Log("CreateTagField - Started");
            var fields = FieldManager.InsertField(range, string.Format("{3}MacroButton {0} {1}{3}ADDIN {2}{4}{4}",
                Constants.FieldDetails.MacroButtonName, displayValue, tagIdentifier, FieldCreator.FieldOpen, FieldCreator.FieldClose));
            Log(string.Format("Inserted field with identifier {0} and display value {1}", tagIdentifier, displayValue));

            var dataField = fields.First();
            dataField.Data = tag.Serialize();
            Log("CreateTagField - Finished");
        }

        /// <summary>
        /// Insert an StatTag field at the currently specified document range.
        /// </summary>
        /// <param name="range">The range to insert the field at</param>
        /// <param name="tagIdentifier">The visible identifier of the tag (does not need to be globablly unique)</param>
        /// <param name="displayValue">The value that should display when the field is shown.</param>
        /// <param name="tag">The tag to be inserted</param>
        protected void CreateTagField(Range range, string tagIdentifier, string displayValue, Tag tag)
        {
            CreateTagField(range, tagIdentifier, displayValue, new FieldTag(tag));
        }

        /// <summary>
        /// Manage the process of editing an tag via a dialog, and processing any
        /// changes within the document.
        /// </summary>
        /// <remarks>This does not call the statistical software to update values.  It assumes that the tag
        /// contains the most up-to-date cached value and that it may be used for display if needed.</remarks>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool EditTag(Tag tag)
        {
            Log("EditTag - Started");

            try
            {
                var dialog = new EditTag(false, this);

                IntPtr hwnd = Process.GetCurrentProcess().MainWindowHandle;
                Log(string.Format("Established main window handle of {0}", hwnd.ToString()));

                dialog.Tag = new Tag(tag);
                var wrapper = new WindowWrapper(hwnd);
                Log(string.Format("WindowWrapper established as: {0}", wrapper.ToString()));
                if (DialogResult.OK == dialog.ShowDialog(wrapper))
                {
                    // If the value format has changed, refresh the values in the document with the
                    // new formatting of the results.
                    // TODO: Sometimes date/time format are null in one and blank strings in the other.  This is causing extra update cycles that aren't needed.
                    if (dialog.Tag.ValueFormat != tag.ValueFormat)
                    {
                        Log("Updating fields after tag value format changed");
                        if (dialog.Tag.TableFormat != tag.TableFormat)
                        {
                            Log("Updating formatted table data");
                            dialog.Tag.UpdateFormattedTableData();
                        }
                        UpdateFields(new UpdatePair<Tag>(tag, dialog.Tag));
                    }
                    else if (dialog.Tag.TableFormat != tag.TableFormat)
                    {
                        Log("Updating fields after tag table format changed");
                        dialog.Tag.UpdateFormattedTableData();
                        UpdateFields(new UpdatePair<Tag>(tag, dialog.Tag));
                    }

                    SaveEditedTag(dialog, tag);
                    Log("EditTag - Finished (action)");
                    return true;
                }
            }
            catch (Exception exc)
            {
                Log("An exception was caught while trying to edit an tag");
                LogException(exc);
            }

            Log("EditTag - Finished (no action)");
            return false;
        }

        /// <summary>
        /// After an tag has been edited in a dialog, handle all reference updates and saving
        /// that tag in its source file.
        /// </summary>
        /// <param name="dialog"></param>
        /// <param name="existingTag"></param>
        public void SaveEditedTag(EditTag dialog, Tag existingTag = null)
        {
            if (dialog.Tag != null && dialog.Tag.CodeFile != null)
            {
                // Update the code file with whatever was in the editor window.  While the code doesn't
                // always change, we will go ahead with the update each time instead of checking.  Note
                // that after this update is done, the indices for the tag objects passed in can 
                // no longer be trusted until we update them.
                var codeFile = dialog.Tag.CodeFile;
                codeFile.UpdateContent(dialog.CodeText);

                // Now that the code file has been updated, we need to add the tag.  This may
                // be a new tag, or an updated one.
                codeFile.AddTag(dialog.Tag, existingTag);
                codeFile.Save();
            }
        }

        /// <summary>
        /// Save all changes to all code files referenced by the current document.
        /// </summary>
        public void SaveAllCodeFiles()
        {
            // Update the code files with their tags
            foreach (var file in Files)
            {
                file.Save();
            }
        }

        public void EditTagField(Field field)
        {
            if (TagManager.IsStatTagField(field))
            {
                var fieldTag = TagManager.GetFieldTag(field);
                var tag = TagManager.FindTag(fieldTag);
                EditTag(tag);
            }
        }

        /// <summary>
        /// This is a specialized utility function to be called whenever the user clicks "Save and Insert"
        /// from the Edit Tag dialog.
        /// </summary>
        /// <param name="dialog"></param>
        public void CheckForInsertSavedTag(EditTag dialog)
        {
            // If the user clicked the "Save and Insert", we will perform the insertion now.
            if (dialog.InsertInDocument)
            {
                Logger.WriteMessage("Inserting into document after defining tag");

                var tag = FindTag(dialog.Tag.Id);
                if (tag == null)
                {
                    Logger.WriteMessage(string.Format("Unable to find tag {0}, so skipping the insert", dialog.Tag.Id));
                    return;
                }

                InsertTagsInDocument(new List<Tag>(new[] { tag }));
            }
        }

        public void InsertTagsInDocument(List<Tag> tags)
        {
            Cursor.Current = Cursors.WaitCursor;
            Globals.ThisAddIn.Application.ScreenUpdating = false;
            try
            {
                var updatedTags = new List<Tag>();
                var refreshedFiles = new List<CodeFile>();
                foreach (var tag in tags)
                {
                    if (!refreshedFiles.Contains(tag.CodeFile))
                    {
                        var result = StatsManager.ExecuteStatPackage(tag.CodeFile,
                            Constants.ParserFilterMode.TagList, tags);
                        if (!result.Success)
                        {
                            break;
                        }

                        updatedTags.AddRange(result.UpdatedTags);
                        refreshedFiles.Add(tag.CodeFile);
                    }

                    InsertField(tag);
                }

                // Now that all of the fields have been inserted, sweep through and update any existing
                // tags that changed.  We do this after the fields are inserted to better manage
                // the cursor position in the document.
                updatedTags = updatedTags.Distinct().ToList();
                foreach (var updatedTag in updatedTags)
                {
                    UpdateFields(new UpdatePair<Tag>(updatedTag, updatedTag));
                }
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when trying to insert the tag output into the Word sdocument.",
                    Logger);
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Conduct an assessment of the active document to see if there are any inserted
        /// tags that do not have an associated code file in the document.
        /// </summary>
        /// <param name="onlyShowDialogIfResultsFound">If true, the results dialog will only display if there is something to report</param>
        public void PerformDocumentCheck(bool onlyShowDialogIfResultsFound = false)
        {
            var unlinkedResults = TagManager.FindAllUnlinkedTags();
            var duplicateResults = TagManager.FindAllDuplicateTags();
            if (onlyShowDialogIfResultsFound 
                && (unlinkedResults == null || unlinkedResults.Count == 0)
                && (duplicateResults == null || duplicateResults.Count == 0))
            {
                return;
            }

            var dialog = new CheckDocument(unlinkedResults, duplicateResults, Files);
            if (DialogResult.OK == dialog.ShowDialog())
            {
                UpdateUnlinkedTagsByTag(dialog.UnlinkedTagUpdates);
                UpdateRenamedTags(dialog.DuplicateTagUpdates);
            }
        }

        #region Wrappers around TagManager calls
        public Dictionary<string, List<Tag>> FindAllUnlinkedTags()
        {
            return TagManager.FindAllUnlinkedTags();
        }

        public List<Tag> GetTags()
        {
            return TagManager.GetTags();
        }

        public Tag FindTag(string id)
        {
            return TagManager.FindTag(id);
        }
        #endregion

        /// <summary>
        /// If code files become unlinked in the document, this method is used to resolve those tags/fields
        /// already in the document that refer to the unlinked code file.  It applies a set of actions to ALL of
        /// the tags in the document for a code file.
        /// </summary>
        /// <remarks>See <see cref="UpdateUnlinkedTagsByTag">UpdateUnlinkedTagsByTag</see>
        /// if you want to perform actions on individual tags.
        /// </remarks>
        /// <param name="actions"></param>
        public void UpdateUnlinkedTagsByCodeFile(Dictionary<string, CodeFileAction> actions)
        {
            TagManager.ProcessStatTagFields(TagManager.UpdateUnlinkedTagsByCodeFile, actions);
        }

        /// <summary>
        /// When reviewing all of the tags/fields in a document for those that have unlinked code files, duplicate
        /// names, etc., this method is used to resolve the errors in those tags/fields.  It applies individual actions
        /// to each tag in the document.
        /// </summary>
        /// <remarks>Some of the actions may in fact affect multiple tags.  For example, re-linking the code file
        /// to the document for a single tag has the effect of re-linking it for all related tags.</remarks>
        /// <remarks>See <see cref="UpdateUnlinkedTagsByCodeFile">UpdateUnlinkedTagsByCodeFile</see>
        /// if you want to process all tags in a code file with a single action.
        /// </remarks>
        /// <param name="actions"></param>
        public void UpdateUnlinkedTagsByTag(Dictionary<string, CodeFileAction> actions)
        {
            TagManager.ProcessStatTagFields(TagManager.UpdateUnlinkedTagsByTag, actions);
        }

        /// <summary>
        /// Add a code file reference to our master list of files in the document.  This should be used when
        /// discovering code files to link to the document.
        /// </summary>
        /// <param name="fileName"></param>
        public void AddCodeFile(string fileName)
        {
            if (Files.Any(x => x.FilePath.Equals(fileName, StringComparison.CurrentCultureIgnoreCase)))
            {
                Log(string.Format("Code file {0} already exists and won't be added again", fileName));
                return;
            }

            string package = CodeFile.GuessStatisticalPackage(fileName);
            var file = new CodeFile { FilePath = fileName, StatisticalPackage = package };
            file.LoadTagsFromContent();
            file.SaveBackup();
            Files.Add(file);
            Log(string.Format("Added code file {0}", fileName));
        }

        private void UpdateRenamedTags(List<UpdatePair<Tag>> updates)
        {
            var affectedCodeFiles = new List<CodeFile>();
            foreach (var update in updates)
            {
                // We assume that updates never affect the code file - we don't give users a way to specify
                // in the UI to change a code file - so we just take the old code file reference to use.
                var codeFile = update.Old.CodeFile;

                if (!affectedCodeFiles.Contains(update.Old.CodeFile))
                {
                    affectedCodeFiles.Add(update.Old.CodeFile);
                }
                UpdateFields(update, true);

                // Add the tag to the code file - replacing the old one.  Note that we require the
                // exact line match, so we don't accidentally replace the wrong duplicate named tag.
                codeFile.AddTag(update.New, update.Old, true);
            }

            foreach (var codeFile in affectedCodeFiles)
            {
                codeFile.Save();
            }
        }
    }
}
