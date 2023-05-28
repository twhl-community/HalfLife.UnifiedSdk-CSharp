using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    /// <summary>
    /// Sets the <c>Remove On Fire</c> spawnflag on the <c>trigger_auto</c> entity
    /// used to start the script on <c>ba_outro</c>.
    /// This fixes the script restarting on save load.
    /// </summary>
    internal sealed class BaOutroDisableTriggerAutoUpgrade : MapSpecificUpgrade
    {
        private const int RemoveOnFire = 1 << 0;

        public BaOutroDisableTriggerAutoUpgrade()
            : base("ba_outro")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            if (context.Map.Entities
                .OfClass("trigger_auto")
                .FirstOrDefault(e => e.HasKeyValue("target", "start_outro")) is { } triggerAuto)
            {
                triggerAuto.SetSpawnFlags(triggerAuto.GetSpawnFlags() | RemoveOnFire);
            }
        }
    }
}
