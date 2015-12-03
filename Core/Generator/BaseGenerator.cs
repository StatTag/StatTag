using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Generator
{
    public abstract class BaseGenerator
    {
        public abstract string CommentCharacter { get; }
        public abstract string CreateOpenTag(Annotation annotation);

        protected string CreateOpenTagBase()
        {
            return string.Format("{0}{0}{1}{2}", CommentCharacter, Constants.AnnotationTags.StartAnnotation,
                Constants.AnnotationTags.AnnotationPrefix);
        }

        public string CreateClosingTag()
        {
            return string.Format("{0}{0}{1}", CommentCharacter, Constants.AnnotationTags.EndAnnotation);
        }
    }
}
