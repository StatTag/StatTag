using StatTag.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Core.Generator
{
    public class R : BaseGenerator
    {
        public override string CommentCharacter
        {
            get { return Constants.CodeFileComment.R; }
        }
    }
}
