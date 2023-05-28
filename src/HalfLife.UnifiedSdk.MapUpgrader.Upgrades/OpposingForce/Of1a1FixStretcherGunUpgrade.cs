using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Updates the stretcher grunt's body value to make the grunt's weapon invisible.
    /// </summary>
    internal sealed class Of1a1FixStretcherGunUpgrade : MapSpecificUpgrade
    {
        public Of1a1FixStretcherGunUpgrade()
            : base("of1a1")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            context.Map.Entities.Find("stretcher_grunt")?.SetInteger("body", 17);
        }
    }
}
