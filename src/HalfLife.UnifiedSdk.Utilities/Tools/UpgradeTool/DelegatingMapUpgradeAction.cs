using System;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// An upgrade action that invokes the given delegate to apply upgrades.
    /// </summary>
    public sealed class DelegatingMapUpgradeAction : IMapUpgradeAction
    {
        private readonly Action<MapUpgradeContext> _action;

        /// <summary>
        /// Creates a new delegating action.
        /// </summary>
        /// <param name="action">Delegate to invoke on apply.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="action"/> is null.</exception>
        public DelegatingMapUpgradeAction(Action<MapUpgradeContext> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        /// <inheritdoc/>
        public void Apply(MapUpgradeContext context)
        {
            _action(context);
        }
    }
}
