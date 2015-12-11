﻿using System;
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
    }
}
