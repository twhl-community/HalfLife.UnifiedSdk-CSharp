using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.HalfLife
{
    /// <summary>
    /// Increases the reload delay after killing the scientist in <c>c3a2</c>
    /// to allow the entire game over message to display.
    /// </summary>
    internal sealed class C3a2FixLoadSavedUpgrade : MapSpecificUpgrade
    {
        public C3a2FixLoadSavedUpgrade()
            : base("c3a2")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            context.Map.Entities.Find("c3a2_restart")?.SetInteger("loadtime", 10);
        }
    }
}
