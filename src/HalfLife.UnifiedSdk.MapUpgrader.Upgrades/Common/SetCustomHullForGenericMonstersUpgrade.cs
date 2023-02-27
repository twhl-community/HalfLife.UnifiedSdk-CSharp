using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;
using System.Numerics;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Sets a custom hull size for <c>monster_generic</c> entities that use a model that was originally hard-coded to use one.
    /// </summary>
    internal sealed class SetCustomHullForGenericMonstersUpgrade : IMapUpgradeAction
    {
        private static readonly ImmutableArray<string> ModelNames = ImmutableArray.Create(
            "models/player.mdl",
            "models/holo.mdl");

        private static readonly Vector3 HullMin = new Vector3(-16, -16, -36);
        private static readonly Vector3 HullMax = new Vector3(16, 16, 36);

        public void Apply(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities
                .OfClass("monster_generic")
                .Where(e => ModelNames.Contains(e.GetModel())))
            {
                entity.SetVector3("custom_hull_min", HullMin);
                entity.SetVector3("custom_hull_max", HullMax);
            }
        }
    }
}
