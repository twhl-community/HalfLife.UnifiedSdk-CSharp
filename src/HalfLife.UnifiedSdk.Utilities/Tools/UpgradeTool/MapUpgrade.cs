using Semver;
using System;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>Represents an upgrade with events to apply changes.</summary>
    public sealed class MapUpgrade : IComparable<MapUpgrade>
    {
        /// <summary>Version this applies to.</summary>
        public SemVersion Version { get; }

        /// <summary>Invoked when a map is being upgraded to the version associated with this upgrade.</summary>
        public event Action<MapUpgradeContext>? Upgrading;

        /// <summary>Invoked when a map has completed the upgrade to the version associated with this upgrade.</summary>
        public event Action<MapUpgradeContext>? Upgraded;

        /// <summary>Creates a new upgrade for the given version.</summary>
        public MapUpgrade(SemVersion version)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        internal void PerformUpgrade(MapUpgradeContext context)
        {
            Upgrading?.Invoke(context);
            Upgraded?.Invoke(context);
        }

        /// <inheritdoc/>
        public int CompareTo(MapUpgrade? other) => Version.CompareTo(other?.Version);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Version.GetHashCode();
        }
    }
}
