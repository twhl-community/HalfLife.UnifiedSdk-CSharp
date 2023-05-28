namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// Represents a single upgrade applied as part of an upgrade collection.
    /// </summary>
    public abstract class MapUpgrade
    {
        /// <summary>
        /// Applies this upgrade to the given map.
        /// </summary>
        public void Apply(MapUpgradeContext context)
        {
            if (!Filter(context))
            {
                return;
            }

            ApplyCore(context);
        }

        /// <summary>
        /// Checks if this upgrade should be applied to the given map.
        /// </summary>
        /// <returns><see langword="true"/> if the upgrade should be applied, <see langword="false"/> otherwise.</returns>
        protected virtual bool Filter(MapUpgradeContext context)
        {
            return true;
        }

        /// <summary>
        /// Performs the actual upgrade.
        /// </summary>
        protected abstract void ApplyCore(MapUpgradeContext context);
    }
}
