using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using Semver;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades
{
    public static class MapUpgradeToolFactory
    {
        /// <summary>
        /// Creates an upgrade tool that applies the actions needed to upgrade a map to the latest version of the Unified SDK.
        /// </summary>
        public static MapUpgradeTool Create()
        {
            var unifiedSdk100UpgradeAction = new MapUpgradeAction(new SemVersion(1, 0, 0));

            unifiedSdk100UpgradeAction.AddHalfLifeUpgrades();
            unifiedSdk100UpgradeAction.AddOpposingForceUpgrades();
            unifiedSdk100UpgradeAction.AddBlueShiftUpgrades();

            return new MapUpgradeTool(unifiedSdk100UpgradeAction);
        }
    }
}
