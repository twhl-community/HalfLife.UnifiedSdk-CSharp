using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    /// <summary>
    /// Remaps <c>monster_rosenberg</c>'s <c>No use</c> flag to <c>monster_scientist</c>'s <c>Allow follow</c> keyvalue.
    /// Technically this flag is the <c>Pre-Disaster</c> flag used by <c>monster_scientist</c>
    /// but it is treated as a separate flag to distinguish the behavior.
    /// </summary>
    internal sealed class RemapRosenbergNoUseFlagUpgrade : MapUpgrade
    {
        private const int SF_ROSENBERG_NO_USE = 1 << 8;

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var rosenberg in context.Map.Entities.OfClass("monster_rosenberg"))
            {
                var spawnflags = rosenberg.GetSpawnFlags();

                var isSet = (spawnflags & SF_ROSENBERG_NO_USE) != 0;

                rosenberg.SetSpawnFlags(spawnflags & ~SF_ROSENBERG_NO_USE);

                rosenberg.SetInteger("allow_follow", isSet ? 0 : 1);
            }
        }
    }
}
