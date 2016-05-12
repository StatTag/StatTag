using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Core.ValueFormatter
{
    public interface IValueFormatter
    {
        /// <summary>
        /// Given a string value that has gone through normal formatting, perform the final formatting step
        /// that is dependent on the statistical package.
        /// <remarks>Currently this just handles formatting empty/missing values.</remarks>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string Finalize(string value);

        /// <summary>
        /// Provide the string that should be used to represent a missing value.  This typically differs by
        /// statistical package, but could be extended to user preference.
        /// </summary>
        /// <returns></returns>
        string GetMissingValue();
    }
}
