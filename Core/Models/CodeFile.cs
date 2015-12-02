using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisManager.Core.Models
{
    public class CodeFile
    {
        public string StatisticalPackage { get; set; }
        public string FilePath { get; set; }
        public DateTime? LastCached { get; set; }
        public List<Annotation> Annotations { get; set; }

        public CodeFile()
        {
            Annotations = new List<Annotation>();
        }

        public override string ToString()
        {
            return FilePath ?? string.Empty;
        }

        public override int GetHashCode()
        {
            return (FilePath != null ? FilePath.GetHashCode() : 0);
        }

        public override bool Equals(object obj)
        {
            var item = obj as CodeFile;
            if (item == null)
            {
                return false;
            }

            return item.FilePath.Equals(FilePath, StringComparison.CurrentCultureIgnoreCase);
        }

        public string[] GetContent()
        {
            return File.ReadAllLines(FilePath);
        }
    }
}
