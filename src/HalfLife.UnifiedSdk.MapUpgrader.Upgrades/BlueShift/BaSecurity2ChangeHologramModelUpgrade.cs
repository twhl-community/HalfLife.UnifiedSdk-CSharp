using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    /// <summary>
    /// Changes Gina model in <c>ba_security2</c> to allow playing push cart sequence.
    /// </summary>
    internal sealed class BaSecurity2ChangeHologramModelUpgrade : MapSpecificUpgrade
    {
        public BaSecurity2ChangeHologramModelUpgrade()
            : base("ba_security2")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            if (context.Map.Entities.Find("gina_push") is { } gina)
            {
                gina.SetModel("models/holo_cart.mdl");
                gina.SetVector3("custom_hull_min", GameConstants.HullMin);
                gina.SetVector3("custom_hull_max", GameConstants.HullMax);
            }
        }
    }
}
