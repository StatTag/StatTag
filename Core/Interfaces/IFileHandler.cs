using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace StatTag.Core.Interfaces
{
    public interface IFileHandler
    {
        string[] ReadAllLines(string filePath);
        bool Exists(string filePath);
        void Copy(string sourceFile, string destinationFile);
        void WriteAllLines(string filePath, IEnumerable<string> content);
        void WriteAllText(string filePath, string contents);
        void AppendAllText(string filePath, string contents);
        FileStream OpenWrite(string filePath);
    }
}
