using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    /// <summary>
    /// Fixes the Luther scientist in <c>ba_tram2</c>'s reflective lab room having white hands.
    /// </summary>
    internal sealed class BaTram2FixScientistSkinColorUpgrade : MapSpecificUpgrade
    {
        public BaTram2FixScientistSkinColorUpgrade()
            : base("ba_tram2")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            context.Map.Entities.Find("joey_normal")?.SetInteger("skin", 1);
            context.Map.Entities.Find("joey_reflect")?.SetInteger("skin", 1);
        }
    }
}
