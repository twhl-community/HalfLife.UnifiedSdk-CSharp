using HalfLife.UnifiedSdk.AssetSynchronizer.Config;
using HalfLife.UnifiedSdk.Utilities.Logging;
using Serilog;
using System.Collections.Immutable;
using System.CommandLine;

namespace HalfLife.UnifiedSdk.AssetSynchronizer
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            var assetsDirectoryOption = new Option<DirectoryInfo>("--assets-directory", description: "Path to the assets directory");
            var modDirectoryOption = new Option<DirectoryInfo>("--mod-directory", description: "Path to the mod directory");
            var assetManifestOption = new Option<FileInfo>("--asset-manifest", description: "Path to the asset manifest file");

            var rootCommand = new RootCommand("Half-Life Unified SDK asset synchronizer")
            {
                assetsDirectoryOption,
                modDirectoryOption,
                assetManifestOption,
            };

            rootCommand.SetHandler((assetsDirectory, modDirectory, assetManifest, logger) =>
            {
                if (!modDirectory.Exists)
                {
                    logger.Error("The given mod directory \"{ModDirectory}\" does not exist", modDirectory);
                    return;
                }

                var manifest = AssetManifest.Load(logger, assetManifest.FullName, assetsDirectory.FullName, modDirectory.FullName);

                //Copy all changed files on startup.
                foreach (var filter in manifest)
                {
                    CopyAllFiles(logger, filter);
                }

                {
                    using var cancellationTokenSource = new CancellationTokenSource();
                    var fileCopier = new FileCopier(logger, cancellationTokenSource.Token);

                    var copyThread = new Thread(fileCopier.Run)
                    {
                        IsBackground = true
                    };

                    copyThread.Start();

                    var watchers = manifest
                        .Select(f => new Watcher(logger, fileCopier, f.Source, f.Destination, f.Pattern, f.Recursive))
                        .ToImmutableList();

                    logger.Information("Watching for file changes");
                    logger.Information("Press any key to stop...");
                    //Block until told to stop.
                    Console.ReadKey();
                    logger.Information("Stopping watchers...");

                    foreach (var watcher in watchers)
                    {
                        watcher.Dispose();
                    }

                    cancellationTokenSource.Cancel();

                    // Wait for pending copy commands to finish.
                    copyThread.Join();
                }

                logger.Information("Done");
            }, assetsDirectoryOption, modDirectoryOption, assetManifestOption, LoggerBinder.Instance);

            return rootCommand.Invoke(args);
        }

        private static void CopyAllFiles(ILogger logger, ManifestFilter filter)
        {
            if (Directory.Exists(filter.Source))
            {
                foreach (var fileName in Directory.EnumerateFiles(filter.Source, filter.Pattern, new EnumerationOptions
                {
                    RecurseSubdirectories = filter.Recursive
                }))
                {
                    var relativePath = Path.GetRelativePath(filter.Source, fileName);
                    var destinationFileName = Path.Combine(filter.Destination, relativePath);

                    CopyIfDifferent(logger, fileName, destinationFileName);
                }
            }
        }

        private static void CopyIfDifferent(ILogger logger, string sourceFileName, string destinationFileName)
        {
            if (!FilesAreEqual(sourceFileName, destinationFileName))
            {
                var directoryName = Path.GetDirectoryName(destinationFileName);

                try
                {
                    if (directoryName is not null)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    File.Copy(sourceFileName, destinationFileName, true);
                }
                catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
                {
                    WatcherHelpers.PrintException(logger, e);
                }
            }
        }

        //Based on https://docs.microsoft.com/en-us/troubleshoot/developer/visualstudio/csharp/language-compilers/create-file-compare
        private static bool FilesAreEqual(string sourceFileName, string destinationFileName)
        {
            if (sourceFileName == destinationFileName)
            {
                return true;
            }

            if (!File.Exists(sourceFileName) || !File.Exists(destinationFileName))
            {
                return false;
            }

            using var fs1 = File.OpenRead(sourceFileName);
            using var fs2 = File.OpenRead(destinationFileName);

            if (fs1.Length != fs2.Length)
            {
                return false;
            }

            int file1byte;
            int file2byte;

            do
            {
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            return file1byte == file2byte;
        }
    }
}