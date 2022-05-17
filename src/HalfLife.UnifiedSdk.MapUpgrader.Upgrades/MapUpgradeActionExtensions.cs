using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades
{
    public static class MapUpgradeActionExtensions
    {
        public static void AddHalfLifeUpgrades(this MapUpgrade action)
        {
            //Nothing.
        }

        public static void AddOpposingForceUpgrades(this MapUpgrade action)
        {
            action.Upgrading += new Of4a4BridgeUpgrade().Upgrade;
        }

        public static void AddBlueShiftUpgrades(this MapUpgrade action)
        {
            action.Upgrading += new BaYard4aSlavesUpgrade().Upgrade;
        }
    }
}
