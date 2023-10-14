using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    /// <summary>
    /// Removes the <c>netname</c> keyvalue from the <c>monstermaker</c>s that spawn in Alien Slaves
    /// to prevent them from waiting for 5 seconds.
    /// </summary>
    internal class BaCanal1FixAlienSlaveSpawnsUpgrade : MapSpecificUpgrade
    {
        private static readonly ImmutableArray<string> MonsterMakerNames = ImmutableArray.Create(
            "tele5_spawner",
            "tele4_spawner");

        public BaCanal1FixAlienSlaveSpawnsUpgrade()
            : base("ba_canal1")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var spawner in context.Map.Entities
                .Where(e => MonsterMakerNames.Contains(e.GetTargetName())))
            {
                spawner.Remove("netname");
            }
        }
    }
}
