using HalfLife.UnifiedSdk.Packager.Config;
using HalfLife.UnifiedSdk.Utilities.Logging;
using Serilog.Events;
using System.CommandLine;
using System.CommandLine.Binding;

namespace HalfLife.UnifiedSdk.Packager
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            var modDirectoryOption = new Option<DirectoryInfo>("--mod-directory", description: "Path to the mod directory")
            {
                IsRequired = true,
            };

            var packageManifestOption = new Option<FileInfo>("--package-manifest", description: "Path to the package manifest file")
            {
                IsRequired = true,
            };

            var packageNameOption = new Option<string>("--package-name", description: "Base name of the package")
            {
                IsRequired = true,
            };

            var verboseOption = new Option<bool>("--verbose",
                getDefaultValue: () => false,
                description: "Log additional information");

            var listOmittedOption = new Option<bool>("--list-omitted",
                getDefaultValue:() => false,
                description: "List files that were omitted from the package");

            var rootCommand = new RootCommand("Half-Life mod packager")
            {
                modDirectoryOption,
                packageManifestOption,
                packageNameOption,
                verboseOption,
                listOmittedOption
            };

            LogEventLevel DetermineLogEventLevel(BindingContext bindingContext)
            {
                // Listing omitted files implies verbose because the list of added files is logged verbose.
                if (bindingContext.ParseResult.GetValueForOption(verboseOption) || bindingContext.ParseResult.GetValueForOption(listOmittedOption))
                {
                    return LogEventLevel.Verbose;
                }
                else
                {
                    return LogEventLevel.Information;
                }
            }

            rootCommand.SetHandler((modDirectory, packageManifest, packageName, listOmitted, logger) =>
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
            listOmittedOption,
            new LoggerBinder(DetermineLogEventLevel));

            return rootCommand.Invoke(args);
        }
    }
}