using HalfLife.UnifiedSdk.MapUpgrader.Upgrades;
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
            new GameInstallData(ValveGames.HalfLife1, MapUpgradeToolFactory.Create),
            new GameInstallData(ValveGames.OpposingForce, MapUpgradeToolFactory.Create),
            new GameInstallData(ValveGames.BlueShift, MapUpgradeToolFactory.Create)
        };

        public static int Main(string[] args)
        {
            var modDirectoryOption = new Option<DirectoryInfo>("--mod-directory", description: "Path to the mod directory");
            var dryRunOption = new Option<bool>("--dry-run", description: "If provided no file changes will be written to disk");

            var rootCommand = new RootCommand("Half-Life game content installer")
            {
                modDirectoryOption,
                dryRunOption
            };

            rootCommand.SetHandler((DirectoryInfo modDirectory, bool dryRun, IConsole console) =>
            {
                if (!modDirectory.Exists)
                {
                    console.Error.WriteLine($"The given mod directory \"{modDirectory}\" does not exist");
                    return;
                }

                if (dryRun)
                {
                    console.Out.WriteLine("Performing dry run.");
                }

                var installer = new GameContentInstaller(console)
                {
                    IsDryRun = dryRun
                };

                installer.Install(modDirectory.FullName, Games);
            }, modDirectoryOption, dryRunOption);

            return rootCommand.Invoke(args);
        }
    }
}