using System;
using System.Collections.Generic;
using System.Linq;
using AnalysisManager.Core.Models;
using AnalysisManager.Core.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Utility
{
    [TestClass]
    public class AnnotationUtilTests
    {
        private readonly List<CodeFile> DistinctAnnotations = new List<CodeFile>(new CodeFile[]
        {
            new CodeFile()
            {
                FilePath = "Test1",
                Annotations = new List<Annotation>(new Annotation[]
                {
                    new Annotation() {OutputLabel = "Test1"},
                    new Annotation() {OutputLabel = "Test2"},
                })
            },
            new CodeFile()
            {
                FilePath = "Test2",
                Annotations = new List<Annotation>(new Annotation[]
                {
                    new Annotation() {OutputLabel = "Test3"},
                    new Annotation() {OutputLabel = "Test4"},
                })
            },
        });

        private readonly List<CodeFile> DuplicateAnnotations = new List<CodeFile>(new CodeFile[]
        {
            new CodeFile()
            {
                FilePath = "Test1",
                Annotations = new List<Annotation>(new Annotation[]
                {
                    new Annotation() {OutputLabel = "Test1"},
                    new Annotation() {OutputLabel = "Test2"},
                    new Annotation() {OutputLabel = "test1"},
                })
            },
            new CodeFile()
            {
                FilePath = "Test2",
                Annotations = new List<Annotation>(new Annotation[]
                {
                    new Annotation() {OutputLabel = "test1"},
                    new Annotation() {OutputLabel = "test2"},
                    new Annotation() {OutputLabel = "Test1"},
                })
            },
        });

        [TestInitialize]
        public void Initialize()
        {
            InitializeCodeFiles(DistinctAnnotations);
            InitializeCodeFiles(DuplicateAnnotations);
        }

        private void InitializeCodeFiles(List<CodeFile> codeFiles)
        {
            foreach (var codeFile in codeFiles)
            {
                foreach (var annotation in codeFile.Annotations)
                {
                    annotation.CodeFile = codeFile;
                }
            }
        }

        [TestMethod]
        public void FindAnnotationsByOutputLabel_NullAndEmpty()
        {
            Assert.IsNull(AnnotationUtil.FindAnnotationsByOutputLabel(string.Empty, null));
            Assert.IsNull(AnnotationUtil.FindAnnotationsByOutputLabel(null, new List<CodeFile>()));
        }

        [TestMethod]
        public void FindAnnotationsByOutputLabel_SingleResults()
        {
            Assert.AreEqual(0, AnnotationUtil.FindAnnotationsByOutputLabel(string.Empty, DistinctAnnotations).Count);
            var annotations = AnnotationUtil.FindAnnotationsByOutputLabel("Test3", DistinctAnnotations);
            Assert.AreEqual(1, annotations.Count);
            Assert.AreEqual("Test2", annotations[0].CodeFile.FilePath);
        }

        [TestMethod]
        public void FindAnnotationsByOutputLabel_MultipleResults()
        {
            Assert.AreEqual(0, AnnotationUtil.FindAnnotationsByOutputLabel(string.Empty, DuplicateAnnotations).Count);
            var annotations = AnnotationUtil.FindAnnotationsByOutputLabel("test1", DuplicateAnnotations);
            Assert.AreEqual(4, annotations.Count);
        }

        [TestMethod]
        public void CheckForDuplicateLabels_Null()
        {
            Assert.IsNull(AnnotationUtil.CheckForDuplicateLabels(null, null));
            Assert.IsNull(AnnotationUtil.CheckForDuplicateLabels(null, new List<CodeFile>()));
            var annotation = new Annotation() { OutputLabel = "test" };
            Assert.IsNull(AnnotationUtil.CheckForDuplicateLabels(annotation, new List<CodeFile>()));
            Assert.IsNull(AnnotationUtil.CheckForDuplicateLabels(annotation, null));
        }

        [TestMethod]
        public void CheckForDuplicateLabels_SingleResults()
        {
            // Start with an exact match (which gets ignored)
            var annotation = DistinctAnnotations.First().Annotations.First();
            var results = AnnotationUtil.CheckForDuplicateLabels(annotation, DistinctAnnotations);
            Assert.AreEqual(0, results.Count);

            // Next find one with an exactly matching label in the same file
            annotation = new Annotation() { OutputLabel = "Test1", CodeFile = DistinctAnnotations.First() };
            results = AnnotationUtil.CheckForDuplicateLabels(annotation, DistinctAnnotations);
            Assert.AreEqual(1, results.Count);

            // Next find one with an exactly matching label in a different file
            annotation = new Annotation() { OutputLabel = "Test1", CodeFile = new CodeFile() { FilePath = "NewCodeFile.r" } };
            results = AnnotationUtil.CheckForDuplicateLabels(annotation, DistinctAnnotations);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Test1", results.First().Key.FilePath);
            Assert.AreEqual(1, results.First().Value[0]);
            Assert.AreEqual(0, results.First().Value[1]);

            // Finally, look for the same label but case insensitive
            annotation = new Annotation() { OutputLabel = "test1", CodeFile = new CodeFile() { FilePath = "NewCodeFile.r" } };
            results = AnnotationUtil.CheckForDuplicateLabels(annotation, DistinctAnnotations);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Test1", results.First().Key.FilePath);
            Assert.AreEqual(0, results.First().Value[0]);
            Assert.AreEqual(1, results.First().Value[1]);
        }

        [TestMethod]
        public void CheckForDuplicateLabels_MultipleResults()
        {
            // Here we simulate creating a new annotation object in an existing file, which is going to have
            // the same name as an existing annotation.  We will also identify those annotations that have
            // case-insensitive name matches.  All of these should be identified by the check.
            var annotation = new Annotation() { OutputLabel = "Test1", CodeFile = DuplicateAnnotations.First() };
            var results = AnnotationUtil.CheckForDuplicateLabels(annotation, DuplicateAnnotations);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(1, results.ElementAt(0).Value[0]);
            Assert.AreEqual(1, results.ElementAt(0).Value[1]);
            Assert.AreEqual(1, results.ElementAt(1).Value[0]);
            Assert.AreEqual(1, results.ElementAt(1).Value[1]);

            // Next find those with matching labels (both exact and non-exact) even if we're in another file
            annotation = new Annotation() { OutputLabel = "Test1", CodeFile = new CodeFile() { FilePath = "NewCodeFile.r" } };
            results = AnnotationUtil.CheckForDuplicateLabels(annotation, DuplicateAnnotations);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(1, results.ElementAt(0).Value[0]);
            Assert.AreEqual(1, results.ElementAt(0).Value[1]);
            Assert.AreEqual(1, results.ElementAt(1).Value[0]);
            Assert.AreEqual(1, results.ElementAt(1).Value[1]);

            // Search with the first annotation which is the same object as an existing one.  We should know that
            // they are the same and not count it.
            annotation = DuplicateAnnotations.First().Annotations.First();
            results = AnnotationUtil.CheckForDuplicateLabels(annotation, DuplicateAnnotations);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(0, results.ElementAt(0).Value[0]);
            Assert.AreEqual(1, results.ElementAt(0).Value[1]);
            Assert.AreEqual(1, results.ElementAt(1).Value[0]);
            Assert.AreEqual(1, results.ElementAt(1).Value[1]);
        }

        [TestMethod]
        public void ShouldCheckForDuplicateLabel()
        {
            Annotation oldAnnotation = null;
            Annotation newAnnotation = null;
            Assert.IsFalse(AnnotationUtil.ShouldCheckForDuplicateLabel(oldAnnotation, newAnnotation));

            // We went from having no annotation (null) to a new annotation.  It should perform the check.
            newAnnotation = new Annotation() { OutputLabel = "Test" };
            Assert.IsTrue(AnnotationUtil.ShouldCheckForDuplicateLabel(oldAnnotation, newAnnotation));

            // We now have the old annotation and the new annotation being the same.  It should not do the check.
            oldAnnotation = new Annotation() { OutputLabel = "Test" };
            Assert.IsFalse(AnnotationUtil.ShouldCheckForDuplicateLabel(oldAnnotation, newAnnotation));

            // The name is slightly different - now it should do the check
            oldAnnotation = new Annotation() { OutputLabel = "test" };
            Assert.IsTrue(AnnotationUtil.ShouldCheckForDuplicateLabel(oldAnnotation, newAnnotation));

            // Finally, the new annotation is null.  There's nothing there, so we do not want to do a check.
            newAnnotation = null;
            Assert.IsFalse(AnnotationUtil.ShouldCheckForDuplicateLabel(oldAnnotation, newAnnotation));
        }

        [TestMethod]
        public void IsDuplicateLabelInSameFile()
        {
            // We will have results for two different code files.  We will have an annotation that is represented in one
            // of the code files, and one that isn't.
            var results = new Dictionary<CodeFile, int[]>();
            results.Add(new CodeFile() { FilePath = "Test1.do"}, new[] { 0, 0 });
            results.Add(new CodeFile() { FilePath = "Test2.do" }, new[] { 0, 0 });

            var annotationInFile = new Annotation() { OutputLabel = "Test", CodeFile = results.First().Key };
            var annotationNotInFile = new Annotation() { OutputLabel = "Test", CodeFile = null };
            var annotationInOtherFile = new Annotation() { OutputLabel = "Test", CodeFile = new CodeFile() { FilePath = "Test3.do"} };

            // Check our null conditions first
            Assert.IsFalse(AnnotationUtil.IsDuplicateLabelInSameFile(null, results));
            Assert.IsFalse(AnnotationUtil.IsDuplicateLabelInSameFile(annotationInFile, null));
            Assert.IsFalse(AnnotationUtil.IsDuplicateLabelInSameFile(annotationNotInFile, results));

            // If the code file isn't found in the results, it means there is no duplicate.
            Assert.IsFalse(AnnotationUtil.IsDuplicateLabelInSameFile(annotationInOtherFile, results));

            // If the code file is found in the results, it only counts if there are duplicates counted in the results.
            Assert.IsFalse(AnnotationUtil.IsDuplicateLabelInSameFile(annotationInFile, results));

            // It's a duplicate once we have any kind of result.
            results[results.First().Key] = new[] { 1, 0 };
            Assert.IsTrue(AnnotationUtil.IsDuplicateLabelInSameFile(annotationInFile, results));
            results[results.First().Key] = new[] { 0, 1 };
            Assert.IsTrue(AnnotationUtil.IsDuplicateLabelInSameFile(annotationInFile, results));
            results[results.First().Key] = new[] { 1, 1 };
            Assert.IsTrue(AnnotationUtil.IsDuplicateLabelInSameFile(annotationInFile, results));
        }
    }
}
