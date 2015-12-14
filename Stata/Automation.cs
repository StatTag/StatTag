using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Parser;
using stata;

namespace Stata
{
    public class Automation : IDisposable
    {
        protected stata.StataOLEApp Application { get; set; }
        protected IParser Parser { get; set; }

        private const int StataHidden = 1;

        public Automation()
        {
            Parser = new AnalysisManager.Core.Parser.Stata();
        }

        public void Initialize()
        {
            Application = new stata.StataOLEApp();
            Application.UtilShowStata(StataHidden);
        }

        public bool IsReturnable(string command)
        {
            return Parser.IsValueDisplay(command) || Parser.IsImageExport(command);
        }

        public string[] RunCommands(string[] commands)
        {
            return commands.Select(command => RunCommand(command)).Where(result => !string.IsNullOrWhiteSpace(result)).ToArray();
        }

        public string RunCommand(string command)
        {
            if (Parser.IsValueDisplay(command))
            {
                return Application.StReturnString(Parser.GetValueName(command));
            }

            int returnCode = Application.DoCommand(command);
            if (returnCode != 0)
            {
                throw new Exception(string.Format("There was an error while executing the Stata command: {0}", command));
            }

            if (Parser.IsImageExport(command))
            {
                return Parser.GetImageSaveLocation(command);
            }
            
            return null;
        }

        public void Dispose()
        {
            Application = null;
        }
    }
}
