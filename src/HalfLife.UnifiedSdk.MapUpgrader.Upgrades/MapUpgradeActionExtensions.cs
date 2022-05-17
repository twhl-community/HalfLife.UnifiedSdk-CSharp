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
            action.Add(new Of4a4BridgeUpgrade());
        }

        public static void AddBlueShiftUpgrades(this MapUpgrade action)
        {
            action.Add(new BaYard4aSlavesUpgrade());
        }
    }
}
