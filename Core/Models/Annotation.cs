using System.Collections.Generic;
using System.Linq;
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
                if (!string.IsNullOrWhiteSpace(Type)
                    && Type.Equals(Constants.AnnotationType.Value) && ValueFormat != null)
                {
                    formattedValue = ValueFormat.Format(lastValue.ToString());
                }

                return string.IsNullOrWhiteSpace(formattedValue) ? Constants.Placeholders.EmptyField : formattedValue;
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

        public Annotation() { }

        public Annotation(Annotation annotation)
        {
            CodeFile = annotation.CodeFile;
            Type = annotation.Type;
            OutputLabel = annotation.OutputLabel;
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
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Create a new Annotation object given a JSON string
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Annotation Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<Annotation>(json);
        }

        public override bool Equals(object other)
        {
            var annotation = other as Annotation;
            if (annotation == null)
            {
                return false;
            }

            return string.Equals(OutputLabel, annotation.OutputLabel) && string.Equals(Type, annotation.Type);
        }

        public override int GetHashCode()
        {
            return ((OutputLabel != null && Type != null) ? (string.Format("{0}--{1}", OutputLabel, Type)).GetHashCode() : 0);
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
    }
}
