using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Changes the loader entity's skin in <c>of1a4b</c> to use the correct crate texture.
    /// </summary>
    internal sealed class Of1a4bChangeLoaderSkinUpgrade : MapSpecificUpgrade
    {
        public Of1a4bChangeLoaderSkinUpgrade()
            : base("of1a4b")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var loader in context.Map.Entities.OfClass("monster_op4loader"))
            {
                loader.SetInteger("skin", 1);
            }
        }
    }
}
