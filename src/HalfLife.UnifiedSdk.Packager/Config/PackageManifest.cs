namespace HalfLife.UnifiedSdk.Packager.Config
{
    internal sealed class PackageManifest
    {
        public IEnumerable<PackageDirectory> Directories { get; set; } = Enumerable.Empty<PackageDirectory>();
    }
}
