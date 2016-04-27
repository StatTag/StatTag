using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core.Utility
{
    public class AnnotationUtil
    {
        /// <summary>
        /// Find all annotations with a matching output label (regardless of case).
        /// </summary>
        /// <param name="outputLabel">The output label to search for</param>
        /// <returns></returns>
        public static List<Annotation> FindAnnotationsByOutputLabel(string outputLabel, List<CodeFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return null;
            }

            return files.SelectMany(file => file.Annotations).Where(
                annotation => annotation.OutputLabel.Equals(outputLabel, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }

        /// <summary>
        /// Determine if we need to perform a check for possible conflicting output label names.
        /// </summary>
        /// <param name="oldAnnotation"></param>
        /// <param name="newAnnotation"></param>
        /// <returns></returns>
        public static bool ShouldCheckForDuplicateLabel(Annotation oldAnnotation, Annotation newAnnotation)
        {
            if (oldAnnotation == null && newAnnotation != null)
            {
                return true;
            }

            if (newAnnotation == null)
            {
                return false;
            }

            return !oldAnnotation.OutputLabel.Equals(newAnnotation.OutputLabel);
        }

        /// <summary>
        /// Looks across all annotations in a collection of code files to find those that have
        /// the same output label.
        /// </summary>
        /// <param name="annotation"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public static Dictionary<CodeFile, int[]> CheckForDuplicateLabels(Annotation annotation, List<CodeFile> files)
        {
            if (annotation == null)
            {
                return null;
            }

            var annotations = FindAnnotationsByOutputLabel(annotation.OutputLabel, files);
            if (annotations == null || annotations.Count == 0)
            {
                return null;
            }

            var duplicateCount = new Dictionary<CodeFile, int[]>();
            foreach (var otherAnnotation in annotations)
            {
                // If the annotation is the exact some object, skip it.
                if (object.ReferenceEquals(otherAnnotation, annotation))
                {
                    continue;
                }

                if (!duplicateCount.ContainsKey(otherAnnotation.CodeFile))
                {
                    duplicateCount.Add(otherAnnotation.CodeFile, new[] { 0, 0 });
                }

                // If the output labels are an exact match, they go into the first bucket.
                // Otherwise, they are a case-insensitive match and go into the second bucket.
                if (annotation.OutputLabel.Equals(otherAnnotation.OutputLabel))
                {
                    duplicateCount[otherAnnotation.CodeFile][0]++;
                }
                else
                {
                    duplicateCount[otherAnnotation.CodeFile][1]++;
                }
            }

            return duplicateCount;
        }

        /// <summary>
        /// This is expected to be paired with the results of CheckForDuplicateLabels to determine if the annotation
        /// has duplicates that appear in the results.  It assumes we have asserted a duplicate may exist.
        /// </summary>
        /// <param name="annotation"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsDuplicateLabelInSameFile(Annotation annotation, Dictionary<CodeFile, int[]> result)
        {
            // If the annotation itself is null, or it has no code file reference, we will assume this isn't in the same file (or that no
            // file exists for it to be the "same" in).  Likewise, if our result structure is null, we have nothing to check so we are done.
            if (annotation == null || result == null || annotation.CodeFile == null)
            {
                return false;
            }

            // Look in the list of code files that had matching results of some degree to see if this annotation's code file is represented.
            var codeFileResult = result.Where(x => x.Key.Equals(annotation.CodeFile)).Select(x => (KeyValuePair<CodeFile, int[]>?)x).FirstOrDefault();

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
