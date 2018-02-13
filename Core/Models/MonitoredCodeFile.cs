using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Core.Models
{
    public class MonitoredCodeFile : CodeFile, IDisposable
    {
        public event EventHandler CodeFileChanged;

        /// <summary>
        /// Contains the information about a specific change that took place
        /// </summary>
        public class FileChangeNotificationData
        {
            /// <summary>
            /// The type of change that took place (rename, deleted, edited)
            /// </summary>
            public WatcherChangeTypes ChangeType { get; set; }

            /// <summary>
            /// The original file path.  This is set as well for delete and edit events.
            /// </summary>
            public string OldPath { get; set; }

            /// <summary>
            /// The new path, used only when a file is renamed
            /// </summary>
            public string NewPath { get; set; }
        }

        /// <summary>
        ///  The code file that is being monitored
        /// </summary>
        //public CodeFile CodeFile { get; set; }

        ///// <summary>
        ///// An MD5 checksum of the file's contents
        ///// </summary>
        //public string LastUpdateChecksum { get; set; }

        /// <summary>
        /// A list containing the history.  The first item will be the first change detected,
        /// and the last item will be the most recent change.
        /// </summary>
        public List<FileChangeNotificationData> ChangeHistory { get; set; }

        private string OriginalFilePath { get; set; }
        private FileSystemWatcher Watcher { get; set; }

        public MonitoredCodeFile(CodeFile codeFile, bool startMonitoring = true) : base(codeFile)
        {
            if (codeFile == null)
            {
                throw new ArgumentNullException("You must specify a code file to monitor.");
            }
            Watcher = CreateCodeFileWatcher(codeFile, startMonitoring);
            // LastUpdateChecksum = codeFile.GetChecksumFromFile();
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

        /// <summary>
        /// Begin the monitor to detect and track changes
        /// </summary>
        public void StartMonitoring()
        {
            Watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Turn off monitoring of the code file.
        /// </summary>
        public void StopMonitoring()
        {
            Watcher.EnableRaisingEvents = false;
        }

        /// <summary>
        /// Is the code file currently being monitored?
        /// </summary>
        /// <returns></returns>
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
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnRenamed;

            return watcher;
        }

        /// <summary>
        /// Handler for an edit, create or delete event
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // If a file has changed, we only care about that as a binary condition.  If it changes
            // multiple times, we don't need to add it again to the history because the file is
            // still flagged as being changed from the first event.  This will help manage the amount
            // of memory we use when tracking files.
            if (e.ChangeType != WatcherChangeTypes.Changed
                || !ChangeHistory.Any(x => x.ChangeType == WatcherChangeTypes.Changed && x.OldPath.Equals(e.FullPath)))
            {
                AddToChangeHistory(new FileChangeNotificationData()
                {
                    ChangeType = e.ChangeType,
                    OldPath = e.FullPath
                });
            }
        }

        /// <summary>
        /// Handler for a file rename event
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            AddToChangeHistory(new FileChangeNotificationData()
            {
                ChangeType = e.ChangeType,
                OldPath = e.OldFullPath,
                NewPath = e.FullPath
            });
        }

        private void AddToChangeHistory(FileChangeNotificationData data)
        {
            ChangeHistory.Add(data);

            if (CodeFileChanged != null)
            {
                CodeFileChanged(this, new EventArgs());
            }
        }

        public override void UpdateContent(string text)
        {
            StopMonitoring();
            base.UpdateContent(text);
            StartMonitoring();
        }

        public override void Save()
        {
            StopMonitoring();
            base.Save();
            StartMonitoring();
        }
    }
}
