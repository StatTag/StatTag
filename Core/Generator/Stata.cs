using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Generator
{
    public class Stata : BaseGenerator
    {
        public override string CommentCharacter
        {
            get { return Constants.CodeFileComment.Stata; }
        }

        //public override string CreateOpenTag(Annotation annotation)
        //{
        //    string openBase = CreateOpenTagBase();

        //    if (annotation.Type.Equals(Constants.AnnotationType.Value))
        //    {
        //        var valueGenerator = new ValueGenerator();
        //        openBase += string.Format("{0}{1}{2}{3}", Constants.AnnotationType.Value, Constants.AnnotationTags.ParamStart, valueGenerator.CreateParameters(annotation), Constants.AnnotationTags.ParamEnd);
        //    }

        //    return openBase;
        //}
    }
}
