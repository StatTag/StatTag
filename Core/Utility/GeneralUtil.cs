using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Utility
{
    public static class GeneralUtil
    {
        /// <summary>
        /// Convert a string array to an object array to avoid potential issues (per ReSharper).
        /// </summary>
        /// <param name="data">The string array to convert</param>
        /// <returns>Null if the string array is null, otherwise an object-cast array representation of the original string array.</returns>
        public static object[] StringArrayToObjectArray(string[] data)
        {
            if (data == null)
            {
                return null;
            }

            return data.Select(x => x as object).ToArray();
        }
    }
}
