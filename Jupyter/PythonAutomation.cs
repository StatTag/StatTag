using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JupyterKernelManager;
using StatTag.Core.Exceptions;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;
using StatTag.Core.Parser;
using StatTag.Core.Utility;

namespace Jupyter
{
    public class PythonAutomation : JupyterAutomation
    {
        public const string PythonKernelName = "python3";
        protected override sealed ICodeFileParser Parser { get; set; }

        public const int MAX_ARRAY_DEPTH = 2;
        private const char ArrayStart = '[';
        private const char ArrayEnd = ']';
        private const char Escape = '\\';
        private const char Delimiter = ',';
        private const char SingleQuote = '\'';
        private const char DoubleQuote = '"';

        public PythonAutomation()
            : base(PythonKernelName)
        {
            Parser = new PythonParser();
        }

        public override CommandResult HandleTableResult(Tag tag, string command, List<Message> result)
        {
            if (tag.Type.Equals(Constants.TagType.Table) && Parser.IsTableResult(command))
            {
                var value = GetValueResult(result.FirstOrDefault());
                return new CommandResult() { TableResult = ParseTableResult(value) };
            }

            return null;
        }

        /// <summary>
        /// Take a string result from the Python kernel and convert it into a Table structure that StatTag
        /// can use to populate a table in Word.
        /// </summary>
        /// <param name="valueString">The string result from the Python kernel</param>
        /// <returns>A populated Table structure</returns>
        public Table ParseTableResult(string valueString)
        {
            // Go through each character and process the state change for each symbol, collecting data
            // along the way.  And yes, we are ignoring the nuances of Python collections and just referring
            // to them as "arrays" within the code.
            int arrayDepth = 0;
            int rows = 0;
            int cols = 0;
            int maxCols = 0;
            string currentValue = "";
            char? activeQuoteChar = null;
            bool rowDataTracked = false;
            bool inOpenArray = false;
            bool isEscaped = false;
            var data = new List<List<string>>();
            foreach (var letter in valueString.ToCharArray())
            {
                if (isEscaped)
                {
                    isEscaped = false;
                    // Fall through so we pick up the escaped character
                }
                else if (letter.Equals(Escape))
                {
                    isEscaped = true;
                    continue;
                }
                else if (letter.Equals(ArrayStart))
                {
                    rowDataTracked = false;
                    inOpenArray = true;
                    currentValue = "";
                    arrayDepth++;
                    cols = 0;

                    if (arrayDepth > MAX_ARRAY_DEPTH)
                    {
                        throw new StatTagUserException("StatTag is only able to handle 2-dimensional collections within Python");
                    }
                    continue;
                }
                else if (letter.Equals(ArrayEnd))
                {
                    // Close out the curent array if we have some data.
                    if (rowDataTracked)
                    {
                        cols++;
                        maxCols = Math.Max(maxCols, cols);
                        data[rows - 1].Add(currentValue);
                    }
                    currentValue = "";
                    arrayDepth--;
                    cols = 0;
                    inOpenArray = false;
                    rowDataTracked = false;
                    continue;
                }
                else if (letter.Equals(Delimiter))
                {
                    // If we are outside of the array and find a delimiter between sub-arrays, we don't want to track this as
                    // having found data.
                    if (!inOpenArray)
                    {
                        continue;
                    }

                    cols++;
                    maxCols = Math.Max(maxCols, cols);
                    data[rows - 1].Add(currentValue);
                    currentValue = "";
                    continue;
                }
                else if ((letter.Equals(SingleQuote) || letter.Equals(DoubleQuote)))
                {
                    if (!activeQuoteChar.HasValue)
                    {
                        // We have come across a quote of some sort, and we're 
                        activeQuoteChar = letter;
                        continue;
                    }
                    else if (activeQuoteChar.Value.Equals(letter))
                    {
                        // Only close out the quote if it's a match.
                        activeQuoteChar = null;
                        continue;
                    }

                    // Otherwise, fall through and pick up the quote character because it's part of a string literal
                    // that we are tracking.
                }
                else if ((letter.Equals(' ') || letter.Equals('\t')) && !activeQuoteChar.HasValue)
                {
                    // If we're not in a quoted string, we don't want to capture any whitespace.  Because we need to
                    // respect the whitespace if it's quoted, we can't just trim the string at the end, so we have this
                    // check in place.
                    continue;
                }

                // If we haven't ruled it out, this character is part of a value that we are tracking and should
                // be included in the string.
                currentValue += letter;

                // If this is our first time tracking data for our row, we need to initialize it
                if (!rowDataTracked)
                {
                    rowDataTracked = true;
                    rows++;
                    data.Add(new List<string>());
                }
            }

            var table = new Table(rows, maxCols, TableUtil.MergeTableVectorsToArray(null, null, FlattenDataToArray(rows, maxCols, data), rows, maxCols));
            return table;
        }

        /// <summary>
        /// Because of how we built StatTag, we have some expectations and existing routines to flatten data to a 1D array.
        /// This is the implementation for Python results.  In the future we should consider revisiting this to avoid
        /// the extra processing.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string[] FlattenDataToArray(int rows, int cols, List<List<string>> data)
        {
            if (rows == 0 || cols == 0)
            {
                return null;
            }

            var flattenedData = new string[rows*cols];
            for (int row = 0; row < rows; row++)
            {
                var rowCols = data[row].Count;
                for (int col = 0; col < cols; col++)
                {
                    // Not every row is guaranteed to have the same number of columns.  We will perform checks to see if
                    // that's the case, and if so we will provide a null placeholder.
                    flattenedData[(row * cols) + col] = col >= rowCols ? null : data[row][col];
                }
            }

            return flattenedData;
        }

    }
}
