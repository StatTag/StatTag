using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.ValueFormatter
{
    /// <summary>
    /// <remarks>11/17 - Changed the underlying behavior to only consider a "missing" value to be when a string is null
    /// or empty.  Previously (prior to 3.2) we would consider a whitespace string to be a missing value.</remarks>
    /// </summary>
    public class BaseValueFormatter : IValueFormatter
    {
        public const string MissingValue = "";

        public virtual string GetMissingValue()
        {
            return MissingValue;
        }

        public virtual string Finalize(string value, DocumentMetadata properties)
        {
            // If there is no value, look at our properties to see how we should behave
            // for formatting empty results.
            if (string.IsNullOrEmpty(value))
            {
                if (properties == null
                    || string.IsNullOrEmpty(properties.RepresentMissingValues)
                    || properties.RepresentMissingValues.Equals(Constants.MissingValueOption.StatPackageDefault))
                {
                    return GetMissingValue();
                }

                if (properties.RepresentMissingValues.Equals(Constants.MissingValueOption.BlankString))
                {
                    return string.Empty;
                }

                if (properties.RepresentMissingValues.Equals(Constants.MissingValueOption.CustomValue))
                {
                    return properties.CustomMissingValue ?? string.Empty;
                }
            }

            // If no special processing was otherwise handled above, we have a catch-all check here to return
            // the missing value if value exists (default behavior).
            return string.IsNullOrEmpty(value) ? GetMissingValue() : value;
        }
    }
}
