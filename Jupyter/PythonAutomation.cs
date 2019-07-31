using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Interfaces;
using StatTag.Core.Parser;

namespace Jupyter
{
    public class PythonAutomation : JupyterAutomation
    {
        public const string PythonKernelName = "python3";
        protected override sealed ICodeFileParser Parser { get; set; }

        public PythonAutomation()
            : base(PythonKernelName)
        {
            Parser = new PythonParser();
        }

    }
}
