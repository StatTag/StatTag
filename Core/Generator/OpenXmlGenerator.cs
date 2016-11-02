using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Office.Interop.Word;
using StatTag.Core.Models;
using System = Microsoft.Office.Interop.Word.System;

namespace StatTag.Core.Generator
{
    public class OpenXmlGenerator
    {
        /// <summary>
        /// Instead of using booleans, Microsoft is using an int to represent certain
        /// flags like bold and italics, and determined -1 would be True.
        /// </summary>
        public const int WdTrue = -1;

        public const int WdUndefined = 9999999;

        public const int Base64ChunkSize = 76;

        /// <summary>
        /// Build the formatting properties for the current range.
        /// <remarks>We need to do this because inserting an OpenXML block doesn't preserve any
        /// of the original formatting at the cursor location.</remarks>
        /// </summary>
        /// <see>Formatting reference is at: http://officeopenxml.com/WPtextFormatting.php </see>
        /// <see cref="http://www.ecma-international.org/publications/standards/Ecma-376.htm" />
        /// <param name="range"></param>
        /// <returns></returns>
        private static string GetFormat(Range range)
        {
            if (range == null)
            {
                return string.Empty;
            }

            var font = range.Font;
            var builder = new StringBuilder();
            if (font.Bold == WdTrue)
            {
                builder.Append("<w:b/>");
            }
            if (font.Italic == WdTrue)
            {
                builder.Append("<w:i/>");
            }

            // Caps and small caps are mutually exclusive
            if (font.AllCaps == WdTrue)
            {
                builder.Append("<w:caps/>");
            }
            else if (font.SmallCaps == WdTrue)
            {
                builder.Append("<w:smallCaps/>");
            }

            // Strike and double-strike are mutually exclusive
            if (font.StrikeThrough == WdTrue)
            {
                builder.Append("<w:strike/>");
            }
            else if (font.DoubleStrikeThrough == WdTrue)
            {
                builder.Append("<w:dstrike/>");
            }

            if (font.Underline != WdUnderline.wdUnderlineNone)
            {
                builder.Append(GetUnderlineFormat(font.Underline));
            }

            // Superscript and subscript are mutually exclusive
            if (font.Superscript == WdTrue)
            {
                builder.Append("<w:vertAlign w:val=\"superscript\"/>");
            }
            else if (font.Subscript == WdTrue)
            {
                builder.Append("<w:vertAlign w:val=\"subscript\"/>");
            }

            // Disabled initially since it was causing errors (although it appears to match the specification)
            if (range.HighlightColorIndex != WdColorIndex.wdNoHighlight)
            {
                builder.AppendFormat("<w:highlight w:val=\"{0}\"/>", GetHighlightColor(range.HighlightColorIndex));
            }

            // Sometimes we will get an undefined value for the color.  It's not optimal, but we will just revert to
            // the default color (by not specifying a color tag).
            if (WdUndefined != (int)font.Color)
            {
                var color = RgbColorRetriever.GetRGBColor(font.Color, range.Document);
                builder.AppendFormat("<w:color w:val=\"{0}\" />", ColorTranslator.ToHtml(color));
            }

            // Set the font each time, regardless.  Note that size is represented
            // in half points (I have no idea why...) so we need to multiply the
            // font size by 2.
            var fontSize = (font.Size*2).ToString();
            if (font.Size >= WdUndefined)
            {
                fontSize = GetFirstFontSizeInRange(range);
            }

            builder.AppendFormat(
                @"<w:rFonts w:ascii=""{0}"" w:h-ansi=""{0}"" w:cs=""{0}""/>
                    <w:sz w:val=""{1}""/>", font.Name, fontSize);

            return builder.ToString();
        }

        private static string GetFirstFontSizeInRange(Range range)
        {
            var tmp = range.XML;
            var xmlSnippet = new XmlDocument();
            xmlSnippet.LoadXml(tmp);

            var xmlnsManager = new XmlNamespaceManager(xmlSnippet.NameTable);
            xmlnsManager.AddNamespace("w", "http://schemas.microsoft.com/office/word/2003/wordml");
            var nodes = xmlSnippet.SelectNodes("//w:r/w:rPr/w:sz", xmlnsManager);
            if (nodes != null && nodes.Count > 0)
            {
                for (int index = 0; index < nodes.Count; index++)
                {
                    if (nodes[index].Attributes.Count > 0 && nodes[index].Attributes["w:val"] != null)
                    {
                        return nodes[index].Attributes["w:val"].Value;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// There are only a few colors supported in OpenXML/WordML for highlights.  We will explicitly return
        /// only those that we know are valid, or "none" otherwise.
        /// </summary>
        /// <param name="wdColorIndex"></param>
        /// <returns></returns>
        private static string GetHighlightColor(WdColorIndex wdColorIndex)
        {
            switch (wdColorIndex)
            {
                case WdColorIndex.wdBlack:
                    return "black";
                case WdColorIndex.wdBlue:
                    return "blue";
                case WdColorIndex.wdGreen:
                    return "green";
                case WdColorIndex.wdRed:
                    return "red";
                case WdColorIndex.wdYellow:
                    return "yellow";
                case WdColorIndex.wdWhite:
                    return "white";
                case WdColorIndex.wdDarkBlue:
                    return "dark-blue";
                case WdColorIndex.wdDarkRed:
                    return "dark-red";
                case WdColorIndex.wdDarkYellow:
                    return "dark-yellow";
                case WdColorIndex.wdGray50:
                    return "dark-gray";
                case WdColorIndex.wdGray25:
                    return "light-gray";
            }

            return "none";
        }

        /// <summary>
        /// The reference for the underline format values can be found at: http://officeopenxml.com/WPtextFormatting.php
        /// </summary>
        /// <param name="wdUnderline"></param>
        /// <returns></returns>
        private static string GetUnderlineFormat(WdUnderline wdUnderline)
        {
            string underlineType = string.Empty;
            switch (wdUnderline)
            {
                case WdUnderline.wdUnderlineDash:
                    underlineType = "dash";
                    break;
                case WdUnderline.wdUnderlineDashHeavy:
                    underlineType = "dashedHeavy";
                    break;
                case WdUnderline.wdUnderlineDashLong:
                    underlineType = "dashLong";
                    break;
                case WdUnderline.wdUnderlineDashLongHeavy:
                    underlineType = "dashLongHeavy";
                    break;
                case WdUnderline.wdUnderlineDotDash:
                    underlineType = "dotDash";
                    break;
                case WdUnderline.wdUnderlineDotDashHeavy:
                    underlineType = "dotDashHeavy";
                    break;
                case WdUnderline.wdUnderlineDotDotDash:
                    underlineType = "dotDotDash";
                    break;
                case WdUnderline.wdUnderlineDotDotDashHeavy:
                    underlineType = "dotDotDashHeavy";
                    break;
                case WdUnderline.wdUnderlineDotted:
                    underlineType = "dotted";
                    break;
                case WdUnderline.wdUnderlineDottedHeavy:
                    underlineType = "dottedHeavy";
                    break;
                case WdUnderline.wdUnderlineDouble:
                    underlineType = "double";
                    break;
                case WdUnderline.wdUnderlineSingle:
                    underlineType = "single";
                    break;
                case WdUnderline.wdUnderlineThick:
                    underlineType = "thick";
                    break;
                case WdUnderline.wdUnderlineWavy:
                    underlineType = "wave";
                    break;
                case WdUnderline.wdUnderlineWavyDouble:
                    underlineType = "wavyDouble";
                    break;
                case WdUnderline.wdUnderlineWavyHeavy:
                    underlineType = "wavyHeavy";
                    break;
                case WdUnderline.wdUnderlineWords:
                    underlineType = "words";
                    break;
            }

            return string.Format("<w:u w:color=\"auto\" w:val=\"{0}\"/>", underlineType);
        }

        /// <summary>
        /// Create a StatTag field (nested Word fields with associated data) in the Open XML format.  This may be inserted directly
        /// into a Word document via InsertXML.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="tagIdentifier"></param>
        /// <param name="displayValue"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string GenerateField(Range range, string tagIdentifier, string displayValue, FieldTag tag)
        {
            return GenerateField(range, tagIdentifier, displayValue, tag.Serialize());
        }

        /// <summary>
        /// Create a StatTag field (nested Word fields with associated data) in the Open XML format.  This may be inserted directly
        /// into a Word document via InsertXML.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="tagIdentifier"></param>
        /// <param name="displayValue"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GenerateField(Range range, string tagIdentifier, string displayValue, string data)
        {
            var result = string.Format(
                "<w:p xmlns:w=\"http://schemas.microsoft.com/office/word/2003/wordml\">\r\n" +
                "    <w:r>\r\n" +
                "        <w:rPr>\r\n" +
                "            {4}\r\n" +
                "        </w:rPr>\r\n" +
                "        <w:fldChar w:fldCharType=\"begin\" />\r\n" +
                "        <w:instrText xml:space=\"preserve\"> MacroButton {0} {1}</w:instrText>\r\n" +
                "        <w:fldChar w:fldCharType=\"begin\">\r\n" +
                "            <w:fldData xml:space=\"preserve\">{3}</w:fldData>\r\n" +
                "        </w:fldChar>\r\n" +
                "        <w:instrText xml:space=\"preserve\"> ADDIN {2}</w:instrText>\r\n" +
                "        <w:fldChar w:fldCharType=\"end\" />\r\n" +
                "        <w:fldChar w:fldCharType=\"end\" />\r\n" +
                "    </w:r>\r\n" +
                "</w:p>", Constants.FieldDetails.MacroButtonName, displayValue, tagIdentifier, Base64EncodeFieldData(data), GetFormat(range));
            return result;
        }

        /// <summary>
        /// Perform the needed encoding for field data.
        /// </summary>
        /// <param name="text">The JSON text string to encode</param>
        /// <returns>The Base64-encoded representation of the JSON data</returns>
        public static string Base64EncodeFieldData(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            // Word expects Unicode to be used.  If not, the data gets garbled when you try to read it back.
            var bytes = Encoding.Unicode.GetBytes(text);

            // Per the Flat OPC specification for OpenXML, the base64-encoded data must be written out in chunks of 76
            // characters, and not have any leading or trailing spaces. Reference:  https://blogs.msdn.microsoft.com/ericwhite/2008/09/29/the-flat-opc-format/
            // HOWEVER - that doesn't work in Word 2016.  After much trial and error, it appears that you DO need to append
            // \r\n after the base64 encoded data.  This thread of thinking is partially documented at:
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/6d677891-79b9-4218-b881-62cf8bf9aacb/data-limit-for-addin-fields-created-via-insertxml-in-word-2016?forum=vsto
            var data = Convert.ToBase64String(bytes);
            return string.Join("\r\n", Chunk(data, Base64ChunkSize)) + "\r\n";
        }

        // Derived from: http://stackoverflow.com/questions/1450774/splitting-a-string-into-chunks-of-a-certain-size/1450889#1450889
        public static IEnumerable<string> Chunk(string str, int maxChunkSize)
        {
            for (int index = 0; index < str.Length; index += maxChunkSize)
            {
                yield return str.Substring(index, Math.Min(maxChunkSize, str.Length - index));
            }
        }
    }
}
