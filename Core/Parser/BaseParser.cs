using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;

namespace StatTag.Core.Parser
{
    public abstract class BaseParser : IParser
    {
        public abstract string CommentCharacter { get; }

        protected Regex StartAnnotationRegEx = null;
        protected Regex EndAnnotationRegEx = null;

        protected BaseParser()
        {
            SetupRegEx();
        }

        public abstract bool IsImageExport(string command);
        public abstract string GetImageSaveLocation(string command);
        public abstract bool IsValueDisplay(string command);
        public abstract string GetValueName(string command);
        public abstract bool IsTableResult(string command);
        public abstract string GetTableName(string command);

        /// <summary>
        /// Perform any necessary pre-processing on the original code file content in order
        /// to allow it to be executed.  This may change the number of lines, so don't assume
        /// that the output will have a 1:1 line mapping from the original.
        /// </summary>
        /// <param name="originalContent">The contents as read from the code file</param>
        /// <returns>A list of strings representing the code that should be executed</returns>
        public abstract List<string> PreProcessContent(List<string> originalContent);

        protected Match DetectAnnotation(Regex annotationRegex, string line)
        {
            if (line == null)
            {
                line = string.Empty;
            }

            return annotationRegex.Match(line);
        }

        private void SetupRegEx()
        {
            if (StartAnnotationRegEx == null)
            {
                StartAnnotationRegEx = new Regex(string.Format(@"\s*[\{0}]{{2,}}\s*{1}\s*{2}(.*)", CommentCharacter, Constants.AnnotationTags.StartAnnotation, Constants.AnnotationTags.AnnotationPrefix), RegexOptions.Singleline);
            }

            if (EndAnnotationRegEx == null)
            {
                EndAnnotationRegEx = new Regex(string.Format(@"\s*[\{0}]{{2,}}\s*{1}", CommentCharacter, Constants.AnnotationTags.EndAnnotation, Constants.AnnotationTags.AnnotationPrefix), RegexOptions.Singleline);
            }
        }

        public Annotation[] Parse(CodeFile file,
            int filterMode = Constants.ParserFilterMode.IncludeAll,
            List<Annotation> annotationsToRun = null)
        {
            SetupRegEx();

            var annotations = new List<Annotation>();
            if (file == null)
            {
                return annotations.ToArray();
            }

            var lines = file.LoadFileContent();
            if (lines == null)
            {
                return annotations.ToArray();
            }

            int? startIndex = null;
            Annotation annotation = null;
            for (int index = 0; index < lines.Count(); index++)
            {
                var line = lines[index].Trim();
                var match = StartAnnotationRegEx.Match(line);
                if (match.Success)
                {
                    annotation = new Annotation()
                    {
                        LineStart = index,
                        CodeFile = file
                    };
                    startIndex = index;
                    ProcessAnnotation(match.Groups[1].Value, annotation);
                }
                else if (startIndex.HasValue)
                {
                    match = EndAnnotationRegEx.Match(line);
                    if (match.Success)
                    {
                        annotation.LineStart = startIndex;
                        annotation.LineEnd = index;
                        annotations.Add(annotation);
                        startIndex = null;
                    }
                }
            }

            if (filterMode == Constants.ParserFilterMode.ExcludeOnDemand)
            {
                return annotations.Where(x => x.RunFrequency == Constants.RunFrequency.Default).ToArray();
            }
            else if (filterMode == Constants.ParserFilterMode.AnnotationList && annotationsToRun != null)
            {
                return annotations.Where(x => annotationsToRun.Contains(x)).ToArray();
            }

            return annotations.ToArray();
        }

        public List<ExecutionStep> GetExecutionSteps(CodeFile file,
            int filterMode = Constants.ParserFilterMode.IncludeAll,
            List<Annotation> annotationsToRun = null)
        {
            var executionSteps = new List<ExecutionStep>();
            var lines = PreProcessContent(file.LoadFileContent());
            if (lines == null || lines.Count == 0)
            {
                return executionSteps;
            }

            int? startIndex = null;
            var isSkipping = false;
            ExecutionStep step = null;
            for (var index = 0; index < lines.Count(); index++)
            {
                var line = lines[index].Trim();

                if (step == null)
                {
                    step = new ExecutionStep();
                }

                var match = StartAnnotationRegEx.Match(line);
                if (match.Success)
                {
                    // If the previous code block had content, save it off and create a new one
                    if (step.Code.Count > 0)
                    {
                        executionSteps.Add(step);
                        step = new ExecutionStep();
                    }

                    var annotation = new Annotation() { CodeFile = file };
                    startIndex = index;
                    ProcessAnnotation(match.Groups[1].Value, annotation);
                    if (filterMode == Constants.ParserFilterMode.ExcludeOnDemand
                        && annotation.RunFrequency == Constants.RunFrequency.OnDemand)
                    {
                        isSkipping = true;
                    }
                    else if (filterMode == Constants.ParserFilterMode.AnnotationList
                             && annotationsToRun != null
                             && !annotationsToRun.Contains(annotation))
                    {
                        isSkipping = true;
                    }
                    else
                    {
                        step.Type = Constants.ExecutionStepType.Annotation;
                        step.Annotation = annotation;
                        step.Code.Add(line);
                    }
                }
                else if (startIndex.HasValue)
                {
                    if (!isSkipping)
                    {
                        step.Code.Add(line);
                    }
                    
                    match = EndAnnotationRegEx.Match(line);
                    if (match.Success)
                    {
                        isSkipping = false;
                        startIndex = null;

                        if (step.Code.Count > 0)
                        {
                            executionSteps.Add(step);
                        }
                        step = new ExecutionStep();
                    }
                }
                else if (!isSkipping)
                {
                    step.Code.Add(line);
                }
            }

            if (step != null && step.Code.Count > 0)
            {
                executionSteps.Add(step);
            }

            return executionSteps;
        }

        protected void ProcessAnnotation(string annotationText, Annotation annotation)
        {
            if (annotationText.StartsWith(Constants.AnnotationType.Value))
            {
                annotation.Type = Constants.AnnotationType.Value;
                ValueParameterParser.Parse(annotationText, annotation);
            }
            else if (annotationText.StartsWith(Constants.AnnotationType.Figure))
            {
                annotation.Type = Constants.AnnotationType.Figure;
                FigureParameterParser.Parse(annotationText, annotation);
            }
            else if (annotationText.StartsWith(Constants.AnnotationType.Table))
            {
                annotation.Type = Constants.AnnotationType.Table;
                TableParameterParser.Parse(annotationText, annotation);
                ValueParameterParser.Parse(annotationText, annotation);
            }
            else
            {
                //throw new Exception("Unsupported annotation type");
            }
        }
    }
}
