﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Parser
{
    public interface IParser
    {
        Annotation[] Parse(string[] lines);
    }
}
