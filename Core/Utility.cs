using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Models;

namespace AnalysisManager.Core
{
    public static class Utility
    {
        public static object[] StringArrayToObjectArray(string[] data)
        {
            // Convert to object[] to avoid potential issues (per ReSharper)
            return data.Select(x => x as object).ToArray();
        }

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
    }
}
