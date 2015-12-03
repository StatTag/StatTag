using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Parser
{
    public abstract class BaseParser
    {
        public const string StartAnnotation = ">>>";
        public const string EndAnnotation = "<<<";
        public const string AnnotationPrefix = "AM:";
        public abstract char CommentCharacter { get; }

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
                StartAnnotationRegEx = new Regex(string.Format(@"\s*[\{0}]{{2,}}\s*{1}\s*{2}(.*)", CommentCharacter, StartAnnotation, AnnotationPrefix), RegexOptions.Singleline);
            }

            if (EndAnnotationRegEx == null)
            {
                EndAnnotationRegEx = new Regex(string.Format(@"\s*[\{0}]{{2,}}\s*{1}", CommentCharacter, EndAnnotation, AnnotationPrefix), RegexOptions.Singleline);
            }
        }

        public Annotation[] Parse(string[] lines)
        {
            SetupRegEx();

            var annotations = new List<Annotation>();
            if (lines == null)
            {
                return annotations.ToArray();
            }

            Annotation annotation = null;
            int? startIndex = null;
            for (int index = 0; index < lines.Length; index++)
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
                        annotations.Add(new Annotation()
                        {
                            LineStart = startIndex,
                            LineEnd = index
                        });
                        startIndex = null;
                    }
                }
            }

            return annotations.ToArray();
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
