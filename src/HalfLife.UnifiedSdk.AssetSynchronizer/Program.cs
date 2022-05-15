using Newtonsoft.Json;
using RoboSharp;
using System.Collections.Immutable;
using System.CommandLine;
using System.CommandLine.IO;
using System.Text;

namespace HalfLife.UnifiedSdk.AssetSynchronizer
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            var assetsDirectoryOption = new Option<DirectoryInfo>("--assets-directory", description: "Path to the assets directory");
            var gameDirectoryOption = new Option<DirectoryInfo>("--game-directory", description: "Path to the game directory");
            var assetManifestOption = new Option<FileInfo>("--asset-manifest", description: "Path to the asset manifest file");

            var rootCommand = new RootCommand("Half-Life Unified SDK asset synchronizer")
{
    assetsDirectoryOption,
    gameDirectoryOption,
    assetManifestOption,
};

            rootCommand.SetHandler((
                DirectoryInfo assetsDirectory, DirectoryInfo gameDirectory,
                FileInfo assetManifest,
                IConsole console) =>
            {
                if (!gameDirectory.Exists)
                {
                    console.Error.WriteLine($"The given game directory \"{gameDirectory}\" does not exist");
                    return;
                }

                //Needed because RoboSharp relies on a codepage not available in NET Core by default.
                CodePagesEncodingProvider.Instance.GetEncoding(437);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                var manifest = JsonConvert.DeserializeObject<List<ManifestFilter>>(File.ReadAllText(assetManifest.FullName)) ?? new();

                var watchers = manifest.Select(filter =>
                {
                    var copy = new RoboCommand();

                    copy.CopyOptions.Source = Path.Combine(assetsDirectory.FullName, filter.Source);
                    copy.CopyOptions.Destination = Path.Combine(gameDirectory.FullName, filter.Destination);
                    copy.CopyOptions.FileFilter = new[] { filter.Pattern };
                    copy.CopyOptions.CopySubdirectories = filter.Recursive;
                    copy.CopyOptions.Purge = true;
                    copy.CopyOptions.MonitorSourceChangesLimit = 1;
                    copy.CopyOptions.MonitorSourceTimeLimit = 1;

                    copy.Start();

                    return copy;
                }).ToImmutableList();

                console.Out.WriteLine("Watching for file changes");
                console.Out.WriteLine("Press any key to stop...");
                //Block until told to stop.
                Console.ReadKey();
                console.Out.Write("Stopping watchers...");

                foreach (var watcher in watchers)
                {
                    watcher.Dispose();
                }

                console.Out.WriteLine("done");
            }, assetsDirectoryOption, gameDirectoryOption, assetManifestOption);

            return rootCommand.Invoke(args);
        }
    }
}