using Semver;
using System;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// Builds <see cref="MapUpgradeCollection"/> objects.
    /// Used as part of <see cref="MapUpgradeToolBuilder"/>.
    /// </summary>
    public sealed class MapUpgradeCollectionBuilder
    {
        private readonly ImmutableList<MapUpgrade>.Builder _upgrades = ImmutableList.CreateBuilder<MapUpgrade>();

        internal MapUpgradeCollectionBuilder()
        {
        }

        internal MapUpgradeCollection Build(SemVersion version)
        {
            return new MapUpgradeCollection(version, _upgrades.ToImmutable());
        }

        /// <summary>
        /// Adds a new upgrade.
        /// </summary>
        /// <param name="upgrade">Upgrade to add.</param>
        public MapUpgradeCollectionBuilder AddUpgrade(MapUpgrade upgrade)
        {
            ArgumentNullException.ThrowIfNull(upgrade);
            _upgrades.Add(upgrade);
            return this;
        }

        /// <summary>
        /// Adds a new upgrade.
        /// </summary>
        /// <param name="upgrade">Upgrade delegate to add.</param>
        public MapUpgradeCollectionBuilder AddUpgrade(Action<MapUpgradeContext> upgrade)
        {
            _upgrades.Add(new DelegatingMapUpgrade(upgrade));
            return this;
        }
    }
}
