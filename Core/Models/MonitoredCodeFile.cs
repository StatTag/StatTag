using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Core.Models
{
    public class MonitoredCodeFile : IDisposable
    {
        public class FileChangeNotificationData
        {
            public WatcherChangeTypes ChangeType { get; set; }
            public string OldPath { get; set; }
            public string NewPath { get; set; }
        }

        public FileSystemWatcher Watcher { get; set; }
        public CodeFile CodeFile { get; set; }
        public string LastUpdateChecksum { get; set; }
        public List<FileChangeNotificationData> ChangeHistory { get; set; }

        private string OriginalFilePath { get; set; }

        public MonitoredCodeFile(CodeFile codeFile, bool startMonitoring = true)
        {
            if (codeFile == null)
            {
                throw new ArgumentNullException("You must specify a code file to monitor.");
            }
            Watcher = CreateCodeFileWatcher(codeFile, startMonitoring);
            LastUpdateChecksum = codeFile.GetChecksumFromFile();
            ChangeHistory = new List<FileChangeNotificationData>();
            OriginalFilePath = codeFile.FilePath;
        }

        public void Dispose()
        {
            if (Watcher != null)
            {
                Watcher.EnableRaisingEvents = false;
                Watcher.Dispose();
            }
        }

        public void StartMonitoring()
        {
            Watcher.EnableRaisingEvents = true;
        }

        public void StopMonitoring()
        {
            Watcher.EnableRaisingEvents = false;
        }

        public bool IsMonitoring()
        {
            return Watcher.EnableRaisingEvents;
        }

        /// <summary>
        /// For a specific code file instance, create a watcher to respond to certain file system events.
        /// </summary>
        /// <param name="codeFile"></param>
        /// <param name="startMonitoring"></param>
        /// <returns></returns>
        private FileSystemWatcher CreateCodeFileWatcher(CodeFile codeFile, bool startMonitoring)
        {
            var watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(codeFile.FilePath),
                Filter = Path.GetFileName(codeFile.FilePath),
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite
            };
            watcher.EnableRaisingEvents = startMonitoring;
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            return watcher;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // If a file has changed, we only care about that as a binary condition.  If it changes
            // multiple times, we don't need to add it again to the history because the file is
            // still flagged as being changed from the first event.  This will help manage the amount
            // of memory we use when tracking files.
            if (e.ChangeType != WatcherChangeTypes.Changed
                || !ChangeHistory.Any(x => x.ChangeType == WatcherChangeTypes.Changed && x.OldPath.Equals(e.FullPath)))
            {
                ChangeHistory.Add(new FileChangeNotificationData()
                {
                    ChangeType = e.ChangeType,
                    OldPath = e.FullPath
                });
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            ChangeHistory.Add(new FileChangeNotificationData()
            {
                ChangeType = e.ChangeType,
                OldPath = e.OldFullPath,
                NewPath = e.FullPath
            });
        }
    }
}
