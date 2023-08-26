using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Adds a missing <c>.wav</c> extension to the sound played when the Geneworm enters in <c>of6a5</c>.
    /// </summary>
    internal sealed class Of6a5FixGenewormArriveSoundUpgrade : MapSpecificUpgrade
    {
        public Of6a5FixGenewormArriveSoundUpgrade()
            : base("of6a5")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            var sound = context.Map.Entities.Find("genearrivesound");

            sound?.SetString("message", "ambience/port_suckout1.wav");
        }
    }
}
