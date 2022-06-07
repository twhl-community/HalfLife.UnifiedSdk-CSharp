namespace HalfLife.UnifiedSdk.Packager.Config
{
    internal sealed class PackageManifest
    {
        public IEnumerable<PackagePatternGroup> PatternGroups { get; set; } = Enumerable.Empty<PackagePatternGroup>();
    }
}
