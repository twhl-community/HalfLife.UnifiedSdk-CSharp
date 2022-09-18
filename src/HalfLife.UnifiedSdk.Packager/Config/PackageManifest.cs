using Newtonsoft.Json;
using Serilog;

namespace HalfLife.UnifiedSdk.Packager.Config
{
    internal sealed class PackageManifest
    {
        [JsonProperty(Required = Required.Always)]
        public IEnumerable<PackagePatternGroup> PatternGroups { get; set; } = Enumerable.Empty<PackagePatternGroup>();

        public static IEnumerable<PackageDirectory> Load(ILogger logger, string packageManifestFileName, string rootDirectory, string modDirectoryName)
        {
            string ResolveSymbols(string input)
            {
                return input.Replace("%ModDirectory%", modDirectoryName);
            }

            var manifest = JsonConvert.DeserializeObject<PackageManifest>(File.ReadAllText(packageManifestFileName));

            if (manifest is null || !manifest.PatternGroups.Any())
            {
                throw new ConfigException("Manifest file is empty");
            }

            //Resolve paths and make them absolute.
            foreach (var directory in manifest.PatternGroups)
            {
                directory.Paths = directory.Paths.ConvertAll(p =>
                {
                    var path = p.Path;

                    path = ResolveSymbols(path);
                    path = Path.Combine(rootDirectory, path);

                    p.Path = path;

                    return p;
                });

                directory.IncludePatterns = directory.IncludePatterns.ConvertAll(ResolveSymbols);
                directory.ExcludePatterns = directory.ExcludePatterns.ConvertAll(ResolveSymbols);
            }

            //Check if any required paths don't exist.
            var flattened = manifest.PatternGroups.SelectMany(d => d.Paths.Select(p => new
            {
                p.Path,
                p.Required,
                Exists = Directory.Exists(p.Path),
                d.IncludePatterns,
                d.ExcludePatterns
            }));

            foreach (var directory in flattened)
            {
                if (directory.Exists)
                {
                    logger.Information("Including directory \"{Path}\"", directory.Path);
                }
                else
                {
                    if (!directory.Required)
                    {
                        logger.Information("Directory \"{Path}\" is optional and does not exist, skipping", directory.Path);
                    }
                    else
                    {
                        throw new ConfigException($"Directory \"{directory.Path}\" is required and does not exist, aborting");
                    }
                }
            }

            return flattened
            .Where(d => d.Exists)
            .Select(d => new PackageDirectory(d.Path, d.IncludePatterns, d.ExcludePatterns))
            .ToList();
        }
    }
}
