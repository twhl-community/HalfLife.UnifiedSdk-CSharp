using Semver;
using System;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>Represents an upgrade action with events to apply changes.</summary>
    public sealed class MapUpgradeAction : IComparable<MapUpgradeAction>
    {
        /// <summary>Version this applies to.</summary>
        public SemVersion Version { get; }

        /// <summary>Invoked when a map is being upgraded to the version associated with this action.</summary>
        public event Action<MapUpgradeContext>? Upgrading;

        /// <summary>Invoked when a map has completed the upgrade to the version associated with this action.</summary>
        public event Action<MapUpgradeContext>? Upgraded;

        /// <summary>Creates a new upgrade action for the given version.</summary>
        public MapUpgradeAction(SemVersion version)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        internal void PerformUpgrade(MapUpgradeContext context)
        {
            Upgrading?.Invoke(context);
            Upgraded?.Invoke(context);
        }

        /// <inheritdoc/>
        public int CompareTo(MapUpgradeAction? other) => Version.CompareTo(other?.Version);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Version.GetHashCode();
        }
    }
}
