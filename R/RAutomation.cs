using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using RDotNet.Internals;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using StatTag.Core.Parser;

namespace R
{
    public class RAutomation : IStatAutomation
    {
        private REngine Engine = null;
        protected RParser Parser { get; set; }

        public RAutomation()
        {
            Parser = new RParser();
        }

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
            //if (Engine != null)
            //{
            //    Engine.Dispose();
            //    Engine = null;
            //}
        }

        public StatTag.Core.Models.CommandResult[] RunCommands(string[] commands)
        {
            var commandResults = new List<CommandResult>();
            foreach (var command in commands)
            {
                var result = RunCommand(command);
                if (result != null && !result.IsEmpty())
                {
                    commandResults.Add(result);
                }
            }

            return commandResults.ToArray();
        }

        public CommandResult RunCommand(string command)
        {
            var result = Engine.Evaluate(command);
            if (result == null || result.IsInvalid)
            {
                return null;
            }

            // If we have a value command, we will pull out the last relevant line from the output.
            if (Parser.IsValueDisplay(command))
            {
                return new CommandResult() { ValueResult = GetValueResult(result) };
            }


            return null;
        }

        public bool IsReturnable(string command)
        {
            return true;
        }

        public string GetInitializationErrorMessage()
        {
            throw new NotImplementedException();
        }

        private string GetValueResult(SymbolicExpression result)
        {
            switch (result.Type)
            {
                case SymbolicExpressionType.NumericVector:
                    return result.AsNumeric().First().ToString();
                case SymbolicExpressionType.IntegerVector:
                    return result.AsInteger().First().ToString();
                case SymbolicExpressionType.CharacterVector:
                    return result.AsCharacter().First();
            }
            return null;
        }
    }
}
