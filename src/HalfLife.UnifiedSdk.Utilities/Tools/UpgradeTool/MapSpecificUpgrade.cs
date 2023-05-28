using System.Collections.Generic;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// Helper class to apply an upgrade to a specific map.
    /// </summary>
    public abstract class MapSpecificUpgrade : IMapUpgrade
    {
        /// <summary>
        /// The maps that this upgrade applies to.
        /// </summary>
        public ImmutableList<string> MapNames { get; }

        /// <summary>
        /// Creates an upgrade that applies only to the specified maps.
        /// </summary>
        /// <param name="mapNames"></param>
        protected MapSpecificUpgrade(params string[] mapNames)
        {
            MapNames = mapNames.ToImmutableList();
        }

        /// <summary>
        /// Creates an upgrade that applies only to the specified maps.
        /// </summary>
        /// <param name="mapNames"></param>
        protected MapSpecificUpgrade(IEnumerable<string> mapNames)
        {
            MapNames = mapNames.ToImmutableList();
        }

        /// <summary>
        /// Upgrades the given map if its name matches <see cref="MapNames"/>.
        /// </summary>
        public void Apply(MapUpgradeContext context)
        {
            if (MapNames.Contains(context.Map.BaseName))
            {
                ApplyCore(context);
            }
        }

        /// <summary>
        /// Performs the actual upgrade.
        /// </summary>
        protected abstract void ApplyCore(MapUpgradeContext context);
    }
}
