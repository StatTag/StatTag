using System.Collections.Generic;
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
        public List<string> CachedResult { get; set; }

        /// <summary>
        /// Format the results for the annotation.  This method assumes that the annotation has
        /// received a cahced copy of the results it should format.  It does not call out to
        /// retrieve results if they are not set.
        /// </summary>
        public string FormattedResult
        {
            get
            {
                if (CachedResult == null)
                {
                    return string.Empty;
                }

                return string.Join("\r\n", CachedResult);
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
            LineStart = annotation.LineStart;
            LineEnd = annotation.LineEnd;
            CachedResult = annotation.CachedResult;
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(OutputLabel))
            {
                return OutputLabel;
            }

            return base.ToString();
        }

        /// <summary>
        /// Serialize the current object, excluding circular elements like CodeFile
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
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
    }
}
