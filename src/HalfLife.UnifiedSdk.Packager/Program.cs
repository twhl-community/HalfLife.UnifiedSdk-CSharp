
using HalfLife.UnifiedSdk.Packager.Config;
using HalfLife.UnifiedSdk.Utilities.Logging;
using Newtonsoft.Json;
using System.CommandLine;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.Packager
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            var modDirectoryOption = new Option<DirectoryInfo>("--mod-directory", description: "Path to the mod directory");
            var packageManifestOption = new Option<FileInfo>("--package-manifest", description: "Path to the package manifest file");
            var packageNameOption = new Option<string>("--package-name", description: "Base name of the package");
            var verboseOption = new Option<bool>("--verbose", description: "Log additional information");

            var rootCommand = new RootCommand("Half-Life mod packager")
            {
                modDirectoryOption,
                packageManifestOption,
                packageNameOption,
                verboseOption
            };

            rootCommand.SetHandler((modDirectory, packageManifest, packageName, verbose, logger) =>
            {
                //Generate name now so the timestamp matches the start of generation.
                var now = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");

                var completePackageName = $"{packageName}-{now}";

                logger.Information("Mod directory is \"{FullName}\"", modDirectory.FullName);

                if (!modDirectory.Exists)
                {
                    logger.Error("Mod directory \"{FullName}\" does not exist", modDirectory.FullName);
                    return;
                }

                //Move to Half-Life directory.
                var halfLifeDirectory = modDirectory.Parent ?? throw new InvalidOperationException("Couldn't get game directory");
                Environment.CurrentDirectory = halfLifeDirectory.FullName;

                // Get the name of the mod directory.
                var modDirectoryName = Path.GetFileNameWithoutExtension(modDirectory.FullName);

                string ResolveSymbols(string input)
                {
                    return input.Replace("%ModDirectory%", modDirectoryName);
                }

                logger.Information("Loading package manifest \"{ManifestName}\"", packageManifest.FullName);

                var manifest = JsonConvert.DeserializeObject<PackageManifest>(File.ReadAllText(packageManifest.FullName));

                if (manifest is null || !manifest.PatternGroups.Any())
                {
                    logger.Error("Manifest file is empty");
                    return;
                }

                //Resolve paths and make them absolute.
                foreach (var directory in manifest.PatternGroups)
                {
                    directory.Paths = directory.Paths.ConvertAll(p =>
                    {
                        var path = p.Path;

                        path = ResolveSymbols(path);
                        path = Path.Combine(halfLifeDirectory.FullName, path);

                        p.Path = path;

                        return p;
                    });

                    directory.IncludePatterns = directory.IncludePatterns.ConvertAll(ResolveSymbols);
                    directory.ExcludePatterns = directory.ExcludePatterns.ConvertAll(ResolveSymbols);
                }

                //Check if any required paths don't exist.
                var flattened = manifest.PatternGroups.SelectMany(d => d.Paths.Select(p => new
                {
                    p.Path,
                    p.Required,
                    Exists = Directory.Exists(p.Path),
                    d.IncludePatterns,
                    d.ExcludePatterns
                }));

                foreach (var directory in flattened)
                {
                    if (directory.Exists)
                    {
                        logger.Information("Including directory \"{Path}\"", directory.Path);
                    }
                    else
                    {
                        if (!directory.Required)
                        {
                            logger.Information("Directory \"{Path}\" is optional and does not exist, skipping", directory.Path);
                        }
                        else
                        {
                            logger.Error("Directory \"{Path}\" is required and does not exist, aborting", directory.Path);
                            return;
                        }
                    }
                }

                var directories = flattened
                .Where(d => d.Exists)
                .Select(d => new PackageDirectory(d.Path, d.IncludePatterns, d.ExcludePatterns))
                .ToList();

                var options = new PackagerOptions(completePackageName, halfLifeDirectory.FullName, directories)
                {
                    Verbose = verbose
                };

                Packager.CreatePackage(logger, options);

                //Now delete old packages.
                var regex = new Regex($@"{Regex.Escape(packageName)}-(\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d){Packager.PackageExtension}$");

                foreach (var file in halfLifeDirectory.EnumerateFiles($"{packageName}-*{Packager.PackageExtension}"))
                {
                    var match = regex.Match(file.FullName);

                    if (match.Success)
                    {
                        if (match.Groups[1].Captures[0].Value.CompareTo(now) < 0)
                        {
                            logger.Information("Deleting old package \"{PackageName}\"", file.FullName);
                            file.Delete();
                        }
                    }
                }
            }, modDirectoryOption, packageManifestOption, packageNameOption, verboseOption, LoggerBinder.Instance);

            return rootCommand.Invoke(args);
        }
    }
}