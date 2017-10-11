using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Interfaces;
using StatTag.Core.Parser;

namespace Stata
{
    public class StataCommands : IResultCommandFormatter
    {
        private static string CleanUpRegex(string value)
        {
            return value.Replace("(?:", "[").Replace(")?", "]");
        }

        public class ValueCommands : IResultCommandList
        {
            public string[] GetCommands()
            {
                return StataParser.ValueCommands;
            }
        }

        public class FigureCommands : IResultCommandList
        {
            public string[] GetCommands()
            {
                return StataParser.GraphCommands.Select(CleanUpRegex).ToArray();
            }
        }

        public class TableCommands : IResultCommandList
        {
            public string[] GetCommands()
            {
                return StataParser.TableCommands.Select(CleanUpRegex).ToArray();
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
