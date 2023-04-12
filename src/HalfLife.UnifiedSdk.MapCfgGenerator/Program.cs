using HalfLife.UnifiedSdk.Utilities.Games;
using System.CommandLine;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.MapCfgGenerator
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            var destinationDirectoryArgument = new Argument<DirectoryInfo>("destination-directory", description: "Where to put the generated files");

            var rootCommand = new RootCommand("Half-Life Unified SDK map cfg generator")
            {
                destinationDirectoryArgument
            };

            rootCommand.SetHandler((DirectoryInfo destinationDirectory) =>
            {
                var whitespaceRegex = new Regex(@"\s+");

                var games = new[]
                {
                    ValveGames.HalfLife1,
                    ValveGames.OpposingForce,
                    ValveGames.BlueShift
                };

                foreach (var game in games)
                {
                    var simpleName = whitespaceRegex.Replace(game.Name.Replace("-", ""), "");

                    //Always use forward slashes.
                    var gameConfig = $"cfg/{simpleName}Config.json";

                    var contents = $@"{{
	""Includes"": [
		""{gameConfig}""
	]
}}
".ReplaceLineEndings();

                    foreach (var map in game.CampaignMaps.Concat(game.TrainingMaps).Select(m => m.Name))
                    {
                        var path = Path.Combine(destinationDirectory.FullName, map + ".json");

                        File.WriteAllText(path, contents);
                    }
                }
            }, destinationDirectoryArgument);

            return rootCommand.Invoke(args);
        }
    }
}