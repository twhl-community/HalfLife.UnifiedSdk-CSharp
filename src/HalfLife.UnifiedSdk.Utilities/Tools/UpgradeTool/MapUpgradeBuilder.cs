using Semver;
using System;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// Builds <see cref="MapUpgrade"/> objects.
    /// Used as part of <see cref="MapUpgradeToolBuilder"/>.
    /// </summary>
    public sealed class MapUpgradeBuilder
    {
        private readonly ImmutableList<IMapUpgradeAction>.Builder _actions = ImmutableList.CreateBuilder<IMapUpgradeAction>();

        internal MapUpgradeBuilder()
        {
        }

        internal MapUpgrade Build(SemVersion version)
        {
            return new MapUpgrade(version, _actions.ToImmutable());
        }

        /// <summary>
        /// Adds a new upgrade action.
        /// </summary>
        /// <param name="action">Action to add.</param>
        public MapUpgradeBuilder AddAction(IMapUpgradeAction action)
        {
            ArgumentNullException.ThrowIfNull(action);
            _actions.Add(action);
            return this;
        }

        /// <summary>
        /// Adds a new upgrade action.
        /// </summary>
        /// <param name="action">Action delegate to add.</param>
        public MapUpgradeBuilder AddAction(Action<MapUpgradeContext> action)
        {
            _actions.Add(new DelegatingMapUpgradeAction(action));
            return this;
        }
    }
}
