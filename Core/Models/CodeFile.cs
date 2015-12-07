using AnalysisManager.Core.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnalysisManager.Core.Models
{
    /// <summary>
    /// A sequence of instructions saved in a text file that can be executed
    /// in a statistical package (e.g. Stata, R, SAS), and will be used within
    /// a Word document to derive values that are placed into the document text.
    /// </summary>
    public class CodeFile
    {
        public string StatisticalPackage { get; set; }
        public string FilePath { get; set; }
        public DateTime? LastCached { get; set; }
        [JsonIgnore]
        public List<Annotation> Annotations { get; set; }

        /// <summary>
        /// This is typically a lightweight wrapper around the standard
        /// File class, but is used to allow us to mock file IO during
        /// our unit tests.
        /// </summary>
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

        /// <summary>
        /// Return the contents of the CodeFile
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetContent()
        {
            return FileHandler.ReadAllLines(FilePath);
        }

        /// <summary>
        /// Using the contents of this file, parse the instrutions and build the list
        /// of annotations that are present and cache them for later use.
        /// </summary>
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

        /// <summary>
        /// Save a backup copy of this code file, in the event we cause issues with the file and the
        /// user needs to restore it.
        /// </summary>
        public void SaveBackup()
        {
            var backupFile = string.Format("{0}.{1}", FilePath, Constants.FileExtensions.Backup);
            if (!FileHandler.Exists(backupFile))
            {
                FileHandler.Copy(FilePath, backupFile);
            }
        }

        /// <summary>
        /// Utility method to serialize the list of code files into a JSON array.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static string SerializeList(List<CodeFile> files)
        {
            return JsonConvert.SerializeObject(files);
        }

        /// <summary>
        /// Utility method to take a JSON array string and convert it back into a list of
        /// CodeFile objects.  This does not resolve the list of annotations that may be
        /// associated with the CodeFile.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<CodeFile> DeserializeList(string value)
        {
            return JsonConvert.DeserializeObject<List<CodeFile>>(value);
        }
        
        /// <summary>
        /// Given a file filter (e.g. "*.txt" or "*.txt;*.t", determine if the supplied
        /// path parameter matches.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="path"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Utility method to take a path and determine which statistical package is
        /// most likely the right one  This only returns a value if there is a high
        /// degree of certainty of the guess, and is based purely on the file name
        /// (not file content).
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
            if (FilterMatches(Constants.FileFilters.SASFilter, path))
            {
                return Constants.StatisticalPackages.SAS;
            }
            if (FilterMatches(Constants.FileFilters.RFilter, path))
            {
                return Constants.StatisticalPackages.R;
            }

            return string.Empty;
        }
    }
}
