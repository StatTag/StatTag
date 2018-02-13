using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using StatTag.Core.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatTag.Core.Models
{
    /// <summary>
    /// A sequence of instructions saved in a text file that can be executed
    /// in a statistical package (e.g. Stata, R, SAS), and will be used within
    /// a Word document to derive values that are placed into the document text.
    /// </summary>
    public class CodeFile
    {
        [JsonIgnore]  // Even though this is private, we want to ensure it never gets serialized
        private List<string> ContentCache = null;

        public string StatisticalPackage { get; set; }
        public string FilePath { get; set; }
        public DateTime? LastCached { get; set; }
        [JsonIgnore]
        public List<Tag> Tags { get; set; }

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

        public CodeFile(CodeFile file)
        {
            Initialize(null);
            if (file != null)
            {
                Tags = file.Tags;
                StatisticalPackage = file.StatisticalPackage;
                FilePath = file.FilePath;
                LastCached = file.LastCached;
                Content = file.Content;
            }
        }

        public CodeFile(IFileHandler handler = null)
        {
            Initialize(handler);
        }

        protected void Initialize(IFileHandler handler)
        {
            Tags = new List<Tag>();
            FileHandler = handler ?? new FileHandler();
        }
        
        public string GetChecksumFromFile()
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(FilePath))
                {
                    return Encoding.Default.GetString(md5.ComputeHash(stream));
                }
            }
        }

        public string GetChecksumFromCache()
        {
            using (var md5 = MD5.Create())
            {
                var data = string.Join("\r\n", Content);
                return Encoding.Default.GetString(md5.ComputeHash(Encoding.Default.GetBytes(data)));
            }
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
        }

        /// <summary>
        /// Determine if this is a valid code file
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return (!string.IsNullOrWhiteSpace(FilePath) & FileHandler.Exists(FilePath));
        }

        /// <summary>
        /// Return the contents of the CodeFile
        /// </summary>
        /// <returns></returns>
        public virtual List<string> LoadFileContent()
        {
            if (!FileHandler.Exists(FilePath))
            {
                ContentCache = null;
            }
            else
            {
                RefreshContent();
            }
            
            return ContentCache;
        }

        /// <summary>
        /// Read the contents of the code file from the underlying file on the file system.
        /// </summary>
        /// <returns></returns>
        public void RefreshContent()
        {
            ContentCache = new List<string>(FileHandler.ReadAllLines(FilePath));
        }

        /// <summary>
        /// Using the contents of this file, parse the instrutions and build the list
        /// of tags that are present and cache them for later use.
        /// </summary>
        public void LoadTagsFromContent(bool preserveCache = true)
        {
            Tag[] savedTags = null;
            if (preserveCache)
            {
                savedTags = new Tag[Tags.Count];
                Tags.CopyTo(savedTags);
            }

            // Any time we try to load, reset the list of tags that may exist
            Tags = new List<Tag>();

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

            Tags = new List<Tag>(parser.Parse(this).Where(x => !string.IsNullOrWhiteSpace(x.Type)));
            Tags.ForEach(x => x.CodeFile = this);

            if (preserveCache)
            {
                // Since we are reloading from a file, at this point if we had any cached results for
                // an tag we want to associate that back with the tag.
                foreach (var tag in Tags)
                {
                    SetCachedTag(savedTags, tag);
                }
            }
        }

        /// <summary>
        /// Given a set of existing tags (which are assumed to have cached results already set), update
        /// the cached results in another tag.
        /// <remarks>This is used primarily when a code file is reloaded, which resets its collection
        /// of tags.  Those tags will be valid, but will have their cached results reset.</remarks>
        /// </summary>
        /// <param name="existingTags">The tags that have cached results</param>
        /// <param name="tag">The tag that needs to receive results</param>
        protected void SetCachedTag(IEnumerable<Tag> existingTags, Tag tag)
        {
            var existingTag = existingTags.FirstOrDefault(x => x.Equals(tag));
            if (existingTag != null && existingTag.CachedResult != null)
            {
                tag.CachedResult = new List<CommandResult>(existingTag.CachedResult);
            }
        }

        /// <summary>
        /// Save the content to the code file
        /// </summary>
        public virtual void Save()
        {
            if (FilePath != null && Content != null)
            {
                FileHandler.WriteAllLines(FilePath, Content);
            }
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
        /// CodeFile objects.  This does not resolve the list of tags that may be
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
        /// Removes an tag from the file, and from the internal cache.
        /// </summary>
        /// <param name="tag"></param>
        public void RemoveTag(Tag tag)
        {
            if (tag == null)
            {
                return;
            }

            if (!Tags.Remove(tag))
            {
                // If the exact object doesn't match, then search by equality
                var foundTag = Tags.Find(x => x.Equals(tag));
                if (foundTag == null)
                {
                    return;
                }
                Tags.Remove(foundTag);
            }

            ContentCache.RemoveAt(tag.LineEnd.Value);
            ContentCache.RemoveAt(tag.LineStart.Value);

            // Any tags below the one being removed need to be adjusted
            foreach (var otherTag in Tags)
            {
                // Tags can't overlap, so we can simply check for the start after the end.
                if (otherTag.LineStart > tag.LineEnd)
                {
                    otherTag.LineStart -= 2;
                    otherTag.LineEnd -= 2;
                }
            }
        }

        /// <summary>
        /// Updates or inserts an tag in the file.  An update takes place only if oldTag
        /// is defined, and it is able to match that old tag.
        /// </summary>
        /// <param name="newTag"></param>
        /// <param name="oldTag"></param>
        /// <param name="matchWithPosition">When looking to replace an existing tag (which assumes that oldTag is
        /// specified), this parameter when set to true will only replace the tag if the line numbers match.  This is to
        /// be used when updating duplicate named tags, but shouldn't be used otherwise.</param>
        /// <returns></returns>
        public Tag AddTag(Tag newTag, Tag oldTag = null, bool matchWithPosition = false)
        {
            // Do some sanity checking before modifying anything
            if (newTag == null || !newTag.LineStart.HasValue || !newTag.LineEnd.HasValue)
            {
                return null;
            }

            if (newTag.LineStart > newTag.LineEnd)
            {
                throw new InvalidDataException("The new tag start index is after the end index, which is not allowed.");
            }

            var updatedTag = new Tag(newTag);
            var content = Content;  // Force cache to load so we can reference it later w/o accessor overhead
            if (oldTag != null)
            {
                //var refreshedOldTag = (matchWithPosition ? Tags.FirstOrDefault(tag => oldTag.EqualsWithPosition(tag)) : Tags.FirstOrDefault(tag => oldTag.Equals(tag)));
                var refreshedOldTag =
                    Tags.FirstOrDefault(tag => oldTag.Equals(tag, matchWithPosition));
                if (refreshedOldTag == null)
                {
                    throw new InvalidDataException("Unable to find the existing tag to update.");
                }

                if (refreshedOldTag.LineStart > refreshedOldTag.LineEnd)
                {
                    throw new InvalidDataException("The existing tag start index is after the end index, which is not allowed.");
                }

                // Remove the starting tag and then adjust indices as appropriate
                ContentCache.RemoveAt(refreshedOldTag.LineStart.Value);
                if (updatedTag.LineStart > refreshedOldTag.LineStart)
                {
                    updatedTag.LineStart -= 1;
                    updatedTag.LineEnd -= 1;  // We know line end >= line start
                }
                else if (updatedTag.LineEnd > refreshedOldTag.LineStart)
                {
                    updatedTag.LineEnd -= 1;
                }

                refreshedOldTag.LineEnd -= 1;  // Don't forget to adjust the old tag index
                ContentCache.RemoveAt(refreshedOldTag.LineEnd.Value);
                if (updatedTag.LineStart > refreshedOldTag.LineEnd)
                {
                    updatedTag.LineStart -= 1;
                    updatedTag.LineEnd -= 1;
                }
                else if (updatedTag.LineEnd >= refreshedOldTag.LineEnd)
                {
                    updatedTag.LineEnd -= 1;
                }

                var index = Tags.FindIndex(x => x.Equals(refreshedOldTag, matchWithPosition));
                Tags.RemoveAt(index);
                //var index = Tags.FindIndex(x => (matchWithPosition) ? x.EqualsWithPosition(refreshedOldTag) : x.Equals(refreshedOldTag));
                //Tags.RemoveAt(index);
            }

            var generator = Factories.GetGenerator(this);
            ContentCache.Insert(updatedTag.LineStart.Value, generator.CreateOpenTag(updatedTag));
            updatedTag.LineEnd += 2;  // Offset one line for the opening tag, the second line is for the closing tag
            ContentCache.Insert(updatedTag.LineEnd.Value, generator.CreateClosingTag());

            // Add to our collection of tags
            Tags.Add(updatedTag);
            return updatedTag;
        }

        /// <summary>
        /// Look at all of the tags that are defined within this code file, and create a list
        /// of any tags that have duplicate names.
        /// </summary>
        /// <returns></returns>
        public Dictionary<Tag, List<Tag>> FindDuplicateTags()
        {
            var duplicates = new Dictionary<Tag, List<Tag>>();
            if (Tags == null)
            {
                return duplicates;
            }

            var distinct = new Dictionary<string, Tag>();
            foreach (var tag in Tags)
            {
                var searchLabel = tag.Name.ToUpper();

                // See if we already have this in the distinct list of tag names
                if (distinct.ContainsKey(searchLabel))
                {
                    // If the duplicates collection hasn't been initialized, we will do that now.
                    if (!duplicates.ContainsKey(distinct[searchLabel]))
                    {
                        duplicates.Add(distinct[searchLabel], new List<Tag>());
                    }
                    duplicates[distinct[searchLabel]].Add(tag);
                }
                else
                {
                    distinct.Add(tag.Name.ToUpper(), tag);
                }
            }
            return duplicates;
        }

        /// <summary>
        /// Given the content passed as a parameter, this method updates the file on disk with the new
        /// content and refreshes the internal cache.
        /// </summary>
        /// <param name="text"></param>
        public virtual void UpdateContent(string text)
        {
            FileHandler.WriteAllText(FilePath, text);
            LoadTagsFromContent();
        }
    }
}
