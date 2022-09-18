
using HalfLife.UnifiedSdk.Packager.Config;
using HalfLife.UnifiedSdk.Utilities.Logging;
using System.CommandLine;

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
            var listOmittedOption = new Option<bool>("--list-omitted", description: "List files that were omitted from the package");

            var rootCommand = new RootCommand("Half-Life mod packager")
            {
                modDirectoryOption,
                packageManifestOption,
                packageNameOption,
                verboseOption,
                listOmittedOption
            };

            rootCommand.SetHandler((modDirectory, packageManifest, packageName, verbose, listOmitted, logger) =>
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

                logger.Information("Loading package manifest \"{ManifestName}\"", packageManifest.FullName);

                try
                {
                    var directories = PackageManifest.Load(logger, packageManifest.FullName, halfLifeDirectory.FullName, modDirectoryName);

                    var options = new PackagerOptions(completePackageName, halfLifeDirectory.FullName, directories)
                    {
                        Verbose = verbose,
                        ListOmittedFiles = listOmitted
                    };

                    Packager.CreatePackage(logger, options);
                }
                catch (ConfigException e)
                {
                    logger.Error("Error loading package manifest: {Message}", e.Message);
                    return;
                }

                Packager.DeleteOldPackages(logger, packageName, halfLifeDirectory, now);
            }, modDirectoryOption, packageManifestOption, packageNameOption,
            verboseOption, listOmittedOption,
            LoggerBinder.Instance);

            return rootCommand.Invoke(args);
        }
    }
}