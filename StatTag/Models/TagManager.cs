using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;
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
        /// Search the active Word document and find all inserted tags.  Determine if the tag's
        /// code file is linked to this document, and report those that are not.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<Tag>> FindAllUnlinkedTags()
        {
            Log("FindAllUnlinkedTags - Started");
            var results = new Dictionary<string, List<Tag>>();

            var application = Globals.ThisAddIn.Application; // Doesn't need to be cleaned up
            var document = application.ActiveDocument;

            var fields = document.Fields;
            int fieldsCount = fields.Count;

            // Fields is a 1-based index
            var files = DocumentManager.GetCodeFileList();
            Log(string.Format("Preparing to process {0} fields", fieldsCount));
            for (int index = fieldsCount; index >= 1; index--)
            {
                var field = fields[index];
                if (field == null)
                {
                    Log(string.Format("Null field detected at index {0}", index));
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

                if (!files.Any(x => x.FilePath.Equals(tag.CodeFilePath)))
                {
                    if (!results.ContainsKey(tag.CodeFilePath))
                    {
                        results.Add(tag.CodeFilePath, new List<Tag>());
                    }

                    results[tag.CodeFilePath].Add(tag);
                }
                Marshal.ReleaseComObject(field);
            }

            Marshal.ReleaseComObject(document);

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

            var application = Globals.ThisAddIn.Application; // Doesn't need to be cleaned up
            var document = application.ActiveDocument;

            var fields = document.Fields;
            int fieldsCount = fields.Count;

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

            Marshal.ReleaseComObject(document);

            Log("ProcessStatTagFields - Finished");
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
            var actions = configuration as Dictionary<string, CodeFileAction>;
            if (actions == null)
            {
                Log("The list of actions to perform is null or of the wrong type");
                return;
            }

            // If there is no action specified for this field, we will exit.  This should happen when we have fields that
            // are still linked in a document.
            if (!actions.ContainsKey(tag.CodeFilePath))
            {
                Log(string.Format("No action is needed for tag in file {0}", tag.CodeFilePath));
                return;
            }

            // Make sure that the action is actually defined.  If no action was specified by the user, we can't continue
            // with doing anything.
            var action = actions[tag.CodeFilePath];
            if (action == null)
            {
                Log("No action was specified - exiting");
                return;
            }

            // Apply the appropriate action to this field, based on what the user specified.
            var codeFile = action.Parameter as CodeFile;
            switch (action.Action)
            {
                case Constants.CodeFileActionTask.ChangeFile:
                    Log(string.Format("Changing tag {0} from {1} to {2}",
                        tag.Name, tag.CodeFilePath, codeFile.FilePath));
                    tag.CodeFile = codeFile;
                    UpdateTagFieldData(field, tag);
                    break;
                case Constants.CodeFileActionTask.RemoveTags:
                    Log(string.Format("Removing {0}", tag.Name));
                    field.Select();
                    var application = Globals.ThisAddIn.Application;
                    application.Selection.Text = Constants.Placeholders.RemovedField;
                    application.Selection.Range.HighlightColorIndex = WdColorIndex.wdYellow;
                    break;
                case Constants.CodeFileActionTask.ReAddFile:
                    Log(string.Format("Linking code file {0}", tag.CodeFilePath));
                    DocumentManager.AddCodeFile(tag.CodeFilePath);
                    break;
                default:
                    Log(string.Format("The action task of {0} is not known and will be skipped", action.Action));
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
            var actions = configuration as Dictionary<string, CodeFileAction>;
            if (actions == null)
            {
                Log("The list of actions to perform is null or of the wrong type");
                return;
            }

            // If there is no action specified for this field, we will exit.  This should happen when we have fields that
            // are still linked in a document.
            if (!actions.ContainsKey(tag.Id))
            {
                Log(string.Format("No action is needed for tag {0}", tag.Id));
                return;
            }

            // Make sure that the action is actually defined.  If no action was specified by the user, we can't continue
            // with doing anything.
            var action = actions[tag.Id];
            if (action == null)
            {
                Log("No action was specified - exiting");
                return;
            }

            // Apply the appropriate action to this field, based on what the user specified.
            var codeFile = action.Parameter as CodeFile;
            switch (action.Action)
            {
                case Constants.CodeFileActionTask.ChangeFile:
                    Log(string.Format("Changing tag {0} from {1} to {2}",
                        tag.Name, tag.CodeFilePath, codeFile.FilePath));
                    tag.CodeFile = codeFile;
                    UpdateTagFieldData(field, tag);
                    break;
                case Constants.CodeFileActionTask.RemoveTags:
                    Log(string.Format("Removing {0}", tag.Name));
                    field.Select();
                    var application = Globals.ThisAddIn.Application;
                    application.Selection.Text = Constants.Placeholders.RemovedField;
                    application.Selection.Range.HighlightColorIndex = WdColorIndex.wdYellow;
                    break;
                case Constants.CodeFileActionTask.ReAddFile:
                    Log(string.Format("Linking code file {0}", tag.CodeFilePath));
                    DocumentManager.AddCodeFile(tag.CodeFilePath);
                    break;
                default:
                    Log(string.Format("The action task of {0} is not known and will be skipped", action.Action));
                    break;
            }
        }
    }
}
