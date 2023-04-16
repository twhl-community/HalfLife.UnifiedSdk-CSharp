using HalfLife.UnifiedSdk.Utilities.Games;
using Newtonsoft.Json;
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

                Action<GameInfo, string, JsonTextWriter>[] decorators = new[]
                {
                    AddCTFConfiguration
                };

                foreach (var game in games)
                {
                    var simpleName = whitespaceRegex.Replace(game.Name.Replace("-", ""), "");

                    //Always use forward slashes.
                    var gameConfig = $"cfg/{simpleName}Config.json";

                    foreach (var map in game.Maps.Select(m => m.Key))
                    {
                        var path = Path.Combine(destinationDirectory.FullName, map + ".json");

                        using var writer = new JsonTextWriter(File.CreateText(path));

                        writer.Formatting = Formatting.Indented;
                        writer.Indentation = 1;
                        writer.IndentChar = '\t';

                        writer.WriteStartObject();

                        writer.WritePropertyName("Includes");

                        writer.WriteStartArray();
                        writer.WriteValue(gameConfig);
                        writer.WriteEndArray();

                        foreach (var decorator in decorators)
                        {
                            decorator(game, map, writer);
                        }

                        writer.WriteEndObject();
                    }
                }
            }, destinationDirectoryArgument);

            return rootCommand.Invoke(args);
        }

        private static void AddCTFConfiguration(GameInfo game, string mapName, JsonTextWriter writer)
        {
            if (!mapName.StartsWith("op4ctf_") && !mapName.StartsWith("op4cp_"))
            {
                return;
            }

            writer.WritePropertyName("GameMode");
            writer.WriteValue("ctf");

            writer.WritePropertyName("AllowGameModeOverride");
            writer.WriteValue(false);
        }
    }
}