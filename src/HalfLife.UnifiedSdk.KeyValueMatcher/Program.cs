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
            var mapsDirectoryOption = new Option<DirectoryInfo>("--maps-directory", description: "Path to the maps directory to search")
            {
                IsRequired = true
            };

            var printKeyValuesOption = new Option<bool>("--print-keyvalues",
                getDefaultValue: () => true,
                description: "Print the keyvalues of matched entities");

            var classNamePatternOption = new Option<string?>("--classname", description: "Classname regex pattern");
            var keyPatternOption = new Option<string?>("--key", description: "Key regex pattern");
            var valuePatternOption = new Option<string?>("--value", description: "Value regex pattern");

            var rootCommand = new RootCommand("Half-Life Unified SDK map batch search tool")
            {
                mapsDirectoryOption,
                printKeyValuesOption,
                classNamePatternOption,
                keyPatternOption,
                valuePatternOption
            };

            rootCommand.SetHandler((mapsDirectory, printKeyValues, classNamePattern, keyPattern, valuePattern, logger) =>
            {
                if (!mapsDirectory.Exists)
                {
                    logger.Error("The given maps directory \"{MapsDirectory}\" does not exist", mapsDirectory);
                    return;
                }

                var matcher = new KeyValueMatcher
                {
                    ClassNamePattern = classNamePattern is not null ? new Regex(classNamePattern) : null,
                    KeyPattern = keyPattern is not null ? new Regex(keyPattern) : null,
                    ValuePattern = valuePattern is not null ? new Regex(valuePattern) : null
                };

                foreach (var mapName in mapsDirectory.EnumerateFiles("*.bsp"))
                {
                    try
                    {
                        var map = MapFormats.Deserialize(mapName.FullName);

                        PrintMatches(map, matcher, logger, printKeyValues);
                    }
                    catch (IOException e)
                    {
                        logger.Error(e, "Error loading BSP file {MapName}", mapName.FullName);
                    }
                }
            }, mapsDirectoryOption, printKeyValuesOption, classNamePatternOption, keyPatternOption, valuePatternOption, LoggerBinder.Instance);

            return rootCommand.Invoke(args);
        }

        private static void PrintMatches(Map map, KeyValueMatcher matcher, ILogger logger, bool printKeyValues)
        {
            logger.Information("Map {MapName}", map.FileName);

            StringBuilder builder = new();

            foreach (var entity in map.Entities)
            {
                if (matcher.IsMatch(entity))
                {
                    if (printKeyValues)
                    {
                        builder.Clear();

                        builder.AppendLine();
                        builder.AppendLine("{");

                        foreach (var keyValue in entity)
                        {
                            builder.AppendFormat("\"{0}\" \"{1}\"", keyValue.Key, keyValue.Value).AppendLine();
                        }

                        builder.Append('}');
                    }

                    logger.Information("{Entity}{KeyValues}", entity.ToString(), builder);
                }
            }
        }
    }
}