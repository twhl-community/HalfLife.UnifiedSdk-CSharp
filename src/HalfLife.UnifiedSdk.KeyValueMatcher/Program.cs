using HalfLife.UnifiedSdk.Utilities.Logging;
using HalfLife.UnifiedSdk.Utilities.Maps;
using HalfLife.UnifiedSdk.Utilities.Tools;
using Serilog;
using System.CommandLine;
using System.Text;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.KeyValueMatcher
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            var printModeOption = new Option<PrintMode>("--print-mode",
                getDefaultValue: () => PrintMode.KeyValue,
                description: "What to print when a match is found");

            var alwaysPrintMapNameOption = new Option<bool>("--always-print-mapname",
                getDefaultValue: () => false,
                description: "Whether to always print the map name or only when a match is found");

            var classNamePatternOption = new Option<string?>("--classname", description: "Classname regex pattern");
            var keyPatternOption = new Option<string?>("--key", description: "Key regex pattern");
            var valuePatternOption = new Option<string?>("--value", description: "Value regex pattern");

            var mapsDirectoriesArgument = new Argument<IEnumerable<DirectoryInfo>>("maps-directory",
                description: "One or more paths to the maps directories to search");

            var rootCommand = new RootCommand("Half-Life Unified SDK map batch search tool")
            {
                printModeOption,
                classNamePatternOption,
                keyPatternOption,
                valuePatternOption,
                mapsDirectoriesArgument
            };

            rootCommand.SetHandler((printMode, alwaysPrintMapName, classNamePattern, keyPattern, valuePattern,
                mapsDirectories, logger) =>
            {
                if (!mapsDirectories.Any())
                {
                    logger.Information("No directories to search");
                    return;
                }

                var validatedMapsDirectories = mapsDirectories.ToLookup(p => p.Exists);

                foreach (var directory in validatedMapsDirectories[false])
                {
                    logger.Warning("The given maps directory \"{MapsDirectory}\" does not exist", directory);
                }

                var matcher = new KeyValueMatcher
                {
                    ClassNamePattern = classNamePattern is not null ? new Regex(classNamePattern) : null,
                    KeyPattern = keyPattern is not null ? new Regex(keyPattern) : null,
                    ValuePattern = valuePattern is not null ? new Regex(valuePattern) : null
                };

                var directoriesToSearch = validatedMapsDirectories[true].ToList();

                logger.Information("Searching {Count} directories", directoriesToSearch.Count);

                int numberOfMatches = 0;

                foreach (var directory in directoriesToSearch)
                {
                    foreach (var mapName in directory.EnumerateFiles("*.bsp"))
                    {
                        try
                        {
                            var map = MapFormats.Deserialize(mapName.FullName);

                            numberOfMatches += PrintMatches(map, matcher, logger, printMode, alwaysPrintMapName);
                        }
                        catch (IOException e)
                        {
                            logger.Error(e, "Error loading BSP file {MapName}", mapName.FullName);
                        }
                    }
                }

                logger.Information("Found {Count} matches", numberOfMatches);
            }, printModeOption, alwaysPrintMapNameOption, classNamePatternOption, keyPatternOption, valuePatternOption,
            mapsDirectoriesArgument,
            LoggerBinder.Instance);

            return rootCommand.Invoke(args);
        }

        private static int PrintMatches(Map map, KeyValueMatcher matcher, ILogger logger,
            PrintMode matchPrinting, bool alwaysPrintMapName)
        {
            void PrintMapName()
            {
                logger.Information("Map {MapName}", map.FileName);
            }

            if (alwaysPrintMapName)
            {
                PrintMapName();
            }

            bool printedMapName = alwaysPrintMapName;

            int numberOfMatches = 0;

            StringBuilder builder = new();

            foreach (var entity in map.Entities)
            {
                if (matcher.Match(entity) is { } match)
                {
                    if (!printedMapName)
                    {
                        printedMapName = true;
                        PrintMapName();
                    }

                    builder.Clear();

                    switch (matchPrinting)
                    {
                        case PrintMode.KeyValue:
                            builder.Append($" \"{match.Key}\" \"{match.Value}\"");
                            break;

                        case PrintMode.Entity:
                            builder.AppendLine();
                            builder.AppendLine("{");

                            foreach (var keyValue in entity)
                            {
                                builder.AppendFormat("\"{0}\" \"{1}\"", keyValue.Key, keyValue.Value).AppendLine();
                            }

                            builder.Append('}');
                            break;
                    }

                    logger.Information("{Entity}{KeyValues}", entity.ToString(), builder);
                    ++numberOfMatches;
                }
            }

            return numberOfMatches;
        }
    }
}