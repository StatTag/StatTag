using System;
using System.Collections.Generic;
using System.Linq;
using AnalysisManager.Core;
using AnalysisManager.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
    [TestClass]
    public class UtilityTests
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
            Assert.IsNull(Utility.FindAnnotationsByOutputLabel(string.Empty, null));
            Assert.IsNull(Utility.FindAnnotationsByOutputLabel(null, new List<CodeFile>()));
        }

        [TestMethod]
        public void FindAnnotationsByOutputLabel_SingleResults()
        {
            Assert.AreEqual(0, Utility.FindAnnotationsByOutputLabel(string.Empty, DistinctAnnotations).Count);
            var annotations = Utility.FindAnnotationsByOutputLabel("Test3", DistinctAnnotations);
            Assert.AreEqual(1, annotations.Count);
            Assert.AreEqual("Test2", annotations[0].CodeFile.FilePath);
        }

        [TestMethod]
        public void FindAnnotationsByOutputLabel_MultipleResults()
        {
            Assert.AreEqual(0, Utility.FindAnnotationsByOutputLabel(string.Empty, DuplicateAnnotations).Count);
            var annotations = Utility.FindAnnotationsByOutputLabel("test1", DuplicateAnnotations);
            Assert.AreEqual(4, annotations.Count);
        }

        [TestMethod]
        public void CheckForDuplicateLabels_Null()
        {
            Assert.IsNull(Utility.CheckForDuplicateLabels(null, null));
            Assert.IsNull(Utility.CheckForDuplicateLabels(null, new List<CodeFile>()));
            var annotation = new Annotation() {OutputLabel = "test"};
            Assert.IsNull(Utility.CheckForDuplicateLabels(annotation, new List<CodeFile>()));
            Assert.IsNull(Utility.CheckForDuplicateLabels(annotation, null));
        }

        [TestMethod]
        public void CheckForDuplicateLabels_SingleResults()
        {
            // Start with an exact match (which gets ignored)
            var annotation = DistinctAnnotations.First().Annotations.First();
            var results = Utility.CheckForDuplicateLabels(annotation, DistinctAnnotations);
            Assert.AreEqual(0, results.Count);

            // Next find one with an exactly matching label in the same file
            annotation = new Annotation() { OutputLabel = "Test1", CodeFile = DistinctAnnotations.First() };
            results = Utility.CheckForDuplicateLabels(annotation, DistinctAnnotations);
            Assert.AreEqual(1, results.Count);

            // Next find one with an exactly matching label in a different file
            annotation = new Annotation() { OutputLabel = "Test1", CodeFile = new CodeFile() { FilePath = "NewCodeFile.r" } };
            results = Utility.CheckForDuplicateLabels(annotation, DistinctAnnotations);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Test1", results.First().Key.FilePath);
            Assert.AreEqual(1, results.First().Value[0]);
            Assert.AreEqual(0, results.First().Value[1]);

            // Finally, look for the same label but case insensitive
            annotation = new Annotation() { OutputLabel = "test1", CodeFile = new CodeFile() { FilePath = "NewCodeFile.r" } };
            results = Utility.CheckForDuplicateLabels(annotation, DistinctAnnotations);
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
            var results = Utility.CheckForDuplicateLabels(annotation, DuplicateAnnotations);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(1, results.ElementAt(0).Value[0]);
            Assert.AreEqual(1, results.ElementAt(0).Value[1]);
            Assert.AreEqual(1, results.ElementAt(1).Value[0]);
            Assert.AreEqual(1, results.ElementAt(1).Value[1]);

            // Next find those with matching labels (both exact and non-exact) even if we're in another file
            annotation = new Annotation() { OutputLabel = "Test1", CodeFile = new CodeFile() { FilePath = "NewCodeFile.r" } };
            results = Utility.CheckForDuplicateLabels(annotation, DuplicateAnnotations);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(1, results.ElementAt(0).Value[0]);
            Assert.AreEqual(1, results.ElementAt(0).Value[1]);
            Assert.AreEqual(1, results.ElementAt(1).Value[0]);
            Assert.AreEqual(1, results.ElementAt(1).Value[1]);

            // Search with the first annotation which is the same object as an existing one.  We should know that
            // they are the same and not count it.
            annotation = DuplicateAnnotations.First().Annotations.First();
            results = Utility.CheckForDuplicateLabels(annotation, DuplicateAnnotations);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(0, results.ElementAt(0).Value[0]);
            Assert.AreEqual(1, results.ElementAt(0).Value[1]);
            Assert.AreEqual(1, results.ElementAt(1).Value[0]);
            Assert.AreEqual(1, results.ElementAt(1).Value[1]);
        }
    }
}
