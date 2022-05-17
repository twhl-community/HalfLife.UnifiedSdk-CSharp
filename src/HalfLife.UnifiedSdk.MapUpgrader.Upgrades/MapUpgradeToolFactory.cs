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
            return MapUpgradeToolBuilder.Build(builder =>
            {
                builder.AddUpgrade(new SemVersion(1, 0, 0), upgrade =>
                {
                    upgrade
                        .AddHalfLifeUpgrades()
                        .AddOpposingForceUpgrades()
                        .AddBlueShiftUpgrades();
                });
            });
        }
    }
}
