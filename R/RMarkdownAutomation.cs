using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;
using StatTag.Core.Parser;

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
