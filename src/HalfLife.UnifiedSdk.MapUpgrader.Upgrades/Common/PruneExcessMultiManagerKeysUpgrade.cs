using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Prunes excess keyvalues specified for <c>multi_manager</c> entities.
    /// In practice this only affects a handful of entities used in retinal scanner scripts.
    /// </summary>
    internal sealed class PruneExcessMultiManagerKeysUpgrade : MapUpgrade
    {
        /// <summary>
        /// Original HL1 SDK limit.
        /// </summary>
        private const int MaxKeys = 16;

        /// <summary>
        /// These keys are not counted as targets by <c>multi_manager</c>.
        /// There are more of these but normally they aren't used.
        /// </summary>
        private static readonly ImmutableHashSet<string> KeysToIgnore = ImmutableHashSet.Create(
            KeyValueUtilities.ClassName,
            KeyValueUtilities.TargetName,
            KeyValueUtilities.Origin,
            "wait");

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var multiManager in context.Map.Entities.OfClass("multi_manager"))
            {
                var keys = multiManager
                    .Select((kv, index) => new { KeyValue = kv, Index = index })
                    .Where(kv => !KeysToIgnore.Contains(kv.KeyValue.Key))
                    .ToList();

                if (keys.Count > MaxKeys)
                {
                    // Need to remove last to first to avoid invalidating indices!
                    foreach (var key in keys.Skip(MaxKeys).Reverse())
                    {
                        multiManager.RemoveAt(key.Index);
                    }
                }
            }
        }
    }
}
