﻿using System;
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
                return SASParser.ValueCommands;
            }
        }

        public class FigureCommands : IResultCommandList
        {
            public string[] GetCommands()
            {
                return SASParser.FigureCommands.Select(x => x.ToUpper()).ToArray();
            }
        }

        public class TableCommands : IResultCommandList
        {
            public string[] GetCommands()
            {
                return SASParser.TableCommands.Select(x => x.ToUpper()).ToArray();
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
