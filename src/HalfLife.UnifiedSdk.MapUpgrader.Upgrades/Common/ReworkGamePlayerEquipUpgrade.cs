using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Changes all unnamed <c>game_player_equip</c> entities to be fired on player spawn.
    /// </summary>
    internal sealed class ReworkGamePlayerEquipUpgrade : MapUpgrade
    {
        private const int UseOnlyFlag = 1 << 0;

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities
                .OfClass("game_player_equip")
                .Where(e => string.IsNullOrEmpty(e.GetTargetName())))
            {
                int spawnFlags = entity.GetSpawnFlags();

                // If it's marked as use only already then it was previously unused, so skip it.
                if ((spawnFlags & UseOnlyFlag) != 0)
                {
                    continue;
                }

                entity.SetTargetName("game_playerspawn");

                entity.SetSpawnFlags(spawnFlags | UseOnlyFlag);
            }
        }
    }
}
