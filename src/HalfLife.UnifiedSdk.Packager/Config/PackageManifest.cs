using Newtonsoft.Json;

namespace HalfLife.UnifiedSdk.Packager.Config
{
    internal sealed class PackageManifest
    {
        [JsonProperty(Required = Required.Always)]
        public IEnumerable<PackagePatternGroup> PatternGroups { get; set; } = Enumerable.Empty<PackagePatternGroup>();
    }
}
