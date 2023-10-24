using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Sets the <c>assassin4_spawn</c> <c>monstermaker</c> in <c>of6a1</c> to spawn a Black Ops assassin immediately
    /// to make the switch from prisoner less obvious.
    /// </summary>
    internal sealed class FixBlackOpsSpawnDelayUpgrade : MapSpecificUpgrade
    {
        public FixBlackOpsSpawnDelayUpgrade()
            : base("of6a1")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            var monstermaker = context.Map.Entities.FirstOrDefault(e => e.ClassName == "monstermaker" && e.GetTargetName() == "assassin4_spawn");

            monstermaker?.SetDouble("delay", 0);
        }
    }
}
