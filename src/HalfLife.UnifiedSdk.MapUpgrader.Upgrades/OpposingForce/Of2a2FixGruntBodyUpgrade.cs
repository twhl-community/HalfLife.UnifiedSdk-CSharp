using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Fixes <c>monster_generic</c> entities that use <c>hgrunt_opfor.mdl</c> to use the correct body value.
    /// </summary>
    internal sealed class Of2a2FixGruntBodyUpgrade : MapSpecificUpgrade
    {
        public Of2a2FixGruntBodyUpgrade()
            : base("of2a2")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            static void ChangeBody(Entity entity)
            {
                entity.SetInteger("body", 1);
            }

            var fgrunt1 = context.Map.Entities.Find("fgrunt1");
            var fgrunt2 = context.Map.Entities.Find("fgrunt2");

            if (fgrunt1 is not null)
            {
                ChangeBody(fgrunt1);
            }

            if (fgrunt2 is not null)
            {
                ChangeBody(fgrunt2);
            }
        }
    }
}
