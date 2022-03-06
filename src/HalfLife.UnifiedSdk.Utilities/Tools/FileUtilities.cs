using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Tools
{
    /// <summary>Utility functions for dealing with files and paths.</summary>
    public static class FileUtilities
    {
        /// <summary>
        /// Gets the extension of a file without the dot.
        /// </summary>
        /// <param name="path">Path to get the extension of.</param>
        /// <returns>
        /// If <paramref name="path"/> is <see langword="null"/>, returns <see langword="null"/>.
        /// If <paramref name="path"/> contains an extension, returns the extension without the dot.
        /// Otherwise returns an empty string.
        /// </returns>
        [return: NotNullIfNotNull("path")]
        public static string? GetExtensionWithoutDot(string? path)
        {
            var extension = Path.GetExtension(path);

            if (string.IsNullOrEmpty(extension))
            {
                return extension;
            }

            return extension[1..];
        }
    }
}
