using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Models
{
    public class GridDataItem
    {
        public string Display { get; set; }
        public CodeFileAction Data { get; set; }


        /// <summary>
        /// Utility method to create an action item for our combo box column
        /// </summary>
        /// <remarks>We're wrapping this up to facilitate the DataGridView and how it works.  The GridDataItem lets us
        /// have a display and value property, and the value (the CodeFileAction) can then be picked up and shared when
        /// it is selected.</remarks>
        /// <param name="display">What to display in the combo box</param>
        /// <param name="action">The resulting action to perform</param>
        /// <param name="parameter">Optional - parameter to use with the specified action.</param>
        /// <returns>The created GridDataItem that wraps and contains a CodeFileAction</returns>
        public static GridDataItem CreateActionItem(string display, int action, object parameter)
        {
            return new GridDataItem()
            {
                Display = display,
                Data = new CodeFileAction()
                {
                    Label = display,
                    Action = action,
                    Parameter = parameter
                }
            };
        }
    }
}
