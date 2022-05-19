using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades
{
    /// <summary>
    /// Converts <c>item_suit</c>'s model to use <c>w_pcv.mdl</c> in Opposing Force maps.
    /// </summary>
    internal sealed class ConvertSuitToPCVUpgrade : IMapUpgradeAction
    {
        public void Apply(MapUpgradeContext context)
        {
            if (!ValveGames.OpposingForce.IsMap(context.Map.BaseName))
            {
                return;
            }

            foreach (var entity in context.Map.Entities.OfClass("item_suit"))
            {
                entity.SetModel("models/w_pcv.mdl");
            }
        }
    }
}
