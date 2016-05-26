using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Interfaces;

namespace Stata
{
    public class StataCommands : IResultCommandFormatter
    {
        public class ValueCommands : IResultCommandList
        {
            public const string Display = "display";

            public string[] GetCommands()
            {
                return new [] { Display };
            }
        }

        public class FigureCommands : IResultCommandList
        {
            public const string GraphExport = "graph export";

            public string[] GetCommands()
            {
                return new[] { GraphExport };
            }
        }

        public class TableCommands : IResultCommandList
        {
            public const string MatrixList = "matrix list";

            public string[] GetCommands()
            {
                return new[] { MatrixList };
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
    }
}
