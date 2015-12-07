using AnalysisManager.Core.Interfaces;
using System.IO;

namespace AnalysisManager.Core.Models
{
    /// <summary>
    /// Lightweight wrapper on top of the File class from System.IO
    /// </summary>
    public class FileHandler : IFileHandler
    {
        public string[] ReadAllLines(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public void Copy(string sourceFile, string destinationFile)
        {
            File.Copy(sourceFile, destinationFile);
        }
    }
}
