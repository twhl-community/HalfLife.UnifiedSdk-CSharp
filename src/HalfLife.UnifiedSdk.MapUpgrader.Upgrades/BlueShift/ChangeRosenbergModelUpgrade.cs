using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    internal sealed class ChangeRosenbergModelUpgrade : IMapUpgradeAction
    {
        private static readonly ImmutableHashSet<string> RosenbergNames = ImmutableHashSet.Create(
            "dr_rosenberg",
            "dr_rosenberg1",
            "dr_rosenberg2",
            "dr_rosenberg_fake"
            );

        public void Apply(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities
                .Where(e => e.ClassName == "monster_rosenberg"
                    || (e.ClassName == "monster_generic"
                        && e.GetModel() == "models/scientist.mdl"
                        && e.GetInteger("body") == 3
                        && RosenbergNames.Contains(e.GetTargetName()))))
            {
                entity.SetModel("models/rosenberg.mdl");
                entity.Remove("body");
            }
        }
    }
}
