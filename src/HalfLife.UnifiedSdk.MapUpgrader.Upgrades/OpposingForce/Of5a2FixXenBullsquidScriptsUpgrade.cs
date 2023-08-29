using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Fixes the Bullsquids in <c>of5a2</c> having the wrong targetnames causing the eating scripts to fail.
    /// </summary>
    internal sealed class Of5a2FixXenBullsquidScriptsUpgrade : MapSpecificUpgrade
    {
        public Of5a2FixXenBullsquidScriptsUpgrade()
            : base("of5a2")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            var feeder1 = context.Map.Entities.Find("t27");
            var feeder2 = context.Map.Entities.Find("t6");

            feeder1?.SetTargetName("feeder_1");
            feeder2?.SetTargetName("feeder_2");
        }
    }
}
