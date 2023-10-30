using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Find all buttons/bell1.wav sounds that have a pitch set to 80.
    /// Change those to use an alternative sound and set their pitch to 100.
    /// </summary>
    internal class ChangeBell1SoundAndPitch : MapUpgrade
    {
        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities
                .OfClass("ambient_generic")
                .WhereString("message", "buttons/bell1.wav")
                .Where(e => e.GetInteger("pitch") == 80))
            {
                entity.SetString("message", "buttons/bell1_alt.wav");
                entity.SetInteger("pitch", 100);
            }
        }
    }
}
