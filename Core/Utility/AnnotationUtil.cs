using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Models;

namespace StatTag.Core.Utility
{
    public class TagUtil
    {
        /// <summary>
        /// Find all tags with a matching tag name (regardless of case).
        /// </summary>
        /// <param name="outputLabel">The tag name to search for</param>
        /// <returns></returns>
        public static List<Tag> FindTagsByOutputLabel(string outputLabel, List<CodeFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return null;
            }

            return files.SelectMany(file => file.Tags).Where(
                tag => tag.OutputLabel.Equals(outputLabel, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }

        /// <summary>
        /// Determine if we need to perform a check for possible conflicting tag name names.
        /// </summary>
        /// <param name="oldTag"></param>
        /// <param name="newTag"></param>
        /// <returns></returns>
        public static bool ShouldCheckForDuplicateLabel(Tag oldTag, Tag newTag)
        {
            if (oldTag == null && newTag != null)
            {
                return true;
            }

            if (newTag == null)
            {
                return false;
            }

            return !oldTag.OutputLabel.Equals(newTag.OutputLabel);
        }

        /// <summary>
        /// Looks across all tags in a collection of code files to find those that have
        /// the same tag name.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public static Dictionary<CodeFile, int[]> CheckForDuplicateLabels(Tag tag, List<CodeFile> files)
        {
            if (tag == null)
            {
                return null;
            }

            var tags = FindTagsByOutputLabel(tag.OutputLabel, files);
            if (tags == null || tags.Count == 0)
            {
                return null;
            }

            var duplicateCount = new Dictionary<CodeFile, int[]>();
            foreach (var otherTag in tags)
            {
                // If the tag is the exact some object, skip it.
                if (object.ReferenceEquals(otherTag, tag))
                {
                    continue;
                }

                if (!duplicateCount.ContainsKey(otherTag.CodeFile))
                {
                    duplicateCount.Add(otherTag.CodeFile, new[] { 0, 0 });
                }

                // If the tag names are an exact match, they go into the first bucket.
                // Otherwise, they are a case-insensitive match and go into the second bucket.
                if (tag.OutputLabel.Equals(otherTag.OutputLabel))
                {
                    duplicateCount[otherTag.CodeFile][0]++;
                }
                else
                {
                    duplicateCount[otherTag.CodeFile][1]++;
                }
            }

            return duplicateCount;
        }

        /// <summary>
        /// This is expected to be paired with the results of CheckForDuplicateLabels to determine if the tag
        /// has duplicates that appear in the results.  It assumes we have asserted a duplicate may exist.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsDuplicateLabelInSameFile(Tag tag, Dictionary<CodeFile, int[]> result)
        {
            // If the tag itself is null, or it has no code file reference, we will assume this isn't in the same file (or that no
            // file exists for it to be the "same" in).  Likewise, if our result structure is null, we have nothing to check so we are done.
            if (tag == null || result == null || tag.CodeFile == null)
            {
                return false;
            }

            // Look in the list of code files that had matching results of some degree to see if this tag's code file is represented.
            var codeFileResult = result.Where(x => x.Key.Equals(tag.CodeFile)).Select(x => (KeyValuePair<CodeFile, int[]>?)x).FirstOrDefault();

            // This really shouldn't happen, but as a guard we'll look to see if the code file exists.  If not, we will assume that there
            // is no duplicate label in this file.
            if (codeFileResult == null)
            {
                return false;
            }

            // The last check is if the code file has any duplicate entries that are found.  We consider matches - even not with exact
            // case - as duplicates.
            var codeFileEntry = codeFileResult.Value;
            return (codeFileEntry.Value[0] > 0 || codeFileEntry.Value[1] > 0);
        }
    }
}
