using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Adjusts the <c>MaxRange</c> keyvalue for specific maps to fix graphical issues
    /// when geometry is further away than the original value.
    /// </summary>
    internal sealed class AdjustMaxRangeUpgrade : MapUpgrade
    {
        private static readonly ImmutableDictionary<string, int> MapsToAdjust = new Dictionary<string, int>
        {
            ["c2a2a"] = 8192
        }.ToImmutableDictionary();

        protected override void ApplyCore(MapUpgradeContext context)
        {
            if (MapsToAdjust.TryGetValue(context.Map.BaseName, out var value))
            {
                context.Map.Entities.Worldspawn.SetInteger("MaxRange", value);
            }
        }
    }
}
