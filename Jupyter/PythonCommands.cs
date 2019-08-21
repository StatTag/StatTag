using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Interfaces;
using StatTag.Core.Parser;

namespace Jupyter
{
    public class PythonCommands : IResultCommandFormatter
    {
        public class ValueCommands : IResultCommandList
        {
            public string[] GetCommands()
            {
                return new[] { "(Any command that returns a value)" };
            }
        }

        public class FigureCommands : IResultCommandList
        {
            public string[] GetCommands()
            {
                return PythonParser.FigureCommands.Select(x => x.ToUpper()).ToArray();
            }
        }

        public class TableCommands : IResultCommandList
        {
            public string[] GetCommands()
            {
                return new[] { "(Any command that returns a data frame, matrix, vector or list)" };
            }
        }

        public class VerbatimCommands : IResultCommandList
        {
            public string[] GetCommands()
            {
                return new[] { "(Any command)" };
            }
        }

        public IResultCommandList ValueResultCommands()
        {
            return new ValueCommands();
        }

        public IResultCommandList FigureResultCommands()
        {
            return new FigureCommands();
        }

        public IResultCommandList TableResultCommands()
        {
            return new TableCommands();
        }

        public IResultCommandList VerbatimResultCommands()
        {
            return new VerbatimCommands();
        }
    }
}
