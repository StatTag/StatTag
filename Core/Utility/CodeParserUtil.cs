using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StatTag.Core.Utility
{
    public class CodeParserUtil
    {
        private static readonly Regex TrailingLineComment = new Regex("(?<!\\*)\\/\\/[^\\r\\n]*");

        /// <summary>
        /// Takes trailing comments and strips them from the input string.  This accounts for newlines so that
        /// all trailing comments in a single string are managed, and it doesn't remove all of the text after
        /// the first comment start it sees.
        /// </summary>
        /// <example>
        /// Input
        ///     Test line one // comment\r\nTest line two
        /// Output
        ///     Test line one \r\nTest line two
        /// </example>
        /// <param name="originalText"></param>
        /// <returns></returns>
        public static string StripTrailingComments(string originalText)
        {
            if (string.IsNullOrWhiteSpace(originalText))
            {
                return originalText;
            }

            return TrailingLineComment.Replace(originalText, "");
        }
    }
}
