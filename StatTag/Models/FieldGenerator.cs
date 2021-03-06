﻿//------------------------------------------------------------------------------
// <copyright file="FieldCreator.cs" company="Florian Wolters">
//     Copyright (c) Florian Wolters. All rights reserved.
// </copyright>
// <author>Florian Wolters &lt;wolters.fl@gmail.com&gt;</author>
// <source>https://gist.github.com/FlorianWolters/6257233</source>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using StatTag.Core.Models;
using Word = Microsoft.Office.Interop.Word;

namespace StatTag.Models
{
    /// <summary>
    /// The class <see cref="FieldGenerator"/> simplifies the creation of <see cref="Word.Field"/>s.
    /// </summary>
    public class FieldGenerator
    {
        public const string FieldOpen = "<";
        public const string FieldClose = ">";

        /// <summary>
        /// Insert a StatTag nested field at the document range specified in the parameters.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="tagIdentifier"></param>
        /// <param name="displayValue"></param>
        /// <param name="tag"></param>
        public static void GenerateField(Word.Range range, string tagIdentifier, string displayValue, FieldTag tag)
        {
            var fields = InsertField(range, string.Format("{3}MacroButton {0} {1}{3}ADDIN {2}{4}{4}",
                Constants.FieldDetails.MacroButtonName, displayValue, tagIdentifier, FieldOpen, FieldClose));
            var dataField = fields.First();
            dataField.Data = tag.Serialize();

            // This is a terrible hack, I know, but it's the only way I've found to get fields
            // to appear correctly after doing this insert.
            range.Document.Fields.ToggleShowCodes();
            range.Document.Fields.ToggleShowCodes();
        }

        /// <summary>
        /// Adds one or more new <see cref="Word.Field"/> to the specified <see cref="Word.Range"/>.
        /// <para>
        /// This method allows to insert nested fields at the specified range.
        /// </para>
        /// <example>
        /// <c>InsertField(Application.Selection.Range, {{= {{PAGE}} - 1}};</c>
        /// will produce
        /// { = { PAGE } - 1 }
        /// </example>
        /// </summary>
        /// <param name="range">The <see cref="Word.Range"/> where to add the <see cref="Word.Field"/>.</param>
        /// <param name="theString">The string to convert to one or more <see cref="Word.Field"/> objects.</param>
        /// <param name="fieldOpen">The special code to mark the start of a <see cref="Word.Field"/>.</param>
        /// <param name="fieldClose">The special code to mark the end of a <see cref="Word.Field"/>.</param>
        /// <returns>The newly created <see cref="Word.Field"/></returns>
        /// <remarks>
        /// A solution for VBA has been taken from <a href="http://stoptyping.co.uk/word/nested-fields-in-vba">this</a>
        /// article and adopted for C# by the author.
        /// </remarks>
        private static Word.Field[] InsertField(
            Word.Range range,
            string theString = FieldOpen + FieldClose,
            string fieldOpen = FieldOpen,
            string fieldClose = FieldClose)
        {
            if (null == range)
            {
                throw new ArgumentNullException("range");
            }

            if (string.IsNullOrEmpty(fieldOpen))
            {
                throw new ArgumentException("fieldOpen");
            }

            if (string.IsNullOrEmpty(fieldClose))
            {
                throw new ArgumentException("fieldClose");
            }

            if (!theString.Contains(fieldOpen) || !theString.Contains(fieldClose))
            {
                throw new ArgumentException("theString");
            }

            var fields = new List<Word.Field>();
            // Special case. If we do not check this, the algorithm breaks.
            if (theString == fieldOpen + fieldClose)
            {
                fields.Add(InsertEmpty(range));
                return fields.ToArray();
            }

            // TODO Implement additional error handling.
            Word.Field result = null;
            var fieldStack = new Stack<Word.Range>();

            range.Text = theString;
            fieldStack.Push(range);

            Word.Range searchRange = range.Duplicate;
            Word.Range fieldRange = null;

            while (searchRange.Start != searchRange.End)
            {
                Word.Range nextOpen = FindNextOpen(searchRange.Duplicate, fieldOpen);
                Word.Range nextClose = FindNextClose(searchRange.Duplicate, fieldClose);

                if (null == nextClose)
                {
                    break;
                }

                // See which marker comes first.
                if (nextOpen.Start < nextClose.Start)
                {
                    nextOpen.Text = string.Empty;
                    searchRange.Start = nextOpen.End;

                    // Field open, so push a new range to the stack.
                    fieldStack.Push(nextOpen.Duplicate);
                }
                else
                {
                    nextClose.Text = string.Empty;

                    // Move start of main search region onwards past the end marker.
                    searchRange.Start = nextClose.End;

                    // Field close, so pop the last range from the stack and insert the field.
                    fieldRange = fieldStack.Pop();
                    fieldRange.End = nextClose.End;
                    result = InsertEmpty(fieldRange);
                    fields.Add(result);
                }
            }

            // To avoid having a blank space at the end of the field, we need to explicitly trim
            // out a blank space that exists between the nested field delimiters.
            var spaceRange = fieldRange.Duplicate;
            spaceRange.Start = spaceRange.End;
            spaceRange.End = spaceRange.Start;
            spaceRange.Delete(Word.WdUnits.wdCharacter, 1);

            // Move the current selection after all inserted fields.
            int newPos = fieldRange.End + fieldRange.Fields.Count + 1;
            fieldRange.SetRange(newPos, newPos);
            fieldRange.Select();

            // Update the result of the outer field object.
            if (result != null)
            {
                result.Update();
            }

            return fields.ToArray();
        }

        /// <summary>
        /// Adds a new empty <see cref="Word.Field"/> to the specified <see cref="Word.Range"/>.
        /// </summary>
        /// <param name="range">The <see cref="Word.Range"/> where to add the <see cref="Word.Field"/>.</param>
        /// <param name="preserveFormatting">
        /// Whether to apply the formatting of the previous <see cref="Word.Field"/> result to the new result.
        /// </param>
        /// <returns>The newly created <see cref="Word.Field"/>.</returns>
        private static Word.Field InsertEmpty(Word.Range range, bool preserveFormatting = false)
        {
            Word.Field result = AddFieldToRange(range, Word.WdFieldType.wdFieldEmpty, preserveFormatting);
            return result;
        }

        /// <summary>
        /// Creates a <see cref="Word.Field"/> and adds it to the specified <see cref="Word.Range"/>
        /// </summary>
        /// <remarks>
        /// The <see cref="Word.Field"/> is added to the <see cref="Word.Fields"/> collection of the specified <see
        /// cref="Word.Range"/>.
        /// </remarks>
        /// <param name="range">The <see cref="Word.Range"/> where to add the <see cref="Word.Field"/>.</param>
        /// <param name="type">The type of <see cref="Word.Field"/> to create.</param>
        /// <param name="preserveFormatting">
        /// Whether to apply the formatting of the previous field result to the new result.
        /// </param>
        /// <param name="text">Additional text needed for the <see cref="Word.Field"/>.</param>
        /// <returns>The newly created <see cref="Word.Field"/>.</returns>
        private static Word.Field AddFieldToRange(
            Word.Range range,
            Word.WdFieldType type,
            bool preserveFormatting = false,
            string text = null)
        {
            return range.Fields.Add(
                range,
                type,
                text ?? Type.Missing,
                preserveFormatting);
        }

        private static Word.Range FindNextOpen(Word.Range range, string text)
        {
            Word.Find find = CreateFind(range, text);
            Word.Range result = range.Duplicate;

            if (!find.Found)
            {
                // Make sure that the next closing field will be found first.
                result.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            }

            return result;
        }

        private static Word.Range FindNextClose(Word.Range range, string text)
        {
            return CreateFind(range, text).Found ? range.Duplicate : null;
        }

        private static Word.Find CreateFind(Word.Range range, string text)
        {
            Word.Find result = range.Find;
            result.Execute(FindText: text, Forward: true, Wrap: Word.WdFindWrap.wdFindStop);

            return result;
        }
    }
}
