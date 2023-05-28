using System;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// An upgrade that invokes the given delegate to apply upgrades.
    /// </summary>
    public sealed class DelegatingMapUpgrade : MapUpgrade
    {
        private readonly Action<MapUpgradeContext> _upgrade;

        /// <summary>
        /// Creates a new delegating upgrade.
        /// </summary>
        /// <param name="upgrade">Delegate to invoke on apply.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="upgrade"/> is null.</exception>
        public DelegatingMapUpgrade(Action<MapUpgradeContext> upgrade)
        {
            _upgrade = upgrade ?? throw new ArgumentNullException(nameof(upgrade));
        }

        /// <inheritdoc/>
        protected override void ApplyCore(MapUpgradeContext context)
        {
            _upgrade(context);
        }
    }
}
