using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AnalysisManager.Core.Models
{
    /// <summary>
    /// A specialized version of Annotation that lives inside of fields within the
    /// Word document.  This contains some additional attributes that pertain to an
    /// instance of an Attribute within the documment, and not the general specification
    /// of the Attribute.
    /// </summary>
    /// <remarks>A good example is with a Table annotation.  The Annotation defines how the
    /// table as a whole is formatted, while the FieldAnnotation specifies a single cell
    /// within the table.</remarks>
    public class FieldAnnotation : Annotation
    {
        public int? TableCellIndex { get; set; }

        public FieldAnnotation()
            : base()
        {
            TableCellIndex = null;
        }

        public FieldAnnotation(Annotation annotation) 
            : base(annotation)
        {
            TableCellIndex = null;
        }

        public FieldAnnotation(Annotation annotation, int? tableCellIndex)
            : base(annotation)
        {
            TableCellIndex = tableCellIndex;
        }

        public FieldAnnotation(FieldAnnotation annotation)
            : base(annotation)
        {
            TableCellIndex = annotation.TableCellIndex;
        }

        /// <summary>
        /// Serialize the current object, excluding circular elements like CodeFile
        /// </summary>
        /// <returns></returns>
        public new string Serialize()
        {
            OutputLabel = NormalizeOutputLabel(OutputLabel);
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Create a new Annotation object given a JSON string
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public new static FieldAnnotation Deserialize(string json)
        {
            var annotation = JsonConvert.DeserializeObject<FieldAnnotation>(json);
            annotation.OutputLabel = NormalizeOutputLabel(annotation.OutputLabel);
            return annotation;
        }
    }
}
