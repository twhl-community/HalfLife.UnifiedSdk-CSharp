using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Logging;
using HalfLife.UnifiedSdk.Utilities.Maps;
using HalfLife.UnifiedSdk.Utilities.Tools;
using Serilog;
using Serilog.Events;
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

            var flagsToMatchOption = new Option<string?>("--flags-name",
                description: "If specified, name of the flags key to match (e.g. spawnflags)");

            var flagsMatchModeOption = new Option<FlagsMatchMode>("--flags-match-mode",
                getDefaultValue: () => FlagsMatchMode.Inclusive,
                description: "How to match flags");

            var flagsOption = new Option<int>("--flags",
                getDefaultValue: () => 0,
                description: "Flag value to match");

            var mapsDirectoriesArgument = new Argument<IEnumerable<DirectoryInfo>>("maps-directory",
                description: "One or more paths to the maps directories to search");

            var rootCommand = new RootCommand("Half-Life Unified SDK map batch search tool")
            {
                printModeOption,
                classNamePatternOption,
                keyPatternOption,
                valuePatternOption,
                flagsToMatchOption,
                flagsMatchModeOption,
                flagsOption,
                mapsDirectoriesArgument
            };

            rootCommand.SetHandler(context =>
            {
                var printMode = context.ParseResult.GetValueForOption(printModeOption);
                var alwaysPrintMapName = context.ParseResult.GetValueForOption(alwaysPrintMapNameOption);
                var classNamePattern = context.ParseResult.GetValueForOption(classNamePatternOption);
                var keyPattern = context.ParseResult.GetValueForOption(keyPatternOption);
                var valuePattern = context.ParseResult.GetValueForOption(valuePatternOption);
                var flagsToMatch = context.ParseResult.GetValueForOption(flagsToMatchOption);
                var flagsMatchMode = context.ParseResult.GetValueForOption(flagsMatchModeOption);
                var flags = context.ParseResult.GetValueForOption(flagsOption);
                var mapsDirectories = context.ParseResult.GetValueForArgument(mapsDirectoriesArgument);
                var logger = LoggerBinder.CreateLogger(LogEventLevel.Information);

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
                    ValuePattern = valuePattern is not null ? new Regex(valuePattern) : null,
                    FlagsToMatch = flagsToMatch,
                    FlagsMatchMode = flagsMatchMode,
                    Flags = flags
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
            });

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
                            builder.Append(" \"").Append(match.Key).Append("\" \"").Append(match.Value).Append('\"');

                            if (matcher.FlagsToMatch is not null && matcher.FlagsToMatch != match.Key)
                            {
                                var flagsValue = entity.GetInteger(matcher.FlagsToMatch);
                                builder.Append(" \"").Append(matcher.FlagsToMatch)
                                    .Append("\" \"").Append(flagsValue).Append('\"');
                            }
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