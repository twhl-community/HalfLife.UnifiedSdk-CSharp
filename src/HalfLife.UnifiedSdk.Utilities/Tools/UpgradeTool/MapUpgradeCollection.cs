using Semver;
using System;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>Represents an upgrade with events to apply changes.</summary>
    public sealed class MapUpgradeCollection : IComparable<MapUpgradeCollection>
    {
        private readonly ImmutableList<MapUpgrade> _upgrades;

        /// <summary>Version this applies to.</summary>
        public SemVersion Version { get; }

        internal MapUpgradeCollection(SemVersion version, ImmutableList<MapUpgrade> upgrades)
        {
            Version = version;
            _upgrades = upgrades;
        }

        internal void PerformUpgrade(MapUpgradeContext context)
        {
            foreach (var upgrade in _upgrades)
            {
                upgrade.Apply(context);
            }
        }

        /// <inheritdoc/>
        public int CompareTo(MapUpgradeCollection? other) => Version.ComparePrecedenceTo(other?.Version);

        /// <inheritdoc/>
        public override int GetHashCode() => Version.GetHashCode();
    }
}
