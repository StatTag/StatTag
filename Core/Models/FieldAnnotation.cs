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
            SetCachedValue();
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

        /// <summary>
        /// Utility function called when a FieldAnnotation is created from an existing annotation and
        /// a cell index (meaning it's a table annotation).  We want to update this annotation to
        /// only carry the specific cell value.
        /// </summary>
        protected void SetCachedValue()
        {
            if (IsTableAnnotation() && TableCellIndex.HasValue
                && CachedResult != null && CachedResult.Count > 0)
            {
                var table = CachedResult.Last().TableResult;
                if (table != null && table.FormattedCells != null)
                {
                    CachedResult = new List<CommandResult>() {
                        new CommandResult()
                        {
                            ValueResult = (TableCellIndex.Value < table.FormattedCells.Length) ? table.FormattedCells[TableCellIndex.Value] : string.Empty
                        } 
                    };
                }
            }
        }
    }
}
