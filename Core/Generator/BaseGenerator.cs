using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Generator
{
    public abstract class BaseGenerator : IGenerator
    {
        /// <summary>
        /// The comment character that is used for single line comments
        /// </summary>
        public abstract string CommentCharacter { get; }

        /// <summary>
        /// The suffix expected at the end of a comment (default is none)
        /// </summary>
        public virtual string CommentSuffixCharacter
        {
            get { return Constants.CodeFileCommentSuffix.Default; }
        }
        
        public string CreateOpenTagBase()
        {
            return string.Format("{0}{0}{1}{2}", CommentCharacter, Constants.TagTags.StartTag,
                Constants.TagTags.TagPrefix);
        }

        public string CreateClosingTag()
        {
            return string.Format("{0}{0}{1}{2}", CommentCharacter, Constants.TagTags.EndTag, CommentSuffixCharacter);
        }

        public string CreateOpenTag(Tag tag)
        {
            string openBase = CreateOpenTagBase();
            if (tag != null)
            {
                if (tag.Type.Equals(Constants.TagType.Value))
                {
                    var valueGenerator = new ValueGenerator();
                    openBase += string.Format("{0}{1}{2}{3}", Constants.TagType.Value,
                        Constants.TagTags.ParamStart, valueGenerator.CreateParameters(tag),
                        Constants.TagTags.ParamEnd);
                }
                else if (tag.Type.Equals(Constants.TagType.Figure))
                {
                    var figureGenerator = new FigureGenerator();
                    openBase += string.Format("{0}{1}{2}{3}", Constants.TagType.Figure,
                        Constants.TagTags.ParamStart, figureGenerator.CreateParameters(tag),
                        Constants.TagTags.ParamEnd);
                }
                else if (tag.IsTableTag())
                {
                    var tableGenerator = new TableGenerator();
                    openBase += string.Format("{0}{1}{2}{3}", Constants.TagType.Table,
                        Constants.TagTags.ParamStart, CombineValueAndTableParameters(tag),
                        Constants.TagTags.ParamEnd);
                }
                else
                {
                    throw new Exception("Unsupported tag type");
                }
            }

            openBase += CommentSuffixCharacter;

            return openBase;
        }

        public string CombineValueAndTableParameters(Tag tag)
        {
            var tableGenerator = new TableGenerator();
            var valueGenerator = new ValueGenerator();
            string tableParameters = tableGenerator.CreateParameters(tag);
            string valueParameters = valueGenerator.CreateValueParameters(tag);
            var temp = string.Join(", ", new[] {tableParameters, valueParameters});
            temp = temp.Trim().Trim(new [] { ',' }).Trim();
            return temp;
        }
    }
}
