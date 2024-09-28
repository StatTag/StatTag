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
    public class UserSettings : ICloneable
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
        /// How StatTag should select the version of R to use.  The allowed values
        /// should come from Constants.RDetectionOption
        /// </summary>
        public string RDetection { get; set; }

        /// <summary>
        /// If the user wants to select the version of R to use, this is the
        /// path to the root R directory to use
        /// </summary>
        public string RLocation { get; set; }

        /// <summary>
        /// If the user needs to navigate to and define a custom R path because
        /// it's not discoverable (not in the registry), this contains that path.
        /// We will save it so the user doesn't have to re-enter it each time,
        /// although it may not be the version of R they have selected (RSelectedPath).
        /// A user may have multiple custom R paths to define, however we are
        /// choosing to only save the last one.
        /// </summary>
        public string RCustomPath { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

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

        public bool RSettingsChanged(UserSettings oldSettings)
        {
            if (oldSettings == null)
            {
                return true;
            }

            // If how we're detecting R changes, we're going to flag that as enough of a change.
            // Technically the auto-detect and the manually selected versions could be the same,
            // but even in that scenario we're going to call this a "change".
            if (!string.Equals(oldSettings.RDetection, this.RDetection))
            {
                return true;
            }

            // If the R location was changed at all, consider that a change.  Again, there are
            // fringe cases where this may not actually be "different", but we are being lenient
            // on what we consider a change.
            if (!string.Equals(oldSettings.RLocation, this.RLocation))
            {
                return true;
            }

            // Note that we are not looking at RCustomPath.  This is because that doesn't make
            // a difference for whether or not the active R is changed.
            return false;
        }
    }
}
