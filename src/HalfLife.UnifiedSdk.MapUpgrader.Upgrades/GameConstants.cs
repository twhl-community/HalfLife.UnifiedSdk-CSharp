using System.Numerics;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades
{
    /// <summary>
    /// Constants used by the game.
    /// TODO: should probably be in a separate assembly.
    /// </summary>
    internal static class GameConstants
    {
        /// <summary>
        /// Player hull minimum bounds.
        /// </summary>
        public static readonly Vector3 HullMin = new(-16, -16, -36);

        /// <summary>
        /// Player hull maximum bounds.
        /// </summary>
        public static readonly Vector3 HullMax = new(16, 16, 36);
    }
}
