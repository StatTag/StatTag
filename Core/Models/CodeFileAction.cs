using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisManager.Core.Models
{
    /// <summary>
    /// Used to specify an action to take when a code file is unlinked from a document, and there
    /// are annotations referenced in the document that it depends on.
    /// </summary>
    public class CodeFileAction
    {
        /// <summary>
        /// The description of the action (for the UI)
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The action that should be performed, from Constants.CodeFileActionTask
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// An optional parameter associated with an action.  For example, linking to a new file will
        /// specify the file to link to as the parameter.
        /// </summary>
        public object Parameter { get; set; }

        public override string ToString()
        {
            return Label;
        }
    }
}
