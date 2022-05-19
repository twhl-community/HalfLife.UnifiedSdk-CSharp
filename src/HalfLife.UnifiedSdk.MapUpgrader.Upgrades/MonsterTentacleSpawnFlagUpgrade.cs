using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades
{
    /// <summary>
    /// Converts the Opposing Force <c>monster_tentacle</c> "Use Lower Model" spawnflag to instead set a custom model on the entity.
    /// </summary>
    internal sealed class MonsterTentacleSpawnFlagUpgrade : IMapUpgradeAction
    {
        private const int UseLowerModel = 1 << 6;

        public void Apply(MapUpgradeContext context)
        {
            foreach (var tentacle in context.Map.Entities
                .OfClass("monster_tentacle")
                .Where(e => (e.GetSpawnFlags() & UseLowerModel) != 0))
            {
                tentacle.SetModel("models/tentacle2_lower.mdl");
            }
        }
    }
}
