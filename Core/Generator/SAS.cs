﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Generator
{
    public class SAS : BaseGenerator
    {
        public override string CommentCharacter
        {
            get { return Constants.CodeFileComment.SAS; }
        }

        public override string CommentSuffixCharacter
        {
            get { return Constants.CodeFileCommentSuffix.SAS; }
        }
    }
}
