using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StatTag.Core.Models
{
    /// <summary>
    /// A specialized version of Tag that lives inside of fields within the
    /// Word document.  This contains some additional attributes that pertain to an
    /// instance of an Attribute within the documment, and not the general specification
    /// of the Attribute.
    /// </summary>
    /// <remarks>A good example is with a Table tag.  The Tag defines how the
    /// table as a whole is formatted, while the FieldTag specifies a single cell
    /// within the table.</remarks>
    public class FieldTag : Tag
    {
        public int? TableCellIndex { get; set; }

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
                if (CodeFile == null)
                {
                    CodeFile = new CodeFile() { FilePath = value };
                }
            }
        }

        public FieldTag()
            : base()
        {
            TableCellIndex = null;
        }

        public FieldTag(Tag tag) 
            : base(tag)
        {
            TableCellIndex = null;
        }

        public FieldTag(Tag tag, int? tableCellIndex)
            : base(tag)
        {
            TableCellIndex = tableCellIndex;
            SetCachedValue();
        }

        public FieldTag(Tag tag, FieldTag fieldTag)
            : base(tag ?? fieldTag)
        {
            TableCellIndex = fieldTag.TableCellIndex;
            CodeFilePath = fieldTag.CodeFilePath;
            SetCachedValue();
        }

        public FieldTag(FieldTag tag)
            : base(tag)
        {
            TableCellIndex = tag.TableCellIndex;
        }

        /// <summary>
        /// Serialize the current object, excluding circular elements like CodeFile
        /// </summary>
        /// <returns></returns>
        public new string Serialize()
        {
            Name = NormalizeName(Name);
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Create a new Tag object given a JSON string
        /// </summary>
        /// <param name="json"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public static FieldTag Deserialize(string json, IEnumerable<CodeFile> files = null)
        {
            var tag = JsonConvert.DeserializeObject<FieldTag>(json);
            tag.Name = NormalizeName(tag.Name);
            LinkToCodeFile(tag, files);
            return tag;
        }

        /// <summary>
        /// Provide a link to a FieldTag from a list of CodeFile objects
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="files"></param>
        public static void LinkToCodeFile(FieldTag tag, IEnumerable<CodeFile> files)
        {
            if (tag == null || files == null)
            {
                return;
            }

            foreach (var file in files)
            {
                if (tag.CodeFilePath.Equals(file.FilePath))
                {
                    tag.CodeFile = file;
                    return;
                }
            }
        }

        /// <summary>
        /// Utility function called when a FieldTag is created from an existing tag and
        /// a cell index (meaning it's a table tag).  We want to update this tag to
        /// only carry the specific cell value.
        /// </summary>
        protected void SetCachedValue()
        {
            if (IsTableTag() && TableCellIndex.HasValue
                && CachedResult != null && CachedResult.Count > 0)
            {
                var table = CachedResult.Last().TableResult;
                if (table != null && table.FormattedCells != null)
                {
                    CachedResult = new List<CommandResult>() {
                        new CommandResult()
                        {
                            //ValueResult = (TableCellIndex.Value < table.FormattedCells.Length) ? 
                            //    table.FormattedCells[TableCellIndex.Value] : string.Empty
                            ValueResult = Table.GetDataAtIndex(table.FormattedCells, TableCellIndex.Value)
                        } 
                    };
                }
            }
        }
    }
}
