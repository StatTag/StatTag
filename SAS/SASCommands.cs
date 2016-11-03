using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Interfaces;
using StatTag.Core.Parser;

namespace SAS
{
    public class SASCommands : IResultCommandFormatter
    {
        public class ValueCommands : IResultCommandList
        {
            public string[] GetCommands()
            {
                return new[] { SASParser.ValueCommand };
            }
        }

        public class FigureCommands : IResultCommandList
        {
            public string[] GetCommands()
            {
                return new[] { SASParser.FigureCommand.ToUpper() };
            }
        }

        public class TableCommands : IResultCommandList
        {
            public string[] GetCommands()
            {
                return new[] { SASParser.TableCommand.ToUpper() };
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
