using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools;
using System.IO.Compression;

namespace HalfLife.UnifiedSdk.Installer
{
    /// <summary>
    /// Installs game content to a mod.
    /// </summary>
    internal sealed class GameContentInstaller
    {
        private const string NodExtension = ".nod";
        private const string NrpExtension = ".nrp";

        /// <summary>
        /// If set to true no file changes will be written to disk.
        /// </summary>
        public bool IsDryRun { get; init; } = false;

        private bool CopyFiles(string rootDirectory, GameInstallData game)
        {
            // Resolve the path so the printed out path looks cleaner.
            string rootGameDirectory = Path.GetFullPath(Path.Combine(rootDirectory, ".."));
            string sourceModDirectory = Path.Combine(rootGameDirectory, game.Info.ModDirectory);
            string sourceMapsDirectory = Path.Combine(sourceModDirectory, "maps");
            string destinationMapsDirectory = Path.Combine(rootDirectory, "maps");
            string destinationGraphsDirectory = Path.Combine(destinationMapsDirectory, "graphs");

            var entFilesName = game.MapEntFiles is not null ? Path.Combine(rootDirectory, "installer", game.MapEntFiles) : null;

            //Install campaign and training maps.
            var mapsToInstall = game.Info.Maps.Values.Where(m => m.Category == MapCategory.Campaign || m.Category == MapCategory.Training);

            //Verify that everything exists before continuing.
            if (!VerifyFilesExist(
                sourceMapsDirectory,
                entFilesName,
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

            using var entFiles = entFilesName is not null ? ZipFile.Open(entFilesName, ZipArchiveMode.Read) : null;

            Console.WriteLine($"Copying maps from \"{sourceMapsDirectory}\" to \"{destinationMapsDirectory}\"");
            Console.WriteLine("Node graph files in destination for maps being copied will be deleted.");

            //Process maps in sorted order to make it easier to read the output and spot problems.
            foreach (var map in mapsToInstall.OrderBy(m => m.Name))
            {
                var baseMapName = map.Name + MapFormats.Bsp.Extension;
                var sourceMapName = Path.Combine(sourceMapsDirectory, baseMapName);
                var destinationMapName = Path.Combine(destinationMapsDirectory, baseMapName);

                //Use relative paths to keep the output small.
                Console.WriteLine($"Copying map \"{baseMapName}\"...");

                // Opening the map auto-converts Blue Shift maps.
                var mapData = MapFormats.Deserialize(sourceMapName);

                var entFileName = map.Name + MapFormats.Ent.Extension;

                if (entFiles?.GetEntry(entFileName) is { } entFile)
                {
                    Console.WriteLine($"\tApplying ent file \"{entFileName}\"...");

                    //Decompress the file data first so we can read it.
                    using var entFileData = entFile.Open();
                    using var decompressedData = new MemoryStream();

                    entFileData.CopyTo(decompressedData);
                    decompressedData.Position = 0;

                    var entMapData = MapFormats.Deserialize(entFileName, decompressedData, MapFormats.Ent);
                    mapData.Entities.ReplaceWith(entMapData.Entities);
                }

                using var stream = File.Open(destinationMapName, FileMode.Create, FileAccess.Write);

                if (!IsDryRun)
                {
                    mapData.Serialize(stream);

                    //Delete any node graph files that exist.
                    File.Delete(Path.Combine(destinationGraphsDirectory, map.Name + NodExtension));
                    File.Delete(Path.Combine(destinationGraphsDirectory, map.Name + NrpExtension));
                }
            }

            Console.WriteLine($"Copied {mapsToInstall.Count()} maps.");

            return true;
        }

        private static bool VerifyFilesExist(string sourceMapsDirectory, string? entFilesName, IEnumerable<string> mapFileNames)
        {
            var filesToCheck = mapFileNames;

            if (entFilesName is not null)
            {
                filesToCheck = filesToCheck.Append(entFilesName);
            }

            IEnumerable<string> directoriesToCheck = new[]
            {
            sourceMapsDirectory
        };

            var missingFiles = filesToCheck.Where(f => !File.Exists(f)).ToList();

            foreach (var fileName in missingFiles)
            {
                Console.WriteLine($"File \"{fileName}\" is missing");
            }

            var missingDirectories = directoriesToCheck.Where(d => !Directory.Exists(d)).ToList();

            foreach (var directory in missingDirectories)
            {
                Console.WriteLine($"Directory \"{directory}\" is missing");
            }

            bool everythingNeededExists = missingFiles.Count == 0 && missingDirectories.Count == 0;

            if (!everythingNeededExists)
            {
                Console.WriteLine("One or more files and/or directories are missing");
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
                Console.WriteLine($"Root directory \"{rootDirectory}\" does not exist");
                return;
            }

            Console.WriteLine($"Installing content to mod directory \"{rootDirectory}\".");

            foreach (var game in games)
            {
                // If liblist.gam exists then the game is probably installed.
                // This is a simple check to prevent people from installing content incorrectly, not a security measure.
                // This isn't a fatal error. Users should be able to install as much content as they can.
                if (!IsGameInstalled(rootDirectory, game.Info.ModDirectory))
                {
                    Console.WriteLine($"Could not find \"{ModUtilities.LiblistFileName}\" for {game.Info.Name}. Make sure the game is installed.");
                    Console.WriteLine("Skipping this game.");
                    continue;
                }

                Console.WriteLine($"Installing {game.Info.Name} content...");

                CopyFiles(rootDirectory, game);

                Console.WriteLine($"Finished installing {game.Info.Name} content.");
            }
        }
    }
}
