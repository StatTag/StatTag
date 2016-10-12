using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using StatTag.Core.Interfaces;

namespace R
{
    public class RAutomation : IStatAutomation
    {
        private REngine Engine = null;

        public bool Initialize()
        {
            if (Engine == null)
            {
                REngine.SetEnvironmentVariables(); // <-- May be omitted; the next line would call it.
                Engine = REngine.GetInstance();
            }

            return (Engine != null);
        }

        public void Dispose()
        {
            if (Engine != null)
            {
                Engine.Dispose();
            }
        }

        public StatTag.Core.Models.CommandResult[] RunCommands(string[] commands)
        {
            throw new NotImplementedException();
        }

        public bool IsReturnable(string command)
        {
            throw new NotImplementedException();
        }

        public string GetInitializationErrorMessage()
        {
            throw new NotImplementedException();
        }
    }
}
