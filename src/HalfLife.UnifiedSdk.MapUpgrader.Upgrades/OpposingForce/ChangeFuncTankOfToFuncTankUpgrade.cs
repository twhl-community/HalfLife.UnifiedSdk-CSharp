using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Renames the Opposing Force <c>func_tank</c> classes to their original versions.
    /// No other changes are needed, as the original versions have been updated to include the new functionality.
    /// </summary>
    internal sealed class ChangeFuncTankOfToFuncTankUpgrade : MapUpgrade
    {
        private const string OpposingForceSuffix = "_of";

        private static readonly ImmutableDictionary<string, string> ClassNames = new[]
        {
            "func_tank",
            "func_tanklaser",
            "func_tankrocket",
            "func_tankmortar",
            "func_tankcontrols"
        }
        .ToImmutableDictionary(k => k + OpposingForceSuffix, v => v);

        protected override void ApplyCore(MapUpgradeContext context)
        {
            if (context.GameInfo != ValveGames.OpposingForce)
            {
                return;
            }

            foreach (var entity in context.Map.Entities)
            {
                if (ClassNames.TryGetValue(entity.ClassName, out var replacement))
                {
                    entity.ClassName = replacement;
                }
            }
        }
    }
}
