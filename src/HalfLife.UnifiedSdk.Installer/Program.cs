using HalfLife.UnifiedSdk.Utilities.Games;
using System.CommandLine;
using System.CommandLine.IO;

namespace HalfLife.UnifiedSdk.Installer
{
    internal static class Program
    {
        //List of games whose content can be installed with this tool.
        private static readonly IEnumerable<GameInstallData> Games = new[]
        {
            new GameInstallData(ValveGames.HalfLife1),
            new GameInstallData(ValveGames.OpposingForce, MapEntFiles: "op4_map_ent_files.zip"),
            new GameInstallData(ValveGames.BlueShift, MapEntFiles: "bs_map_ent_files.zip")
        };

        public static int Main(string[] args)
        {
            var gameDirectoryOption = new Option<DirectoryInfo>("--game-directory", description: "Path to the game directory");
            var dryRunOption = new Option<bool>("--dry-run", description: "If provided no file changes will be written to disk");

            var rootCommand = new RootCommand("Half-Life game content installer")
            {
                gameDirectoryOption,
                dryRunOption
            };

            rootCommand.SetHandler((DirectoryInfo gameDirectory, bool dryRun, IConsole console) =>
            {
                if (!gameDirectory.Exists)
                {
                    console.Error.WriteLine($"The given game directory \"{gameDirectory}\" does not exist");
                    return;
                }

                if (dryRun)
                {
                    console.Out.WriteLine("Performing dry run.");
                }

                var installer = new GameContentInstaller
                {
                    IsDryRun = dryRun
                };

                installer.Install(gameDirectory.FullName, Games);
            }, gameDirectoryOption, dryRunOption);

            return rootCommand.Invoke(args);
        }
    }
}