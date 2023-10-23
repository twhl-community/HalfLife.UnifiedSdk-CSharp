using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Converts <c>monster_barney_dead</c> entities with custom body value to use the new <c>bodystate</c> keyvalue.
    /// </summary>
    internal sealed class ConvertBarneyModelUpgrade : MapUpgrade
    {
        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var barney in context.Map.Entities.OfClass("monster_barney_dead"))
            {
                if (!barney.ContainsKey("body"))
                {
                    continue;
                }

                var bodystate = barney.GetInteger("body") switch
                {
                    0 => 1, // holstered
                    2 => 0, // blank
                    _ => 2 // drawn
                };

                barney.Remove("body");
                barney.SetInteger("bodystate", bodystate);
            }
        }
    }
}
