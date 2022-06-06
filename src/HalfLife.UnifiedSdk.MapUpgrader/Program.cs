using HalfLife.UnifiedSdk.MapUpgrader.Upgrades;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.CommandLine;
using System.CommandLine.IO;

namespace HalfLife.UnifiedSdk.Installer
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            var mapsOption = new Option<IEnumerable<FileInfo>>("--maps", description: "List of maps to upgrade");

            var rootCommand = new RootCommand("Half-Life Unified SDK map upgrader")
            {
                mapsOption
            };

            rootCommand.SetHandler((IEnumerable<FileInfo> maps, IConsole console) =>
            {
                var upgradeTool = MapUpgradeToolFactory.Create();

                console.WriteLine($"Upgrading maps to version {upgradeTool.LatestVersion}");

                foreach (var map in maps)
                {
                    var mapData = MapFormats.Deserialize(map.FullName);

                    var currentVersion = upgradeTool.GetVersion(mapData);

                    console.Out.Write($"Upgrading \"{map.FullName}\" from version {currentVersion}");

                    //TODO: let user specify which game the map is from.
                    upgradeTool.Upgrade(new MapUpgradeCommand(mapData, ValveGames.HalfLife1));

                    using var stream = File.Open(map.FullName, FileMode.Create, FileAccess.Write);

                    mapData.Serialize(stream);
                }
            }, mapsOption);

            return rootCommand.Invoke(args);
        }
    }
}