using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades
{
    public static class MapUpgradeBuilderExtensions
    {
        public static MapUpgradeBuilder AddSharedUpgrades(this MapUpgradeBuilder builder)
        {
            //Must come before any other upgrades.
            builder.AddAction(new ConvertAngleToAnglesUpgrade());
            builder.AddAction(new AdjustShotgunAnglesUpgrade());
            return builder;
        }

        public static MapUpgradeBuilder AddHalfLifeUpgrades(this MapUpgradeBuilder builder)
        {
            //Nothing.
            return builder;
        }

        public static MapUpgradeBuilder AddOpposingForceUpgrades(this MapUpgradeBuilder builder)
        {
            builder.AddAction(new ConvertSuitToPCVUpgrade());
            builder.AddAction(new ConvertScientistItemUpgrade());
            builder.AddAction(new RenameOtisAnimationsUpgrade());
            builder.AddAction(new MonsterTentacleSpawnFlagUpgrade());
            builder.AddAction(new Of1a4bChangeLoaderSkinUpgrade());
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
