using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Adjust <c>monster_male_assassin</c> NPCs to use the correct head and skin value.
    /// </summary>
    internal sealed class AdjustBlackOpsSkinUpgrade : MapUpgrade
    {
        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities.OfClass("monster_male_assassin"))
            {
                var head = entity.GetInteger("head");

                int skin = 0;

                switch (head)
                {
                    case 1:
                        head = 0;
                        skin = 1;
                        break;

                    case 2:
                        head = 1;
                        skin = 1;
                        break;
                }

                entity.SetInteger("head", head);
                entity.SetInteger("skin", skin);
            }
        }
    }
}
