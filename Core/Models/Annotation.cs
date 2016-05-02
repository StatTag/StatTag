using System;
using System.Collections.Generic;
using System.Linq;
using AnalysisManager.Core.Interfaces;
using Newtonsoft.Json;

namespace AnalysisManager.Core.Models
{
    /// <summary>
    /// An annotation is a sequence of lines in a CodeFile that is defined by special
    /// comment tags.  It contains configuration information on how to interpret and
    /// format the result of the code block within the document.
    /// </summary>
    public class Annotation
    {
        [JsonIgnore]
        public CodeFile CodeFile { get; set; }
        public string Type { get; set; }
        public string OutputLabel { get; set; }
        public string RunFrequency { get; set; }
        public ValueFormat ValueFormat { get; set; }
        public FigureFormat FigureFormat { get; set; }
        public TableFormat TableFormat { get; set; }
        public List<CommandResult> CachedResult { get; set; }

        public string Id
        {
            get
            {
                return string.Format("{0}--{1}", OutputLabel, (CodeFile == null ? string.Empty : CodeFile.FilePath));
            }
        }

        /// <summary>
        /// Format the results for the annotation.  This method assumes that the annotation has
        /// received a cahced copy of the results it should format.  It does not call out to
        /// retrieve results if they are not set.
        /// </summary>
        public string FormattedResult
        {
            get
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
                    formattedValue = ValueFormat.Format(lastValue.ToString(), Factories.GetValueFormatter(CodeFile));
                }

                // Table annotations should never return the placeholder.  We assume that there could reasonably
                // be empty cells at some point, so we will not correct those like we do for individual values.
                return (!IsTableAnnotation() && string.IsNullOrWhiteSpace(formattedValue)) ? 
                    Constants.Placeholders.EmptyField : formattedValue;
            }
        }

        /// <summary>
        /// The starting line is the 0-based line index where the opening
        /// annotation tag exists.
        /// </summary>
        public int? LineStart { get; set; }

        /// <summary>
        /// The ending line is the 0-based line index where the closing
        /// annotation tag exists.
        /// </summary>
        public int? LineEnd { get; set; }

        public Annotation()
        {
        }

        public Annotation(Annotation annotation)
        {
            if (annotation == null)
            {
                return;
            }

            CodeFile = annotation.CodeFile;
            Type = annotation.Type;
            OutputLabel = NormalizeOutputLabel(annotation.OutputLabel);
            RunFrequency = annotation.RunFrequency;
            ValueFormat = annotation.ValueFormat;
            FigureFormat = annotation.FigureFormat;
            TableFormat = annotation.TableFormat;
            LineStart = annotation.LineStart;
            LineEnd = annotation.LineEnd;
            CachedResult = annotation.CachedResult;
        }

        /// <summary>
        /// Serialize the current object, excluding circular elements like CodeFile
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            OutputLabel = NormalizeOutputLabel(OutputLabel);
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Create a new Annotation object given a JSON string
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Annotation Deserialize(string json)
        {
            var annotation = JsonConvert.DeserializeObject<Annotation>(json);
            annotation.OutputLabel = NormalizeOutputLabel(annotation.OutputLabel);
            return annotation;
        }

        public override bool Equals(object other)
        {
            var annotation = other as Annotation;
            if (annotation == null)
            {
                return false;
            }

            if (!OutputLabel.Equals(annotation.OutputLabel))
            {
                return false;
            }
            
            // Now check for equality, considering if CodeFile values are null
            if (CodeFile == null && annotation.CodeFile == null)
            {
                return true;
            }
            else if (CodeFile == null || annotation.CodeFile == null)
            {
                return false;
            }

            return CodeFile.Equals(annotation.CodeFile);
        }

        public override int GetHashCode()
        {
            return ((OutputLabel != null && CodeFile != null) ? (string.Format("{0}--{1}", OutputLabel, CodeFile.FilePath)).GetHashCode() : 0);
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(OutputLabel))
            {
                return OutputLabel;
            }

            if (!string.IsNullOrWhiteSpace(Type))
            {
                return Type;
            }

            return base.ToString();
        }

        public bool Equals(Annotation other, bool usePosition)
        {
            return (usePosition) ? this.EqualsWithPosition(other) : this.Equals(other);
        }

        /// <summary>
        /// A more specialized version of Equals that takes into account line numbers.  This is used when trying
        /// to disambiguate annotations that have the same label in the same code file.
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns></returns>
        public bool EqualsWithPosition(Annotation annotation)
        {
            return (this.Equals(annotation) &&
                    this.LineStart == annotation.LineStart &&
                    this.LineEnd == annotation.LineEnd);
        }

        /// <summary>
        /// Ensure that all reserved characters that appear in an output label are removed
        /// and replaced with a space.
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static string NormalizeOutputLabel(string label)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                return string.Empty;
            }

            return label.Replace(Constants.ReservedCharacters.AnnotationTableCellDelimiter, ' ').Trim();
        }

        /// <summary>
        /// Determine if this annotation is to represent a table
        /// </summary>
        /// <returns></returns>
        public bool IsTableAnnotation()
        {
            return Type != null && Type.Equals(Constants.AnnotationType.Table, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Determine if there is any table data saved and available for this annotation.  It will perform this check
        /// regardless of the annotation type (although it's not expected to be called for non-table annotations).
        /// If the table was set but has 0 dimension, this will still return true.  It asserts that a table result
        /// was initialized.
        /// </summary>
        /// <returns></returns>
        public bool HasTableData()
        {
            return !(CachedResult == null || CachedResult.Count == 0 || CachedResult[0].TableResult == null);
        }

        /// <summary>
        /// Update the underlying table data associated with this annotation.
        /// </summary>
        public void UpdateFormattedTableData()
        {
            if (!IsTableAnnotation() || !HasTableData())
            {
                return;
            }

            var table = CachedResult[0].TableResult;
            table.FormattedCells = TableFormat.Format(table, Factories.GetValueFormatter(CodeFile));
        }

        /// <summary>
        /// Get the dimensions for the displayable table.  This factors in not only the data, but if column and
        /// row labels are included.
        /// </summary>
        /// <returns></returns>
        public int[] GetTableDisplayDimensions()
        {
            if (!IsTableAnnotation() || TableFormat == null || !HasTableData())
            {
                return null;
            }

            var tableData = CachedResult[0].TableResult;
            var dimensions = new[] { tableData.RowSize, tableData.ColumnSize };
            if (TableFormat.IncludeColumnNames && tableData.ColumnNames != null)
            {
                dimensions[Constants.DimensionIndex.Rows]++;
            }

            if (TableFormat.IncludeRowNames && tableData.RowNames != null)
            {
                dimensions[Constants.DimensionIndex.Columns]++;
            }

            return dimensions;
        }

        /// <summary>
        /// Provide a string representation of the range of lines that this Annotation spans in
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

            return string.Format("{0} - {1}", LineStart, LineEnd);
        }
    }
}
