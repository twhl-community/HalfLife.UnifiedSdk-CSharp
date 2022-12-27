using Newtonsoft.Json;
using Serilog;

namespace HalfLife.UnifiedSdk.AssetSynchronizer.Config
{
    internal sealed class AssetManifest
    {
        [JsonProperty(Required = Required.Always)]
        public IEnumerable<AssetPatternGroup> PatternGroups { get; set; } = Enumerable.Empty<AssetPatternGroup>();

        public static List<ManifestFilter> Load(ILogger logger, string assetManifestFileName, string assetsDirectoryName, string modDirectoryName)
        {
            string ResolveSymbols(string input)
            {
                return input.Replace("%ModDirectory%", modDirectoryName);
            }

            var manifest = JsonConvert.DeserializeObject<AssetManifest>(File.ReadAllText(assetManifestFileName)) ?? new();

            //Convert paths to absolute.
            foreach (var group in manifest.PatternGroups)
            {
                var groupModDirectoryName = ResolveSymbols(group.Path);

                foreach (var filter in group.Filters)
                {
                    filter.Source = Path.Combine(assetsDirectoryName, filter.Source);
                    filter.Destination = Path.Combine(groupModDirectoryName, filter.Destination);

                    //Verify that the paths are valid.
                    if (File.Exists(filter.Source))
                    {
                        throw new ConfigException($"The source directory \"{filter.Source}\" is a file");
                    }

                    if (File.Exists(filter.Destination))
                    {
                        throw new ConfigException($"The destination directory \"{filter.Destination}\" is a file");
                    }

                    if (!Directory.Exists(filter.Source))
                    {
                        logger.Warning("The source directory \"{Source}\" does not exist and will not be monitored", filter.Source);
                    }
                }
            }

            return manifest.PatternGroups
                .SelectMany(g => g.Filters)
                .Where(f => Directory.Exists(f.Source))
                .ToList();
        }
    }
}
