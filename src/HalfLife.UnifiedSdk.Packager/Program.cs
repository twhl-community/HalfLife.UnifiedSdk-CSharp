
using HalfLife.UnifiedSdk.Utilities.Tools;
using Newtonsoft.Json;
using System.CommandLine;
using System.CommandLine.IO;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.Packager
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            var gameDirectoryOption = new Option<DirectoryInfo>("--game-directory", description: "Path to the game directory");
            var packageManifestOption = new Option<FileInfo>("--package-manifest", description: "Path to the package manifest file");
            var packageNameOption = new Option<string>("--package-name", description: "Base name of the package");

            var rootCommand = new RootCommand("Half-Life game packager")
            {
                gameDirectoryOption,
                packageManifestOption,
                packageNameOption
            };

            rootCommand.SetHandler((DirectoryInfo gameDirectory, FileInfo packageManifest, string packageName, IConsole console) =>
            {
                //Generate name now so the timestamp matches the start of generation.
                var now = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");

                var completePackageName = $"{packageName}-{now}";

                console.Out.WriteLine($"Game directory is \"{gameDirectory.FullName}\"");

                if (!gameDirectory.Exists)
                {
                    console.Error.WriteLine($"Game directory \"{gameDirectory.FullName}\" does not exist");
                    return;
                }

                //Move to Half-Life directory.
                var halfLifeDirectory = gameDirectory.Parent ?? throw new InvalidOperationException("Couldn't get game directory");
                Environment.CurrentDirectory = halfLifeDirectory.FullName;

                // Get the name of the mod directory.
                var modDirectory = Path.GetFileNameWithoutExtension(gameDirectory.FullName);

                console.Out.WriteLine($"Loading package manifest \"{packageManifest.FullName}\"");

                var manifest = JsonConvert.DeserializeObject<PackageManifest>(File.ReadAllText(packageManifest.FullName));

                if (manifest is null)
                {
                    console.Error.WriteLine("Manifest file is empty");
                    return;
                }

                var directories = new[]
                {
                    new PackageDirectory(gameDirectory.FullName, manifest.IncludePatterns, manifest.ExcludePatterns)
                }
                    //Include content directories if they exist.
                    .Concat(ModUtilities.AllPublicModDirectorySuffixes
                        .Select(s => ModUtilities.FormatModDirectory(modDirectory, s))
                        .Where(Directory.Exists)
                        .Select(p => new PackageDirectory(p, new[] { "**/*" }, Array.Empty<string>())));

                Packager.CreatePackage(console, completePackageName, halfLifeDirectory.FullName, directories);

                //Now delete old packages.
                var regex = new Regex($@"{Regex.Escape(packageName)}-(\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d){Packager.PackageExtension}$");

                foreach (var file in halfLifeDirectory.EnumerateFiles($"{packageName}-*{Packager.PackageExtension}"))
                {
                    var match = regex.Match(file.FullName);

                    if (match.Success)
                    {
                        if (match.Groups[1].Captures[0].Value.CompareTo(now) < 0)
                        {
                            console.Out.WriteLine($"Deleting old package \"{file.FullName}\"");
                            file.Delete();
                        }
                    }
                }
            }, gameDirectoryOption, packageManifestOption, packageNameOption);

            return rootCommand.Invoke(args);
        }
    }
}