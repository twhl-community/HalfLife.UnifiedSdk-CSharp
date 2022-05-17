using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades
{
    /// <summary>
    /// Fixes the Pit Worm's Nest bridge possibly breaking if triggered too soon.
    /// </summary>
    internal sealed class Of4a4BridgeUpgrade : MapSpecificUpgradeAction
    {
        public Of4a4BridgeUpgrade()
            : base("of4a4")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            var manager = context.Map.Entities.FirstOrDefault(e => e.GetTargetName() == "kill_pitworm_mm");

            if (manager is null)
            {
                return;
            }

            manager.SetInteger("bridge_global_trigger", 30);
            manager.SetInteger("bridge_mm", 30);
        }
    }
}
