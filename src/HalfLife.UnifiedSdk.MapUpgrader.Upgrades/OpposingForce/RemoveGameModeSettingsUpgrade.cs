using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Removes the CTF game mode settings from Opposing Force maps.
    /// </summary>
    internal sealed class RemoveGameModeSettingsUpgrade : GameSpecificMapUpgrade
    {
        public RemoveGameModeSettingsUpgrade()
            : base(ValveGames.OpposingForce)
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            var worldspawn = context.Map.Entities.Worldspawn;

            worldspawn.Remove("defaultctf");
        }
    }
}
