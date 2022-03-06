using System;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>Represents errors that occur during the upgrading of a map.</summary>
    public sealed class MapUpgradeException : Exception
    {
        /// <inheritdoc/>
        public MapUpgradeException()
        {
        }

        /// <inheritdoc/>
        public MapUpgradeException(string? message)
            : base(message)
        {
        }

        /// <inheritdoc/>
        public MapUpgradeException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
