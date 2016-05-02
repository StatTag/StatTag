using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Models
{
    public class DuplicateAnnotationResults : Dictionary<CodeFile, Dictionary<Annotation, List<Annotation>>>
    {
    }
}
