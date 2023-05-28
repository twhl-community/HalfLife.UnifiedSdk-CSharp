using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Converts the Opposing Force <c>monster_tentacle</c> "Use Lower Model" spawnflag to instead set a custom model on the entity,
    /// and changes other uses to use the alternate model.
    /// </summary>
    internal sealed class MonsterTentacleSpawnFlagUpgrade : GameSpecificMapUpgrade
    {
        private const int UseLowerModel = 1 << 6;

        public MonsterTentacleSpawnFlagUpgrade()
            : base(ValveGames.OpposingForce)
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var tentacle in context.Map.Entities
                .OfClass("monster_tentacle"))
            {
                if ((tentacle.GetSpawnFlags() & UseLowerModel) != 0)
                {
                    tentacle.SetModel("models/tentacle3.mdl");
                }
                else
                {
                    tentacle.SetModel("models/tentacle2_lower.mdl");
                }

                //Adjust height values to match Opposing Force's.
                tentacle.SetDouble("height0", 0);
                tentacle.SetDouble("height1", 136);
                tentacle.SetDouble("height2", 190);
                tentacle.SetDouble("height3", 328);
            }
        }
    }
}
