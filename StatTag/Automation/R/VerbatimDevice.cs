using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet.Internals;

namespace R
{
    public class VerbatimDevice : RDotNet.Devices.ICharacterDevice
    {
        protected List<string> Cache { get; set; }
        protected bool CachEnabled { get; set; }

        public VerbatimDevice()
        {
            Cache = new List<string>();
        }

        public void StartCache()
        {
            CachEnabled = true;
            Cache.Clear();
        }

        public void StopCache()
        {
            CachEnabled = false;
        }

        public List<string> GetCache()
        {
            return Cache;
        }


        public new void WriteConsole(string output, int length, RDotNet.Internals.ConsoleOutputType outputType)
        {
            if (CachEnabled)
            {
                Cache.Add(output);
            }
        }

        #region Sink methods (similar to NullCharacterDevice)
        public RDotNet.SymbolicExpression AddHistory(RDotNet.Language call, RDotNet.SymbolicExpression operation, RDotNet.Pairlist args, RDotNet.REnvironment environment)
        {
            return environment.Engine.NilValue;
        }

        public RDotNet.Internals.YesNoCancel Ask(string question)
        {
            return default(YesNoCancel);
        }

        public void Busy(RDotNet.Internals.BusyType which)
        {
        }

        public void Callback()
        {
        }

        public string ChooseFile(bool create)
        {
            return string.Empty;
        }

        public void CleanUp(RDotNet.Internals.StartupSaveAction saveAction, int status, bool runLast)
        {
        }

        public void ClearErrorConsole()
        {
        }

        public void EditFile(string file)
        {
        }

        public void FlushConsole()
        {
        }

        public RDotNet.SymbolicExpression LoadHistory(RDotNet.Language call, RDotNet.SymbolicExpression operation, RDotNet.Pairlist args, RDotNet.REnvironment environment)
        {
            return environment.Engine.NilValue;
        }

        public string ReadConsole(string prompt, int capacity, bool history)
        {
            return string.Empty;
        }

        public void ResetConsole()
        {
        }

        public RDotNet.SymbolicExpression SaveHistory(RDotNet.Language call, RDotNet.SymbolicExpression operation, RDotNet.Pairlist args, RDotNet.REnvironment environment)
        {
            return environment.Engine.NilValue;
        }

        public bool ShowFiles(string[] files, string[] headers, string title, bool delete, string pager)
        {
            return false;
        }

        public void ShowMessage(string message)
        {
        }

        public void Suicide(string message)
        {
        }

        #endregion
    }
}
