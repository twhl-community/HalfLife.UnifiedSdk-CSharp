using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    /// <summary>
    /// Adjusts the body values of the Human Grunts in <c>ba_outro</c> to match the Half-Life script
    /// and to avoid clipping weapons into Freeman.
    /// </summary>
    internal sealed class BaOutroFixGruntsBodyUpgrade : MapSpecificUpgrade
    {
        public BaOutroFixGruntsBodyUpgrade()
            : base("ba_outro")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            var grunt1 = context.Map.Entities.Find("drag_grunt1");
            var grunt2 = context.Map.Entities.Find("drag_grunt2");

            grunt1?.SetInteger("body", 4);
            grunt2?.SetInteger("body", 1);
        }
    }
}
