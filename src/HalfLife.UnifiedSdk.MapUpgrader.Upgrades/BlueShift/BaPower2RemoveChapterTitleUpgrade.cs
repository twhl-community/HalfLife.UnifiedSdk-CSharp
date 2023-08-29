using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    /// <summary>
    /// Removes the <c>chaptertitle</c> key from <c>worldspawn</c> in <c>ba_power2</c> to remove the redundant chapter title text.
    /// </summary>
    internal sealed class BaPower2RemoveChapterTitleUpgrade : MapSpecificUpgrade
    {
        public BaPower2RemoveChapterTitleUpgrade()
            : base("ba_power2")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            context.Map.Entities.Worldspawn.Remove("chaptertitle");
        }
    }
}
