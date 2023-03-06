using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Renames weapon and item classnames to their primary name.
    /// </summary>
    internal sealed class RenameEntityClassNamesUpgrade : IMapUpgradeAction
    {
        private static readonly ImmutableDictionary<string, string> ClassNames = new Dictionary<string, string>
        {
            ["weapon_glock"] = "weapon_9mmhandgun",
            ["ammo_glockclip"] = "ammo_9mmclip",
            ["weapon_mp5"] = "weapon_9mmAR",
            ["ammo_mp5clip"] = "ammo_9mmAR",
            ["ammo_mp5grenades"] = "ammo_ARgrenades",
            ["weapon_python"] = "weapon_357",
            ["weapon_shockroach"] = "weapon_shockrifle"
        }.ToImmutableDictionary();

        public void Apply(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities)
            {
                if (ClassNames.TryGetValue(entity.ClassName, out var className))
                {
                    entity.ClassName = className;
                }
            }
        }
    }
}
