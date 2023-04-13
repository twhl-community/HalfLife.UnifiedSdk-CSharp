using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Removes the CTF and Co-op game mode settings from Opposing Force maps.
    /// </summary>
    internal sealed class RemoveGameModeSettingsUpgrade : GameSpecificMapUpgradeAction
    {
        private const int CTFFlag = 1 << 3;
        private const int CoopFlag = 1 << 4;

        public RemoveGameModeSettingsUpgrade()
            : base(ValveGames.OpposingForce)
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            var worldspawn = context.Map.Entities.Worldspawn;

            int spawnFlags = worldspawn.GetSpawnFlags();

            spawnFlags &= ~(CTFFlag | CoopFlag);

            worldspawn.SetSpawnFlags(spawnFlags);

            worldspawn.Remove("defaultctf");
        }
    }
}
