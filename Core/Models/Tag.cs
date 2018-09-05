using System;
using System.Collections.Generic;
using System.Linq;
using StatTag.Core.Interfaces;
using Newtonsoft.Json;

namespace StatTag.Core.Models
{
    /// <summary>
    /// An tag is a sequence of lines in a CodeFile that is defined by special
    /// comment tags.  It contains configuration information on how to interpret and
    /// format the result of the code block within the document.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// This will be the most recent version.  Please document here any changes in the version over time.
        /// </summary>
        public const string CurrentTagFormatVersion = "1.0.0";

        public const string IdentifierDelimiter = "--";

        [JsonIgnore]
        public CodeFile CodeFile { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string RunFrequency { get; set; }
        public ValueFormat ValueFormat { get; set; }
        public FigureFormat FigureFormat { get; set; }
        public TableFormat TableFormat { get; set; }
        public List<CommandResult> CachedResult { get; set; }

        /// <summary>
        /// Shortcut to the file path of the CodeFile.  This is used for serialization.
        /// </summary>
        public string CodeFilePath
        {
            get
            {
                if (CodeFile != null)
                {
                    return CodeFile.FilePath;
                }

                return string.Empty;
            }

            set
            {
                // If the code file doesn't exist, we will only allocate it if the code file
                // path is not a null or empty string.  This maintains expected behavior where
                // the code file isn't allocated until it has an actual path.
                if (CodeFile == null && !string.IsNullOrEmpty(value))
                {
                    CodeFile = new CodeFile() { FilePath = value };
                }
            }
        }

        public string Id
        {
            get
            {
                return string.Format("{0}{1}{2}", Name, IdentifierDelimiter, (CodeFile == null ? string.Empty : CodeFile.FilePath));
            }
        }

        /// <summary>
        /// Format the results for the tag.  This method assumes that the tag has
        /// received a cahced copy of the results it should format.  It does not call out to
        /// retrieve results if they are not set.
        /// </summary>
        public string FormattedResult(DocumentMetadata properties)
        {
            if (CachedResult == null || CachedResult.Count == 0)
            {
                return Constants.Placeholders.EmptyField;
            }

            // When formatting a value, it is possible the user has selected multiple 
            // display commands.  We will only return the last cached result, and format
            // that if our formatter is available.
            var lastValue = CachedResult.Last();
            string formattedValue = lastValue.ToString();
            if (!string.IsNullOrWhiteSpace(Type) && ValueFormat != null)
            {
                formattedValue = ValueFormat.Format(lastValue.ToString(), Factories.GetValueFormatter(CodeFile), properties);
            }

            // Table tags should never return the placeholder.  We assume that there could reasonably
            // be empty cells at some point, so we will not correct those like we do for individual values.
            return (!IsTableTag() && string.IsNullOrWhiteSpace(formattedValue)) ? 
                Constants.Placeholders.EmptyField : formattedValue;
        }

        /// <summary>
        /// The starting line is the 0-based line index where the opening
        /// tag tag exists.
        /// </summary>
        public int? LineStart { get; set; }

        /// <summary>
        /// The ending line is the 0-based line index where the closing
        /// tag tag exists.
        /// </summary>
        public int? LineEnd { get; set; }

        public Tag()
        {
        }

        public Tag(Tag tag)
        {
            if (tag == null)
            {
                return;
            }

            CodeFile = tag.CodeFile;
            Type = tag.Type;
            Name = NormalizeName(tag.Name);
            RunFrequency = tag.RunFrequency;
            ValueFormat = tag.ValueFormat;
            FigureFormat = tag.FigureFormat;
            TableFormat = tag.TableFormat;
            LineStart = tag.LineStart;
            LineEnd = tag.LineEnd;
            CachedResult = tag.CachedResult;
            CodeFilePath = tag.CodeFilePath;
        }

        /// <summary>
        /// Serialize the current object, excluding circular elements like CodeFile
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            Name = NormalizeName(Name);
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Create a new Tag object given a JSON string
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Tag Deserialize(string json)
        {
            var tag = JsonConvert.DeserializeObject<Tag>(json);
            tag.Name = NormalizeName(tag.Name);
            return tag;
        }

        public override bool Equals(object other)
        {
            var tag = other as Tag;
            if (tag == null)
            {
                return false;
            }

            if (!Name.Equals(tag.Name))
            {
                return false;
            }
            
            // Now check for equality, considering if CodeFile values are null
            if (CodeFile == null && tag.CodeFile == null)
            {
                return true;
            }
            else if (CodeFile == null || tag.CodeFile == null)
            {
                return false;
            }

            return CodeFile.Equals(tag.CodeFile);
        }

        public override int GetHashCode()
        {
            return ((Name != null && CodeFile != null) ? (string.Format("{0}--{1}", Name, CodeFile.FilePath)).GetHashCode() : 0);
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                return Name;
            }

            if (!string.IsNullOrWhiteSpace(Type))
            {
                return Type;
            }

            return base.ToString();
        }

        public bool Equals(Tag other, bool usePosition)
        {
            return (usePosition) ? this.EqualsWithPosition(other) : this.Equals(other);
        }

        /// <summary>
        /// A more specialized version of Equals that takes into account line numbers.  This is used when trying
        /// to disambiguate tags that have the same label in the same code file.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool EqualsWithPosition(Tag tag)
        {
            return (this.Equals(tag) &&
                    this.LineStart == tag.LineStart &&
                    this.LineEnd == tag.LineEnd);
        }

        /// <summary>
        /// Ensure that all reserved characters that appear in an tag name are removed
        /// and replaced with a space.
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static string NormalizeName(string label)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                return string.Empty;
            }

            return label.Replace(Constants.ReservedCharacters.TagTableCellDelimiter, ' ').Trim();
        }

        /// <summary>
        /// Determine if this tag is to represent a table
        /// </summary>
        /// <returns></returns>
        public bool IsTableTag()
        {
            return Type != null && Type.Equals(Constants.TagType.Table, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Determine if there is any table data saved and available for this tag.  It will perform this check
        /// regardless of the tag type (although it's not expected to be called for non-table tags).
        /// If the table was set but has 0 dimension, this will still return true.  It asserts that a table result
        /// was initialized.
        /// </summary>
        /// <returns></returns>
        public bool HasTableData()
        {
            return !(CachedResult == null || CachedResult.Count == 0 || CachedResult.First().TableResult == null);
        }

        /// <summary>
        /// Update the underlying table data associated with this tag.
        /// </summary>
        public void UpdateFormattedTableData(DocumentMetadata properties)
        {
            if (!IsTableTag() || !HasTableData())
            {
                return;
            }

            var table = CachedResult.First().TableResult;
            table.FormattedCells = TableFormat.Format(table, Factories.GetValueFormatter(CodeFile), properties);
        }

        /// <summary>
        /// Helper to be used for one particular dimension (row or column)
        /// </summary>
        /// <param name="originalDimension"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private int GetDisplayDimension(int originalDimension, FilterFormat filter)
        {
            int dimension = originalDimension;
            if (filter != null && filter.Enabled)
            {
                if (filter.Type != Constants.FilterType.Exclude)
                {
                    throw new Exception(string.Format("Currently only the {0} filter type is supported", Constants.FilterType.Exclude));
                }

                var filterValue = filter.ExpandValue();
                if (filterValue != null)
                {
                    // Take away the number of rows we are filtering out.  If it means we filter out more than we actually
                    // have, just make it 0.
                    dimension -= filterValue.Length;
                    dimension = Math.Max(dimension, 0);
                }
            }

            return dimension;
        }

        /// <summary>
        /// Get the dimensions for the displayable table.  This factors in not only the data, but if column and
        /// row labels are included.
        /// </summary>
        /// <returns></returns>
        public int[] GetTableDisplayDimensions()
        {
            if (!IsTableTag() || TableFormat == null || !HasTableData())
            {
                return null;
            }

            var tableData = CachedResult.First().TableResult;
            var dimensions = new[]
            {
                GetDisplayDimension(tableData.RowSize, TableFormat.RowFilter),
                GetDisplayDimension(tableData.ColumnSize, TableFormat.ColumnFilter)
            };

            return dimensions;
        }

        /// <summary>
        /// Provide a string representation of the range of lines that this Tag spans in
        /// its code file.  If there is only one line, just that line number is returned.
        /// </summary>
        /// <returns></returns>
        public string FormatLineNumberRange()
        {
            if (LineStart == 0 || LineEnd == 0)
            {
                return string.Empty;
            }

            if (LineStart == LineEnd)
            {
                return LineStart.ToString();
            }

            return string.Format("{0} - {1}", LineStart+1, LineEnd+1);
        }
    }
}
