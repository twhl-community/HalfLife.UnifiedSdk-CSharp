using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Converts <c>func_breakable</c>'s spawn object keyvalue from an index to a classname.
    /// </summary>
    internal sealed class ConvertBreakableItemUpgrade : MapUpgrade
    {
        private const string ItemKey = "spawnobject";

        private static readonly ImmutableList<string> ClassNames = ImmutableList.Create(
            "item_battery",
            "item_healthkit",
            "weapon_9mmhandgun",
            "ammo_9mmclip",
            "weapon_9mmar",
            "ammo_9mmar",
            "ammo_argrenades",
            "weapon_shotgun",
            "ammo_buckshot",
            "weapon_crossbow",
            "ammo_crossbow",
            "weapon_357",
            "ammo_357",
            "weapon_rpg",
            "ammo_rpgclip",
            "ammo_gaussclip",
            "weapon_handgrenade",
            "weapon_tripmine",
            "weapon_satchel",
            "weapon_snark",
            "weapon_hornetgun",
            "weapon_penguin"
            );

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities
                .Where(e => e.ClassName == "func_breakable" || e.ClassName == "func_pushable"))
            {
                int index = entity.GetInteger(ItemKey, 0);

                if (index == 0)
                {
                    entity.Remove(ItemKey);
                }
                else if (index > 0 && index <= ClassNames.Count)
                {
                    entity.SetString(ItemKey, ClassNames[index - 1]);
                }
                else
                {
                    // TODO: log error
                }
            }
        }
    }
}
