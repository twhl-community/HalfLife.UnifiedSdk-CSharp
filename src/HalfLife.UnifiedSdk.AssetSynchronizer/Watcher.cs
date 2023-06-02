using Serilog;

namespace HalfLife.UnifiedSdk.AssetSynchronizer
{
    /// <summary>
    /// Watches for file changes, creations and renaming and mirrors it to the destination.
    /// </summary>
    internal sealed class Watcher : IDisposable
    {
        private readonly ILogger _logger;

        private readonly FileCopier _fileCopier;

        private readonly FileSystemWatcher _watcher;

        public string SourcePath => _watcher.Path;

        public string DestinationPath { get; }

        public Watcher(ILogger logger, FileCopier fileCopier, string source, string destination, string pattern, bool recursive)
        {
            _logger = logger;
            _fileCopier = fileCopier;

            _watcher = new FileSystemWatcher(source, pattern)
            {
                NotifyFilter = NotifyFilters.FileName
                    | NotifyFilters.LastWrite,

                IncludeSubdirectories = recursive,
                InternalBufferSize = 64 * 1024, // Max is 64 KB
            };

            _watcher.Changed += OnChanged;
            _watcher.Created += OnCreated;
            _watcher.Renamed += OnRenamed;
            _watcher.Error += OnError;

            DestinationPath = destination;

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

        private void OnError(object sender, ErrorEventArgs e) => WatcherHelpers.PrintException(_logger, e.GetException());

        private void CopyFile(string fileName)
        {
            _fileCopier.Add(new(this, fileName));
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }
    }
}
