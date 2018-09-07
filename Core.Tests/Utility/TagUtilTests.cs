using System;
using System.Collections.Generic;
using System.Linq;
using StatTag.Core.Models;
using StatTag.Core.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Utility
{
    [TestClass]
    public class TagUtilTests
    {
        private readonly List<CodeFile> DistinctTags = new List<CodeFile>(new CodeFile[]
        {
            new CodeFile()
            {
                FilePath = "Test1",
                Tags = new List<Tag>(new Tag[]
                {
                    new Tag() {Name = "Test1"},
                    new Tag() {Name = "Test2"},
                })
            },
            new CodeFile()
            {
                FilePath = "Test2",
                Tags = new List<Tag>(new Tag[]
                {
                    new Tag() {Name = "Test3"},
                    new Tag() {Name = "Test4"},
                })
            },
        });

        private readonly List<CodeFile> DuplicateTags = new List<CodeFile>(new CodeFile[]
        {
            new CodeFile()
            {
                FilePath = "Test1",
                Tags = new List<Tag>(new Tag[]
                {
                    new Tag() {Name = "Test1"},
                    new Tag() {Name = "Test2"},
                    new Tag() {Name = "test1"},
                })
            },
            new CodeFile()
            {
                FilePath = "Test2",
                Tags = new List<Tag>(new Tag[]
                {
                    new Tag() {Name = "test1"},
                    new Tag() {Name = "test2"},
                    new Tag() {Name = "Test1"},
                })
            },
        });

        [TestInitialize]
        public void Initialize()
        {
            InitializeCodeFiles(DistinctTags);
            InitializeCodeFiles(DuplicateTags);
        }

        private void InitializeCodeFiles(List<CodeFile> codeFiles)
        {
            foreach (var codeFile in codeFiles)
            {
                foreach (var tag in codeFile.Tags)
                {
                    tag.CodeFile = codeFile;
                }
            }
        }

        [TestMethod]
        public void FindTagsByName_NullAndEmpty()
        {
            Assert.IsNull(TagUtil.FindTagsByName(string.Empty, null));
            Assert.IsNull(TagUtil.FindTagsByName(null, new List<CodeFile>()));
        }

        [TestMethod]
        public void FindTagsByName_SingleResults()
        {
            Assert.AreEqual(0, TagUtil.FindTagsByName(string.Empty, DistinctTags).Count);
            var tags = TagUtil.FindTagsByName("Test3", DistinctTags);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual("Test2", tags[0].CodeFile.FilePath);
        }

        [TestMethod]
        public void FindTagsByName_MultipleResults()
        {
            Assert.AreEqual(0, TagUtil.FindTagsByName(string.Empty, DuplicateTags).Count);
            var tags = TagUtil.FindTagsByName("test1", DuplicateTags);
            Assert.AreEqual(4, tags.Count);
        }

        [TestMethod]
        public void CheckForDuplicateLabels_Null()
        {
            Assert.IsNull(TagUtil.CheckForDuplicateLabels(null, null));
            Assert.IsNull(TagUtil.CheckForDuplicateLabels(null, new List<CodeFile>()));
            var tag = new Tag() { Name = "test" };
            Assert.IsNull(TagUtil.CheckForDuplicateLabels(tag, new List<CodeFile>()));
            Assert.IsNull(TagUtil.CheckForDuplicateLabels(tag, null));
        }

        [TestMethod]
        public void CheckForDuplicateLabels_SingleResults()
        {
            // Start with an exact match (which gets ignored)
            var tag = DistinctTags.First().Tags.First();
            var results = TagUtil.CheckForDuplicateLabels(tag, DistinctTags);
            Assert.AreEqual(0, results.Count);

            // Next find one with an exactly matching label in the same file
            tag = new Tag() { Name = "Test1", CodeFile = DistinctTags.First() };
            results = TagUtil.CheckForDuplicateLabels(tag, DistinctTags);
            Assert.AreEqual(1, results.Count);

            // Next find one with an exactly matching label in a different file
            tag = new Tag() { Name = "Test1", CodeFile = new CodeFile() { FilePath = "NewCodeFile.r" } };
            results = TagUtil.CheckForDuplicateLabels(tag, DistinctTags);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Test1", results.First().Key.FilePath);
            Assert.AreEqual(1, results.First().Value[0]);
            Assert.AreEqual(0, results.First().Value[1]);

            // Finally, look for the same label but case insensitive
            tag = new Tag() { Name = "test1", CodeFile = new CodeFile() { FilePath = "NewCodeFile.r" } };
            results = TagUtil.CheckForDuplicateLabels(tag, DistinctTags);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Test1", results.First().Key.FilePath);
            Assert.AreEqual(0, results.First().Value[0]);
            Assert.AreEqual(1, results.First().Value[1]);
        }

        [TestMethod]
        public void CheckForDuplicateLabels_MultipleResults()
        {
            // Here we simulate creating a new tag object in an existing file, which is going to have
            // the same name as an existing tag.  We will also identify those tags that have
            // case-insensitive name matches.  All of these should be identified by the check.
            var tag = new Tag() { Name = "Test1", CodeFile = DuplicateTags.First() };
            var results = TagUtil.CheckForDuplicateLabels(tag, DuplicateTags);
            Assert.AreEqual(2, results.Count);
            var fileKey = DuplicateTags[0];
            Assert.AreEqual(1, results[fileKey][0]);
            Assert.AreEqual(1, results[fileKey][1]);
            fileKey = DuplicateTags[1];
            Assert.AreEqual(1, results[fileKey][0]);
            Assert.AreEqual(1, results[fileKey][1]);

            // Next find those with matching labels (both exact and non-exact) even if we're in another file
            tag = new Tag() { Name = "Test1", CodeFile = new CodeFile() { FilePath = "NewCodeFile.r" } };
            results = TagUtil.CheckForDuplicateLabels(tag, DuplicateTags);
            Assert.AreEqual(2, results.Count);
            fileKey = DuplicateTags[0];
            Assert.AreEqual(1, results[fileKey][0]);
            Assert.AreEqual(1, results[fileKey][1]);
            fileKey = DuplicateTags[1];
            Assert.AreEqual(1, results[fileKey][0]);
            Assert.AreEqual(1, results[fileKey][1]);

            // Search with the first tag which is the same object as an existing one.  We should know that
            // they are the same and not count it.
            tag = DuplicateTags.First().Tags.First();
            results = TagUtil.CheckForDuplicateLabels(tag, DuplicateTags);
            Assert.AreEqual(2, results.Count);
            fileKey = DuplicateTags[0];
            Assert.AreEqual(0, results[fileKey][0]);
            Assert.AreEqual(1, results[fileKey][1]);
            fileKey = DuplicateTags[1];
            Assert.AreEqual(1, results[fileKey][0]);
            Assert.AreEqual(1, results[fileKey][1]);
        }

        [TestMethod]
        public void ShouldCheckForDuplicateLabel()
        {
            Tag oldTag = null;
            Tag newTag = null;
            Assert.IsFalse(TagUtil.ShouldCheckForDuplicateLabel(oldTag, newTag));

            // We went from having no tag (null) to a new tag.  It should perform the check.
            newTag = new Tag() { Name = "Test" };
            Assert.IsTrue(TagUtil.ShouldCheckForDuplicateLabel(oldTag, newTag));

            // We now have the old tag and the new tag being the same.  It should not do the check.
            oldTag = new Tag() { Name = "Test" };
            Assert.IsFalse(TagUtil.ShouldCheckForDuplicateLabel(oldTag, newTag));

            // The name is slightly different - now it should do the check
            oldTag = new Tag() { Name = "test" };
            Assert.IsTrue(TagUtil.ShouldCheckForDuplicateLabel(oldTag, newTag));

            // Finally, the new tag is null.  There's nothing there, so we do not want to do a check.
            newTag = null;
            Assert.IsFalse(TagUtil.ShouldCheckForDuplicateLabel(oldTag, newTag));
        }

        [TestMethod]
        public void IsDuplicateLabelInSameFile()
        {
            // We will have results for two different code files.  We will have an tag that is represented in one
            // of the code files, and one that isn't.
            var results = new Dictionary<CodeFile, int[]>();
            results.Add(new CodeFile() { FilePath = "Test1.do"}, new[] { 0, 0 });
            results.Add(new CodeFile() { FilePath = "Test2.do" }, new[] { 0, 0 });

            var tagInFile = new Tag() { Name = "Test", CodeFile = results.First().Key };
            var tagNotInFile = new Tag() { Name = "Test", CodeFile = null };
            var tagInOtherFile = new Tag() { Name = "Test", CodeFile = new CodeFile() { FilePath = "Test3.do"} };

            // Check our null conditions first
            Assert.IsFalse(TagUtil.IsDuplicateLabelInSameFile(null, results));
            Assert.IsFalse(TagUtil.IsDuplicateLabelInSameFile(tagInFile, null));
            Assert.IsFalse(TagUtil.IsDuplicateLabelInSameFile(tagNotInFile, results));

            // If the code file isn't found in the results, it means there is no duplicate.
            Assert.IsFalse(TagUtil.IsDuplicateLabelInSameFile(tagInOtherFile, results));

            // If the code file is found in the results, it only counts if there are duplicates counted in the results.
            Assert.IsFalse(TagUtil.IsDuplicateLabelInSameFile(tagInFile, results));

            // It's a duplicate once we have any kind of result.
            results[results.First().Key] = new[] { 1, 0 };
            Assert.IsTrue(TagUtil.IsDuplicateLabelInSameFile(tagInFile, results));
            results[results.First().Key] = new[] { 0, 1 };
            Assert.IsTrue(TagUtil.IsDuplicateLabelInSameFile(tagInFile, results));
            results[results.First().Key] = new[] { 1, 1 };
            Assert.IsTrue(TagUtil.IsDuplicateLabelInSameFile(tagInFile, results));
        }

        [TestMethod]
        public void DetectTagCollision_NullEmpty()
        {
            Assert.IsNull(TagUtil.DetectTagCollision(null));
            Assert.IsNull(TagUtil.DetectTagCollision(new Tag()));  // No code file
            Assert.IsNull(TagUtil.DetectTagCollision(new Tag() { CodeFile = new CodeFile() }));  // Code file with no other tags
        }

        [TestMethod]
        public void DetectTagCollision_NoOverlap()
        {
            var codeFile = new CodeFile() {Tags = new List<Tag>()};
            var existingTag = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 5,
                LineEnd = 10,
                Name = "Test 1"
            };
            codeFile.Tags.Add(existingTag);

            // Add the new tag after the first tag
            var newTag = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 15,
                LineEnd = 20,
                Name = "Test 2"
            };

            var result = TagUtil.DetectTagCollision(newTag);
            Assert.AreEqual(TagUtil.TagCollisionResult.CollisionType.NoOverlap, result.Collision);
            codeFile.Tags.Add(newTag);

            // Add another new tag before the first tag
            var tag3 = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 1,
                LineEnd = 4,
                Name = "Test 3"
            };
            result = TagUtil.DetectTagCollision(tag3);
            Assert.AreEqual(TagUtil.TagCollisionResult.CollisionType.NoOverlap, result.Collision);
            codeFile.Tags.Add(tag3);

            // Add one more in the middle of existing tags
            var tag4 = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 11,
                LineEnd = 14,
                Name = "Test 4"
            };
            result = TagUtil.DetectTagCollision(tag4);
            Assert.AreEqual(TagUtil.TagCollisionResult.CollisionType.NoOverlap, result.Collision);
        }

        [TestMethod]
        public void DetectTagCollision_EmbeddedWithin()
        {
            var codeFile = new CodeFile() {Tags = new List<Tag>()};
            var existingTag = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 1,
                LineEnd = 10,
                Name = "Test 1"
            };
            codeFile.Tags.Add(existingTag);

            // Add the new tag immediately within the existing tag
            var newTag = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 2,
                LineEnd = 9,
                Name = "Test 2"
            };

            var result = TagUtil.DetectTagCollision(newTag);
            Assert.AreEqual(TagUtil.TagCollisionResult.CollisionType.EmbeddedWithin, result.Collision);
            Assert.AreEqual(existingTag, result.CollidingTag);
            codeFile.Tags.Add(newTag);

            // Add the new tag within the 2nd tag (to show we can detect multiple
            // levels of nesting)
            var tag3 = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 4,
                LineEnd = 7,
                Name = "Test 3"
            };

            result = TagUtil.DetectTagCollision(tag3);
            Assert.AreEqual(TagUtil.TagCollisionResult.CollisionType.EmbeddedWithin, result.Collision);
            Assert.AreEqual(existingTag, result.CollidingTag);
            codeFile.Tags.Add(newTag);  // It should find the outermost tag first in the search
        }

        [TestMethod]
        public void DetectTagCollision_EmbeddedWithin_BoundaryConditions()
        {
            var codeFile = new CodeFile() { Tags = new List<Tag>() };
            var existingTag = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 2,
                LineEnd = 4,
                Name = "Test 1"
            };
            codeFile.Tags.Add(existingTag);

            // The new tag starts at the same position as the old tag, and ends within it
            var newTag = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 2,
                LineEnd = 3,
                Name = "Test 2"
            };

            var result = TagUtil.DetectTagCollision(newTag);
            Assert.AreEqual(TagUtil.TagCollisionResult.CollisionType.EmbeddedWithin, result.Collision);
            Assert.AreEqual(existingTag, result.CollidingTag);

            // The new tag starts inside the old tag, and ends at the same position as the old tag
            var tag3 = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 3,
                LineEnd = 4,
                Name = "Test 3"
            };

            result = TagUtil.DetectTagCollision(tag3);
            Assert.AreEqual(TagUtil.TagCollisionResult.CollisionType.EmbeddedWithin, result.Collision);
            Assert.AreEqual(existingTag, result.CollidingTag);
        }

        [TestMethod]
        public void DetectTagCollision_OverlapsFront()
        {
            var codeFile = new CodeFile() { Tags = new List<Tag>() };
            var existingTag = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 5,
                LineEnd = 10,
                Name = "Test 1"
            };
            codeFile.Tags.Add(existingTag);

            // First we do a true overlap, where the start and end are at different indexes
            // Note that we are NOT adding these to the code file as we go along, we're replacing
            // each of the tags with a new one for these tests
            var newTag = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 1,
                LineEnd = 6,
                Name = "Test 2"
            };

            var result = TagUtil.DetectTagCollision(newTag);
            Assert.AreEqual(TagUtil.TagCollisionResult.CollisionType.OverlapsFront, result.Collision);
            Assert.AreEqual(existingTag, result.CollidingTag);

            // Next, overlap where the end of the new tag just touches the start of the existing tag
            newTag = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 1,
                LineEnd = 5,
                Name = "Test 2"
            };

            result = TagUtil.DetectTagCollision(newTag);
            Assert.AreEqual(TagUtil.TagCollisionResult.CollisionType.OverlapsFront, result.Collision);
            Assert.AreEqual(existingTag, result.CollidingTag);

            // Finally, overlap where the end of the new tag touches the end of the existing tag
            newTag = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 1,
                LineEnd = 10,
                Name = "Test 2"
            };

            result = TagUtil.DetectTagCollision(newTag);
            Assert.AreEqual(TagUtil.TagCollisionResult.CollisionType.OverlapsFront, result.Collision);
            Assert.AreEqual(existingTag, result.CollidingTag);
        }

        [TestMethod]
        public void DetectTagCollision_OverlapsBack()
        {
            var codeFile = new CodeFile() { Tags = new List<Tag>() };
            var existingTag = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 5,
                LineEnd = 10,
                Name = "Test 1"
            };
            codeFile.Tags.Add(existingTag);

            // First we do a true overlap, where the start and end are at different indexes
            // Note that we are NOT adding these to the code file as we go along, we're replacing
            // each of the tags with a new one for these tests
            var newTag = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 6,
                LineEnd = 11,
                Name = "Test 2"
            };

            var result = TagUtil.DetectTagCollision(newTag);
            Assert.AreEqual(TagUtil.TagCollisionResult.CollisionType.OverlapsBack, result.Collision);
            Assert.AreEqual(existingTag, result.CollidingTag);

            // Next, overlap where the start of the new tag just touches the start of the existing tag
            newTag = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 5,
                LineEnd = 11,
                Name = "Test 2"
            };

            result = TagUtil.DetectTagCollision(newTag);
            Assert.AreEqual(TagUtil.TagCollisionResult.CollisionType.OverlapsBack, result.Collision);
            Assert.AreEqual(existingTag, result.CollidingTag);

            // Finally, overlap where the start of the new tag touches the end of the existing tag
            newTag = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 10,
                LineEnd = 15,
                Name = "Test 2"
            };

            result = TagUtil.DetectTagCollision(newTag);
            Assert.AreEqual(TagUtil.TagCollisionResult.CollisionType.OverlapsBack, result.Collision);
            Assert.AreEqual(existingTag, result.CollidingTag);
        }

        [TestMethod]
        public void DetectTagCollision_Embeds()
        {
            var codeFile = new CodeFile() { Tags = new List<Tag>() };
            var existingTag = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 5,
                LineEnd = 10,
                Name = "Test 1"
            };
            codeFile.Tags.Add(existingTag);

            // Add the new tag immediately within the existing tag
            var newTag = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 4,
                LineEnd = 11,
                Name = "Test 2"
            };

            var result = TagUtil.DetectTagCollision(newTag);
            Assert.AreEqual(TagUtil.TagCollisionResult.CollisionType.Embeds, result.Collision);
            Assert.AreEqual(existingTag, result.CollidingTag);
            codeFile.Tags.Add(newTag);

            // Add the new tag within the 2nd tag (to show we can detect multiple
            // levels of nesting)
            var tag3 = new Tag()
            {
                CodeFile = codeFile,
                LineStart = 3,
                LineEnd = 12,
                Name = "Test 3"
            };

            result = TagUtil.DetectTagCollision(tag3);
            Assert.AreEqual(TagUtil.TagCollisionResult.CollisionType.Embeds, result.Collision);
            Assert.AreEqual(existingTag, result.CollidingTag);
            codeFile.Tags.Add(newTag);  // It should find the outermost tag first in the search
        }
    }
}
