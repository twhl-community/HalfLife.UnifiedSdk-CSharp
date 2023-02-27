using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.HalfLife
{
    /// <summary>
    /// Fixes the barrels in <c>c2a5</c> not flying as high as they're supposed to on modern systems due to high framerates.
    /// </summary>
    internal sealed class C2a5FixBarrelPushTriggersUpgrade : MapSpecificUpgradeAction
    {
        [Flags]
        private enum TriggerPushSpawnFlag
        {
            None = 0,
            OnceOnly = 1 << 0,
        }

        private record BarrelSetup(string MultiManagerName, string PushName);

        private static readonly ImmutableArray<BarrelSetup> BarrelSetups = ImmutableArray.Create(
            new BarrelSetup("can_expl2_mm", "can_expl2_push"),
            new BarrelSetup("can_expl4_mm", "can_expl4_push"),
            new BarrelSetup("can_expl5_mm", "can_expl5_push"));

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

                if (multiManager is null || push is null)
                {
                    continue;
                }

                // Remove the second push trigger to avoid re-enabling it.
                multiManager.Remove($"{setup.PushName}#1");

                var spawnFlags = push.GetSpawnFlags();
                spawnFlags |= (int)TriggerPushSpawnFlag.OnceOnly;
                push.SetSpawnFlags(spawnFlags);
            }
        }
    }
}
