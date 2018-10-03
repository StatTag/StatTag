using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Office.Core;
using StatTag.Core;
using StatTag.Core.Exceptions;
using StatTag.Core.Generator;
using StatTag.Core.Models;
using Microsoft.Office.Interop.Word;
using StatTag.Core.Utility;

namespace StatTag.Models
{
    /// <summary>
    /// Manages interactions with the Word document, including managing attributes and tags.
    /// </summary>
    public sealed class DocumentManager : BaseManager, IDisposable
    {
        /// <summary>
        /// Sent whenever the contents of a single code file have changed.
        /// </summary>
        public event EventHandler CodeFileContentsChanged;

        /// <summary>
        /// Sent whenever the number of tags or the content of one or more tags changes.  This will alert
        /// the listener that they should refresh any and all lists of tags or displays of a tag.
        /// </summary>
        public event EventHandler TagListChanged;

        /// <summary>
        /// Sent whenever the number of code files changes.  This will alert the listener that they should
        /// refresh any and all lists of code files, as well as associated tag lists.  The listener will want
        /// to perform the same actions for TagListChanged events when this is received.
        /// </summary>
        public event EventHandler CodeFileListChanged;

        /// <summary>
        /// Sent whenever the active document within Microsoft Word has changed.  This will alert any
        /// listeners that they should update their content for the current active document.
        /// </summary>
        public event EventHandler ActiveDocumentChanged;

        /// <summary>
        /// Sent as progress changes in the code execution phases
        /// </summary>
        public class ProgressEventArgs : EventArgs
        {
            public int Index { get; set; }
            public int TotalItems { get; set; }
            public string Description { get; set; }
        }
        public event EventHandler<ProgressEventArgs> ExecutionUpdated;

        /// <summary>
        /// Sent whenever a tag is being edited.
        /// </summary>
        public class TagEventArgs : EventArgs { public Tag Tag { get; set; } }
        public event EventHandler<TagEventArgs> EditingTag;
        public event EventHandler<TagEventArgs> EditedTag;

        private Dictionary<string, List<MonitoredCodeFile>> DocumentCodeFiles { get; set; }

        private EditTag EditTagForm { get; set; }

        private Document activeDocument = null;
        public Document ActiveDocument
        {
            get { return activeDocument; }
            
            set
            {
                activeDocument = value;

                if (activeDocument != null && !DocumentCodeFiles.ContainsKey(activeDocument.FullName))
                {
                    Log("Active document changed - not in cache, so we will load it");
                    GetManagedCodeFileList(activeDocument);
                }

                if (ActiveDocumentChanged != null)
                {
                    ActiveDocumentChanged(this, new EventArgs());
                }
            }
        }
        public TagManager TagManager { get; set; }
        public StatsManager StatsManager { get; set; }
        public SettingsManager SettingsManager { get; set; }

        public const string ConfigurationAttribute = "StatTag Configuration";
        public const string MetadataAttribute = "StatTag Metadata";

        // Singleton pattern implementation informed by: http://csharpindepth.com/Articles/General/Singleton.aspx#cctor
        private static readonly DocumentManager instance = new DocumentManager();
        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        static DocumentManager() {}
        public static DocumentManager Instance
        {
            get
            {
                return instance;
            }
        }

        private DocumentManager()
        {
            SettingsManager = null;
            DocumentCodeFiles = new Dictionary<string, List<MonitoredCodeFile>>();
            TagManager = new TagManager(this);
            StatsManager = new StatsManager(this, SettingsManager);
        }

        public void SetSettingsManager(SettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
            if (StatsManager == null)
            {
                StatsManager = new StatsManager(this, SettingsManager);
            }
            else
            {
                StatsManager.SettingsManager = SettingsManager;
            }
        }

        /// <summary>
        /// Provider a wrapper to check if a variable exists in the document.
        /// </summary>
        /// <remarks>Needed because Word interop doesn't provide a nice check mechanism, and uses exceptions instead.</remarks>
        /// <param name="variable">The variable to check</param>
        /// <returns>True if a variable exists and has a value, false otherwise</returns>
        private bool DocumentVariableExists(Variable variable)
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
        /// Creates a document metadata container that will hold information about the StatTag environment
        /// used to create the Word document.
        /// </summary>
        /// <returns></returns>
        private DocumentMetadata CreateDocumentMetadata()
        {
            var metadata = new DocumentMetadata()
            {
                StatTagVersion = UIUtility.GetVersionLabel(),
                RepresentMissingValues = SettingsManager.Settings.RepresentMissingValues,
                CustomMissingValue = SettingsManager.Settings.CustomMissingValue,
                MetadataFormatVersion = DocumentMetadata.CurrentMetadataFormatVersion,
                TagFormatVersion = Tag.CurrentTagFormatVersion
            };
            return metadata;
        }

        /// <summary>
        /// Saves associated metadata about StatTag to the properties in the supplied document.
        /// </summary>
        /// <param name="document">The Word Document object we are saving the metadata to</param>
        /// <param name="metadata">The metadata object to be serialized and saved</param>
        public void SaveMetadataToDocument(Document document, DocumentMetadata metadata)
        {
            Log("SaveMetadataToDocument - Started");

            var variables = document.Variables;
            var variable = variables[MetadataAttribute];
            try
            {
                if (metadata == null)
                {
                    metadata = CreateDocumentMetadata();
                }

                var attribute = metadata.Serialize();
                if (!DocumentVariableExists(variable))
                {
                    Log(string.Format("Metadata variable does not exist.  Adding attribute value of {0}", attribute));
                    variables.Add(MetadataAttribute, attribute);
                }
                else
                {
                    Log(string.Format("Metadata variable already exists.  Updating attribute value to {0}", attribute));
                    variable.Value = attribute;
                }
            }
            finally
            {
                Marshal.ReleaseComObject(variable);
                Marshal.ReleaseComObject(variables);
            }


            // Historically we just saved the code file list.  Starting in v3.1 we save more metadata, so the original
            // call to save the code file list is just called afterwards.
            SaveCodeFileListToDocument(document);

            Log("SaveMetadataToDocument - Finished");
        }

        public DocumentMetadata LoadMetadataFromCurrentDocument(bool createIfEmpty)
        {
            var document = Globals.ThisAddIn.SafeGetActiveDocument();
            return LoadMetadataFromDocument(document, createIfEmpty);
        }


        /// <summary>
        /// Loads associated metadata about StatTag from the properties in the supplied document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="createIfEmpty">If true, and there is no metadata for the document, a default instance of the metadata will be created.  If false, and no metadata exists, null will be returned.</param>
        public DocumentMetadata LoadMetadataFromDocument(Document document, bool createIfEmpty)
        {
            Log("LoadMetadataFromDocument - Started");
            DocumentMetadata metadata = null;
            if (document == null)
            {
                Log("Document is null - exiting method");
                return metadata;
            }

            // Right now, we don't worry about holding on to metadata from the document (outside of the code file list),
            // we just read it and log it so we know a little more about the document.
            var variables = document.Variables;
            var variable = variables[MetadataAttribute];
            try
            {
                if (DocumentVariableExists(variable))
                {
                    metadata = DocumentMetadata.Deserialize(variable.Value);
                }
                else if (createIfEmpty)
                {
                    metadata = CreateDocumentMetadata();
                }
            }
            finally
            {
                Marshal.ReleaseComObject(variable);
                Marshal.ReleaseComObject(variables);
            }

            Log("LoadMetadataFromDocument - Finished");

            return metadata;
        }

        /// <summary>
        /// Save the referenced code files to the current Word document.
        /// </summary>
        /// <param name="document">The Word document of interest</param>
        protected void SaveCodeFileListToDocument(Document document)
        {
            Log("SaveCodeFileListToDocument - Started");

            var variables = document.Variables;
            var variable = variables[ConfigurationAttribute];
            try
            {
                var files = GetCodeFileList(document);
                var hasCodeFiles = (files != null && files.Count > 0) ;
                var attribute = CodeFile.SerializeList(files);
                if (!DocumentVariableExists(variable))
                {
                    if (hasCodeFiles)
                    {
                        Log(string.Format("Document variable does not exist.  Adding attribute value of {0}", attribute));
                        variables.Add(ConfigurationAttribute, attribute);
                    }
                    else
                    {
                        Log("There are no code files to add.");
                    }
                }
                else
                {
                    if (hasCodeFiles)
                    {
                        Log(string.Format("Document variable already exists.  Updating attribute value to {0}",
                            attribute));
                        variable.Value = attribute;
                    }
                    else
                    {
                        Log("There are no code files - removing existing variable");
                        variable.Delete();
                    }
                }
            }
            finally
            {
                Marshal.ReleaseComObject(variable);
                Marshal.ReleaseComObject(variables);
            }

            Log("SaveCodeFileListToDocument - Finished");
        }

        /// <summary>
        /// Forces the list of associated Code Files from a Word document to load, and refreshes
        /// the internally managed list of Code Files for that document.
        /// </summary>
        /// <param name="document">The Word document of interest</param>
        public void LoadCodeFileListFromDocument(Document document)
        {
            Log("LoadCodeFileListFromDocument - Started");

            var variables = document.Variables;
            var variable = variables[ConfigurationAttribute];
            try
            {
                if (DocumentVariableExists(variable))
                {
                    var list = CodeFile.DeserializeList(variable.Value);
                    RefreshCodeFileListForDocument(document, list);
                    Log(string.Format("Document variable existed, loaded {0} code files", list.Count));
                }
                else
                {
                    DocumentCodeFiles[document.FullName] = new List<MonitoredCodeFile>();
                    Log("Document variable does not exist, no code files loaded");
                }

                // Alert our listeners that the list of code files has changed
                if (CodeFileListChanged != null)
                {
                    Log("Alerting listeners that the list of code files has changed");
                    CodeFileListChanged(this, new EventArgs());
                }
            }
            finally
            {
                Marshal.ReleaseComObject(variable);
                Marshal.ReleaseComObject(variables);
            }

            Log("LoadCodeFileListFromDocument - Finished");
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

            string originalFileName = tag.CachedResult.First().FigureResult;
            // Because different statistical packages let you format file paths different ways (e.g., R will
            // let you use C:/test/test.pdf), we will convert the path to a standard Windows format before
            // we use it.  This maybe should be done when the path is set in the tag, but for now I thought
            // we should preserve the original value that was generated by the statistical code.
            string fileName = Path.GetFullPath(originalFileName);
            Log(string.Format("Normalized path {0} to {1}", originalFileName, fileName));

            if (!File.Exists(fileName))
            {
                Log(string.Format("Could not find file at {0}", fileName));
                throw new StatTagUserException(string.Format("The figure you created could not be found at {0}.\r\n\r\nPlease ensure that the figure is being created by your statistical program.  If it is, and you are using relative paths for the figure output, please try changing your statistical code to provide the full path to the figure.", fileName));
            }

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

            if (!tagUpdatePair.Old.TableFormat.ColumnFilter.Equals(tagUpdatePair.New.TableFormat.ColumnFilter)
                || !tagUpdatePair.Old.TableFormat.RowFilter.Equals(tagUpdatePair.New.TableFormat.RowFilter))
            {
                Log("Table dimensions have changed based on filter settings");
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

            var tableRefreshed = true;

            // First iterate over all of the shapes
            var shapes = document.Shapes;
            foreach (var shape in shapes.OfType<Microsoft.Office.Interop.Word.Shape>())
            {
                var fields = WordUtil.SafeGetFieldsFromShape(shape);
                if (fields != null && fields.Count > 0)
                {
                    var shapeTableRefreshed = HandleRefreshTableTagFields(fields, tag, document);
                    if (!shapeTableRefreshed)
                    {
                        Log(string.Format("Table refresh failed for shape of type {0}", shape.Type));
                        tableRefreshed = shapeTableRefreshed;
                    }
                    Marshal.ReleaseComObject(fields);
                }
            }

            // Then iterate over all of the story ranges - this will include text areas as well as text boxes.
            foreach (var story in document.StoryRanges.OfType<Range>())
            {
                var fields = story.Fields;
                if (fields != null)
                {
                    var storyTableRefreshed = HandleRefreshTableTagFields(fields, tag, document);
                    if (fields != null && fields.Count > 0 && !storyTableRefreshed)
                    {
                        Log(string.Format("Table refresh failed for story of type {0}", story.StoryType));
                        tableRefreshed = storyTableRefreshed;
                    }
                    Marshal.ReleaseComObject(fields);
                }
            }

            Log(string.Format("RefreshTableTagFields - Finished, Returning {0}", tableRefreshed));
            return tableRefreshed;
        }

        private bool HandleRefreshTableTagFields(Fields fields, Tag tag, Document document)
        {
            int fieldsCount = fields.Count;
            bool tableRefreshed = false;

            // Fields is a 1-based index
            Log(string.Format("Preparing to process {0} fields", fieldsCount));
            for (int index = fieldsCount; index >= 1; index--)
            {
                var field = fields[index];
                if (field == null)
                {
                    Log(string.Format("Null field detected at index {0}", index));
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
                        InsertField(tag, false);
                        tableRefreshed = true;
                    }
                }

                Marshal.ReleaseComObject(field);
            }

            return tableRefreshed;
        }

        /// <summary>
        /// Processes all inline shapes within the document, which will include our inserted figures.
        /// If the shape can be updated, we will process the update.
        /// </summary>
        /// <param name="document"></param>
        private List<string> UpdateInlineShapes(Document document)
        {
            var pathsNotUpdated = new List<string>();
            var shapes = document.InlineShapes;
            if (shapes == null)
            {
                return pathsNotUpdated;
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
                        // Attempt to update the linked file.  If it fails / throws an exception, catch that
                        // and track the image file path that did not update.
                        try
                        {
                            linkFormat.Update();
                        }
                        catch
                        {
                            string fileName = string.Empty;
                            try
                            {
                                fileName = linkFormat.SourceFullName;
                            }
                            catch
                            {
                                // If we can't safely get the name, we will just provide a generic name.
                                fileName = "Additional image(s) (unable to access their file names)";
                            }

                            if (!pathsNotUpdated.Contains(fileName))
                            {
                                pathsNotUpdated.Add(fileName);
                            }
                        }
                        finally
                        {
                            Marshal.ReleaseComObject(linkFormat);
                        }
                    }
                    Marshal.ReleaseComObject(shape);
                }
            }

            Marshal.ReleaseComObject(shapes);

            return pathsNotUpdated;
        }

        /// <summary>
        /// Processes all content controls within the document, which may include verbatim output.
        /// Handles renaming of tags as well, if applicable.
        /// </summary>
        /// <param name="document">The current document to process</param>
        /// <param name="tagUpdatePair"></param>
        private void UpdateVerbatimEntries(Document document, UpdatePair<Tag> tagUpdatePair = null)
        {
            var shapes = document.Shapes;
            if (shapes == null)
            {
                return;
            }

            var metadata = LoadMetadataFromDocument(document, true);
            int shapeCount = shapes.Count;
            for (int index = 1; index <= shapeCount; index++)
            {
                var shape = shapes[index];
                if (shape != null)
                {
                    if (TagManager.IsStatTagShape(shape))
                    {
                        var tag = TagManager.FindTag(shape.Name);
                        if (tag == null)
                        {
                            Log(string.Format("No tag was found for the control with ID: {0}", shape.Name));
                            continue;
                        }

                        if (tag.Type != Constants.TagType.Verbatim)
                        {
                            Log(string.Format("The tag ({0}) was inserted as verbatim but is now a different type.  We are unable to update it.", tag.Id));
                        }

                        // If the tag update pair is set, it will be in response to renaming.  Make sure
                        // we apply the new tag name to the control
                        if (tagUpdatePair != null && tagUpdatePair.Old.Equals(tag))
                        {
                            shape.Name = tagUpdatePair.New.Id;
                        }

                        shape.TextFrame.TextRange.Text = tag.FormattedResult(metadata);
                    }
                    Marshal.ReleaseComObject(shape);
                }
            }

            Marshal.ReleaseComObject(shapes);
        }

        private void HandleUpdateFieldsCollection(Fields fields, UpdatePair<Tag> tagUpdatePair, bool matchOnPosition, List<Tag> tagFilter, IProgressReporter reporter = null)
        {
            bool hasTagFilter = (tagFilter != null && tagFilter.Count > 0);
            var fieldsCount = fields.Count;
            // Fields is a 1-based index
            Log(string.Format("Preparing to process {0} fields", fieldsCount));
            int filterCount = (tagFilter == null ? 0 : tagFilter.Count);
            for (int index = fieldsCount; index >= 1; index--)
            {
                var field = fields[index];
                if (reporter != null)
                {
                    if (reporter.IsCancelling())
                    {
                        return;
                    }
                    
                    int progressIndex = (fieldsCount - index + 1);
                    var progressMessage = string.Format("Updating {0} {1} - processing field {2} of {3} in the document",
                        filterCount, "tag".Pluralize(filterCount), progressIndex, fieldsCount);

                    reporter.ReportProgress((int)(((progressIndex*1.0)/fieldsCount) * 100), progressMessage);
                }

                if (field == null)
                {
                    Log(string.Format("Null field detected at index {0}", index));
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
                else if (hasTagFilter)
                {
                    if (!tagFilter.Contains(tag))
                    {
                        Log(string.Format("Tag {0} is not in update filter, skipping", tag.Name));
                        continue;
                    }
                }

                Log(string.Format("Inserting field for tag: {0}", tag.Name));
                field.Select();
                InsertField(tag, false);

                Marshal.ReleaseComObject(field);
            }
        }

        public void UpdateFields(List<Tag> tagFilter, IProgressReporter reporter)
        {
            UpdateFields(null, false, tagFilter, reporter);
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
        public void UpdateFields(UpdatePair<Tag> tagUpdatePair = null, bool matchOnPosition = false, List<Tag> tagFilter = null, IProgressReporter reporter = null)
        {
            Log("UpdateFields - Started");

            var application = Globals.ThisAddIn.Application; // Doesn't need to be cleaned up
            var document = application.ActiveDocument;
            Cursor.Current = Cursors.WaitCursor;
            application.ScreenUpdating = false;
            List<string> shapesNotUpdated = null;
            try
            {
                if (reporter != null) { reporter.ReportProgress(0, "Beginning to update your document"); }

                if (tagUpdatePair != null)
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
                }

                if (reporter != null) { reporter.ReportProgress(10, "Updating all embedded Word shapes"); }
                shapesNotUpdated = UpdateInlineShapes(document);
                if (reporter != null) { reporter.ReportProgress(25, "Updated all Word shapes"); }

                if (reporter != null) { reporter.ReportProgress(26, "Updating all verbatim results"); }
                UpdateVerbatimEntries(document, tagUpdatePair);
                if (reporter != null) { reporter.ReportProgress(50, "Updated all verbatim results"); }

                if (reporter != null) { reporter.ReportProgress(51, "Updating all shapes and images"); }
                var shapes = document.Shapes;
                foreach (var shape in shapes.OfType<Microsoft.Office.Interop.Word.Shape>())
                {
                    var fields = WordUtil.SafeGetFieldsFromShape(shape);
                    if (fields != null)
                    {
                        HandleUpdateFieldsCollection(fields, tagUpdatePair, matchOnPosition, tagFilter, reporter);
                        Marshal.ReleaseComObject(fields);
                    }
                }
                if (reporter != null) { reporter.ReportProgress(75, "Updated all shapes and images"); }

                if (reporter != null) { reporter.ReportProgress(76, "Updating all tagged fields"); }
                int totalFields = 0;
                foreach (var story in document.StoryRanges.OfType<Range>())
                {
                    if (story.Fields != null)
                    {
                        var fields = story.Fields;
                        totalFields += fields.Count;
                        Marshal.ReleaseComObject(fields);
                    }
                }
                foreach (var story in document.StoryRanges.OfType<Range>())
                {
                    if (story.Fields != null)
                    {
                        var fields = story.Fields;
                        HandleUpdateFieldsCollection(fields, tagUpdatePair, matchOnPosition, tagFilter, reporter);
                        Marshal.ReleaseComObject(fields);
                    }
                }
                if (reporter != null) { reporter.ReportProgress(100, "Update all tagged fields"); }
            }
            finally
            {
                Marshal.ReleaseComObject(document);
                Cursor.Current = Cursors.Default;
                application.ScreenUpdating = true;
            }

            // Report this as an exception AFTER all of the fields have been updated.
            if (shapesNotUpdated != null && shapesNotUpdated.Count > 0)
            {
                throw new StatTagUserException(string.Format("StatTag was unable to find the following figure(s), and so they could not be updated in the document.  You will need to delete each figure from Word and re-add it:\r\n\r\n{0}",
                    string.Join("\r\n", shapesNotUpdated)));
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
            table.Select();  // Select the target table directly instead of trying to select through the document.Range

            var cells = GetCells(application.Selection);

            Marshal.ReleaseComObject(endCell);
            Marshal.ReleaseComObject(columns);
            Marshal.ReleaseComObject(rows);
            Marshal.ReleaseComObject(document);
            return cells;
        }

        /// <summary>
        /// Inserts a rich text edit control
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="tag"></param>
        public void InsertVerbatim(Selection selection, Tag tag)
        {
            Log("InsertVerbatim - Started");

            if (tag == null)
            {
                Log("Unable to insert the verbatim output because the tag is null");
                return;
            }

            if (tag.CachedResult == null)
            {
                Log("Unable to insert the verbatim output because the tag cached result is null");
                return;
            }

            var result = tag.CachedResult.FirstOrDefault();
            if (result != null)
            {
                var range = selection.Range;
                // When creating the textbox with the range as the anchor (last parameter), we need to be sure to
                // specify the right top/left starting position based on the selection's position relative to the page.
                // Otherwise the verbatim control always shows up at the top (at least in Word 2016, this doesn't seem
                // to be an issue in Word 2010).
                // Note that in v3.5 of StatTag we also had to add a tiny offset (+1 point) to the vertical range.  In
                // Word 2016, we saw verbatim fields going into the paragraph above.  From reading this article:
                // https://social.msdn.microsoft.com/Forums/office/en-US/f175055e-7b40-4bcb-8409-d4e0910022f4/documentshapesaddtextbox-ignores-anchor-word-2013?forum=worddev
                // it sounds like putting the top anchor on the position of the cursor was making Word think it was still
                // in the previous paragraph.  This tiny offset makes all the difference.
                var shape = selection.Document.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal,
                    (float)selection.Information[WdInformation.wdHorizontalPositionRelativeToPage],
                    (float)selection.Information[WdInformation.wdVerticalPositionRelativeToPage] + 1, 100, 100, range);
                var textFrame = shape.TextFrame;
                textFrame.TextRange.Text = result.VerbatimResult;
                textFrame.AutoSize = -1;
                textFrame.WordWrap = 0;
                shape.Line.Visible = MsoTriState.msoFalse;
                textFrame.TextRange.Font.Name = "Courier New";
                textFrame.TextRange.Font.Size = 9.0f;
                textFrame.TextRange.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                textFrame.TextRange.ParagraphFormat.SpaceAfter = 0;
                textFrame.TextRange.ParagraphFormat.SpaceBefore = 0;
                shape.WrapFormat.Type = WdWrapType.wdWrapInline;
                shape.Name = tag.Id;

                Marshal.ReleaseComObject(textFrame);
                Marshal.ReleaseComObject(shape);
                Marshal.ReleaseComObject(range);
            }

            Log("InsertVerbatim - Finished");
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

            var metadata = LoadMetadataFromDocument(selection.Document, true);
            tag.UpdateFormattedTableData(metadata);
            var table = tag.CachedResult.First().TableResult;

            var dimensions = tag.GetTableDisplayDimensions();

            var cellsCount = cells == null ? 0 : cells.Count;  // Because of the issue we mention below, pull the cell count right away

            // Insert a new table if there is none selected.
            if (cellsCount == 0)
            {
                Log("No cells selected, creating a new table");
                CreateWordTableForTableResult(selection, table, tag.TableFormat, dimensions);
                // The table will be the size we need.  Update these tracking variables with the cells and
                // total size so that we can begin inserting data.
                cells = GetCells(selection);
                cellsCount = dimensions[0] * dimensions[1];
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

            var displayData = TableUtil.GetDisplayableVector(table.FormattedCells, tag.TableFormat);

            if (displayData == null || displayData.Length == 0)
            {
                UIUtility.WarningMessageBox("There are no table results to insert.", Logger);
                return;
            }

            if (cells == null)
            {
                Log("Unable to insert the table because the cells collection came back as null.");
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
                if (index >= displayData.Length)
                {
                    Log(string.Format("Index {0} is beyond result cell length of {1}", index, displayData.Length));
                    break;
                }

                var range = cell.Range;

                // Make a copy of the tag and set the cell index.  This will let us discriminate which cell an tag
                // value is related with, since we have multiple fields (and therefore multiple copies of the tag) in the
                // document.  Note that we are wiping out the cached value to just have the individual cell value present.
                var innerTag = new FieldTag(tag, index)
                {
                    CachedResult =
                        new List<CommandResult>() { new CommandResult() { ValueResult = displayData[index] } }
                };
                CreateTagField(range,
                    string.Format("{0}{1}{2}", tag.Name, Constants.ReservedCharacters.TagTableCellDelimiter, index),
                    innerTag.FormattedResult(metadata), innerTag);
                index++;
                Marshal.ReleaseComObject(range);
            }

            WarnOnMismatchedCellCount(cellsCount, displayData.Length);

            Marshal.ReleaseComObject(cells);

            // Once the table has been inserted, re-select it (inserting fields messes with the previous selection) and
            // insert a new line after it.  This gives us spacing after a table so inserting multiple tables doesn't have
            // them all glued together.
            selection.Tables[1].Select();
            var tableSelection = Globals.ThisAddIn.Application.Selection;
            InsertNewLineAndMoveDown(tableSelection);
            Marshal.ReleaseComObject(tableSelection);

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
        /// <param name="dimensions"></param>
        public void CreateWordTableForTableResult(Selection selection, Core.Models.Table table, TableFormat format, int[] dimensions)
        {
            Log("CreateWordTableForTableResult - Started");

            var application = Globals.ThisAddIn.Application; // Doesn't need to be cleaned up
            var document = application.ActiveDocument;
            try
            {
                int rowCount = dimensions[0];
                int columnCount = dimensions[1];

                Log(string.Format("Table dimensions r={0}, c={1}", rowCount, columnCount));
                if (rowCount == 0 && columnCount == 0)
                {
                    Log("Invalid row/column count - throwing exception");
                    throw new StatTagUserException(string.Format("The number of rows and columns ({0} x {1}) are not valid for creating a table.\r\n\r\nPlease make sure your statistical code runs properly.  If the table data is coming from a file, please make sure that the file was created and that StatTag has permissions to read it.\r\n\r\nIf the table is being created correctly, please inform the StatTag team (StatTag@northwestern.edu).",
                        rowCount, columnCount));
                }

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
        public void InsertField(Tag tag, bool isPlaceholder)
        {
            Log("InsertField for Tag");
            InsertField(new FieldTag(tag), isPlaceholder);
        }

        /// <summary>
        /// Given an tag, insert the result into the document at the current cursor position.
        /// <remarks>This method assumes the tag result is already refreshed.  It does not
        /// attempt to refresh or recalculate it.</remarks>
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="isPlaceholder">If the field to be inserted should be a placeholder, or a value</param>
        public void InsertField(FieldTag tag, bool isPlaceholder)
        {
            Log(string.Format("InsertField - Started (isPlaceholder = {0})", isPlaceholder));

            if (tag == null)
            {
                Log("The tag is null");
                return;
            }

            if (!isPlaceholder && tag.Type == Constants.TagType.Figure)
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

                if (!isPlaceholder && tag.Type == Constants.TagType.Verbatim)
                {
                    Log("Inserting verbatim output");
                    selection.Delete();
                    InsertVerbatim(selection, tag);
                }
                // If the tag is a table, and the cell index is not set, it means we are inserting the entire
                // table into the document.  Otherwise, we are able to just insert a single table cell.
                else if (!isPlaceholder && tag.IsTableTag() && !tag.TableCellIndex.HasValue)
                {
                    Log("Inserting a new table tag");
                    InsertTable(selection, tag);
                }
                else
                {
                    Log("Inserting a single tag field");
                    var range = selection.Range;
                    var displayName = (isPlaceholder
                        ? string.Format("[ {0} ]", tag.Name)
                        : tag.FormattedResult(LoadMetadataFromDocument(document, true)));
                    CreateTagField(range, tag.Name, displayName, tag);
                    Marshal.ReleaseComObject(range);
                }

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
        /// <param name="placeholder">Is the field a placeholder field or not</param>
        protected void CreateTagField(Range range, string tagIdentifier, string displayValue, FieldTag tag, bool placeholder = false)
        {
            Log("CreateTagField - Started");
            if (tag.Type == Constants.TagType.Verbatim && !placeholder)
            {
                FieldGenerator.GenerateField(range, tagIdentifier, displayValue, tag);
                return;
            }
            var xml = OpenXmlGenerator.GenerateField(range, tagIdentifier, displayValue, tag);
            Log("Field XML: " + xml);
            range.InsertXML(xml);
            Log("CreateTagField - Finished");
        }

        /// <summary>
        /// Insert an StatTag field at the currently specified document range.
        /// </summary>
        /// <param name="range">The range to insert the field at</param>
        /// <param name="tagIdentifier">The visible identifier of the tag (does not need to be globablly unique)</param>
        /// <param name="displayValue">The value that should display when the field is shown.</param>
        /// <param name="tag">The tag to be inserted</param>
        /// <param name="placeholder">Is the field a placeholder field or not</param>
        protected void CreateTagField(Range range, string tagIdentifier, string displayValue, Tag tag, bool placeholder = false)
        {
            CreateTagField(range, tagIdentifier, displayValue, new FieldTag(tag), placeholder);
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
                // Alert all listeners that we are beginning to edit a tag
                if (EditingTag != null)
                {
                    EditingTag(this, new TagEventArgs() {Tag = tag});
                }

                EditTagForm = new EditTag(false, this);

                var document = Globals.ThisAddIn.SafeGetActiveDocument();
                var metadata = LoadMetadataFromDocument(document, true);

                IntPtr hwnd = Process.GetCurrentProcess().MainWindowHandle;
                Log(string.Format("Established main window handle of {0}", hwnd.ToString()));

                EditTagForm.Tag = new Tag(tag);
                var wrapper = new WindowWrapper(hwnd);
                Log(string.Format("WindowWrapper established as: {0}", wrapper.ToString()));
                if (DialogResult.OK == EditTagForm.ShowDialog(wrapper))
                {
                    // Save the tag first, before trying to update the tags.  This way even if there is
                    // an error during the updates, our results are saved.
                    SaveEditedTag(EditTagForm, tag);

                    // If the value format has changed, refresh the values in the document with the
                    // new formatting of the results.
                    // TODO: Sometimes date/time format are null in one and blank strings in the other.  This is causing extra update cycles that aren't needed.
                    if (EditTagForm.Tag.ValueFormat != tag.ValueFormat)
                    {
                        Log("Updating fields after tag value format changed");
                        if (EditTagForm.Tag.TableFormat != tag.TableFormat)
                        {
                            Log("Updating formatted table data");
                            EditTagForm.Tag.UpdateFormattedTableData(metadata);
                        }
                        UpdateFields(new UpdatePair<Tag>(tag, EditTagForm.Tag));
                    }
                    else if (EditTagForm.Tag.TableFormat != tag.TableFormat)
                    {
                        Log("Updating fields after tag table format changed");
                        EditTagForm.Tag.UpdateFormattedTableData(metadata);
                        UpdateFields(new UpdatePair<Tag>(tag, EditTagForm.Tag));
                    }
                    else if (EditTagForm.Tag.Id != tag.Id)
                    {
                        Log("Updating fields after tag renamed");
                        UpdateFields(new UpdatePair<Tag>(tag, EditTagForm.Tag));
                    }

                    // Alert all listeners that we are done editing a tag
                    if (EditedTag != null)
                    {
                        Log("Alerting listener that tag editing is complete");
                        EditedTag(this, new TagEventArgs() {Tag = tag});
                    }

                    Log("EditTag - Finished (action)");
                    return true;
                }

                // Alert all listeners that we are done editing a tag
                if (EditedTag != null)
                {
                    Log("Alerting listener that tag editing is complete");
                    EditedTag(this, new TagEventArgs() {Tag = tag});
                }
            }
            catch (Exception exc)
            {
                Log("An exception was caught while trying to edit an tag");
                LogException(exc);
            }
            finally
            {
                EditTagForm = null;
            }

            Log("EditTag - Finished (no action)");
            return false;
        }

        /// <summary>
        /// Helper function to remove tags.  While not very difficult, it wraps
        /// up the responsibility of what needs to be done to clean up tags so
        /// it's done consistently in all instances.
        /// </summary>
        /// <param name="tags"></param>
        public void RemoveTags(List<Tag> tags)
        {
            HandleRemoveTags(tags, TagManager.RemoveTags);
        }

        /// <summary>
        /// Helper function to remove tags that collide with other tags.
        /// This is different from "RemoveTags", which will simply remove
        /// valid tags.
        /// </summary>
        /// <param name="tags"></param>
        public void RemoveCollidingTags(List<Tag> tags)
        {
            HandleRemoveTags(tags, TagManager.RemoveCollidingTags);
        }

        private void HandleRemoveTags(List<Tag> tags, Action<List<Tag>> removeMethod)
        {
            if (tags == null || tags.Count == 0)
            {
                return;
            }

            removeMethod(tags);

            // The TagManager will remove the references between the tags and the code files.
            // We also need to save this change to the code files themselves, and then refresh
            // the known tags within the code file.
            var codeFiles = tags.Select(x => x.CodeFile).Distinct();
            foreach (var codeFile in codeFiles)
            {
                codeFile.Save();
                codeFile.LoadTagsFromContent(true);
            }

            // Alert our listeners that at least one tag has changed
            if (TagListChanged != null)
            {
                TagListChanged(this, new EventArgs());
            }
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

                // Make sure to reload the tags.  While this does give some overhead, it also provides
                // assurances that tag indexes will be correct after the new tag is added.
                codeFile.LoadTagsFromContent(true);

                // Alert our listeners that at least one tag has changed
                if (TagListChanged != null)
                {
                    TagListChanged(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Save all changes to all code files referenced by the current document.
        /// </summary>
        public void SaveAllCodeFiles(Document document)
        {
            // Update the code files with their tags
            foreach (var file in GetCodeFileList(document))
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

        public void EditTagShape(Microsoft.Office.Interop.Word.Shape shape)
        {
            if (TagManager.IsStatTagShape(shape))
            {
                var tag = TagManager.FindTag(shape.Name);
                if (tag != null)
                {
                    EditTag(tag);
                }
            }
        }

        //public void CheckForInsertSavedTag(EditTag dialog)
        //{
        //    // If the user clicked the "Save and Insert", we will perform the insertion now.
        //    if (dialog.DefineAnother)
        //    {
        //        Log("Inserting into document after defining tag");

        //        var tag = FindTag(dialog.Tag.Id);
        //        if (tag == null)
        //        {
        //            Log(string.Format("Unable to find tag {0}, so skipping the insert", dialog.Tag.Id));
        //            return;
        //        }

        //        InsertTagsInDocument(new List<Tag>(new[] { tag }));
        //    }
        //}
        /// <summary>
        /// This is a specialized utility function to be called whenever the user clicks "Save and Insert"
        /// from the Edit Tag dialog.
        /// </summary>
        /// <param name="dialog"></param>
        /// <summary>
        /// Performs the insertion of tags into a document as fields.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="reporter"></param>
        public void InsertTagsInDocument(List<Tag> tags, bool isPlaceholder, IProgressReporter reporter)
        {
            Cursor.Current = Cursors.WaitCursor;
            Globals.ThisAddIn.Application.ScreenUpdating = false;
            try
            {
                // Get all of our unique code files that need to be run.  We are going to execute these in the first
                // phase.
                var updatedTags = new List<Tag>();
                if (!isPlaceholder)
                {
                    var codeFiles = tags.Select(x => x.CodeFile).Distinct().ToArray();
                    for (int index = 0; index < codeFiles.Length; index++)
                    {
                        var codeFile = codeFiles[index];
                        if (ExecutionUpdated != null)
                        {
                            ExecutionUpdated(this, new ProgressEventArgs()
                            {
                                Index = (index + 1),
                                TotalItems = codeFiles.Length,
                                Description =
                                    string.Format("Running code file {0} of {1}", (index + 1), codeFiles.Length)
                            });
                        }
                        var result = StatsManager.ExecuteStatPackage(codeFile,
                            Constants.ParserFilterMode.TagList, tags);
                        if (!result.Success)
                        {
                            break;
                        }

                        updatedTags.AddRange(result.UpdatedTags);
                    }
                }

                int tagCount = tags.Count;
                for (int index = 0; index < tagCount; index++)
                {
                    var tag = tags[index];
                    if (reporter != null)
                    {
                        if (reporter.IsCancelling())
                        {
                            return;
                        }

                        reporter.ReportProgress((int)(((index + 1 * 1.0) / tagCount) * 100),
                            string.Format("Inserting tag {0} of {1}", (index + 1), tagCount));
                    }
                    InsertField(tag, true);
                }

                // Now that all of the fields have been inserted, sweep through and update any existing
                // tags that changed.  We do this after the fields are inserted to better manage
                // the cursor position in the document.
                if (!isPlaceholder)
                {
                    updatedTags = updatedTags.Distinct().ToList();
                    foreach (var updatedTag in updatedTags)
                    {
                        UpdateFields(new UpdatePair<Tag>(updatedTag, updatedTag));
                    }
                }
            }
            catch (StatTagUserException uex)
            {
                UIUtility.ReportException(uex, uex.Message, Logger);
            }
            catch (Exception exc)
            {
                UIUtility.ReportException(exc,
                    "There was an unexpected error when trying to insert the tag output into the Word document.",
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
        /// <param name="document">The Word document to analyze.</param>
        /// <param name="onlyShowDialogIfResultsFound">If true, the results dialog will only display if there is something to report</param>

        public void PerformDocumentCheck(Document document, bool onlyShowDialogIfResultsFound = false, CheckDocument.DefaultTab defaultTab = CheckDocument.DefaultTab.FirstWithData)
        {
            CheckDocument checkDocumentDialog = null;
            PerformDocumentCheck(document, ref checkDocumentDialog, onlyShowDialogIfResultsFound, defaultTab);
        }

        /// <summary>
        /// Conduct an assessment of the active document to see if there are any inserted
        /// tags that do not have an associated code file in the document.
        /// </summary>
        /// <param name="document">The Word document to analyze.</param>
        /// <param name="checkDocumentDialog">A reference to use for the CheckDocument form.  This method will create the dialog, but the
        /// reference allows the caller to have a reference when the method completes.</param>
        /// <param name="onlyShowDialogIfResultsFound">If true, the results dialog will only display if there is something to report</param>
        public void PerformDocumentCheck(Document document, ref CheckDocument checkDocumentDialog, bool onlyShowDialogIfResultsFound = false, CheckDocument.DefaultTab defaultTab = CheckDocument.DefaultTab.FirstWithData)
        {
            var unlinkedResults = TagManager.FindAllUnlinkedTags();
            var duplicateResults = TagManager.FindAllDuplicateTags();
            var overlappingResults = TagManager.FindAllOverlappingTags();
            if (onlyShowDialogIfResultsFound 
                && (unlinkedResults == null || unlinkedResults.Count == 0)
                && (duplicateResults == null || duplicateResults.Count == 0)
                && (overlappingResults == null || overlappingResults.Count == 0))
            {
                return;
            }

            checkDocumentDialog = new CheckDocument(unlinkedResults, duplicateResults, overlappingResults,
                GetCodeFileList(document), defaultTab);
            if (DialogResult.OK == checkDocumentDialog.ShowDialog())
            {
                UpdateUnlinkedTagsByTag(checkDocumentDialog.UnlinkedTagUpdates, checkDocumentDialog.UnlinkedAffectedCodeFiles);
                UpdateRenamedTags(checkDocumentDialog.DuplicateTagUpdates);
                RemoveCollidingTags(checkDocumentDialog.CollidingTagUpdates);
            }
        }

        #region Wrappers around TagManager calls
        public Dictionary<string, List<Tag>> FindAllUnlinkedTags()
        {
            return TagManager.FindAllUnlinkedTags();
        }

        /// <summary>
        /// For all of the code files associated with the current document, get all of the
        /// tags as a single list.
        /// </summary>
        /// <returns>A List of Tag objects from all code files</returns>
        public List<Tag> GetTags()
        {
            return TagManager.GetTags();
        }

        /// <summary>
        /// Find the master reference of an tag, which is contained in the code files
        /// associated with the current document
        /// </summary>
        /// <param name="id">The tag identifier to search for</param>
        /// <returns>The found Tag, or null of no tag was found</returns>
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
        /// <param name="unlinkedAffectedCodeFiles"></param>
        public void UpdateUnlinkedTagsByCodeFile(Dictionary<string, CodeFileAction> actions, List<string> unlinkedAffectedCodeFiles)
        {
            TagManager.ProcessStatTagFields(TagManager.UpdateUnlinkedTagsByCodeFile, actions);
            TagManager.ProcessStatTagShapes(TagManager.UpdateUnlinkedShapesByCodeFile, actions);
            ProcessUnlinkedCodeFiles(unlinkedAffectedCodeFiles);

            // Alert our listeners that at least one tag has changed
            if (TagListChanged != null)
            {
                TagListChanged(this, new EventArgs());
            }
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
        /// <param name="unlinkedAffectedCodeFiles"></param>
        public void UpdateUnlinkedTagsByTag(Dictionary<string, CodeFileAction> actions, List<string> unlinkedAffectedCodeFiles)
        {
            TagManager.ProcessStatTagFields(TagManager.UpdateUnlinkedTagsByTag, actions);
            TagManager.ProcessStatTagShapes(TagManager.UpdateUnlinkedShapesByTag, actions);
            ProcessUnlinkedCodeFiles(unlinkedAffectedCodeFiles);

            // Alert our listeners that at least one tag has changed
            if (TagListChanged != null)
            {
                TagListChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Given a list of code files that have been unlinked from certain tags, determine if they can be
        /// fully removed from the list of code files associated with the document.
        /// </summary>
        /// <param name="unlinkedAffectedCodeFiles">The list of code files that have been unlinked from one or more tags</param>
        public void ProcessUnlinkedCodeFiles(List<string> unlinkedAffectedCodeFiles)
        {
            Log("ProcessUnlinkedCodeFiles - Start");
            // If we replaced a code file, we are going to check to see if all references to that old code file are gone.
            // If so, we can remove the code file reference from the document, so a potentially unused (or invalid)
            // code file reference doesn't get carried aong.
            var unusedFiles = TagManager.FindUnusedCodeFiles();
            if (unusedFiles != null && unusedFiles.Count > 0)
            {
                var filesToRemove = unlinkedAffectedCodeFiles.Intersect(unusedFiles.Select(x => x.FilePath)).ToList();
                if (filesToRemove.Any())
                {
                    var codeFiles = GetCodeFileList();
                    var updatedCodeFileList = codeFiles.Where(x => !filesToRemove.Contains(x.FilePath)).ToList();
                    SetCodeFileList(updatedCodeFileList);

                    // Alert our listeners that at least one code file has changed
                    if (CodeFileListChanged != null)
                    {
                        Log("Alerting listeners that the list of code files has changed");
                        CodeFileListChanged(this, new EventArgs());
                    }
                }
            }
            Log("ProcessUnlinkedCodeFiles - Finished");
        }

        /// <summary>
        /// Add a code file reference to our master list of files in the document.  This should be used when
        /// discovering code files to link to the document.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="document"></param>
        public void AddCodeFile(string fileName, Document document = null)
        {
            //var files = GetCodeFileList(document);
            var files = GetManagedCodeFileList(document);
            if (files.Any(x => x.FilePath.Equals(fileName, StringComparison.CurrentCultureIgnoreCase)))
            {
                Log(string.Format("Code file {0} already exists and won't be added again", fileName));
                return;
            }

            string package = CodeFile.GuessStatisticalPackage(fileName);
            var file = new CodeFile { FilePath = fileName, StatisticalPackage = package };
            file.LoadTagsFromContent();
            file.SaveBackup();
            try
            {
                var monitoredCodeFile = new MonitoredCodeFile(file);
                monitoredCodeFile.CodeFileChanged += OnCodeFileChanged;
                files.Add(monitoredCodeFile);
                Log(string.Format("Added code file {0}", fileName));

                // Alert our listeners that we have added a code file
                if (CodeFileListChanged != null)
                {
                    Log("Alerting listeners that the list of code files has changed");
                    CodeFileListChanged(this, new EventArgs());
                }
            }
            catch (Exception exc)
            {
                throw new FileMonitoringException(fileName, exc);
            }
        }

        /// <summary>
        /// This just dispatches to the next level for handling.  This is because the DocumentManager class
        /// is responsible for collecting and managing all code files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnCodeFileChanged(object sender, EventArgs args)
        {
            var monitoredCodeFile = sender as MonitoredCodeFile;
            if (monitoredCodeFile == null)
            {
                return;
            }

            if (CodeFileContentsChanged != null)
            {
                CodeFileContentsChanged(monitoredCodeFile, args);
            }
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

            // Alert our listeners that tags have been updated
            if (TagListChanged != null)
            {
                TagListChanged(this, new EventArgs());
            }

            foreach (var codeFile in affectedCodeFiles)
            {
                codeFile.Save();
            }
        }

        private void RemoveCollidingTags(Dictionary<Tag, List<Tag>> updates)
        {
            Logger.WriteMessage(string.Format("Resolving {0} groups of colliding tags", updates.Count));
            if (updates.Count > 0)
            {
                var tagsToRemove = updates.SelectMany(x => x.Value).ToList();
                Logger.WriteMessage(string.Format("Removing {0} tags in total", tagsToRemove.Count));
                RemoveCollidingTags(tagsToRemove);
            }
        }

        private void RefreshCodeFileListForDocument(Document document, List<CodeFile> files)
        {
            // If there is no entry for this particular document, we can exit (there's nothing to clean up)
            if (document == null || !DocumentCodeFiles.ContainsKey(document.FullName))
            {
                return;
            }

            // Shut down and dispose of all the existing managed code files (including their watchers).
            var existingFiles = DocumentCodeFiles[document.FullName];
            existingFiles.ForEach(x => x.Dispose());

            // Now set up the new code file list, and subscribe new watchers for them so they are
            // properly managed
            var managedFiles = new List<MonitoredCodeFile>();
            foreach (var file in files)
            {
                try
                {
                    var monitoredCodeFile = new MonitoredCodeFile(file);
                    monitoredCodeFile.CodeFileChanged += OnCodeFileChanged;
                    managedFiles.Add(monitoredCodeFile);
                }
                catch (Exception exc)
                {
                    throw new FileMonitoringException(file.FilePath, exc);
                }
            }
            DocumentCodeFiles[document.FullName] = managedFiles;
        }

        /// <summary>
        /// Given a document, force all known code files to refresh their contents.
        /// <remarks>This uses the list of internally managed code files associated with a document.  Even if the code file has
        /// had its contents cached, this forces it to completely reload.  It should be called only when we know that the code
        /// file is invalid (such as when a file changes on disk)</remarks>
        /// </summary>
        /// <param name="document">The document we want to reload code file contents for</param>
        public void RefreshContentInDocumentCodeFiles(Document document)
        {
            Log("RefreshContentInDocumentCodeFiles - Start");
            if (document == null)
            {
                Log("No document was specified to refresh - will exit");
                return;
            }
            var codeFiles = GetManagedCodeFileList(document);
            Log(string.Format("Document has {0} existing code file(s) associated", (codeFiles == null ) ? "(null)" : codeFiles.Count.ToString()));
            if (codeFiles != null)
            {
                foreach (var codeFile in codeFiles)
                {
                    codeFile.StopMonitoring();
                    codeFile.LoadTagsFromContent();
                    codeFile.ChangeHistory.Clear();  // We have reloaded the code file, so clear out any outstanding changes.
                    codeFile.StartMonitoring();
                }
            }

            if (CodeFileListChanged != null)
            {
                Log("Alerting listeners that the list of code files has changed");
                CodeFileListChanged(this, new EventArgs());
            }

            Log("RefreshContentInDocumentCodeFiles - Finish");
        }

        /// <summary>
        /// Helper accessor to get the list of code files associated with a document.  If the code file list
        /// hasn't been established yet for the document, it will be created and returned.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public List<MonitoredCodeFile> GetManagedCodeFileList(Document document = null)
        {
            if (document == null)
            {
                Log("No document specified, so fetching code file list for active document.");
                document = Globals.ThisAddIn.SafeGetActiveDocument();
            }
            if (document == null)
            {
                Log("Attempted to access code files for a null document.  Returning empty collection.");
                return new List<MonitoredCodeFile>();
            }

            var fullName = document.FullName;
            if (!DocumentCodeFiles.ContainsKey(fullName))
            {
                Log(string.Format("Code file list for {0} is not yet cached.", fullName));
                DocumentCodeFiles.Add(fullName, new List<MonitoredCodeFile>());
                LoadCodeFileListFromDocument(document);
                var files = DocumentCodeFiles[fullName];
                foreach (var file in files)
                {
                    if (!File.Exists(file.FilePath))
                    {
                        Log(string.Format("Code file: {0} not found", file.FilePath));
                    }
                    else
                    {
                        file.LoadTagsFromContent(false); // Skip saving the cache, since this is the first load
                    }
                }
                Log(string.Format("Loaded {0} code files from document", DocumentCodeFiles[fullName].Count));
            }

            return DocumentCodeFiles[fullName];
        }

        public bool IsCodeFileLinkedToDocument(Document document, string codeFilePath)
        {
            if (document == null || string.IsNullOrWhiteSpace(codeFilePath))
            {
                return false;
            }

            // If our internal map of documents doesn't know about the document being passed in, there's no
            // way we've tracked the code files associated with it, so we have to say it's not linked.
            if (!DocumentCodeFiles.ContainsKey(document.FullName))
            {
                return false;
            }

            return DocumentCodeFiles[document.FullName].Select(x => x.FilePath).Any(x => x.Equals(codeFilePath));
        }

        public List<CodeFile> GetCodeFileList(Document document = null)
        {
            var codeFileList = GetManagedCodeFileList(document);
            if (codeFileList == null)
            {
                return new List<CodeFile>();
            }

            return codeFileList.Select(x => (CodeFile)x).ToList<CodeFile>();
        }

        /// <summary>
        /// Helper setter to update a document's list of associated code files.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="document"></param>
        public void SetCodeFileList(List<CodeFile> files, Document document = null)
        {
            if (document == null)
            {
                Log("No document specified, so getting a reference to the active document.");
                document = Globals.ThisAddIn.SafeGetActiveDocument();
            }
            if (document == null)
            {
                Log("Attempted to set the code files for a null document.  Throwing exception.");
                throw new ArgumentNullException("The Word document must be specified.");
            }

            RefreshCodeFileListForDocument(document, files);

            if (CodeFileListChanged != null)
            {
                Log("Alerting listeners that the list of code files has changed");
                CodeFileListChanged(this, new EventArgs());
            }
        }

        public void Dispose()
        {
            // To help clean things up, go through all of the documents we are managing code files for,
            // and dispose of those to stop managed objects (e.g., the file system watcher) and clean them up.
            if (DocumentCodeFiles != null)
            {
                foreach (var docFile in DocumentCodeFiles)
                {
                    docFile.Value.ForEach(x => x.Dispose());
                }
            }
        }

        /// <summary>
        /// Called when we need to forcibly close any and all open dialogs, such as
        /// for system shutdown, or when a linked code file has been changed.
        /// </summary>
        public void CloseAllChildDialogs()
        {
            UIUtility.ManageCloseDialog(EditTagForm);
        }
    }
}
