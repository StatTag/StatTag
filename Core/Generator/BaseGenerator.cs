using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Generator
{
    public abstract class BaseGenerator : IGenerator
    {
        public abstract string CommentCharacter { get; }
        
        public string CreateOpenTagBase()
        {
            return string.Format("{0}{0}{1}{2}", CommentCharacter, Constants.AnnotationTags.StartAnnotation,
                Constants.AnnotationTags.AnnotationPrefix);
        }

        public string CreateClosingTag()
        {
            return string.Format("{0}{0}{1}", CommentCharacter, Constants.AnnotationTags.EndAnnotation);
        }

        public string CreateOpenTag(Annotation annotation)
        {
            string openBase = CreateOpenTagBase();
            if (annotation != null)
            {
                if (annotation.Type.Equals(Constants.AnnotationType.Value))
                {
                    var valueGenerator = new ValueGenerator();
                    openBase += string.Format("{0}{1}{2}{3}", Constants.AnnotationType.Value,
                        Constants.AnnotationTags.ParamStart, valueGenerator.CreateParameters(annotation),
                        Constants.AnnotationTags.ParamEnd);
                }
            }

            return openBase;
        }
    }
}
