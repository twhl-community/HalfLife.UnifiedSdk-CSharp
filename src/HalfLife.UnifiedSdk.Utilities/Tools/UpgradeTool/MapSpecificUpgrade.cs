using System.Collections.Generic;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// Helper class to apply an upgrade to a specific map.
    /// </summary>
    public abstract class MapSpecificUpgrade : MapUpgrade
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

        /// <inheritdoc/>
        protected override bool Filter(MapUpgradeContext context)
        {
            return MapNames.Contains(context.Map.BaseName);
        }
    }
}
