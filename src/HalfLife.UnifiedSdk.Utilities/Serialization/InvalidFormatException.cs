using System;

namespace HalfLife.UnifiedSdk.Utilities.Serialization
{
    /// <summary>
    /// Represents errors that occur during map loading.
    /// </summary>
    public sealed class InvalidFormatException : Exception
    {
        /// <inheritdoc/>
        public InvalidFormatException()
        {
        }

        /// <inheritdoc/>
        public InvalidFormatException(string? message)
            : base(message)
        {
        }

        /// <inheritdoc/>
        public InvalidFormatException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
