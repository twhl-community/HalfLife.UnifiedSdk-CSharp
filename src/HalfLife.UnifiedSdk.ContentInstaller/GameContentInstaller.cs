using HalfLife.UnifiedSdk.Utilities.Games;
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
        private const string NodExtension = ".nod";
        private const string NrpExtension = ".nrp";

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
            _logger.Information("Node graph files in destination for maps being copied will be deleted.");

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

                    //Delete any node graph files that exist.
                    File.Delete(Path.Combine(destinationGraphsDirectory, map.Name + NodExtension));
                    File.Delete(Path.Combine(destinationGraphsDirectory, map.Name + NrpExtension));
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

        public static bool IsGameInstalled(string rootDirectory, string modDirectory)
        {
            var liblistLocation = Path.GetFullPath(Path.Combine(rootDirectory, "..", modDirectory, ModUtilities.LiblistFileName));
            return File.Exists(liblistLocation);
        }

        public void Install(string rootDirectory, MapUpgradeTool upgradeTool, IEnumerable<GameInstallData> games)
        {
            //Sanity check in case things go seriously wrong somehow.
            if (!Directory.Exists(rootDirectory))
            {
                _logger.Error("Root directory \"{RootDirectory}\" does not exist", rootDirectory);
                return;
            }

            _logger.Information("Installing content to mod directory \"{RootDirectory}\".", rootDirectory);

            foreach (var game in games)
            {
                // If liblist.gam exists then the game is probably installed.
                // This is a simple check to prevent people from installing content incorrectly, not a security measure.
                // This isn't a fatal error. Users should be able to install as much content as they can.
                if (!IsGameInstalled(rootDirectory, game.Info.ModDirectory))
                {
                    _logger.Error("Could not find \"{LiblistFileName}\" for {GameName}. Make sure the game is installed.",
                        ModUtilities.LiblistFileName, game.Info.Name);
                    _logger.Error("Skipping this game.");
                    continue;
                }

                _logger.Information("Installing {GameName} content...", game.Info.Name);

                CopyFiles(rootDirectory, upgradeTool, game);

                _logger.Information("Finished installing {GameName} content.", game.Info.Name);
            }
        }
    }
}
