using HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift;
using HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common;
using HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce;
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
            builder.AddAction(new ConvertOtisModelUpgrade());
            builder.AddAction(new RenameMessagesUpgrade());
            builder.AddAction(new ReworkMusicPlaybackUpgrade());
            builder.AddAction(new ConvertWorldspawnGameTitleValueUpgrade());
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
            builder.AddAction(new Of0a0DisableItemDroppingUpgrade());
            builder.AddAction(new Of1a4bChangeLoaderSkinUpgrade());
            builder.AddAction(new Of4a4BridgeUpgrade());
            builder.AddAction(new RenameBlackOpsAnimationsUpgrade());
            builder.AddAction(new AdjustBlackOpsSkinUpgrade());
            builder.AddAction(new RenameIntroGruntAnimationsUpgrade());
            return builder;
        }

        public static MapUpgradeBuilder AddBlueShiftUpgrades(this MapUpgradeBuilder builder)
        {
            builder.AddAction(new BaYard4aSlavesUpgrade());
            builder.AddAction(new RenameConsoleCivAnimationsUpgrade());
            builder.AddAction(new ChangeRosenbergModelUpgrade());
            builder.AddAction(new RemapRosenbergNoUseFlagUpgrade());
            return builder;
        }
    }
}
