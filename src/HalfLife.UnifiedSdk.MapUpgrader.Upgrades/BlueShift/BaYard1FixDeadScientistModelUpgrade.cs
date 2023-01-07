using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    /// <summary>
    /// Changes incorrect dead scientist head (Rosenberg) in ba_yard1 to use the same as in ba_yard4 (Glasses).
    /// </summary>
    internal sealed class BaYard1FixDeadScientistModelUpgrade : MapSpecificUpgradeAction
    {
        public BaYard1FixDeadScientistModelUpgrade()
            : base("ba_yard1")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities
                .OfClass("monster_scientist_dead")
                .Where(e => e.GetInteger("body") == 3))
            {
                entity.SetInteger("body", 0);
            }
        }
    }
}
