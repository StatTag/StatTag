using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisManager.Core.Parser
{
    /// <summary>
    /// Reads through a file containing stata commands and identifies the blocks of
    /// code that use the Analysis Manager annotation syntax
    /// </summary>
    public sealed class Stata : BaseParser
    {
        public const char StataCommentCharacter = '*';

        public override char CommentCharacter
        {
            get { return StataCommentCharacter; }
        }
    }
}
