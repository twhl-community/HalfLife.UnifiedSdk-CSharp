using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.CommandLine;
using System.CommandLine.IO;

namespace HalfLife.UnifiedSdk.Installer
{
    /// <summary>
    /// Installs game content to a mod.
    /// </summary>
    internal sealed class GameContentInstaller
    {
        private const string NodExtension = ".nod";
        private const string NrpExtension = ".nrp";

        private readonly IConsole _console;

        /// <summary>
        /// If set to true no file changes will be written to disk.
        /// </summary>
        public bool IsDryRun { get; init; } = false;

        public GameContentInstaller(IConsole console)
        {
            _console = console;
        }

        private bool CopyFiles(string rootDirectory, GameInstallData game)
        {
            // Resolve the path so the printed out path looks cleaner.
            string rootGameDirectory = Path.GetFullPath(Path.Combine(rootDirectory, ".."));
            string sourceModDirectory = Path.Combine(rootGameDirectory, game.Info.ModDirectory);
            string sourceMapsDirectory = Path.Combine(sourceModDirectory, "maps");
            string destinationMapsDirectory = Path.Combine(rootDirectory, "maps");
            string destinationGraphsDirectory = Path.Combine(destinationMapsDirectory, "graphs");

            //Install campaign and training maps.
            var mapsToInstall = game.Info.Maps.Values.Where(m => m.Category == MapCategory.Campaign || m.Category == MapCategory.Training);

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

            _console.Out.WriteLine($"Copying maps from \"{sourceMapsDirectory}\" to \"{destinationMapsDirectory}\"");
            _console.Out.WriteLine("Node graph files in destination for maps being copied will be deleted.");

            var upgradeTool = game.GetUpgradeTool();

            //Process maps in sorted order to make it easier to read the output and spot problems.
            foreach (var map in mapsToInstall.OrderBy(m => m.Name))
            {
                var baseMapName = map.Name + MapFormats.Bsp.Extension;
                var sourceMapName = Path.Combine(sourceMapsDirectory, baseMapName);
                var destinationMapName = Path.Combine(destinationMapsDirectory, baseMapName);

                //Use relative paths to keep the output small.
                _console.Out.WriteLine($"Copying map \"{baseMapName}\"...");

                // Opening the map auto-converts Blue Shift maps.
                var mapData = MapFormats.Deserialize(sourceMapName);

                upgradeTool.Upgrade(new MapUpgradeCommand(mapData, game.Info));

                using var stream = File.Open(destinationMapName, FileMode.Create, FileAccess.Write);

                if (!IsDryRun)
                {
                    mapData.Serialize(stream);

                    //Delete any node graph files that exist.
                    File.Delete(Path.Combine(destinationGraphsDirectory, map.Name + NodExtension));
                    File.Delete(Path.Combine(destinationGraphsDirectory, map.Name + NrpExtension));
                }
            }

            _console.Out.WriteLine($"Copied {mapsToInstall.Count()} maps.");

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
                _console.Error.WriteLine($"File \"{fileName}\" is missing");
            }

            var missingDirectories = directoriesToCheck.Where(d => !Directory.Exists(d)).ToList();

            foreach (var directory in missingDirectories)
            {
                _console.Error.WriteLine($"Directory \"{directory}\" is missing");
            }

            bool everythingNeededExists = missingFiles.Count == 0 && missingDirectories.Count == 0;

            if (!everythingNeededExists)
            {
                _console.Error.WriteLine("One or more files and/or directories are missing");
            }

            return everythingNeededExists;
        }

        public static bool IsGameInstalled(string rootDirectory, string modDirectory)
        {
            var liblistLocation = Path.GetFullPath(Path.Combine(rootDirectory, "..", modDirectory, ModUtilities.LiblistFileName));
            return File.Exists(liblistLocation);
        }

        public void Install(string rootDirectory, IEnumerable<GameInstallData> games)
        {
            //Sanity check in case things go seriously wrong somehow.
            if (!Directory.Exists(rootDirectory))
            {
                _console.Error.WriteLine($"Root directory \"{rootDirectory}\" does not exist");
                return;
            }

            _console.Out.WriteLine($"Installing content to mod directory \"{rootDirectory}\".");

            foreach (var game in games)
            {
                // If liblist.gam exists then the game is probably installed.
                // This is a simple check to prevent people from installing content incorrectly, not a security measure.
                // This isn't a fatal error. Users should be able to install as much content as they can.
                if (!IsGameInstalled(rootDirectory, game.Info.ModDirectory))
                {
                    _console.Error.WriteLine($"Could not find \"{ModUtilities.LiblistFileName}\" for {game.Info.Name}. Make sure the game is installed.");
                    _console.Error.WriteLine("Skipping this game.");
                    continue;
                }

                _console.Out.WriteLine($"Installing {game.Info.Name} content...");

                CopyFiles(rootDirectory, game);

                _console.Out.WriteLine($"Finished installing {game.Info.Name} content.");
            }
        }
    }
}
