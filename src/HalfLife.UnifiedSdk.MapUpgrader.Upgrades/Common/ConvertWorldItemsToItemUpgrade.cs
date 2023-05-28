using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Converts <c>world_items</c> entities to their equivalent entity.
    /// </summary>
    internal sealed class ConvertWorldItemsToItemUpgrade : MapUpgrade
    {
        private static readonly ImmutableDictionary<int, string> IdToClassNameMap = new Dictionary<int, string>
        {
            [42] = "item_antidote",
            [43] = "item_security",
            [44] = "item_battery",
            [45] = "item_suit"
        }.ToImmutableDictionary();

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities.OfClass("world_items").ToList())
            {
                var type = entity.GetInteger("type");

                if (IdToClassNameMap.TryGetValue(type, out var className))
                {
                    entity.ClassName = className;
                    entity.Remove("type");
                }
                else
                {
                    // TODO: log warning about unknown type.
                    context.Map.Entities.Remove(entity);
                }
            }
        }
    }
}
