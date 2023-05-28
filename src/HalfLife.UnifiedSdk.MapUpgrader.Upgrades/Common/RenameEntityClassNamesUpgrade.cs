using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Renames weapon and item classnames to their primary name.
    /// </summary>
    internal sealed class RenameEntityClassNamesUpgrade : MapUpgrade
    {
        private static readonly ImmutableDictionary<string, string> ClassNames = new Dictionary<string, string>
        {
            // Old weapon classnames.
            ["weapon_glock"] = "weapon_9mmhandgun",
            ["ammo_glockclip"] = "ammo_9mmclip",
            ["weapon_mp5"] = "weapon_9mmar",
            ["ammo_mp5clip"] = "ammo_9mmar",
            ["ammo_mp5grenades"] = "ammo_argrenades",
            ["weapon_python"] = "weapon_357",
            ["weapon_shockroach"] = "weapon_shockrifle",

            // Uppercase conversions.
            ["weapon_9mmAR"] = "weapon_9mmar",
            ["ammo_9mmAR"] = "ammo_9mmar",
            ["ammo_ARgrenades"] = "ammo_argrenades",
            ["monster_ShockTrooper_dead"] = "monster_shocktrooper_dead"
        }.ToImmutableDictionary();

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities)
            {
                {
                    if (ClassNames.TryGetValue(entity.ClassName, out var className))
                    {
                        entity.ClassName = className;
                    }
                }

                // Update references in this entity.
                if (entity.ClassName == "game_player_equip")
                {
                    foreach (var kv in entity.WithoutClassName().ToList())
                    {
                        if (ClassNames.TryGetValue(kv.Key, out var className))
                        {
                            entity.Remove(kv.Key);
                            entity.SetString(className, kv.Value);
                        }
                    }
                }
            }
        }
    }
}
