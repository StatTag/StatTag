using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Parser;

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

        public virtual string[] GetContent()
        {
            return File.ReadAllLines(FilePath);
        }

        public void LoadAnnotationsFromContent()
        {
            Annotations = new List<Annotation>(); // Any time we try to load, reset the list of annotations that may exist
            var content = GetContent();
            if (content == null || content.Length == 0)
            {
                return;
            }

            var parser = Factories.GetParser(this);
            if (parser == null)
            {
                return;
            }

            Annotations = new List<Annotation>(parser.Parse(content).Where(x => !string.IsNullOrWhiteSpace(x.Type)));
            Annotations.ForEach(x => x.CodeFile = this);
        }
    }
}
