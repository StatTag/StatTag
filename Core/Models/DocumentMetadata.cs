using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StatTag.Core.Models
{
    /// <summary>
    /// This class provides supplemental metadata that we want to track within a
    /// Word document regarding StatTag's use and configuration.
    /// </summary>
    public class DocumentMetadata
    {
        /// <summary>
        /// The version of StatTag last used to save the document
        /// </summary>
        public string StatTagVersion { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static DocumentMetadata Deserialize(string value)
        {
            return JsonConvert.DeserializeObject<DocumentMetadata>(value);
        }
    }
}
