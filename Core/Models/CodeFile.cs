using System.IO;
using System.Runtime.InteropServices;
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
        private List<string> ContentCache = null;

        public string StatisticalPackage { get; set; }
        public string FilePath { get; set; }
        public DateTime? LastCached { get; set; }
        [JsonIgnore]
        public List<Annotation> Annotations { get; set; }

        [JsonIgnore]
        public List<string> Content
        {
            get { return ContentCache ?? (ContentCache = LoadFileContent()); }
            set { ContentCache = value; }
        }

        /// <summary>
        /// This is typically a lightweight wrapper around the standard
        /// File class, but is used to allow us to mock file IO during
        /// our unit tests.
        /// </summary>
        protected IFileHandler FileHandler { get; set; }

        public CodeFile()
        {
            Initialize(null);
        }

        public CodeFile(IFileHandler handler = null)
        {
            Initialize(handler);
        }

        protected void Initialize(IFileHandler handler)
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

            return (string.Compare(item.FilePath, FilePath, StringComparison.CurrentCultureIgnoreCase) == 0);

            //return item.FilePath.Equals(FilePath, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Return the contents of the CodeFile
        /// </summary>
        /// <returns></returns>
        public virtual List<string> LoadFileContent()
        {
            RefreshContent();
            return ContentCache;
        }

        /// <summary>
        /// Return the contents of the CodeFile
        /// </summary>
        /// <returns></returns>
        public void RefreshContent()
        {
            ContentCache = new List<string>(FileHandler.ReadAllLines(FilePath));
        }

        /// <summary>
        /// Using the contents of this file, parse the instrutions and build the list
        /// of annotations that are present and cache them for later use.
        /// </summary>
        public void LoadAnnotationsFromContent(bool preserveCache = true)
        {
            Annotation[] savedAnnotations = null;
            if (preserveCache)
            {
                savedAnnotations = new Annotation[Annotations.Count];
                Annotations.CopyTo(savedAnnotations);
            }

            // Any time we try to load, reset the list of annotations that may exist
            Annotations = new List<Annotation>();

            var content = LoadFileContent();
            if (content == null || !content.Any())
            {
                return;
            }

            var parser = Factories.GetParser(this);
            if (parser == null)
            {
                return;
            }

            Annotations = new List<Annotation>(parser.Parse(this).Where(x => !string.IsNullOrWhiteSpace(x.Type)));
            Annotations.ForEach(x => x.CodeFile = this);

            if (preserveCache)
            {
                // Since we are reloading from a file, at this point if we had any cached results for
                // an annotation we want to associate that back with the annotation.
                foreach (var annotation in Annotations)
                {
                    SetCachedAnnotation(savedAnnotations, annotation);
                }
            }
        }

        /// <summary>
        /// Given a set of existing annotations (which are assumed to have cached results already set), update
        /// the cached results in another annotation.
        /// <remarks>This is used primarily when a code file is reloaded, which resets its collection
        /// of annotations.  Those annotations will be valid, but will have their cached results reset.</remarks>
        /// </summary>
        /// <param name="existingAnnotations">The annotations that have cached results</param>
        /// <param name="annotation">The annotation that needs to receive results</param>
        protected void SetCachedAnnotation(IEnumerable<Annotation> existingAnnotations, Annotation annotation)
        {
            var existingAnnotation = existingAnnotations.FirstOrDefault(x => x.Equals(annotation));
            if (existingAnnotation != null && existingAnnotation.CachedResult != null)
            {
                annotation.CachedResult = new List<CommandResult>(existingAnnotation.CachedResult);
            }
        }

        /// <summary>
        /// Save the content to the code file
        /// </summary>
        public void Save()
        {
            FileHandler.WriteAllLines(FilePath, Content);
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

        /// <summary>
        /// Removes an annotation from the file, and from the internal cache.
        /// </summary>
        /// <param name="annotation"></param>
        public void RemoveAnnotation(Annotation annotation)
        {
            if (annotation == null)
            {
                return;
            }

            if (!Annotations.Remove(annotation))
            {
                // If the exact object doesn't match, then search by equality
                var foundAnnotation = Annotations.Find(x => x.Equals(annotation));
                if (foundAnnotation == null)
                {
                    return;
                }
                Annotations.Remove(foundAnnotation);
            }

            ContentCache.RemoveAt(annotation.LineEnd.Value);
            ContentCache.RemoveAt(annotation.LineStart.Value);

            // Any annotations below the one being removed need to be adjusted
            foreach (var otherAnnotation in Annotations)
            {
                // Annotations can't overlap, so we can simply check for the start after the end.
                if (otherAnnotation.LineStart > annotation.LineEnd)
                {
                    otherAnnotation.LineStart -= 2;
                    otherAnnotation.LineEnd -= 2;
                }
            }
        }

        /// <summary>
        /// Updates or inserts an annotation in the file.  An update takes place only if oldAnnotation
        /// is defined, and it is able to match that old annotation.
        /// </summary>
        /// <param name="newAnnotation"></param>
        /// <param name="oldAnnotation"></param>
        /// <param name="matchWithPosition">When looking to replace an existing annotation (which assumes that oldAnnotation is
        /// specified), this parameter when set to true will only replace the annotation if the line numbers match.  This is to
        /// be used when updating duplicate named annotations, but shouldn't be used otherwise.</param>
        /// <returns></returns>
        public Annotation AddAnnotation(Annotation newAnnotation, Annotation oldAnnotation = null, bool matchWithPosition = false)
        {
            // Do some sanity checking before modifying anything
            if (newAnnotation == null || !newAnnotation.LineStart.HasValue || !newAnnotation.LineEnd.HasValue)
            {
                return null;
            }

            if (newAnnotation.LineStart > newAnnotation.LineEnd)
            {
                throw new InvalidDataException("The new annotation start index is after the end index, which is not allowed.");
            }

            var updatedAnnotation = new Annotation(newAnnotation);
            var content = Content;  // Force cache to load so we can reference it later w/o accessor overhead
            if (oldAnnotation != null)
            {
                //var refreshedOldAnnotation = (matchWithPosition ? Annotations.FirstOrDefault(annotation => oldAnnotation.EqualsWithPosition(annotation)) : Annotations.FirstOrDefault(annotation => oldAnnotation.Equals(annotation)));
                var refreshedOldAnnotation =
                    Annotations.FirstOrDefault(annotation => oldAnnotation.Equals(annotation, matchWithPosition));
                if (refreshedOldAnnotation == null)
                {
                    throw new InvalidDataException("Unable to find the existing annotation to update.");
                }

                if (refreshedOldAnnotation.LineStart > refreshedOldAnnotation.LineEnd)
                {
                    throw new InvalidDataException("The existing annotation start index is after the end index, which is not allowed.");
                }

                // Remove the starting annotation and then adjust indices as appropriate
                ContentCache.RemoveAt(refreshedOldAnnotation.LineStart.Value);
                if (updatedAnnotation.LineStart > refreshedOldAnnotation.LineStart)
                {
                    updatedAnnotation.LineStart -= 1;
                    updatedAnnotation.LineEnd -= 1;  // We know line end >= line start
                }
                else if (updatedAnnotation.LineEnd > refreshedOldAnnotation.LineStart)
                {
                    updatedAnnotation.LineEnd -= 1;
                }

                refreshedOldAnnotation.LineEnd -= 1;  // Don't forget to adjust the old annotation index
                ContentCache.RemoveAt(refreshedOldAnnotation.LineEnd.Value);
                if (updatedAnnotation.LineStart > refreshedOldAnnotation.LineEnd)
                {
                    updatedAnnotation.LineStart -= 1;
                    updatedAnnotation.LineEnd -= 1;
                }
                else if (updatedAnnotation.LineEnd >= refreshedOldAnnotation.LineEnd)
                {
                    updatedAnnotation.LineEnd -= 1;
                }

                var index = Annotations.FindIndex(x => x.Equals(refreshedOldAnnotation, matchWithPosition));
                Annotations.RemoveAt(index);
                //var index = Annotations.FindIndex(x => (matchWithPosition) ? x.EqualsWithPosition(refreshedOldAnnotation) : x.Equals(refreshedOldAnnotation));
                //Annotations.RemoveAt(index);
            }

            var generator = Factories.GetGenerator(this);
            ContentCache.Insert(updatedAnnotation.LineStart.Value, generator.CreateOpenTag(updatedAnnotation));
            updatedAnnotation.LineEnd += 2;  // Offset one line for the opening tag, the second line is for the closing tag
            ContentCache.Insert(updatedAnnotation.LineEnd.Value, generator.CreateClosingTag());

            // Add to our collection of annotations
            Annotations.Add(updatedAnnotation);
            return updatedAnnotation;
        }

        /// <summary>
        /// Look at all of the annotations that are defined within this code file, and create a list
        /// of any annotations that have duplicate names.
        /// </summary>
        /// <returns></returns>
        public Dictionary<Annotation, List<Annotation>> FindDuplicateAnnotations()
        {
            var duplicates = new Dictionary<Annotation, List<Annotation>>();
            if (Annotations == null)
            {
                return duplicates;
            }

            var distinct = new Dictionary<string, Annotation>();
            foreach (var annotation in Annotations)
            {
                var searchLabel = annotation.OutputLabel.ToUpper();

                // See if we already have this in the distinct list of annotation names
                if (distinct.ContainsKey(searchLabel))
                {
                    // If the duplicates collection hasn't been initialized, we will do that now.
                    if (!duplicates.ContainsKey(distinct[searchLabel]))
                    {
                        duplicates.Add(distinct[searchLabel], new List<Annotation>());
                    }
                    duplicates[distinct[searchLabel]].Add(annotation);
                }
                else
                {
                    distinct.Add(annotation.OutputLabel.ToUpper(), annotation);
                }
            }
            return duplicates;
        }

        /// <summary>
        /// Given the content passed as a parameter, this method updates the file on disk with the new
        /// content and refreshes the internal cache.
        /// </summary>
        /// <param name="text"></param>
        public void UpdateContent(string text)
        {
            FileHandler.WriteAllText(FilePath, text);
            LoadAnnotationsFromContent();
        }
    }
}
