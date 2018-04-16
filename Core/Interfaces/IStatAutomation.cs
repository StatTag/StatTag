using System;
using StatTag.Core.Models;

namespace StatTag.Core.Interfaces
{
    public interface IStatAutomation : IDisposable
    {
        StatPackageState State { get; set; }
        bool Initialize(CodeFile file);
        CommandResult[] RunCommands(string[] commands, Tag tag = null);
        bool IsReturnable(string command);
        string GetInitializationErrorMessage();
        void Hide();

        /// <summary>
        /// If an exception is thrown during execution, this method can be used to
        /// format a more helpful error message given the raw exception.
        /// </summary>
        /// <param name="exc"></param>
        /// <returns></returns>
        string FormatErrorMessageFromExecution(Exception exc);
    }
}