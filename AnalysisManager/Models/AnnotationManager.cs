using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;
using Microsoft.Office.Interop.Word;

namespace AnalysisManager.Models
{
    /// <summary>
    /// The AnnotationManager is responsible for finding, editing, and otherwise managing specific annotations.
    /// This has some overlap (conceptually) with the DocumentManager class.  Functionality split out here is
    /// meant to relieve the DocumentManager of needing to accomplish everything.
    /// 
    /// At times, this class needs to reference the DocumentManager to perform an action.  For example - an
    /// action for an unlinked annotation is to re-link the code file, which is handled by the DocumentManager.
    /// A reference is included back to the DocumentManager class to allow this.
    /// 
    /// The relationship is that an AnnotationManager only exists in the context of a DocumentManager.  An instance
    /// of this class should not exist outside of the DocumentManager.  These methods should only be accessed by
    /// the DocumentManager instance that contains it.
    /// </summary>
    public class AnnotationManager : BaseManager
    {
        public DocumentManager DocumentManager { get; set; }

        public AnnotationManager(DocumentManager manager)
        {
            DocumentManager = manager;
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
            if (DocumentManager.Files == null)
            {
                Log("Unable to find an annotation because the Files collection is null");
                return null;
            }

            return DocumentManager.Files.SelectMany(file => file.Annotations).FirstOrDefault(annotation => annotation.Id.Equals(id));
        }

        /// <summary>
        /// For all of the code files associated with the current document, get all of the
        /// annotations as a single list.
        /// </summary>
        /// <returns></returns>
        public List<Annotation> GetAnnotations()
        {
            var annotations = new List<Annotation>();
            DocumentManager.Files.ForEach(file => annotations.AddRange(file.Annotations));
            return annotations;
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
                && field.Code != null && field.Code.Text.Contains(Constants.FieldDetails.MacroButtonName)
                && field.Code.Fields.Count > 0);
        }

        /// <summary>
        /// Determine if a field is a linked field.  While linked fields can take on various forms, we
        /// use them in Analysis Manager to represent images.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool IsLinkedField(Field field)
        {
            return (field != null
                    && field.Type == WdFieldType.wdFieldLink);
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
            var fieldAnnotation = FieldAnnotation.Deserialize(nestedField.Data.ToString(CultureInfo.InvariantCulture),
                DocumentManager.Files);
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
            return new FieldAnnotation(annotation, fieldAnnotation);
        }

        public Dictionary<string, List<Annotation>> FindAllUnlinkedAnnotations()
        {
            Log("FindAllUnlinkedAnnotations - Started");
            var results = new Dictionary<string, List<Annotation>>();

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

                if (!DocumentManager.Files.Any(x => x.FilePath.Equals(annotation.CodeFilePath)))
                {
                    if (!results.ContainsKey(annotation.CodeFilePath))
                    {
                        results.Add(annotation.CodeFilePath, new List<Annotation>());
                    }

                    results[annotation.CodeFilePath].Add(annotation);
                }
                Marshal.ReleaseComObject(field);
            }

            Marshal.ReleaseComObject(document);

            Log("FindAllUnlinkedAnnotations - Finished");
            return results;
        }

        /// <summary>
        /// A generic method that will iterate over the fields in the active document, and apply a function to
        /// each Analysis Manager field.
        /// </summary>
        /// <param name="function">The function to apply to each relevant field</param>
        /// <param name="configuration">A set of configuration information specific to the function</param>
        public void ProcessAnalysisManagerFields(Action<Field, FieldAnnotation, object> function, object configuration)
        {
            Log("ProcessAnalysisManagerFields - Started");

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

                function(field, annotation, configuration);

                Marshal.ReleaseComObject(field);
            }

            Marshal.ReleaseComObject(document);

            Log("ProcessAnalysisManagerFields - Finished");
        }

        /// <summary>
        /// Update the annotation data in a field.
        /// </summary>
        /// <remarks>Assumes that the field parameter is known to be an annotation field</remarks>
        /// <param name="field">The field to update.  This is the outermost layer of the annotation field.</param>
        /// <param name="annotation">The annotation to be updated.</param>
        public void UpdateAnnotationFieldData(Field field, FieldAnnotation annotation)
        {
            var code = field.Code;
            var nestedField = code.Fields[1];
            nestedField.Data = annotation.Serialize();
            Marshal.ReleaseComObject(nestedField);
            Marshal.ReleaseComObject(code);
        }

        /// <summary>
        /// Given a field and its annotation, update it to link to a new code file
        /// </summary>
        /// <param name="field">The document Field that contains the annotation</param>
        /// <param name="annotation">The annotation that will be updated</param>
        /// <param name="configuration">A collection of the actions to apply (of type Dictionary&lt;string, CodeFileAction&gt;)</param>
        public void UpdateUnlinkedAnnotationsByCodeFile(Field field, FieldAnnotation annotation, object configuration)
        {
            var actions = configuration as Dictionary<string, CodeFileAction>;
            if (actions == null)
            {
                Log("The list of actions to perform is null or of the wrong type");
                return;
            }

            // If there is no action specified for this field, we will exit.  This should happen when we have fields that
            // are still linked in a document.
            if (!actions.ContainsKey(annotation.CodeFilePath))
            {
                Log(string.Format("No action is needed for annotation in file {0}", annotation.CodeFilePath));
                return;
            }

            // Make sure that the action is actually defined.  If no action was specified by the user, we can't continue
            // with doing anything.
            var action = actions[annotation.CodeFilePath];
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
                    Log(string.Format("Changing annotation {0} from {1} to {2}",
                        annotation.OutputLabel, annotation.CodeFilePath, codeFile.FilePath));
                    annotation.CodeFile = codeFile;
                    UpdateAnnotationFieldData(field, annotation);
                    break;
                case Constants.CodeFileActionTask.RemoveAnnotations:
                    Log(string.Format("Removing {0}", annotation.OutputLabel));
                    field.Select();
                    var application = Globals.ThisAddIn.Application;
                    application.Selection.Text = Constants.Placeholders.RemovedField;
                    application.Selection.Range.HighlightColorIndex = WdColorIndex.wdYellow;
                    break;
                case Constants.CodeFileActionTask.ReAddFile:
                    Log(string.Format("Linking code file {0}", annotation.CodeFilePath));
                    DocumentManager.AddCodeFile(annotation.CodeFilePath);
                    break;
                default:
                    Log(string.Format("The action task of {0} is not known and will be skipped", action.Action));
                    break;
            }
        }

        /// <summary>
        /// Given a field and its annotation, update it to link to a new code file
        /// </summary>
        /// <param name="field">The document Field that contains the annotation</param>
        /// <param name="annotation">The annotation that will be updated</param>
        /// <param name="configuration">A collection of the actions to apply (of type Dictionary&lt;Annotation, CodeFileAction&gt;)</param>
        public void UpdateUnlinkedAnnotationsByAnnotation(Field field, FieldAnnotation annotation, object configuration)
        {
            var actions = configuration as Dictionary<Annotation, CodeFileAction>;
            if (actions == null)
            {
                Log("The list of actions to perform is null or of the wrong type");
                return;
            }

            // If there is no action specified for this field, we will exit.  This should happen when we have fields that
            // are still linked in a document.
            if (!actions.ContainsKey(annotation))
            {
                Log(string.Format("No action is needed for annotation {0}", annotation.Id));
                return;
            }

            // Make sure that the action is actually defined.  If no action was specified by the user, we can't continue
            // with doing anything.
            var action = actions[annotation];
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
                    Log(string.Format("Changing annotation {0} from {1} to {2}",
                        annotation.OutputLabel, annotation.CodeFilePath, codeFile.FilePath));
                    annotation.CodeFile = codeFile;
                    UpdateAnnotationFieldData(field, annotation);
                    break;
                case Constants.CodeFileActionTask.RemoveAnnotations:
                    Log(string.Format("Removing {0}", annotation.OutputLabel));
                    field.Select();
                    var application = Globals.ThisAddIn.Application;
                    application.Selection.Text = Constants.Placeholders.RemovedField;
                    application.Selection.Range.HighlightColorIndex = WdColorIndex.wdYellow;
                    break;
                case Constants.CodeFileActionTask.ReAddFile:
                    Log(string.Format("Linking code file {0}", annotation.CodeFilePath));
                    DocumentManager.AddCodeFile(annotation.CodeFilePath);
                    break;
                default:
                    Log(string.Format("The action task of {0} is not known and will be skipped", action.Action));
                    break;
            }
        }
    }
}
