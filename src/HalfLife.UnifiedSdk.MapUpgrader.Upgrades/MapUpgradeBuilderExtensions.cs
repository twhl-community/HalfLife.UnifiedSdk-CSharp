using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades
{
    public static class MapUpgradeBuilderExtensions
    {
        public static MapUpgradeBuilder AddHalfLifeUpgrades(this MapUpgradeBuilder builder)
        {
            //Nothing.
            return builder;
        }

        public static MapUpgradeBuilder AddOpposingForceUpgrades(this MapUpgradeBuilder builder)
        {
            builder.AddAction(new Of4a4BridgeUpgrade());
            return builder;
        }

        public static MapUpgradeBuilder AddBlueShiftUpgrades(this MapUpgradeBuilder builder)
        {
            builder.AddAction(new BaYard4aSlavesUpgrade());
            return builder;
        }
    }
}
