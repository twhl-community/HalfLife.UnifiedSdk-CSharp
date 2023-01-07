using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    internal sealed class ChangeRosenbergModelUpgrade : GameSpecificMapUpgradeAction
    {
        public ChangeRosenbergModelUpgrade()
            : base(ValveGames.BlueShift)
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities
                .Where(e => e.ClassName == "monster_rosenberg"
                    || (e.ClassName == "monster_generic"
                        && e.GetModel() == "models/scientist.mdl"
                        && e.GetInteger("body") == 3)))
            {
                entity.SetModel("models/rosenberg.mdl");
                entity.Remove("body");
            }
        }
    }
}
