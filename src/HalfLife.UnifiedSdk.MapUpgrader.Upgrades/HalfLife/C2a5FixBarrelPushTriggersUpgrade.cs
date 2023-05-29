using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.HalfLife
{
    /// <summary>
    /// Fixes the barrels in <c>c2a5</c> not flying as high as they're supposed to on modern systems due to high framerates.
    /// </summary>
    internal sealed class C2a5FixBarrelPushTriggersUpgrade : MapSpecificUpgrade
    {
        [Flags]
        private enum TriggerPushSpawnFlag
        {
            None = 0,
            OnceOnly = 1 << 0,
        }

        private record BarrelSetup(string MultiManagerName, string PushName, string BarrelName);

        private static readonly ImmutableArray<BarrelSetup> BarrelSetups = ImmutableArray.Create(
            new BarrelSetup("can_expl2_mm", "can_expl2_push", "can_expl2_shoot"),
            new BarrelSetup("can_expl4_mm", "can_expl4_push", "can_expl4_shoot"),
            new BarrelSetup("can_expl5_mm", "can_expl5_push", "can_expl5_shoot"));

        public C2a5FixBarrelPushTriggersUpgrade()
            : base("c2a5")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var setup in BarrelSetups)
            {
                var multiManager = context.Map.Entities.Find(setup.MultiManagerName);
                var push = context.Map.Entities.Find(setup.PushName);
                var pushable = context.Map.Entities.Find(setup.BarrelName);

                if (multiManager is null || push is null || pushable is null)
                {
                    continue;
                }

                // Remove the second push trigger to avoid re-enabling it.
                multiManager.Remove($"{setup.PushName}#1");

                var spawnFlags = push.GetSpawnFlags();
                spawnFlags |= (int)TriggerPushSpawnFlag.OnceOnly;
                push.SetSpawnFlags(spawnFlags);

                // Nudge this pushable up by a unit.
                // This prevents the pushable getting stuck in the ground.
                // If the player uses a radius damage attack directed at the middle of the road
                // (dead ahead exiting the tunnel)
                // the specific entity setup used here won't allow the pushable to fly up otherwise
                // because the engine thinks the pushable is somewhere else/outside the world.
                // Specifically, when breakables are destroyed they clear the groundentity variable for entities
                // (see CBreakable::Die)
                // This particular entity setup happens to cause the variable to be cleared at just the right time
                // for the pushable to touch the push trigger.
                // This doesn't happen if a RadiusDamage attack is used due to slight timing changes.
                // Only can_expl2_shoot has shown this behavior but just to be safe all of them are modified.
                var origin = pushable.GetOrigin();
                ++origin.Z;
                pushable.SetOrigin(origin);
            }
        }
    }
}
