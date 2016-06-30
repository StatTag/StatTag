using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;

namespace SAS
{
    public class SASAutomation : IDisposable, IStatAutomation
    {
        private SasServer Server = null;

        public string GetInitializationErrorMessage()
        {
            return "Could not communicate with SAS.  SAS may not be fully installed, or be missing some of the automation pieces that StatTag requires.";
        }

        public void Dispose()
        {
            if (Server != null)
            {
                Server.Close();
            }
        }

        public bool Initialize()
        {
            //TODO Do we want to allow remote connections, or just localhost?
            Server = new SasServer()
            {
                UseLocal = true
            };
            Server.Connect();
            return true;
        }

        public CommandResult[] RunCommands(string[] commands)
        {
            Array carriageControls;
            Array lineTypes;
            Array logLines;
            Array listLines;
            Server.Workspace.LanguageService.Submit(string.Join("\r\n", commands));
            //For some reason, these two declarations need to be here
            SAS.LanguageServiceCarriageControl CarriageControl = new SAS.LanguageServiceCarriageControl();
            SAS.LanguageServiceLineType LineType = new SAS.LanguageServiceLineType();

            Server.Workspace.LanguageService.FlushLogLines(10000, out carriageControls, out lineTypes, out logLines);

            Server.Workspace.LanguageService.FlushListLines(1000, out carriageControls, out lineTypes, out listLines);
            return null;
        }

        public bool IsReturnable(string command)
        {
            return false;
        }
    }
}
