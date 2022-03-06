using HalfLife.UnifiedSdk.Utilities.Maps;
using Semver;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>A map upgrade command.</summary>
    public sealed record MapUpgrade(Map Map)
    {
        /// <summary>
        /// Version to upgrade from. If left as <see langword="null"/>, the map will be upgraded from its current version.
        /// If no current version key can be found in the map, the map will be upgraded from the first known version.
        /// </summary>
        public SemVersion? From { get; init; }

        /// <summary>
        /// Version to upgrade to. If left as <see langword="null"/>, the map will be upgraded to the latest version.
        /// <see cref="MapUpgradeTool.LatestVersion"/>
        /// </summary>
        public SemVersion? To { get; init; }

        /// <summary>
        /// If <see cref="From"/> is older than the version set by the map, throw an exception.
        /// Default true. This protects against upgrading maps that are already upgraded, which could break entity setups.
        /// </summary>
        public bool ThrowOnTooOldVersion { get; init; } = true;
    }
}
