using HalfLife.UnifiedSdk.Utilities.Games;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// Helper class to apply an upgrade to a specific game.
    /// </summary>
    public abstract class GameSpecificMapUpgrade : MapUpgrade
    {
        /// <summary>
        /// The game that this upgrade applies to.
        /// </summary>
        public GameInfo GameInfo { get; }

        /// <summary>
        /// Creates an upgrade that applies only to the specified game.
        /// </summary>
        /// <param name="gameInfo"></param>
        protected GameSpecificMapUpgrade(GameInfo gameInfo)
        {
            GameInfo = gameInfo;
        }

        /// <inheritdoc/>
        protected override bool Filter(MapUpgradeContext context)
        {
            return context.GameInfo == GameInfo;
        }
    }
}
