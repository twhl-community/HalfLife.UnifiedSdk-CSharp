using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Sets a custom hull size for <c>monster_generic</c> entities that use a model that was originally hard-coded to use one.
    /// </summary>
    internal sealed class SetCustomHullForGenericMonstersUpgrade : MapUpgrade
    {
        private static readonly ImmutableArray<string> ModelNames = ImmutableArray.Create(
            "models/player.mdl",
            "models/holo.mdl");

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities
                .OfClass("monster_generic")
                .Where(e => ModelNames.Contains(e.GetModel())))
            {
                entity.SetVector3("custom_hull_min", GameConstants.HullMin);
                entity.SetVector3("custom_hull_max", GameConstants.HullMax);
            }
        }
    }
}
