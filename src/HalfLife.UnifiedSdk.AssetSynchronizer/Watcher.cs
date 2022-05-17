using System.CommandLine;

namespace HalfLife.UnifiedSdk.AssetSynchronizer
{
    /// <summary>
    /// Watches for file changes, creations and renaming and mirrors it to the destination.
    /// </summary>
    internal sealed class Watcher : IDisposable
    {
        private readonly IConsole _console;

        private readonly FileSystemWatcher _watcher;

        private readonly string _destination;

        public Watcher(IConsole console, string source, string destination, string pattern, bool recursive)
        {
            _console = console;

            _watcher = new FileSystemWatcher(source, pattern)
            {
                NotifyFilter = NotifyFilters.FileName
                    | NotifyFilters.LastWrite,

                IncludeSubdirectories = recursive
            };

            _watcher.Changed += OnChanged;
            _watcher.Created += OnCreated;
            _watcher.Renamed += OnRenamed;
            _watcher.Error += OnError;

            _destination = destination;

            _watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            CopyFile(e.FullPath);
        }

        private void OnCreated(object sender, FileSystemEventArgs e) => CopyFile(e.FullPath);

        private void OnRenamed(object sender, RenamedEventArgs e) => CopyFile(e.FullPath);

        private void OnError(object sender, ErrorEventArgs e) => WatcherHelpers.PrintException(_console, e.GetException());

        private void CopyFile(string fileName)
        {
            //Rebase the filename to the destination.
            var relativePath = Path.GetRelativePath(_watcher.Path, fileName);
            var destinationFileName = Path.Combine(_destination, relativePath);

            try
            {
                Directory.CreateDirectory(_destination);
                File.Copy(fileName, destinationFileName, true);
            }
            catch(Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                //If anything goes wrong the user should know about it so they can correct the problem.
                //E.g. trying to copy a file but a directory with the same name exists.
                WatcherHelpers.PrintException(_console, e);
            }
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }
    }
}
