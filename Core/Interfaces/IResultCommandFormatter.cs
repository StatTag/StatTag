using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Core.Interfaces
{
    public interface IResultCommandFormatter
    {
        IResultCommandList ValueResultCommands();
        IResultCommandList FigureResultCommands();
        IResultCommandList TableResultCommands();
    }
}
