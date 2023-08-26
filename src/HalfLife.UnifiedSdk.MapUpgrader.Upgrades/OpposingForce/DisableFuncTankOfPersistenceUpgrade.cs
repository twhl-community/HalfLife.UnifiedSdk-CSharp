using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Disables the <c>persistence</c> behavior for all Opposing Force tank entities to match the original's behavior.
    /// </summary>
    internal sealed class DisableFuncTankOfPersistenceUpgrade : GameSpecificMapUpgrade
    {
        private static readonly ImmutableHashSet<string> TankClassNames = ImmutableHashSet.Create(
            "func_tank_of",
            "func_tanklaser_of",
            "func_tankrocket_of",
            "func_tankmortar_of");

        public DisableFuncTankOfPersistenceUpgrade()
            : base(ValveGames.OpposingForce)
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities.Where(e => TankClassNames.Contains(e.ClassName)))
            {
                entity.SetDouble("persistence", 0);
            }
        }
    }
}
