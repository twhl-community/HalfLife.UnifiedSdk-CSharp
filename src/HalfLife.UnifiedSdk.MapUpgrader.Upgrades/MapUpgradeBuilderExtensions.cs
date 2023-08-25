using HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift;
using HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common;
using HalfLife.UnifiedSdk.MapUpgrader.Upgrades.HalfLife;
using HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades
{
    public static class MapUpgradeBuilderExtensions
    {
        public static MapUpgradeCollectionBuilder AddSharedUpgrades(this MapUpgradeCollectionBuilder builder)
        {
            //Must come before any other upgrades.
            builder.AddUpgrade(new ConvertAngleToAnglesUpgrade());
            builder.AddUpgrade(new RenameEntityClassNamesUpgrade());
            // Must come before any upgrades that deal with the resulting entities!
            builder.AddUpgrade(new ConvertWorldItemsToItemUpgrade());

            builder.AddUpgrade(new AdjustShotgunAnglesUpgrade());
            builder.AddUpgrade(new ConvertOtisModelUpgrade());
            builder.AddUpgrade(new RenameMessagesUpgrade());
            builder.AddUpgrade(new ReworkMusicPlaybackUpgrade());
            builder.AddUpgrade(new ConvertWorldspawnGameTitleValueUpgrade());
            builder.AddUpgrade(new ConvertSoundIndicesToNamesUpgrade());
            builder.AddUpgrade(new SetCustomHullForGenericMonstersUpgrade());
            builder.AddUpgrade(new FixRenderColorFormatUpgrade());
            builder.AddUpgrade(new RemoveDMDelayFromChargersUpgrade());
            builder.AddUpgrade(new ConvertBreakableItemUpgrade());
            builder.AddUpgrade(new ReworkGamePlayerEquipUpgrade());
            builder.AddUpgrade(new FixNonLoopingSoundsUpgrade());
            builder.AddUpgrade(new PruneExcessMultiManagerKeysUpgrade());
            return builder;
        }

        public static MapUpgradeCollectionBuilder AddHalfLifeUpgrades(this MapUpgradeCollectionBuilder builder)
        {
            builder.AddUpgrade(new C2a5FixBarrelPushTriggersUpgrade());
            builder.AddUpgrade(new C4a3FixFlareSpritesUpgrade());
            builder.AddUpgrade(new C3a2bFixWaterValvesUpgrade());
            builder.AddUpgrade(new C3a2FixLoadSavedUpgrade());
            builder.AddUpgrade(new C4a2FixNihilanthDialogueUpgrade());
            builder.AddUpgrade(new C2a5FixCrateGlobalNameUpgrade());
            return builder;
        }

        public static MapUpgradeCollectionBuilder AddOpposingForceUpgrades(this MapUpgradeCollectionBuilder builder)
        {
            builder.AddUpgrade(new ConvertSuitToPCVUpgrade());
            builder.AddUpgrade(new ConvertScientistItemUpgrade());
            builder.AddUpgrade(new RenameOtisAnimationsUpgrade());
            builder.AddUpgrade(new MonsterTentacleSpawnFlagUpgrade());
            builder.AddUpgrade(new Of0a0DisableItemDroppingUpgrade());
            builder.AddUpgrade(new Of1a4bChangeLoaderSkinUpgrade());
            builder.AddUpgrade(new Of4a4BridgeUpgrade());
            builder.AddUpgrade(new RenameBlackOpsAnimationsUpgrade());
            builder.AddUpgrade(new AdjustBlackOpsSkinUpgrade());
            builder.AddUpgrade(new RenameIntroGruntAnimationsUpgrade());
            builder.AddUpgrade(new ConvertOtisBodyStateUpgrade());
            builder.AddUpgrade(new FixBlackOpsSpawnDelayUpgrade());
            builder.AddUpgrade(new ChangeFuncTankOfToFuncTankUpgrade());
            builder.AddUpgrade(new Of2a2FixGruntBodyUpgrade());
            builder.AddUpgrade(new RemoveGameModeSettingsUpgrade());
            builder.AddUpgrade(new Of1a1FixStretcherGunUpgrade());
            builder.AddUpgrade(new OfBoot1FixOspreyScriptUpgrade());
            return builder;
        }

        public static MapUpgradeCollectionBuilder AddBlueShiftUpgrades(this MapUpgradeCollectionBuilder builder)
        {
            builder.AddUpgrade(new BaYard1FixDeadScientistModelUpgrade());
            builder.AddUpgrade(new BaYard4aSlavesUpgrade());
            builder.AddUpgrade(new RenameConsoleCivAnimationsUpgrade());
            builder.AddUpgrade(new ChangeRosenbergModelUpgrade());
            builder.AddUpgrade(new RemapRosenbergNoUseFlagUpgrade());
            builder.AddUpgrade(new ChangeBlueShiftSentencesUpgrade());
            builder.AddUpgrade(new BaSecurity2ChangeHologramModelUpgrade());
            builder.AddUpgrade(new BaOutroDisableTriggerAutoUpgrade());
            builder.AddUpgrade(new BaTram1FixSuitUpgrade());
            return builder;
        }
    }
}
