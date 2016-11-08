using System;
using StatTag.Core.Models;

namespace StatTag.Core.Interfaces
{
    public interface IStatAutomation : IDisposable
    {
        bool Initialize();
        CommandResult[] RunCommands(string[] commands, Tag tag = null);
        bool IsReturnable(string command);
        string GetInitializationErrorMessage();
    }
}