﻿using Newtonsoft.Json;

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
    }
}
