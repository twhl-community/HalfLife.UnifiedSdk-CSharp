using Serilog;
using System.Collections.Concurrent;

namespace HalfLife.UnifiedSdk.AssetSynchronizer
{
    /// <summary>
    /// Runs file copy operations on a separate thread to minimize time spent responding to FileSystemWatcher events.
    /// </summary>
    internal sealed class FileCopier
    {
        private readonly ILogger _logger;

        private readonly CancellationToken _cancellationToken;

        private readonly ConcurrentQueue<FileCopyItem> _queue = new();

        public FileCopier(ILogger logger, CancellationToken cancellationToken)
        {
            _logger = logger;
            _cancellationToken = cancellationToken;
        }

        public void Run()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                while (_queue.TryDequeue(out var item))
                {
                    CopyFile(item);
                }
            }
        }

        public void Add(FileCopyItem item)
        {
            _queue.Enqueue(item);
        }

        private void CopyFile(FileCopyItem item)
        {
            //Rebase the filename to the destination.
            var relativePath = Path.GetRelativePath(item.Watcher.SourcePath, item.FileName);
            var destinationFileName = Path.Combine(item.Watcher.DestinationPath, relativePath);

            try
            {
                Directory.CreateDirectory(item.Watcher.DestinationPath);
                File.Copy(item.FileName, destinationFileName, true);
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                //If anything goes wrong the user should know about it so they can correct the problem.
                //E.g. trying to copy a file but a directory with the same name exists.
                WatcherHelpers.PrintException(_logger, e);
            }
        }
    }
}
