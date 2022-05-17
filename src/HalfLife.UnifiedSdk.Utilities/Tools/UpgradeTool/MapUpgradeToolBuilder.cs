using Semver;
using System;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// Builds <see cref="MapUpgradeTool"/> objects.
    /// </summary>
    public sealed class MapUpgradeToolBuilder
    {
        private readonly ImmutableList<MapUpgrade>.Builder _upgrades = ImmutableList.CreateBuilder<MapUpgrade>();

        private MapUpgradeToolBuilder()
        {
        }

        /// <summary>
        /// Builds a map upgrade tool by invoking a callback which populates the list of upgrades.
        /// </summary>
        /// <param name="callback">Callback to invoke.</param>
        public static MapUpgradeTool Build(Action<MapUpgradeToolBuilder> callback)
        {
            ArgumentNullException.ThrowIfNull(callback);

            MapUpgradeToolBuilder builder = new();

            callback(builder);

            return builder.BuildCore();
        }

        private MapUpgradeTool BuildCore()
        {
            return new MapUpgradeTool(_upgrades.ToImmutable().Sort());
        }

        /// <summary>
        /// Adds an upgrade to the tool.
        /// </summary>
        /// <param name="version">Version to associate to this upgrade.</param>
        /// <param name="callback">Callback to invoke.</param>
        /// <exception cref="ArgumentException">If the given version is already used by an existing upgrade.</exception>
        public MapUpgradeToolBuilder AddUpgrade(SemVersion version, Action<MapUpgradeBuilder> callback)
        {
            ArgumentNullException.ThrowIfNull(version);
            ArgumentNullException.ThrowIfNull(callback);

            if (_upgrades.Find(u => u.Version == version) is not null)
            {
                throw new ArgumentException("Only one upgrade may be associated with a specific version", nameof(version));
            }

            MapUpgradeBuilder builder = new();

            callback(builder);

            _upgrades.Add(builder.Build(version));

            return this;
        }
    }
}
