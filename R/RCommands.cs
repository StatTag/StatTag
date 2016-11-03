using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Interfaces;
using StatTag.Core.Parser;

namespace R
{
    public class RCommands : IResultCommandFormatter
    {
        public class ValueCommands : IResultCommandList
        {
            public string[] GetCommands()
            {
                return RParser.ValueCommands;
            }
        }

        public class FigureCommands : IResultCommandList
        {
            public string[] GetCommands()
            {
                return RParser.FigureCommands;
            }
        }

        public class TableCommands : IResultCommandList
        {
            public const string MatrixList = "TODO";

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
