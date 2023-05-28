namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// Represents a single upgrade applied as part of an upgrade collection.
    /// </summary>
    public interface IMapUpgrade
    {
        /// <summary>
        /// Applies this upgrade to the given map.
        /// </summary>
        void Apply(MapUpgradeContext context);
    }
}
