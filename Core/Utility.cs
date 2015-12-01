using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisManager.Core
{
    public static class Utility
    {
        public static object[] StringArrayToObjectArray(string[] data)
        {
            // Convert to object[] to avoid potential issues (per ReSharper)
            return data.Select(x => x as object).ToArray();
        }
    }
}
