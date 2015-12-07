using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Interfaces;
using AnalysisManager.Core.Parser;
using Newtonsoft.Json;

namespace AnalysisManager.Core.Models
{
    public class CodeFile
    {
        public string StatisticalPackage { get; set; }
        public string FilePath { get; set; }
        public DateTime? LastCached { get; set; }
        [JsonIgnore]
        public List<Annotation> Annotations { get; set; }

        protected IFileHandler FileHandler { get; set; }

        public CodeFile(IFileHandler handler = null)
        {
            Annotations = new List<Annotation>();
            FileHandler = handler ?? new FileHandler();
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
            return FileHandler.ReadAllLines(FilePath);
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

        public void SaveBackup()
        {
            var backupFile = string.Format("{0}.{1}", FilePath, Constants.FileExtensions.Backup);
            if (!FileHandler.Exists(backupFile))
            {
                FileHandler.Copy(FilePath, backupFile);
            }
        }

        public static string SerializeList(List<CodeFile> files)
        {
            return JsonConvert.SerializeObject(files);
        }

        public static List<CodeFile> DeserializeList(string value)
        {
            return JsonConvert.DeserializeObject<List<CodeFile>>(value);
        }

        private static bool FilterMatches(string filter, string path)
        {
            string[] extensions = filter.Split(';');
            string normalizedPath = path.ToUpper();
            if (extensions.Any(extension => normalizedPath.EndsWith(extension.Replace("*", "").ToUpper())))
            {
                return true;
            }

            return false;
        }

        public static string GuessStatisticalPackage(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            path = path.Trim();

            if (FilterMatches(Constants.FileFilters.StataFilter, path))
            {
                return Constants.StatisticalPackages.Stata;
            }
            else if (FilterMatches(Constants.FileFilters.SASFilter, path))
            {
                return Constants.StatisticalPackages.SAS;
            }
            else if (FilterMatches(Constants.FileFilters.RFilter, path))
            {
                return Constants.StatisticalPackages.R;
            }

            return string.Empty;
        }
    }
}
