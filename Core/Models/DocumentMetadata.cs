using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StatTag.Core.Models
{
    /// <summary>
    /// This class provides supplemental metadata that we want to track within a
    /// Word document regarding StatTag's use and configuration.
    /// </summary>
    public class DocumentMetadata
    {
        /// <summary>
        /// This will be the most recent version.  Please document here any changes in the version over time.
        /// </summary>
        public const string CurrentMetadataFormatVersion = "1.0.0";

        /// <summary>
        /// The version of the format used for metadata in this document (for this class).  This is
        /// defined by the version of StatTag that created the document.
        /// </summary>
        public string MetadataFormatVersion { get; set; }

        /// <summary>
        /// The version of the tag format that is used by this document.  This is defined by the version
        /// of StatTag that created the document.
        /// </summary>
        public string TagFormatVersion { get; set; }

        /// <summary>
        /// The version of StatTag last used to save the document
        /// </summary>
        public string StatTagVersion { get; set; }

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
        /// A catch-all collection of unknown fields that are preserved across the
        /// serialization and de-serialization process.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> ExtraMetadata;

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static DocumentMetadata Deserialize(string value)
        {
            return JsonConvert.DeserializeObject<DocumentMetadata>(value);
        }

        public string GetMissingValueReplacementAsString()
        {
            return GetMissingValueReplacementAsString(this.RepresentMissingValues, this.CustomMissingValue);
        }

        /// <summary>
        /// Take our settings for how missing values are to be represented, and convert them into
        /// a text representation that can be used in the user interface.
        /// </summary>
        /// <returns></returns>
        public static string GetMissingValueReplacementAsString(string representMissingValues, string customMissingValue)
        {
            switch (representMissingValues)
            {
                case Constants.MissingValueOption.BlankString:
                    return "an empty (blank) string";
                case Constants.MissingValueOption.StatPackageDefault:
                    return "the statistical program's default";
                case Constants.MissingValueOption.CustomValue:
                    return string.Format("'{0}'", customMissingValue);
                default:
                    return "an unspecified value";
            }
        }
    }
}
