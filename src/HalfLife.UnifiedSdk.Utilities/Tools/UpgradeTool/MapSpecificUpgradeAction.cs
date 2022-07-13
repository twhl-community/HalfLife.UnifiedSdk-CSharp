namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// Helper class to apply an upgrade to a specific map.
    /// </summary>
    public abstract class MapSpecificUpgradeAction : IMapUpgradeAction
    {
        /// <summary>
        /// The map that this upgrade applies to.
        /// </summary>
        public string MapName { get; }

        /// <summary>
        /// Creates an upgrade that applies only to the specified map.
        /// </summary>
        /// <param name="mapName"></param>
        protected MapSpecificUpgradeAction(string mapName)
        {
            MapName = mapName;
        }

        /// <summary>
        /// Upgrades the given map if its name matches <see cref="MapName"/>.
        /// </summary>
        public void Apply(MapUpgradeContext context)
        {
            if (context.Map.BaseName == MapName)
            {
                ApplyCore(context);
            }
        }

        /// <summary>
        /// Performs the actual upgrade.
        /// </summary>
        protected abstract void ApplyCore(MapUpgradeContext context);
    }
}
