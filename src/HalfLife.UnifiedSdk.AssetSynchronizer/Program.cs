using Newtonsoft.Json;
using System.Collections.Immutable;
using System.CommandLine;
using System.CommandLine.IO;

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

            rootCommand.SetHandler((
                DirectoryInfo assetsDirectory, DirectoryInfo modDirectory,
                FileInfo assetManifest,
                IConsole console) =>
            {
                if (!modDirectory.Exists)
                {
                    console.Error.WriteLine($"The given mod directory \"{modDirectory}\" does not exist");
                    return;
                }

                var manifest = JsonConvert.DeserializeObject<List<ManifestFilter>>(File.ReadAllText(assetManifest.FullName)) ?? new();

                //Convert paths to absolute.
                foreach (var filter in manifest)
                {
                    filter.Source = Path.Combine(assetsDirectory.FullName, filter.Source);
                    filter.Destination = Path.Combine(modDirectory.FullName, filter.Destination);

                    //Verify that the paths are valid.
                    if (File.Exists(filter.Source))
                    {
                        console.Error.WriteLine($"The source directory \"{filter.Source}\" is a file");
                        return;
                    }

                    if (File.Exists(filter.Destination))
                    {
                        console.Error.WriteLine($"The destination directory \"{filter.Destination}\" is a file");
                        return;
                    }
                }

                //Copy all changed files on startup.
                foreach (var filter in manifest)
                {
                    CopyAllFiles(console, filter);
                }

                {
                    var watchers = manifest
                        .Select(f => new Watcher(console, f.Source, f.Destination, f.Pattern, f.Recursive))
                        .ToImmutableList();

                    console.Out.WriteLine("Watching for file changes");
                    console.Out.WriteLine("Press any key to stop...");
                    //Block until told to stop.
                    Console.ReadKey();
                    console.Out.Write("Stopping watchers...");

                    foreach (var watcher in watchers)
                    {
                        watcher.Dispose();
                    }
                }

                console.Out.WriteLine("done");
            }, assetsDirectoryOption, modDirectoryOption, assetManifestOption);

            return rootCommand.Invoke(args);
        }

        private static void CopyAllFiles(IConsole console, ManifestFilter filter)
        {
            foreach (var fileName in Directory.EnumerateFiles(filter.Source, filter.Pattern, new EnumerationOptions
            {
                RecurseSubdirectories = filter.Recursive
            }))
            {
                var relativePath = Path.GetRelativePath(filter.Source, fileName);
                var destinationFileName = Path.Combine(filter.Destination, relativePath);

                CopyIfDifferent(console, fileName, destinationFileName);
            }
        }

        private static void CopyIfDifferent(IConsole console, string sourceFileName, string destinationFileName)
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
                    WatcherHelpers.PrintException(console, e);
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