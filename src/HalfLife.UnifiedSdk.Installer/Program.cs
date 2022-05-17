using HalfLife.UnifiedSdk.Installer.Upgrades;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using Semver;
using System.CommandLine;
using System.CommandLine.IO;

namespace HalfLife.UnifiedSdk.Installer
{
    internal static class Program
    {
        private static MapUpgradeTool GetHalfLifeUpgradeTool()
        {
            var unifiedSdk100UpgradeAction = new MapUpgradeAction(new SemVersion(1, 0, 0));

            unifiedSdk100UpgradeAction.Upgrading += new Of4a4BridgeUpgrade().Upgrade;
            unifiedSdk100UpgradeAction.Upgrading += new BaYard4aSlavesUpgrade().Upgrade;

            return new MapUpgradeTool(unifiedSdk100UpgradeAction);
        }

        //List of games whose content can be installed with this tool.
        private static readonly IEnumerable<GameInstallData> Games = new[]
        {
            new GameInstallData(ValveGames.HalfLife1, GetHalfLifeUpgradeTool),
            new GameInstallData(ValveGames.OpposingForce, GetHalfLifeUpgradeTool),
            new GameInstallData(ValveGames.BlueShift, GetHalfLifeUpgradeTool)
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

                var installer = new GameContentInstaller(console)
                {
                    IsDryRun = dryRun
                };

                installer.Install(gameDirectory.FullName, Games);
            }, gameDirectoryOption, dryRunOption);

            return rootCommand.Invoke(args);
        }
    }
}