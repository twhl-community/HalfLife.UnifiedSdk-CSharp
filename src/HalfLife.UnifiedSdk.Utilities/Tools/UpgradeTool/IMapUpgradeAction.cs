namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// Represents a single upgrade action applied as part of an upgrade.
    /// </summary>
    public interface IMapUpgradeAction
    {
        /// <summary>
        /// Applies the upgrade associated with this action to the given map.
        /// </summary>
        void Apply(MapUpgradeContext context);
    }
}
