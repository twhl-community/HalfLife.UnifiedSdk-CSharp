using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    /// <summary>
    /// Changes Gina model in ba_security2 to allow playing push cart sequence.
    /// </summary>
    internal sealed class BaSecurity2ChangeHologramModelUpgrade : MapSpecificUpgradeAction
    {
        public BaSecurity2ChangeHologramModelUpgrade()
            : base("ba_security2")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            context.Map.Entities.WhereTargetName("gina_push")
                .FirstOrDefault()?.SetModel("models/holo_cart.mdl");
        }
    }
}
