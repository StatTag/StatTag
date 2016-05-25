using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using StatTag.Core.Models;
using System = Microsoft.Office.Interop.Word.System;

namespace StatTag.Models
{
    public class OpenXmlGenerator
    {
        /// <summary>
        /// Create a StatTag field (nested Word fields with associated data) in the Open XML format.  This may be inserted directly
        /// into a Word document via InsertXML.
        /// </summary>
        /// <param name="tagIdentifier"></param>
        /// <param name="displayValue"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string GenerateField(string tagIdentifier, string displayValue, FieldTag tag)
        {
            var result = string.Format(
                @"<w:p xmlns:w=""http://schemas.microsoft.com/office/word/2003/wordml"">
                    <w:r>
                        <w:fldChar w:fldCharType=""begin"" />
                        <w:instrText xml:space=""preserve""> MacroButton {0} {1}</w:instrText>
                        <w:fldChar w:fldCharType=""begin"">
                            <w:fldData xml:space=""preserve"">{3}</w:fldData>
                        </w:fldChar>
                        <w:instrText xml:space=""preserve""> ADDIN {2}</w:instrText>
                        <w:fldChar w:fldCharType=""end"" />
                        <w:fldChar w:fldCharType=""end"" />
                    </w:r>
                </w:p>", Constants.FieldDetails.MacroButtonName, displayValue, tagIdentifier, Base64EncodeFieldData(tag.Serialize()));
            return result;
        }

        /// <summary>
        /// Perform the needed encoding for field data.
        /// </summary>
        /// <param name="text">The JSON text string to encode</param>
        /// <returns>The Base64-encoded representation of the JSON data</returns>
        private static string Base64EncodeFieldData(string text)
        {
            // Word expects Unicode to be used.  If not, the data gets garbled when you try to read it back.
            var bytes = Encoding.Unicode.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }
    }
}
