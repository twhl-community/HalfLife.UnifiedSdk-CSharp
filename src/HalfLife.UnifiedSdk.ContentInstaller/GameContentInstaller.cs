using HalfLife.UnifiedSdk.Utilities.Tools;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using Serilog;

namespace HalfLife.UnifiedSdk.ContentInstaller
{
    /// <summary>
    /// Installs game content to a mod.
    /// </summary>
    internal sealed class GameContentInstaller
    {
        private readonly ILogger _logger;

        /// <summary>
        /// If set to true no file changes will be written to disk.
        /// </summary>
        public bool IsDryRun { get; init; } = false;

        public GameContentInstaller(ILogger logger)
        {
            _logger = logger;
        }

        private bool CopyFiles(string rootDirectory, MapUpgradeTool upgradeTool, GameInstallData game)
        {
            // Resolve the path so the printed out path looks cleaner.
            string rootGameDirectory = Path.GetFullPath(Path.Combine(rootDirectory, ".."));
            string sourceModDirectory = Path.Combine(rootGameDirectory, game.Info.ModDirectory);
            string sourceMapsDirectory = Path.Combine(sourceModDirectory, "maps");
            string destinationMapsDirectory = Path.Combine(rootDirectory, "maps");
            string destinationGraphsDirectory = Path.Combine(destinationMapsDirectory, "graphs");

            //Install campaign and training maps.
            var mapsToInstall = game.Info.Maps.Values;

            //Verify that everything exists before continuing.
            if (!VerifyFilesExist(
                sourceMapsDirectory,
                mapsToInstall.Select(map => Path.Combine(sourceMapsDirectory, map.Name + MapFormats.Bsp.Extension))))
            {
                return false;
            }

            if (!IsDryRun)
            {
                //Create the maps directory if it doesn't exist yet.
                Directory.CreateDirectory(destinationMapsDirectory);
                //Create the graphs directory if it doesn't exist yet so node graph deletion doesn't complain.
                Directory.CreateDirectory(destinationGraphsDirectory);
            }

            _logger.Information("Copying maps from \"{SourceMapsDirectory}\" to \"{DestinationMapsDirectory}\"",
                sourceMapsDirectory, destinationMapsDirectory);

            //Process maps in sorted order to make it easier to read the output and spot problems.
            foreach (var map in mapsToInstall.OrderBy(m => m.Name))
            {
                var baseMapName = map.Name + MapFormats.Bsp.Extension;
                var sourceMapName = Path.Combine(sourceMapsDirectory, baseMapName);
                var destinationMapName = Path.Combine(destinationMapsDirectory, baseMapName);

                //Use relative paths to keep the output small.
                _logger.Information("Copying map \"{BaseMapName}\"...", baseMapName);

                // Opening the map auto-converts Blue Shift maps.
                var mapData = MapFormats.Deserialize(sourceMapName);

                upgradeTool.Upgrade(new MapUpgradeCommand(mapData, game.Info));

                if (!IsDryRun)
                {
                    using var stream = File.Open(destinationMapName, FileMode.Create, FileAccess.Write);
                    mapData.Serialize(stream);
                }
            }

            _logger.Information("Copied {Count} maps.", mapsToInstall.Count());

            game.AdditionalCopySteps?.Invoke(sourceModDirectory, rootDirectory);

            return true;
        }

        private bool VerifyFilesExist(string sourceMapsDirectory, IEnumerable<string> mapFileNames)
        {
            var filesToCheck = mapFileNames;

            IEnumerable<string> directoriesToCheck = new[]
            {
                sourceMapsDirectory
            };

            var missingFiles = filesToCheck.Where(f => !File.Exists(f)).ToList();

            foreach (var fileName in missingFiles)
            {
                _logger.Error("File \"{FileName}\" is missing", fileName);
            }

            var missingDirectories = directoriesToCheck.Where(d => !Directory.Exists(d)).ToList();

            foreach (var directory in missingDirectories)
            {
                _logger.Error("Directory \"{Directory}\" is missing", directory);
            }

            bool everythingNeededExists = missingFiles.Count == 0 && missingDirectories.Count == 0;

            if (!everythingNeededExists)
            {
                _logger.Error("One or more files and/or directories are missing");
            }

            return everythingNeededExists;
        }

        private void InstallConfigFiles(string rootDirectory)
        {
            // HACK: if there is no config file, install a default one.
            // TODO: this should work like Steam's depot system where files are marked as userconfig
            // to allow initial copies to be installed without overwriting on subsequent installs.

            _logger.Information("Installing configuration files");

            var configFileNames = new Dictionary<string, string>
            {
                ["default_config.cfg"] = "config.cfg"
            };

            foreach (var (sourceFileName, destinationFileName) in configFileNames)
            {
                var absoluteSourceFileName = Path.Combine(rootDirectory, sourceFileName);
                var absoluteDestinationFileName = Path.Combine(rootDirectory, destinationFileName);

                if (!File.Exists(absoluteDestinationFileName))
                {
                    _logger.Information("Installing default configuration file \"{ConfigFileName}\"",
                        destinationFileName);

                    if (!IsDryRun)
                    {
                        try
                        {
                            File.Move(absoluteSourceFileName, absoluteDestinationFileName);
                        }
                        catch (Exception e)
                        {
                            _logger.Error(e, "Error renaming configuration file \"{ConfigFileName}\"",
                                absoluteSourceFileName);
                        }
                    }
                }
                else
                {
                    _logger.Information(
                        "Skipping installation of configuration file \"{ConfigFileName}\": file already exists",
                        destinationFileName);

                    if (!IsDryRun)
                    {
                        try
                        {
                            File.Delete(absoluteSourceFileName);
                        }
                        catch (Exception e)
                        {
                            _logger.Error(e, "Error deleting default configuration file \"{ConfigFileName}\"",
                                absoluteSourceFileName);
                        }
                    }
                }
            }
        }

        public static bool IsGameInstalled(string rootDirectory, string modDirectory)
        {
            var liblistLocation = Path.GetFullPath(Path.Combine(rootDirectory, "..", modDirectory, ModUtilities.LiblistFileName));
            return File.Exists(liblistLocation);
        }

        /// <summary>
        /// Install files to a given mod directory.
        /// </summary>
        /// <param name="rootDirectory">Absolute path to the mod directory to install to.</param>
        /// <param name="upgradeTool">Upgrade tool to use for upgrading maps.</param>
        /// <param name="games">Games to install content from.</param>
        public void Install(string rootDirectory, MapUpgradeTool upgradeTool, IEnumerable<GameInstallData> games)
        {
            //Sanity check in case things go seriously wrong somehow.
            if (!Directory.Exists(rootDirectory))
            {
                _logger.Error("Root directory \"{RootDirectory}\" does not exist", rootDirectory);
                return;
            }

            _logger.Information("Installing content to mod directory \"{RootDirectory}\".", rootDirectory);

            InstallConfigFiles(rootDirectory);

            foreach (var game in games)
            {
                // If liblist.gam exists then the game is probably installed.
                // This is a simple check to prevent people from installing content incorrectly, not a security measure.
                // This isn't a fatal error. Users should be able to install as much content as they can.
                if (!IsGameInstalled(rootDirectory, game.Info.ModDirectory))
                {
                    if (game.IsRequired)
                    {
                        _logger.Warning("Could not find \"{LiblistFileName}\" for {GameName}. Make sure the game is installed.",
                            ModUtilities.LiblistFileName, game.Info.Name);
                        _logger.Warning("Skipping this game.");
                    }
                    continue;
                }

                _logger.Information("Installing {GameName} content...", game.Info.Name);

                CopyFiles(rootDirectory, upgradeTool, game);

                _logger.Information("Finished installing {GameName} content.", game.Info.Name);
            }

            _logger.Information("Updating last modified time for node graphs");

            foreach (var fileName in Directory.EnumerateFiles(Path.Combine(rootDirectory, "maps", "graphs")))
            {
                _logger.Information("Updating {FileName}", fileName);

                File.SetLastWriteTimeUtc(fileName, DateTime.UtcNow);
            }

            _logger.Information("Finishing updating node graphs");
        }
    }
}
