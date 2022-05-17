using Semver;
using System;
using System.Collections.Generic;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>Represents an upgrade with events to apply changes.</summary>
    public sealed class MapUpgrade : IComparable<MapUpgrade>
    {
        private readonly List<IMapUpgradeAction> _actions = new();

        /// <summary>Version this applies to.</summary>
        public SemVersion Version { get; }

        /// <summary>Creates a new upgrade for the given version.</summary>
        public MapUpgrade(SemVersion version)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        /// <summary>
        /// Adds a new upgrade action.
        /// </summary>
        /// <param name="action">Action to add.</param>
        public void Add(IMapUpgradeAction action)
        {
            ArgumentNullException.ThrowIfNull(action);

            _actions.Add(action);
        }

        /// <summary>
        /// Adds a new upgrade action.
        /// </summary>
        /// <param name="action">Action delegate to add.</param>
        public void Add(Action<MapUpgradeContext> action)
        {
            _actions.Add(new DelegatingMapUpgradeAction(action));
        }

        internal void PerformUpgrade(MapUpgradeContext context)
        {
            foreach (var action in _actions)
            {
                action.Apply(context);
            }
        }

        /// <inheritdoc/>
        public int CompareTo(MapUpgrade? other) => Version.CompareTo(other?.Version);

        /// <inheritdoc/>
        public override int GetHashCode() => Version.GetHashCode();
    }
}
