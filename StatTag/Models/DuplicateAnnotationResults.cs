﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Models
{
    public class DuplicateAnnotationResults : Dictionary<CodeFile, Dictionary<Annotation, List<Annotation>>>
    {
    }
}
