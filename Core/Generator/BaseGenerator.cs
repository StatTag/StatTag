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
                else if (annotation.Type.Equals(Constants.AnnotationType.Figure))
                {
                    var figureGenerator = new FigureGenerator();
                    openBase += string.Format("{0}{1}{2}{3}", Constants.AnnotationType.Figure,
                        Constants.AnnotationTags.ParamStart, figureGenerator.CreateParameters(annotation),
                        Constants.AnnotationTags.ParamEnd);
                }
                else if (annotation.IsTableAnnotation())
                {
                    var tableGenerator = new TableGenerator();
                    openBase += string.Format("{0}{1}{2}{3}", Constants.AnnotationType.Table,
                        Constants.AnnotationTags.ParamStart, CombineValueAndTableParameters(annotation),
                        Constants.AnnotationTags.ParamEnd);
                }
                else
                {
                    throw new Exception("Unsupported annotation type");
                }
            }

            return openBase;
        }

        public string CombineValueAndTableParameters(Annotation annotation)
        {
            var tableGenerator = new TableGenerator();
            var valueGenerator = new ValueGenerator();
            string tableParameters = tableGenerator.CreateParameters(annotation);
            string valueParameters = valueGenerator.CreateValueParameters(annotation);
            var temp = string.Join(", ", new[] {tableParameters, valueParameters});
            temp = temp.Trim().Trim(new [] { ',' }).Trim();
            return temp;
        }
    }
}
