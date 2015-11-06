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

        protected readonly Regex StartAnnotationRegEx = null;
        protected readonly Regex EndAnnotationRegEx = null;

        protected BaseParser()
        {
            StartAnnotationRegEx = new Regex(string.Format(@"\s*[\{0}]{{2,}}\s*{1}\s*{2}.*", CommentCharacter, StartAnnotation, AnnotationPrefix), RegexOptions.Singleline);
            EndAnnotationRegEx = new Regex(string.Format(@"\s*[\{0}]{{2,}}\s*{1}", CommentCharacter, EndAnnotation, AnnotationPrefix), RegexOptions.Singleline);
        }

        protected Match DetectAnnotation(Regex annotationRegex, string line)
        {
            if (line == null)
            {
                line = string.Empty;
            }

            return annotationRegex.Match(line);
        }

        public Annotation[] Parse(string[] lines)
        {
            var annotations = new List<Annotation>();
            if (lines == null)
            {
                return annotations.ToArray();
            }

            
            foreach (var line in lines)
            {
                // Strip whitespace
                
            }

            return annotations.ToArray();
        }
    }
}
