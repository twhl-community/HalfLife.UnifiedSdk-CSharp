using HalfLife.UnifiedSdk.Utilities.Games;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// Helper class to apply an upgrade to a specific game.
    /// </summary>
    public abstract class GameSpecificMapUpgrade : IMapUpgrade
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

        /// <summary>
        /// Upgrades the given map if the game matches <see cref="GameInfo"/>.
        /// </summary>
        public void Apply(MapUpgradeContext context)
        {
            if (context.GameInfo == GameInfo)
            {
                ApplyCore(context);
            }
        }

        /// <summary>
        /// Performs the actual upgrade.
        /// </summary>
        protected abstract void ApplyCore(MapUpgradeContext context);
    }
}
