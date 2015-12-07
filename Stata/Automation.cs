using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using stata;

namespace Stata
{
    public class Automation : IDisposable
    {
        protected stata.StataOLEApp Application { get; set; }

        private const int StataHidden = 1;

        public void Initialize()
        {
            Application = new stata.StataOLEApp();
            //Application.UtilShowStata(StataHidden);
        }

        public bool IsReturnable(string command)
        {
            return command.Trim().StartsWith("display ");
        }

        public string[] RunCommands(string[] commands)
        {
            return commands.Select(command => RunCommand(command)).Where(result => !string.IsNullOrWhiteSpace(result)).ToArray();
        }

        public string RunCommand(string command)
        {
            if (IsReturnable(command))
            {
                return Application.StReturnString(command.Trim().Replace("display ", ""));
            }

            int returnCode = Application.DoCommand(command);
            if (returnCode != 0)
            {
                throw new Exception(string.Format("There was an error while executing the Stata command: {0}", command));
            }
            return null;
        }

        public void Dispose()
        {
            Application = null;
        }
    }
}
