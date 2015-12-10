using System.Collections;
using System.Collections.Generic;

namespace AnalysisManager.Core.Interfaces
{
    public interface IFileHandler
    {
        string[] ReadAllLines(string filePath);
        bool Exists(string filePath);
        void Copy(string sourceFile, string destinationFile);
        void WriteAllLines(string filePath, IEnumerable<string> content);
    }
}
