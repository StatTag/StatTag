using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Core.Models
{
    /// <summary>
    /// User preferences and settings for StatTag.
    /// </summary>
    public class UserSettings
    {
        public const ulong MaxLogFileSizeMin = (1 * Constants.BytesToMegabytesConversion);
        public const ulong MaxLogFileSizeMax = (10000 * Constants.BytesToMegabytesConversion);
        public const ulong MaxLogFileSizeDefault = (10 * Constants.BytesToMegabytesConversion);

        public const ulong MaxLogFilesMin = 1;
        public const ulong MaxLogFilesMax = 100;
        public const ulong MaxLogFilesDefault = 5;

        /// <summary>
        /// The full path to the Stata executable on the user's machine.
        /// </summary>
        public string StataLocation { get; set; }

        /// <summary>
        /// If the user would like to have debug logging enabled.
        /// </summary>
        public bool EnableLogging { get; set; }

        /// <summary>
        /// The path of the log file to write to.
        /// </summary>
        public string LogLocation { get; set; }

        /// <summary>
        /// The maximum size a log file can grow to (in bytes)
        /// </summary>
        public ulong? MaxLogFileSize { get; set; }

        /// <summary>
        /// The maximum number of log files to keep
        /// </summary>
        public ulong? MaxLogFiles { get; set; }

        /// <summary>
        /// Automatically run attached statistical code and update a document
        /// when the Word document is opened.
        /// </summary>
        public bool RunCodeOnOpen { get; set; }

        /// <summary>
        /// How StatTag should represent missing values within the Word document.
        /// The allowed values should come from Constants.MissingValueOption
        /// </summary>
        public string RepresentMissingValues { get; set; }

        /// <summary>
        /// If a missing value is represented by a user-defined string, this will
        /// be the string to use.  It may be set even if another missing value
        /// option is selected, just to preserve the user's previous choice.
        /// </summary>
        public string CustomMissingValue { get; set; }

        /// <summary>
        /// Helper method to return a value that is within our allowed ranges.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public ulong GetValueInRange(ulong? value, ulong min, ulong max, ulong defaultValue)
        {
            if (value.HasValue)
            {
                if (value.Value < min)
                {
                    return min;
                }

                if (value.Value > max)
                {
                    return max;
                }

                return value.Value;
            }

            return defaultValue;
        }
    }
}
