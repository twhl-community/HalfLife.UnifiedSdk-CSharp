namespace HalfLife.UnifiedSdk.Packager
{
    internal sealed record PackageDirectoryEntry(string Path, IEnumerable<string> IncludePatterns, IEnumerable<string> ExcludePatterns);
}
