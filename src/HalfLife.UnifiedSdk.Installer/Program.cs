using HalfLife.UnifiedSdk.MapUpgrader.Upgrades;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Logging;
using System.CommandLine;

namespace HalfLife.UnifiedSdk.Installer
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            var modDirectoryOption = new Option<DirectoryInfo>("--mod-directory", description: "Path to the mod directory");
            var dryRunOption = new Option<bool>("--dry-run", description: "If provided no file changes will be written to disk");

            var rootCommand = new RootCommand("Half-Life game content installer")
            {
                modDirectoryOption,
                dryRunOption
            };

            rootCommand.SetHandler((modDirectory, dryRun, logger) =>
            {
                if (!modDirectory.Exists)
                {
                    logger.Error("The given mod directory \"{ModDirectory}\" does not exist", modDirectory);
                    return;
                }

                if (dryRun)
                {
                    logger.Information("Performing dry run.");
                }

                var installer = new GameContentInstaller(logger)
                {
                    IsDryRun = dryRun
                };

                var tool = MapUpgradeToolFactory.Create();

                //List of games whose content can be installed with this tool.
                var games = new[]
                {
                    new GameInstallData(ValveGames.HalfLife1),
                    new GameInstallData(ValveGames.OpposingForce, CopyOpposingForceSoundtrack),
                    new GameInstallData(ValveGames.BlueShift, CopyBlueShiftSoundtrack)
                };

                installer.Install(modDirectory.FullName, tool, games);
            }, modDirectoryOption, dryRunOption, LoggerBinder.Instance);

            return rootCommand.Invoke(args);
        }

        private static void CopySoundtrack(string sourceDirectory, string destinationDirectory, string gameName)
        {
            Directory.CreateDirectory(Path.Combine(destinationDirectory, GameMedia.MediaDirectory));

            foreach (var fileName in GameMedia.MusicFileNames)
            {
                var destinationFileName = GameMedia.GetGameSpecificMusicName(fileName, gameName);

                var sourceFileName = Path.Combine(sourceDirectory, GameMedia.GetMusicFileName(fileName));
                destinationFileName = Path.Combine(destinationDirectory, GameMedia.GetMusicFileName(destinationFileName));

                if (File.Exists(sourceFileName))
                {
                    File.Copy(sourceFileName, destinationFileName, true);
                }
            }
        }

        private static void CopyOpposingForceSoundtrack(string sourceDirectory, string destinationDirectory)
        {
            CopySoundtrack(sourceDirectory, destinationDirectory, GameMedia.OpposingForceMusicPrefix);
        }

        private static void CopyBlueShiftSoundtrack(string sourceDirectory, string destinationDirectory)
        {
            CopySoundtrack(sourceDirectory, destinationDirectory, GameMedia.BlueShiftMusicPrefix);
        }
    }
}