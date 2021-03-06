﻿using System.Collections;
using System.Collections.Generic;
using StatTag.Core.Interfaces;
using System.IO;

namespace StatTag.Core.Models
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

        public void WriteAllLines(string filePath, IEnumerable<string> content)
        {
            File.WriteAllLines(filePath, content);
        }

        public void WriteAllText(string filePath, string contents)
        {
            File.WriteAllText(filePath, contents);
        }

        public void AppendAllText(string filePath, string contents)
        {
            File.AppendAllText(filePath, contents);
        }

        public FileStream OpenWrite(string filePath)
        {
            return File.OpenWrite(filePath);
        }

        public void Move(string sourceFileName, string destFileName)
        {
            File.Move(sourceFileName, destFileName);
        }

        public void Delete(string path)
        {
            File.Delete(path);
        }
    }
}
