using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Interfaces;

namespace SAS
{
    public class SASCommands : IResultCommandFormatter
    {
        public class ValueCommands : IResultCommandList
        {
            public const string Display = "%put";

            public string[] GetCommands()
            {
                return new[] { Display };
            }
        }

        public class FigureCommands : IResultCommandList
        {
            //TODO Define this
            public const string GraphExport = "";

            public string[] GetCommands()
            {
                return new[] { GraphExport };
            }
        }

        public class TableCommands : IResultCommandList
        {
            //TODO Define this
            public const string MatrixList = "";

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
