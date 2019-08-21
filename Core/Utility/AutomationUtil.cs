using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;

namespace StatTag.Core.Utility
{
    public class AutomationUtil
    {
        private const string TemporaryImageFileFolder = "STTemp-Figures";

        public static string InitializeTemporaryImageDirectory(CodeFile file, LogManager logger)
        {
            var temporaryImageFilePath = string.Empty;
            if (file == null)
            {
                return temporaryImageFilePath;
            }

            // Initialize the temporary directory we'll use for figures
            var path = Path.GetDirectoryName(file.FilePath);
            if (!string.IsNullOrEmpty(path))
            {
                temporaryImageFilePath = Path.Combine(path, TemporaryImageFileFolder);
                logger.WriteMessage(string.Format("Creating a temporary image folder at full path: {0}", temporaryImageFilePath));
            }
            else
            {
                // If we don't know what the path is, we'll just create a relative path and hope for the best.
                temporaryImageFilePath = string.Format(".\\{0}", TemporaryImageFileFolder);
                logger.WriteMessage(string.Format("Creating temporary image folder at relative path: {0}", temporaryImageFilePath));
            }

            // Make sure the temp image folder is established.
            if (Directory.Exists(temporaryImageFilePath))
            {
                logger.WriteMessage("Temporary image folder already exists");
            }
            else
            {
                logger.WriteMessage("Temporary image folder does not exist.  We will create it.");
                Directory.CreateDirectory(temporaryImageFilePath);
                logger.WriteMessage("Created the temporary image folder");
            }

            return temporaryImageFilePath;
        }

        /// <summary>
        /// Helper method to clean out the temporary folder used for storing images
        /// </summary>
        /// <param name="deleteFolder"></param>
        public static void CleanTemporaryImageFolder(IStatAutomation automation, string[] cleanupCommands, string temporaryImageFilePath, string temporaryImageFileFilter, bool deleteFolder = false)
        {
            if (Directory.Exists(temporaryImageFilePath))
            {
                if (cleanupCommands != null && automation != null && cleanupCommands.Length > 0)
                {
                    automation.RunCommands(cleanupCommands);
                }

                var files = Directory.GetFiles(temporaryImageFilePath, temporaryImageFileFilter);
                foreach (var file in files)
                {
                    File.Delete(file);
                }

                if (deleteFolder)
                {
                    Directory.Delete(temporaryImageFilePath);
                }
            }
        }
    }
}
