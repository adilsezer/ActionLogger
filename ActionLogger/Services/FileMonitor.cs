using System;
using System.IO;

namespace ActionLogger.Services
{
    public static class FileMonitor
    {
        public static event EventHandler<FileEventArgs> FileAccessed = delegate { };

        /// <summary>
        /// Starts monitoring the specified directory for file changes.
        /// </summary>
        /// <param name="directoryPath">The path of the directory to monitor.</param>
        public static void StartMonitoring(string directoryPath)
        {
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = directoryPath,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = "*.*"
            };

            watcher.Changed += OnFileAccessed;
            watcher.Created += OnFileAccessed;
            watcher.Deleted += OnFileAccessed;
            watcher.Renamed += OnFileRenamed;

            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Handles file change, create, and delete events with user-friendly descriptions.
        /// </summary>
        private static void OnFileAccessed(object sender, FileSystemEventArgs e)
        {
            string actionDescription;

            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    actionDescription = $"You updated the document '{e.Name}' in the {Path.GetDirectoryName(e.FullPath)} folder.";
                    break;
                case WatcherChangeTypes.Created:
                    actionDescription = $"You created a new file '{e.Name}' in the {Path.GetDirectoryName(e.FullPath)} folder.";
                    break;
                case WatcherChangeTypes.Deleted:
                    actionDescription = $"You deleted the file '{e.Name}' from the {Path.GetDirectoryName(e.FullPath)} folder.";
                    break;
                default:
                    actionDescription = $"File action detected: {e.Name}.";
                    break;
            }

            FileAccessed(null, new FileEventArgs { FileName = e.Name, Action = actionDescription });
        }

        /// <summary>
        /// Handles file renaming events with user-friendly descriptions.
        /// </summary>
        private static void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            string description = $"You renamed the file '{e.OldName}' to '{e.Name}' in the {Path.GetDirectoryName(e.FullPath)} folder.";
            FileAccessed(null, new FileEventArgs { FileName = e.Name, Action = description });
        }
    }

    /// <summary>
    /// Represents the details of a file access event.
    /// </summary>
    public class FileEventArgs : EventArgs
    {
        public string FileName { get; set; }
        public string Action { get; set; }
    }
}
