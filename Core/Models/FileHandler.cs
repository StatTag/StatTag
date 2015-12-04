using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisManager.Core.Interfaces;

namespace AnalysisManager.Core.Models
{
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
