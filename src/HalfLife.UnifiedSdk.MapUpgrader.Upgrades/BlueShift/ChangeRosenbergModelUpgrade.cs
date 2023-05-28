using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    internal sealed class ChangeRosenbergModelUpgrade : GameSpecificMapUpgrade
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
                UpdateEntity(entity);
            }

            // Remap Rosenberg monster_scientist to monster_rosenberg.
            foreach (var entity in context.Map.Entities
                .Where(e => e.ClassName == "monster_scientist"
                    && e.GetInteger("body") == 3))
            {
                entity.ClassName = "monster_rosenberg";
                UpdateEntity(entity);
            }
        }
        private static void UpdateEntity(Entity entity)
        {
            // The default model works fine for monster_rosenberg
            if (entity.ClassName == "monster_rosenberg")
            {
                entity.Remove("model");
            }
            else
            {
                entity.SetModel("models/rosenberg.mdl");
            }

            entity.Remove("body");
        }
    }
}
