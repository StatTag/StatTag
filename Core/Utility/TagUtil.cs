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
        /// <param name="files">The list of code files the tag should be contained in.</param>
        /// <returns></returns>
        public static List<Tag> FindTagsByName(string outputLabel, List<CodeFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return null;
            }

            return files.SelectMany(file => file.Tags).Where(
                tag => tag.Name.Equals(outputLabel, StringComparison.CurrentCultureIgnoreCase)).ToList();
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

            return !oldTag.Name.Equals(newTag.Name);
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

            var tags = FindTagsByName(tag.Name, files);
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
                if (tag.Name.Equals(otherTag.Name))
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

        public class TagCollisionResult
        {
            public enum CollisionType
            {
                NoOverlap,
                OverlapsExact,
                EmbeddedWithin,
                OverlapsFront,
                OverlapsBack,
                Embeds
            }

            public Tag CollidingTag { get; set; }
            public CollisionType Collision { get; set; }
        }

        /// <summary>
        /// Checks if tag1 and tag2 are completely outside of each other (they have
        /// no overlap)
        /// </summary>
        /// <param name="tag1"></param>
        /// <param name="tag2"></param>
        /// <returns></returns>
        private static bool TagsOutsideEachOther(Tag tag1, Tag tag2)
        {
            return (tag1.LineStart > tag2.LineEnd && tag1.LineEnd > tag2.LineEnd)
                   || (tag1.LineStart < tag2.LineStart && tag1.LineEnd < tag2.LineStart);
        }

        /// <summary>
        /// Checks if tag1 and tag2 overlap exactly
        /// </summary>
        /// <param name="tag1"></param>
        /// <param name="tag2"></param>
        /// <returns></returns>
        private static bool TagsOverlapExact(Tag tag1, Tag tag2)
        {
            return (tag1.LineStart == tag2.LineStart && tag1.LineEnd == tag2.LineEnd && !tag1.Id.Equals(tag2.Id));
        }

        /// <summary>
        /// Checks if tag1 is completedly embedded within tag2
        /// </summary>
        /// <param name="tag1"></param>
        /// <param name="tag2"></param>
        /// <returns></returns>
        private static bool TagEmbeddedWithin(Tag tag1, Tag tag2)
        {
            return (tag1.LineStart >= tag2.LineStart && tag1.LineEnd <= tag2.LineEnd)
                && !(TagsOverlapExact(tag1, tag2));
        }

        /// <summary>
        /// Checks if tag1 starts before tag2 starts, and ends before or at the
        /// same point as tag2.
        /// </summary>
        /// <param name="tag1"></param>
        /// <param name="tag2"></param>
        /// <returns></returns>
        private static bool TagOverlapsFront(Tag tag1, Tag tag2)
        {
            return (tag1.LineStart < tag2.LineStart && tag1.LineEnd <= tag2.LineEnd && tag1.LineEnd >= tag2.LineStart);
        }

        /// <summary>
        /// Checks if tag1 starts before or at the same time as tag2 ends, and ends after
        /// the end of
        /// </summary>
        /// <param name="tag1"></param>
        /// <param name="tag2"></param>
        /// <returns></returns>
        private static bool TagOverlapsBack(Tag tag1, Tag tag2)
        {
            return (tag1.LineEnd > tag2.LineEnd && tag1.LineStart <= tag2.LineEnd && tag1.LineStart >= tag2.LineStart);
        }

        public static TagCollisionResult DetectTagCollision(Tag tag1, Tag tag2)
        {
            if (tag1 == null || tag2 == null)
            {
                return null;
            }

            if (TagsOutsideEachOther(tag1, tag2))
            {
                return new TagCollisionResult() { Collision = TagCollisionResult.CollisionType.NoOverlap };
            }
            if (TagsOverlapExact(tag1, tag2))
            {
                return new TagCollisionResult() { Collision = TagCollisionResult.CollisionType.OverlapsExact, CollidingTag = tag2 };
            }
            if (TagEmbeddedWithin(tag1, tag2))
            {
                return new TagCollisionResult() { Collision = TagCollisionResult.CollisionType.EmbeddedWithin, CollidingTag = tag2 };
            }
            if (TagOverlapsFront(tag1, tag2))
            {
                return new TagCollisionResult() { Collision = TagCollisionResult.CollisionType.OverlapsFront, CollidingTag = tag2 };
            }
            if (TagOverlapsBack(tag1, tag2))
            {
                return new TagCollisionResult() { Collision = TagCollisionResult.CollisionType.OverlapsBack, CollidingTag = tag2 };
            }
            if (TagEmbeddedWithin(tag2, tag1))
            {
                return new TagCollisionResult() { Collision = TagCollisionResult.CollisionType.Embeds, CollidingTag = tag2 };
            }

            return null;
        }

        /// <summary>
        /// Given a tag, look within the same code file to determine if the tag is embedded within (or overlaps with) another tag.
        /// If so, provide the tag that we overlap with.  Note that we will only provide the first tag if there are multiple nested
        /// tags.  Calling this multiple times would resolve that scenario.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static TagCollisionResult DetectTagCollision(IEnumerable<Tag> allTags, Tag tag)
        {
            // If the tag is null, if the tag has no start or end, or if the list of tags is null
            // or empty, we will return null since there is no way another for us to actually check
            // for tag collisions.
            if (tag == null || !tag.LineStart.HasValue || !tag.LineEnd.HasValue || allTags == null || !allTags.Any())
            {
                return null;
            }

            // Scenarios.  Tag 1 is an existing tag in the code file, Tag 2 represents the new tag that would be 
            //   passed to this method as the tag parameter.

            // 0. No overlap
            //    [ - Tag 1 - ]
            //                 [ - Tag 2 - ]
            if (allTags.All(x => TagsOutsideEachOther(x, tag)))
            {
                return new TagCollisionResult() { Collision = TagCollisionResult.CollisionType.NoOverlap };
            }

            foreach (var arrayTag in allTags)
            {
                var result = DetectTagCollision(tag, arrayTag);
                if (result != null)
                {
                    return result;
                }
                //// 1. Overlaps exact
                ////    [ - Tag 1 - ]
                ////    [ - Tag 2 - ]
                //if (TagsOverlapExact(arrayTag, tag))
                //{
                //    return new TagCollisionResult() { Collision = TagCollisionResult.CollisionType.OverlapsExact, CollidingTag = arrayTag };
                //}

                //// 2. Embedded within
                ////     [ --- Tag 1 --- ]
                ////       [ - Tag 2 - ]
                //// Note the order of parameters here, we want to see if our new tag (tag) is embedded within an existing tag (x)
                //if (TagEmbeddedWithin(tag, arrayTag))
                //{
                //    return new TagCollisionResult() { Collision = TagCollisionResult.CollisionType.EmbeddedWithin, CollidingTag = arrayTag };
                //}

                //// 3. Overlap front
                ////       [ - Tag 1 - ]
                ////    [ -- Tag 2 -- ]
                //if (TagOverlapsFront(tag, arrayTag))
                //{
                //    return new TagCollisionResult() { Collision = TagCollisionResult.CollisionType.OverlapsFront, CollidingTag = arrayTag };
                //}

                //// 4. Overlap back
                ////    [ - Tag 1 - ]
                ////         [ - Tag 2 - ]
                //if (TagOverlapsBack(tag, arrayTag))
                //{
                //    return new TagCollisionResult() { Collision = TagCollisionResult.CollisionType.OverlapsBack, CollidingTag = arrayTag };
                //}

                //// 5. Embeds
                ////       [ - Tag 1 - ]
                ////     [ --- Tag 2 --- ]
                //// Note the order of parameters here, we want to see if an existing tag (x) is embedded within our new tag (tag)
                //// Said another way, we check if our new tag (tag) embeds an existing tag (x) within it
                //if (TagEmbeddedWithin(arrayTag, tag))
                //{
                //    return new TagCollisionResult() { Collision = TagCollisionResult.CollisionType.Embeds, CollidingTag = arrayTag };
                //}
            }

            return null;
        }

        /// <summary>
        /// Given a tag, look within the same code file to determine if the tag is embedded within (or overlaps with) another tag.
        /// If so, provide the tag that we overlap with.  Note that we will only provide the first tag if there are multiple nested
        /// tags.  Calling this multiple times would resolve that scenario.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static TagCollisionResult DetectTagCollision(Tag tag)
        {
            // If the tag is null, if the tag has no code file reference, if the code file has no
            // tags collection, or if the tag has no start or end set we will return null since there 
            // is no way another for us to actually check for tag collisions.
            if (tag == null || tag.CodeFile == null || tag.CodeFile.Tags == null
                || !tag.LineStart.HasValue || !tag.LineEnd.HasValue)
            {
                return null;
            }

            // We know that we have a tag file that's linked up.  So now, check to see if there are
            // any other code files.  If not, we have no overlap.
            var allTags = tag.CodeFile.Tags;
            if (allTags.Count == 0)
            {
                return null;
            }

            return DetectTagCollision(allTags, tag);
        }
    }
}
