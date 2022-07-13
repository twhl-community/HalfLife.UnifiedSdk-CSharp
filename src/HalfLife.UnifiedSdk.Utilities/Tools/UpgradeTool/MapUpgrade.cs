using Semver;
using System;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>Represents an upgrade with events to apply changes.</summary>
    public sealed class MapUpgrade : IComparable<MapUpgrade>
    {
        private readonly ImmutableList<IMapUpgradeAction> _actions;

        /// <summary>Version this applies to.</summary>
        public SemVersion Version { get; }

        internal MapUpgrade(SemVersion version, ImmutableList<IMapUpgradeAction> actions)
        {
            Version = version;
            _actions = actions;
        }

        internal void PerformUpgrade(MapUpgradeContext context)
        {
            foreach (var action in _actions)
            {
                action.Apply(context);
            }
        }

        /// <inheritdoc/>
        public int CompareTo(MapUpgrade? other) => Version.ComparePrecedenceTo(other?.Version);

        /// <inheritdoc/>
        public override int GetHashCode() => Version.GetHashCode();
    }
}
