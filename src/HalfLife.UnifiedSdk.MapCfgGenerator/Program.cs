using HalfLife.UnifiedSdk.Utilities.Games;
using Newtonsoft.Json;
using System.Collections.Immutable;
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

                Action<GameInfo, string, List<Section>, JsonTextWriter>[] decorators = new[]
                {
                    AddBarneySuit,
                    AddCTFConfiguration,
                    AddEmptySpawnInventory
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

                        List<Section> sections = new();

                        foreach (var decorator in decorators)
                        {
                            decorator(game, map, sections, writer);
                        }

                        if (sections.Count > 0)
                        {
                            writer.WritePropertyName("SectionGroups");
                            writer.WriteStartArray();

                            var sectionGroups = sections.GroupBy(s => s.Condition);

                            if (sectionGroups.SingleOrDefault(g => g.Key.Length == 0) is { } unconditionalSectionGroup)
                            {
                                writer.WriteStartObject();

                                WriteSections(unconditionalSectionGroup, writer);

                                writer.WriteEndObject();
                            }

                            foreach (var sectionGroup in sectionGroups
                                .Where(g => g.Key.Length > 0)
                                .OrderBy(g => g.Key))
                            {
                                writer.WriteStartObject();

                                writer.WritePropertyName("Condition");
                                writer.WriteValue(sectionGroup.Key);

                                WriteSections(sectionGroup, writer);

                                writer.WriteEndObject();
                            }

                            writer.WriteEndArray();
                        }

                        writer.WriteEndObject();
                    }
                }
            }, destinationDirectoryArgument);

            return rootCommand.Invoke(args);
        }

        private static void WriteSections(IEnumerable<Section> sections, JsonTextWriter writer)
        {
            writer.WritePropertyName("Sections");
            writer.WriteStartObject();

            foreach (var section in sections)
            {
                section.WriterCallback(writer);
            }

            writer.WriteEndObject();
        }

        private static void AddBarneySuit(GameInfo game, string mapName, List<Section> sections, JsonTextWriter writer)
        {
            if (mapName != "ba_tram1")
            {
                return;
            }

            sections.Add(new(AddBarneySuitCallback));
        }

        private static void AddBarneySuitCallback(JsonTextWriter writer)
        {
            writer.WritePropertyName("SpawnInventory");
            writer.WriteStartObject();

            writer.WriteComment("Give the player the HEV suit so they can see the HUD");

            writer.WritePropertyName("HasSuit");
            writer.WriteValue(true);

            writer.WriteEndObject();
        }

        private static void AddCTFConfiguration(GameInfo game, string mapName, List<Section> sections, JsonTextWriter writer)
        {
            if (!mapName.StartsWith("op4ctf_") && !mapName.StartsWith("op4cp_"))
            {
                return;
            }

            writer.WritePropertyName("GameMode");
            writer.WriteValue("ctf");

            writer.WritePropertyName("LockGameMode");
            writer.WriteValue(true);
        }

        private static readonly ImmutableArray<string> MapsWithGamePlayerEquip = ImmutableArray.Create(
            "op4cp_park",
            "op4ctf_power",
            "op4ctf_xendance",
            "op4_meanie"
            );

        private static void AddEmptySpawnInventory(GameInfo game, string mapName, List<Section> sections, JsonTextWriter writer)
        {
            if (!MapsWithGamePlayerEquip.Contains(mapName))
            {
                return;
            }

            sections.Add(new(AddEmptySpawnInventoryCallback, "Multiplayer"));
        }

        private static void AddEmptySpawnInventoryCallback(JsonTextWriter writer)
        {
            writer.WritePropertyName("SpawnInventory");
            writer.WriteStartObject();

            writer.WriteComment("This map uses game_player_equip to set player inventory");

            writer.WritePropertyName("Reset");
            writer.WriteValue(true);

            writer.WritePropertyName("HasSuit");
            writer.WriteValue(true);

            writer.WriteEndObject();
        }
    }
}