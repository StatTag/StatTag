using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Generator
{
    public interface IGenerator
    {
        string CommentCharacter { get; }
        string CreateOpenTagBase();
        string CreateClosingTag();
        string CreateOpenTag(Annotation annotation);
    }
}
