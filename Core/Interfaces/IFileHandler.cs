namespace AnalysisManager.Core.Interfaces
{
    public interface IFileHandler
    {
        string[] ReadAllLines(string filePath);
        bool Exists(string filePath);
        void Copy(string sourceFile, string destinationFile);
    }
}
