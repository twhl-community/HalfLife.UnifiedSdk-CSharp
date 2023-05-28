﻿using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Removes the <c>dmdelay</c> keyvalue from charger entities. The original game ignores these.
    /// </summary>
    internal sealed class RemoveDMDelayFromChargersUpgrade : IMapUpgrade
    {
        private static readonly ImmutableArray<string> ClassNames = ImmutableArray.Create(
            "func_healthcharger",
            "func_recharge");

        public void Apply(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities.Where(e => ClassNames.Contains(e.ClassName)))
            {
                entity.Remove("dmdelay");
            }
        }
    }
}
