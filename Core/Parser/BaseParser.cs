using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Parser
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

        public Annotation[] Parse(IList<string> lines, int filterMode = Constants.ParserFilterMode.IncludeAll)
        {
            SetupRegEx();

            var annotations = new List<Annotation>();
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
                        LineStart = index
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
                return annotations.Where(x => x.RunFrequency == Constants.RunFrequency.Always).ToArray();
            }

            return annotations.ToArray();
        }

        public string[] Filter(IList<string> lines, int filterMode = Constants.ParserFilterMode.IncludeAll)
        {
            SetupRegEx();

            if (lines == null)
            {
                return null;
            }

            var filteredLines = new List<string>();
            var isSkipping = false;
            int? startIndex = null;
            for (var index = 0; index < lines.Count(); index++)
            {
                var line = lines[index].Trim();
                var match = StartAnnotationRegEx.Match(line);
                if (match.Success)
                {
                    var annotation = new Annotation();
                    startIndex = index;
                    ProcessAnnotation(match.Groups[1].Value, annotation);
                    if (filterMode == Constants.ParserFilterMode.ExcludeOnDemand
                        && annotation.RunFrequency == Constants.RunFrequency.OnDemand)
                    {
                        isSkipping = true;
                    }
                    else
                    {
                        filteredLines.Add(lines[index]);
                    }
                }
                else if (startIndex.HasValue)
                {
                    if (!isSkipping)
                    {
                        filteredLines.Add(lines[index]);
                    }

                    match = EndAnnotationRegEx.Match(line);
                    if (match.Success)
                    {
                        isSkipping = false;
                        startIndex = null;
                    }
                }
                else if (!isSkipping)
                {
                    filteredLines.Add(lines[index]);
                }
            }

            return filteredLines.ToArray();
        }

        protected void ProcessAnnotation(string annotationText, Annotation annotation)
        {
            if (annotationText.StartsWith(Constants.AnnotationType.Value))
            {
                annotation.Type = Constants.AnnotationType.Value;
                ValueParser.Parse(annotationText, annotation);
            }
        }
    }
}
