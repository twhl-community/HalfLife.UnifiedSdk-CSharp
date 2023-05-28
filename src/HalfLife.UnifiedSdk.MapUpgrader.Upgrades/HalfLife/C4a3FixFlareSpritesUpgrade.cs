using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.HalfLife
{
    /// <summary>
    /// Fixes the flare sprites shown during Nihilanth's death script using the wrong render mode.
    /// </summary>
    internal sealed class C4a3FixFlareSpritesUpgrade : MapSpecificUpgrade
    {
        public C4a3FixFlareSpritesUpgrade()
            : base("c4a3")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities
                .OfClass("env_sprite")
                .WhereString("model", "sprites/XFlare3.spr"))
            {
                entity.SetRenderMode(RenderMode.RenderTransAdd);
                entity.SetRenderAmount(255);
            }
        }
    }
}
