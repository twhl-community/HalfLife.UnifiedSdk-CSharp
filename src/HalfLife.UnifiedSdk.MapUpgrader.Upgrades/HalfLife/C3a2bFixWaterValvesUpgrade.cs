using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.HalfLife
{
    /// <summary>
    /// Prevents players from soft-locking the game by turning both valves at the same time in
    /// <c>c3a2b</c> (Lambda Core reactor water flow).
    /// </summary>
    internal sealed class C3a2bFixWaterValvesUpgrade : MapSpecificUpgrade
    {
        public C3a2bFixWaterValvesUpgrade()
            : base("c3a2b")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            var valves = context.Map.Entities.Where(e => e.GetTarget() == "ms1" || e.GetTarget() == "ms2");
            var multiManagers = context.Map.Entities.Where(e => e.GetTargetName() == "mm3" || e.GetTargetName() == "mm4");

            var firstStop = context.Map.Entities.Find("core1");
            var secondStop = context.Map.Entities.Find("core2");

            if (valves.Count() != 2 || multiManagers.Count() != 2 || firstStop is null || secondStop is null)
            {
                return;
            }

            // Lock the valves while the water is moving
            var multisource = context.Map.Entities.CreateNewEntity("multisource");

            multisource.SetTargetName("valve_ms");

            var relay = context.Map.Entities.CreateNewEntity("trigger_relay");

            relay.SetTargetName("valve_ms_relay");
            relay.SetTarget("valve_ms");

            // Set initial multisource state to enabled
            var triggerAuto = context.Map.Entities.CreateNewEntity("trigger_auto");

            // Remove on fire
            triggerAuto.SetSpawnFlags(1);
            triggerAuto.SetTarget("valve_ms_relay");

            foreach (var valve in valves)
            {
                valve.SetString("master", "valve_ms");
            }

            // Disable valves on start
            foreach (var mm in multiManagers)
            {
                mm.SetDouble("valve_ms_relay", 0);
            }

            // Enable when the water stops moving
            firstStop.SetString("message", "valve_ms_relay");
            secondStop.SetString("message", "valve_ms_relay");
        }
    }
}
