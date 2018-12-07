using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using StatTag.Core.Models;
using StatTag.Core.Parser;
using StatTag.Core.Utility;

namespace R
{
    public class RMarkdownAutomation : RAutomation
    {
        public RMarkdownAutomation()
        {
            Parser = new RMarkdownParser();
            State = new StatPackageState();
        }
    }
}
