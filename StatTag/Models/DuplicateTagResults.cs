using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Models
{
    [Serializable]
    public class DuplicateTagResults : Dictionary<CodeFile, Dictionary<Tag, List<Tag>>>
    {
    }
}
