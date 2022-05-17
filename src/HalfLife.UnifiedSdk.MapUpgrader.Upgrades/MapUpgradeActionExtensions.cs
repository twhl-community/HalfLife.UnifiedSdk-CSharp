using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades
{
    public static class MapUpgradeActionExtensions
    {
        public static void AddHalfLifeUpgrades(this MapUpgradeAction action)
        {
            //Nothing.
        }

        public static void AddOpposingForceUpgrades(this MapUpgradeAction action)
        {
            action.Upgrading += new Of4a4BridgeUpgrade().Upgrade;
        }

        public static void AddBlueShiftUpgrades(this MapUpgradeAction action)
        {
            action.Upgrading += new BaYard4aSlavesUpgrade().Upgrade;
        }
    }
}
