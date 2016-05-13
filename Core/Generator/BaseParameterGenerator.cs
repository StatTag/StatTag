using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Generator
{
    public class BaseParameterGenerator
    {
        public string GetLabelParameter(Tag tag)
        {
            if (!string.IsNullOrWhiteSpace(tag.OutputLabel))
            {
                return string.Format("{0}=\"{1}\", ", Constants.TagParameters.Label, tag.OutputLabel);
            }

            return string.Empty;
        }

        public string GetRunFrequencyParameter(Tag tag)
        {
            if (!string.IsNullOrWhiteSpace(tag.RunFrequency))
            {
                return string.Format("{0}=\"{1}\", ", Constants.TagParameters.Frequency, tag.RunFrequency);
            }

            return string.Empty;
        }

        public string CleanResult(string result)
        {
            return result.Trim().Trim(new[] {','});
        }
    }
}
