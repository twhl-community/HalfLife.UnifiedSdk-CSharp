using HalfLife.UnifiedSdk.MapUpgrader.Upgrades;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Logging;
using HalfLife.UnifiedSdk.Utilities.Tools;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.CommandLine;

namespace HalfLife.UnifiedSdk.MapUpgrader
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            var games = ValveGames.GoldSourceGames;

            var defaultGame = ValveGames.HalfLife1;

            var gameOption = new Option<string>("--game",
                getDefaultValue: () => defaultGame.ModDirectory,
                description: "The name of a game's mod directory to apply upgrades for that game");

            gameOption.AddCompletions(games.Select(g => g.ModDirectory).ToArray());

            gameOption.AddValidator((result) =>
            {
                var game = result.GetValueForOption(gameOption)!;

                if (!games.Any(g => g.ModDirectory == game))
                {
                    result.ErrorMessage = $"Invalid game \"{game}\" specified.";
                }
            });

            var diagnosticsLevelOption = new Option<DiagnosticsLevel>("--diagnostics-level",
                getDefaultValue: () => DiagnosticsLevel.Disabled,
                description: "The diagnostics level to set");

            var mapsArgument = new Argument<IEnumerable<FileInfo>>("maps", description: "List of maps to upgrade");

            var rootCommand = new RootCommand("Half-Life Unified SDK map upgrader")
            {
                gameOption,
                diagnosticsLevelOption,
                mapsArgument
            };

            rootCommand.SetHandler((game, diagnosticsLevel, maps, logger) =>
            {
                var gameInfo = games.SingleOrDefault(g => g.ModDirectory == game);

                if (gameInfo is null)
                {
                    //Should never get here.
                    return;
                }

                var upgradeTool = MapUpgradeToolFactory.Create(logger, diagnosticsLevel);

                logger.Information("Upgrading maps for game {GameName} ({ModDirectory}) to version {LatestVersion}",
                    gameInfo.Name, gameInfo.ModDirectory, upgradeTool.LatestVersion);

                if (!maps.Any())
                {
                    logger.Information("No maps to upgrade");
                    return;
                }

                foreach (var map in maps)
                {
                    var mapData = MapFormats.Deserialize(map.FullName);

                    var currentVersion = upgradeTool.GetVersion(mapData);

                    logger.Information("Upgrading \"{FullName}\" from version {CurrentVersion}", map.FullName, currentVersion);

                    upgradeTool.Upgrade(new MapUpgradeCommand(mapData, gameInfo));

                    using var stream = File.Open(map.FullName, FileMode.Create, FileAccess.Write);

                    mapData.Serialize(stream);
                }
            }, gameOption, diagnosticsLevelOption, mapsArgument, LoggerBinder.Instance);

            return rootCommand.Invoke(args);
        }
    }
}