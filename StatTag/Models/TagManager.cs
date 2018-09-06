using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using StatTag.Core;
using StatTag.Core.Models;
using StatTag.Core.Utility;
using Microsoft.Office.Interop.Word;

namespace StatTag.Models
{
    /// <summary>
    /// The TagManager is responsible for finding, editing, and otherwise managing specific tags.
    /// This has some overlap (conceptually) with the DocumentManager class.  Functionality split out here is
    /// meant to relieve the DocumentManager of needing to accomplish everything.
    /// 
    /// At times, this class needs to reference the DocumentManager to perform an action.  For example - an
    /// action for an unlinked tag is to re-link the code file, which is handled by the DocumentManager.
    /// A reference is included back to the DocumentManager class to allow this.
    /// 
    /// The relationship is that an TagManager only exists in the context of a DocumentManager.  An instance
    /// of this class should not exist outside of the DocumentManager.  These methods should only be accessed by
    /// the DocumentManager instance that contains it.
    /// </summary>
    public class TagManager : BaseManager
    {
        public DocumentManager DocumentManager { get; set; }

        public TagManager(DocumentManager manager)
        {
            DocumentManager = manager;
            Logger = (DocumentManager == null ? null : DocumentManager.Logger);
        }

        /// <summary>
        /// Find the master reference of an tag, which is contained in the code files
        /// associated with the current document
        /// </summary>
        /// <param name="tag">The tag to find</param>
        /// <returns></returns>
        public Tag FindTag(Tag tag)
        {
            return FindTag(tag.Id);
        }

        /// <summary>
        /// Find the master reference of an tag, which is contained in the code files
        /// associated with the current document
        /// </summary>
        /// <param name="id">The tag identifier to search for</param>
        /// <returns></returns>
        public Tag FindTag(string id)
        {
            var files = DocumentManager.GetCodeFileList();
            if (files == null)
            {
                Log("Unable to find an tag because the Files collection is null");
                return null;
            }

            return files.SelectMany(file => file.Tags).FirstOrDefault(tag => tag.Id.Equals(id));
        }

        /// <summary>
        /// For all of the code files associated with the current document, get all of the
        /// tags as a single list.
        /// </summary>
        /// <returns></returns>
        public List<Tag> GetTags()
        {
            var files = DocumentManager.GetCodeFileList();
            var tags = new List<Tag>();
            files.ForEach(file => tags.AddRange(file.Tags));
            return tags;
        }

        /// <summary>
        /// Given a Word field, determine if it is our specialized StatTag field type given
        /// its composition.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool IsStatTagField(Field field)
        {
            return (field != null
                && field.Type == WdFieldType.wdFieldMacroButton
                && field.Code != null && field.Code.Text.Contains(Constants.FieldDetails.MacroButtonName)
                && field.Code.Fields.Count > 0);
        }

        /// <summary>
        /// Determine if a field is a linked field.  While linked fields can take on various forms, we
        /// use them in StatTag to represent images.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool IsLinkedField(Field field)
        {
            return (field != null
                    && field.Type == WdFieldType.wdFieldLink);
        }

        /// <summary>
        /// Deserialize a field to extract the FieldTag data
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public FieldTag DeserializeFieldTag(Field field)
        {
            var files = DocumentManager.GetCodeFileList();
            var code = field.Code;
            var nestedField = code.Fields[1];
            var fieldTag = FieldTag.Deserialize(nestedField.Data.ToString(CultureInfo.InvariantCulture),
                files);
            Marshal.ReleaseComObject(nestedField);
            Marshal.ReleaseComObject(code);
            return fieldTag;
        }

        /// <summary>
        /// Given a Word document Field, extracts the embedded StatTag tag
        /// associated with it.
        /// </summary>
        /// <param name="field">The Word field object to investigate</param>
        /// <returns></returns>
        public FieldTag GetFieldTag(Field field)
        {
            var fieldTag = DeserializeFieldTag(field);
            if (fieldTag == null)
            {
                return null;
            }
            var tag = FindTag(fieldTag);

            // The result of FindTag is going to be a document-level tag, not a
            // cell specific one that exists as a field.  We need to re-set the cell index
            // from the tag we found, to ensure it's available for later use.
            return new FieldTag(tag, fieldTag);
        }

        public DuplicateTagResults FindAllDuplicateTags()
        {
            var files = DocumentManager.GetCodeFileList();
            var duplicateTags = new DuplicateTagResults();
            foreach (var file in files)
            {
                var result = file.FindDuplicateTags();
                if (result != null && result.Count > 0)
                {
                    duplicateTags.Add(file, result);
                }
            }

            return duplicateTags;
        }

        /// <summary>
        /// Look at all of the tags in the document, and identify which code files are being used.  Then
        /// determine from that list and all of the code files linked to the document which code files are
        /// not actually being used at this time.
        /// <remarks>Just because a code file is not being used does not mean there is a problem with it.  A 
        /// user may have linked a code file and not yet inserted a tag, but plans to.</remarks>
        /// </summary>
        /// <returns></returns>
        public List<CodeFile> FindUnusedCodeFiles()
        {
            Log("FindUnusedCodeFiles - Started");

            var document = DocumentManager.ActiveDocument;
            if (document == null)
            {
                Log("Null document - exiting method");
                return null;
            }

            var usedFiles = new List<string>();

            // First iterate over all of the shapes
            var shapes = document.Shapes;
            foreach (var shape in shapes.OfType<Microsoft.Office.Interop.Word.Shape>())
            {
                var fields = WordUtil.SafeGetFieldsFromShape(shape);
                if (fields != null)
                {
                    var shapeUsedFiles = HandleFindUnusedCodeFiles(fields);
                    if (shapeUsedFiles.Any())
                    {
                        Log(string.Format("Found {0} files", shapeUsedFiles.Count));
                        usedFiles.AddRange(shapeUsedFiles);
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
                    var storyUsedFiles = HandleFindUnusedCodeFiles(fields);
                    if (storyUsedFiles.Any())
                    {
                        Log(string.Format("Found {0} files", storyUsedFiles.Count));
                        usedFiles.AddRange(storyUsedFiles);
                    }
                    Marshal.ReleaseComObject(fields);
                }
            }

            var unusedFiles = DocumentManager.GetCodeFileList().Where(x => !usedFiles.Contains(x.FilePath)).ToList();
            Log(string.Format("Found {0} unused files", unusedFiles.Count));

            Log("FindUnusedCodeFiles - Finished");
            return unusedFiles;
        }

        private List<string> HandleFindUnusedCodeFiles(Fields fields)
        {
            int fieldsCount = fields.Count;
            var usedFiles = new List<string>();

            Log(String.Format("Preparing to process {0} fields", fieldsCount));
            // Fields is a 1-based index
            for (int index = fieldsCount; index >= 1; index--)
            {
                var field = fields[index];
                if (field == null)
                {
                    Log(String.Format("Null field detected at index {0}", index));
                    continue;
                }

                if (!IsStatTagField(field))
                {
                    Marshal.ReleaseComObject(field);
                    continue;
                }

                Log("Processing StatTag field");
                var tag = GetFieldTag(field);
                if (tag == null)
                {
                    Log("The field tag is null or could not be found");
                    Marshal.ReleaseComObject(field);
                    continue;
                }

                if (!usedFiles.Contains(tag.CodeFilePath))
                {
                    usedFiles.Add(tag.CodeFilePath);
                }
                Marshal.ReleaseComObject(field);
            }

            return usedFiles;
        }

        /// <summary>
        /// Create a stub tag and code file based on the name in the shape.  These just have the bare
        /// minimum file and name references so we can use them in existing methods.
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public Tag CreatePlaceholderTagFromShape(Shape shape)
        {
            var nameParts = shape.Name.Split(new[] { Tag.IdentifierDelimiter }, StringSplitOptions.None);
            if (nameParts.Length != 2)
            {
                Log(string.Format("The shape name ({0}), when split, is not exactly 2 parts.  Currently StatTag is not equipped to process these results.", shape.Name));
                return null;
            }

            var codeFile = new CodeFile() { FilePath = nameParts[1] };
            var tag = new Tag()
            {
                Name = nameParts[0],
                Type = Constants.TagType.Verbatim,
                CodeFile = codeFile
            };
            return tag;
        }

        public OverlappingTagResults FindAllOverlappingTags()
        {
            Log("FindAllOverlappingTags - Started");
            var results = new OverlappingTagResults();

            var document = DocumentManager.ActiveDocument;
            if (document == null)
            {
                Log("Null active document - existing method");
                return results;
            }

            // When checking unlinked tags, we will look to see what tags are present in
            // linked code files.  Because a code file may be linked but no longer exist, we
            // need to filter down our list of files to only include those that are found at
            // the specified path.
            var files = DocumentManager.GetCodeFileList().Where(x => File.Exists(x.FilePath)).ToList();
            foreach (var file in files)
            {
                var parser = Factories.GetParser(file);
                if (parser == null)
                {
                    Log(string.Format("Unable to find a parser for {0}", file.FilePath));
                    continue;
                }

                var tags = parser.ParseIncludingInvalidTags(file);

                // If there are 0 or 1 tags, there's no way we'll have them overlap.  Just continue;
                if (tags.Length < 2)
                {
                    continue;
                }

                for (int index = 0; index < (tags.Length - 1); index++)
                {
                    var tag1 = tags[index];
                    var tag2 = tags[index + 1];
                    var result = TagUtil.DetectTagCollision(tag1, tag2);
                    if (result != null && result.Collision != TagUtil.TagCollisionResult.CollisionType.NoOverlap && result.CollidingTag != null)
                    {
                        Log(string.Format("Collision: Tag {0} {1} Tag {2}", tag1.Name, result.Collision,
                                result.CollidingTag.Name)); 
                        
                        if (!results.ContainsKey(file))
                        {
                            Log(string.Format("Starting a colliding tags collection for code file {0}", file.FilePath));
                            results.Add(file, new List<List<Tag>>());
                        }

                        // Our code file entry is established, now we need to figure out if these colliding tags are
                        // in a collision group already.  If so, we'll add the tags that are missing from the group.
                        // If not, we will create a new group.
                        var collection = results[file];
                        var foundTagGroup1 = collection.FirstOrDefault(x => x.Contains(tag1));
                        var foundTagGroup2 = collection.FirstOrDefault(x => x.Contains(tag2));
                        if (foundTagGroup1 == null && foundTagGroup2 == null)
                        {
                            Log("Creating a new tag collision group");
                            var group = new List<Tag> {tag1, tag2};
                            collection.Add(group);
                        }
                        else if (foundTagGroup1 == null)
                        {
                            Log("Adding to tag2 collision group");
                            foundTagGroup2.Add(tag1);
                        }
                        else if (foundTagGroup2 == null)
                        {
                            Log("Adding to tag1 collision group");
                            foundTagGroup1.Add(tag2);
                        }
                    }
                }
            }


            Log("FindAllUnlinkedTags - Finished");
            return results;
        }

        /// <summary>
        /// Search the active Word document and find all inserted tags.  Determine if the tag's
        /// code file is linked to this document, and report those that are not.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<Tag>> FindAllUnlinkedTags()
        {
            Log("FindAllUnlinkedTags - Started");
            var results = new Dictionary<string, List<Tag>>();

            var document = DocumentManager.ActiveDocument;
            if (document == null)
            {
                Log("Null active document - exiting method");
                return results;
            }

            // When checking unlinked tags, we will look to see what tags are present in
            // linked code files.  Because a code file may be linked but no longer exist, we
            // need to filter down our list of files to only include those that are found at
            // the specified path.
            var files = DocumentManager.GetCodeFileList().Where(x => File.Exists(x.FilePath)).ToList();

            // Then iterate over all of the story ranges - this will include text areas as well as text boxes.
            foreach (var story in document.StoryRanges.OfType<Range>())
            {
                var fields = story.Fields;
                int fieldsCount = fields.Count;

                Log(String.Format("Preparing to process {0} fields", fieldsCount));
                // Fields is a 1-based index
                for (int index = fieldsCount; index >= 1; index--)
                {
                    var field = fields[index];
                    if (field == null)
                    {
                        Log(String.Format("Null field detected at index {0}", index));
                        continue;
                    }

                    if (!IsStatTagField(field))
                    {
                        Marshal.ReleaseComObject(field);
                        continue;
                    }

                    Log("Processing StatTag field");
                    var tag = GetFieldTag(field);
                    if (tag == null)
                    {
                        Log("The field tag is null or could not be found");
                        Marshal.ReleaseComObject(field);
                        continue;
                    }

                    // If the file associated with the tag is not in our known list of code files, or if the code file is linked
                    // and we just can't find the tag identifier anymore (e.g., if it was deleted from the code file without us
                    // knowing), we track the tag as being unlinked.
                    bool fileLinked = files.Any(x => x.FilePath.Equals(tag.CodeFilePath));
                    if (!fileLinked || (fileLinked && !(files.First(x => x.FilePath.Equals(tag.CodeFilePath)).Tags.Any(x => x.Id.Equals(tag.Id)))))
                    {
                        if (!results.ContainsKey(tag.CodeFilePath))
                        {
                            results.Add(tag.CodeFilePath, new List<Tag>());
                        }

                        results[tag.CodeFilePath].Add(tag);
                    }
                    Marshal.ReleaseComObject(field);
                }                
            }

            var shapes = document.Shapes;
            int shapesCount = shapes.Count;
            Log(String.Format("Preparing to process {0} shapes", shapesCount));
            // Shapes is a 1-based index
            for (int index = shapesCount; index >= 1; index--)
            {
                var shape = shapes[index];
                if (shape == null)
                {
                    Log(String.Format("Null shape detected at index {0}", index));
                    continue;
                }

                if (!IsStatTagShape(shape))
                {
                    Marshal.ReleaseComObject(shape);
                    continue;
                }

                Log("Processing StatTag shape");
                var tag = CreatePlaceholderTagFromShape(shape);
                if (tag == null)
                {
                    Marshal.ReleaseComObject(shape);
                    throw new NullReferenceException(
                        "This Word document element appears to have been created by StatTag, but there was an error trying to load it.  Please contact StatTag@northwestern.edu to report this problem.");
                }

                // If the file associated with the tag is not in our known list of code files, or if the code file is linked
                // and we just can't find the tag identifier anymore (e.g., if it was deleted from the code file without us
                // knowing), we track the tag as being unlinked.
                bool fileLinked = files.Any(x => x.FilePath.Equals(tag.CodeFilePath));
                if (!fileLinked || (fileLinked && !(files.First(x => x.FilePath.Equals(tag.CodeFilePath)).Tags.Any(x => x.Id.Equals(tag.Id)))))
                {
                    if (!results.ContainsKey(tag.CodeFilePath))
                    {
                        results.Add(tag.CodeFilePath, new List<Tag>());
                    }

                    results[tag.CodeFilePath].Add(tag);
                }
                Marshal.ReleaseComObject(shape);
            }
            
            Log("FindAllUnlinkedTags - Finished");
            return results;
        }

        /// <summary>
        /// A generic method that will iterate over the fields in the active document, and apply a function to
        /// each StatTag field.
        /// </summary>
        /// <param name="function">The function to apply to each relevant field</param>
        /// <param name="configuration">A set of configuration information specific to the function</param>
        public void ProcessStatTagFields(Action<Field, FieldTag, object> function, object configuration)
        {
            Log("ProcessStatTagFields - Started");

            var document = DocumentManager.ActiveDocument;
            if (document == null)
            {
                Log("Null active document - exiting method");
                return;
            }

            // First iterate over all of the shapes
            var shapes = document.Shapes;
            foreach (var shape in shapes.OfType<Microsoft.Office.Interop.Word.Shape>())
            {
                var fields = WordUtil.SafeGetFieldsFromShape(shape);
                if (fields != null)
                {
                    HandleProcessStatTagFields(fields, function, configuration);
                    Marshal.ReleaseComObject(fields);
                }
            }

            // Then iterate over all of the story ranges - this will include text areas as well as text boxes.
            foreach (var story in document.StoryRanges.OfType<Range>())
            {
                var fields = story.Fields;
                if (fields != null)
                {
                    HandleProcessStatTagFields(fields, function, configuration);
                    Marshal.ReleaseComObject(fields);
                }
            }

            Log("ProcessStatTagFields - Finished");
        }

        private void HandleProcessStatTagFields(Fields fields, Action<Field, FieldTag, object> function,
            object configuration)
        {
            int fieldsCount = fields.Count;

            // Fields is a 1-based index
            Log(String.Format("Preparing to process {0} fields", fieldsCount));
            for (int index = fieldsCount; index >= 1; index--)
            {
                var field = fields[index];
                if (field == null)
                {
                    Log(String.Format("Null field detected at index {0}", index));
                    continue;
                }

                if (!IsStatTagField(field))
                {
                    Marshal.ReleaseComObject(field);
                    continue;
                }

                Log("Processing StatTag field");
                var tag = GetFieldTag(field);
                if (tag == null)
                {
                    Log("The field tag is null or could not be found");
                    Marshal.ReleaseComObject(field);
                    continue;
                }

                function(field, tag, configuration);

                Marshal.ReleaseComObject(field);
            }
        }

        /// <summary>
        /// A generic method that will iterate over the shapes in the active document, and apply a function to
        /// each StatTag shape.
        /// </summary>
        /// <param name="function">The function to apply to each relevant shape</param>
        /// <param name="configuration">A set of configuration information specific to the function</param>
        public void ProcessStatTagShapes(Action<Shape, Tag, object> function, object configuration)
        {
            Log("ProcessStatTagShapes - Started");

            var document = DocumentManager.ActiveDocument;
            if (document == null)
            {
                Log("Null active document - exiting method");
                return;
            }

            var shapes = document.Shapes;
            int shapesCount = shapes.Count;

            // Shapes is a 1-based index
            Log(String.Format("Preparing to process {0} shapes", shapesCount));
            for (int index = shapesCount; index >= 1; index--)
            {
                var shape = shapes[index];
                if (shape == null)
                {
                    Log(String.Format("Null shape detected at index {0}", index));
                    continue;
                }

                if (!IsStatTagShape(shape))
                {
                    Log("The shape tag does not have a name, and so isn't considered a StatTag shape");
                    Marshal.ReleaseComObject(shape);
                    continue;
                }

                Log("Processing StatTag shape");
                var tag = CreatePlaceholderTagFromShape(shape);
                if (tag == null)
                {
                    Marshal.ReleaseComObject(shape);
                    throw new NullReferenceException(
                        "This Word document element appears to have been created by StatTag, but there was an error trying to load it.  Please contact StatTag@northwestern.edu to report this problem.");
                }

                function(shape, tag, configuration);

                Marshal.ReleaseComObject(shape);
            }
            
            Log("ProcessStatTagShapes - Finished");
        }

        /// <summary>
        /// Update the tag identifier associated with a verbatim field (a Shape in Word)
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="tag"></param>
        public void UpdateShapeTagReference(Shape shape, Tag tag)
        {
            if (shape == null || !IsStatTagShape(shape))
            {
                return;
            }

            shape.Name = tag.Id;
        }

        /// <summary>
        /// Update the tag data in a field.
        /// </summary>
        /// <remarks>Assumes that the field parameter is known to be an tag field</remarks>
        /// <param name="field">The field to update.  This is the outermost layer of the tag field.</param>
        /// <param name="tag">The tag to be updated.</param>
        public void UpdateTagFieldData(Field field, FieldTag tag)
        {
            var code = field.Code;
            var nestedField = code.Fields[1];
            nestedField.Data = tag.Serialize();
            Marshal.ReleaseComObject(nestedField);
            Marshal.ReleaseComObject(code);
        }

        /// <summary>
        /// Given a field and its tag, update it to link to a new code file
        /// </summary>
        /// <param name="field">The document Field that contains the tag</param>
        /// <param name="tag">The tag that will be updated</param>
        /// <param name="configuration">A collection of the actions to apply (of type Dictionary&lt;string, CodeFileAction&gt;)</param>
        public void UpdateUnlinkedTagsByCodeFile(Field field, FieldTag tag, object configuration)
        {
            if (!CheckAction(tag.CodeFilePath, configuration))
            {
                return;
            }

            var actions = configuration as Dictionary<string, CodeFileAction>;
            var action = actions[tag.CodeFilePath];

            // Apply the appropriate action to this field, based on what the user specified.
            var codeFile = action.Parameter as CodeFile;
            switch (action.Action)
            {
                case Constants.CodeFileActionTask.ChangeFile:
                    Log(String.Format("Changing tag {0} from {1} to {2}",
                        tag.Name, tag.CodeFilePath, codeFile.FilePath));
                    tag.CodeFile = codeFile;
                    DocumentManager.AddCodeFile(tag.CodeFilePath);
                    UpdateTagFieldData(field, tag);
                    break;
                case Constants.CodeFileActionTask.RemoveTags:
                    Log(String.Format("Removing {0}", tag.Name));
                    field.Select();
                    var application = Globals.ThisAddIn.Application;
                    application.Selection.Text = Constants.Placeholders.RemovedField;
                    application.Selection.Range.HighlightColorIndex = WdColorIndex.wdYellow;
                    break;
                case Constants.CodeFileActionTask.ReAddFile:
                    Log(String.Format("Linking code file {0}", tag.CodeFilePath));
                    DocumentManager.AddCodeFile(tag.CodeFilePath);
                    break;
                default:
                    Log(String.Format("The action task of {0} is not known and will be skipped", action.Action));
                    break;
            }
        }

        /// <summary>
        /// Given a shape and its tag, update it to link to a new code file
        /// </summary>
        /// <param name="shape">The document Shape that contains the tag</param>
        /// <param name="tag">The tag that will be updated</param>
        /// <param name="configuration">A collection of the actions to apply (of type Dictionary&lt;string, CodeFileAction&gt;)</param>
        public void UpdateUnlinkedShapesByCodeFile(Shape shape, Tag tag, object configuration)
        {
            if (!CheckAction(tag.CodeFilePath, configuration))
            {
                return;
            }

            var actions = configuration as Dictionary<string, CodeFileAction>;
            var action = actions[tag.CodeFilePath];

            // Apply the appropriate action to this field, based on what the user specified.
            var codeFile = action.Parameter as CodeFile;
            switch (action.Action)
            {
                case Constants.CodeFileActionTask.ChangeFile:
                    Log(String.Format("Changing tag {0} from {1} to {2}",
                        tag.Name, tag.CodeFilePath, codeFile.FilePath));
                    tag.CodeFile = codeFile;
                    DocumentManager.AddCodeFile(tag.CodeFilePath);
                    var actualTag = FindTag(tag.Id);  // Try to load the real tag reference now from the loaded code file
                    tag = (actualTag ?? tag);
                    UpdateShapeTagReference(shape, tag);
                    break;
                case Constants.CodeFileActionTask.RemoveTags:
                    Log(String.Format("Removing {0}", tag.Name));
                    shape.Select();
                    var application = Globals.ThisAddIn.Application;
                    application.Selection.Text = Constants.Placeholders.RemovedField;
                    application.Selection.Range.HighlightColorIndex = WdColorIndex.wdYellow;
                    break;
                case Constants.CodeFileActionTask.ReAddFile:
                    Log(String.Format("Linking code file {0}", tag.CodeFilePath));
                    DocumentManager.AddCodeFile(tag.CodeFilePath);
                    break;
                default:
                    Log(String.Format("The action task of {0} is not known and will be skipped", action.Action));
                    break;
            }
        }

        /// <summary>
        /// Helper method to check that an action to perform is valid.  This reduces code duplication in a few of our
        /// other methods.
        /// </summary>
        /// <param name="identifier">The identifier used to uniquely identify the action to perform</param>
        /// <param name="configuration">A generic object parameter that is expected to be cast as a Dictionary&lt;string, CodeFileAction&gt;</param>
        /// <returns>true if all checks pass, and false otherwise.</returns>
        private bool CheckAction(string identifier, object configuration)
        {
            var actions = configuration as Dictionary<string, CodeFileAction>;
            if (actions == null)
            {
                Log("The list of actions to perform is null or of the wrong type");
                return false;
            }

            // If there is no action specified for this field, we will exit.  This should happen when we have fields that
            // are still linked in a document.
            if (!actions.ContainsKey(identifier))
            {
                Log(String.Format("No action is needed for tag identified by {0}", identifier));
                return false;
            }

            // Make sure that the action is actually defined.  If no action was specified by the user, we can't continue
            // with doing anything.
            var action = actions[identifier];
            if (action == null)
            {
                Log("No action was specified - exiting");
                return false;
            }

            return true;
        }

        public void UpdateUnlinkedShapesByTag(Shape shape, Tag tag, object configuration)
        {
            if (!CheckAction(tag.Id, configuration))
            {
                return;
            }

            var actions = configuration as Dictionary<string, CodeFileAction>;
            var action = actions[tag.Id];

            // Apply the appropriate action to this shape, based on what the user specified.
            var codeFile = action.Parameter as CodeFile;
            switch (action.Action)
            {
                case Constants.CodeFileActionTask.ChangeFile:
                    Log(String.Format("Changing tag {0} from {1} to {2}",
                        tag.Name, tag.CodeFilePath, codeFile.FilePath));
                    tag.CodeFile = codeFile;
                    DocumentManager.AddCodeFile(tag.CodeFilePath);
                    var actualTag = FindTag(tag.Id);  // Try to load the real tag reference now from the loaded code file
                    tag = (actualTag ?? tag);
                    UpdateShapeTagReference(shape, tag);
                    break;
                case Constants.CodeFileActionTask.RemoveTags:
                    Log(String.Format("Removing {0}", tag.Name));
                    shape.Select();
                    var application = Globals.ThisAddIn.Application;
                    application.Selection.Text = Constants.Placeholders.RemovedField;
                    application.Selection.Range.HighlightColorIndex = WdColorIndex.wdYellow;
                    break;
                case Constants.CodeFileActionTask.ReAddFile:
                    Log(String.Format("Linking code file {0}", tag.CodeFilePath));
                    DocumentManager.AddCodeFile(tag.CodeFilePath);
                    break;
                default:
                    Log(String.Format("The action task of {0} is not known and will be skipped", action.Action));
                    break;
            }
        }

        /// <summary>
        /// Given a field and its tag, update it to link to a new code file
        /// </summary>
        /// <param name="field">The document Field that contains the tag</param>
        /// <param name="tag">The tag that will be updated</param>
        /// <param name="configuration">A collection of the actions to apply (of type Dictionary&lt;string, CodeFileAction&gt;)</param>
        public void UpdateUnlinkedTagsByTag(Field field, FieldTag tag, object configuration)
        {
            if (!CheckAction(tag.Id, configuration))
            {
                return;
            }

            var actions = configuration as Dictionary<string, CodeFileAction>;
            var action = actions[tag.Id];

            // Apply the appropriate action to this field, based on what the user specified.
            var codeFile = action.Parameter as CodeFile;
            switch (action.Action)
            {
                case Constants.CodeFileActionTask.ChangeFile:
                    Log(String.Format("Changing tag {0} from {1} to {2}",
                        tag.Name, tag.CodeFilePath, codeFile.FilePath));
                    tag.CodeFile = codeFile;
                    DocumentManager.AddCodeFile(tag.CodeFilePath);
                    UpdateTagFieldData(field, tag);
                    break;
                case Constants.CodeFileActionTask.RemoveTags:
                    Log(String.Format("Removing {0}", tag.Name));
                    field.Select();
                    var application = Globals.ThisAddIn.Application;
                    application.Selection.Text = Constants.Placeholders.RemovedField;
                    application.Selection.Range.HighlightColorIndex = WdColorIndex.wdYellow;
                    break;
                case Constants.CodeFileActionTask.ReAddFile:
                    Log(String.Format("Linking code file {0}", tag.CodeFilePath));
                    DocumentManager.AddCodeFile(tag.CodeFilePath);
                    break;
                default:
                    Log(String.Format("The action task of {0} is not known and will be skipped", action.Action));
                    break;
            }
        }

        /// <summary>
        /// Is this possibly a StatTag shape?  This is somewhat of a weak check as we are just
        /// able to look for the presence of a name field, but if it can reduce overhead in
        /// processing shapes we'll take it.
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public bool IsStatTagShape(Shape shape)
        {
            return (shape != null
                    && !string.IsNullOrWhiteSpace(shape.Name)
                    && shape.Name.Contains(Tag.IdentifierDelimiter));
        }
        
        /// <summary>
        /// Helper function to remove tags.  While not very difficult, it wraps
        /// up the responsibility of what needs to be done to clean up tags so
        /// it's done consistently in all instances.
        /// </summary>
        /// <param name="tags"></param>
        public void RemoveTags(List<Tag> tags)
        {
            foreach (var tag in tags)
            {
                tag.CodeFile.RemoveTag(tag);
            }
        }

        public void RemoveCollidingTags(List<Tag> tags)
        {
            // By removing these in descending order, it helps us manage the
            // offsets for the tags that we want to remove.  Otherwise, we will
            // remove the first tag, and when we go to remove the second tag it
            // is pointing to indices that have changed and it's not aware of.
            var sortedTags = tags.OrderByDescending(x => x.LineStart);
            foreach (var tag in sortedTags)
            {
                tag.CodeFile.RemoveCollidingTag(tag);
            }
        }
    }
}
