using Sledge.Formats.Valve;
using System;
using System.Collections.Generic;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Tools
{
    /// <summary>Constants and utility functions for dealing with mods.</summary>
    public static class ModUtilities
    {
        /// <summary>The name of the liblist.gam file used to describe mod information.</summary>
        public const string LiblistFileName = "liblist.gam";

        /// <summary>
        /// Returns an enumerable collection of mod directory names in a specified path.
        /// A directory is considered to be a mod directory if it contains a <c>liblist.gam</c> file.
        /// </summary>
        /// <param name="gameDirectory">
        /// Absolute or relative path to the game installation directory
        /// (e.g. <c>C:\Program Files (x86)\Steam\steamapps\common\Half-Life</c>)
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="gameDirectory"/> is <see langword="null"/>.</exception>
        public static IEnumerable<string> EnumerateMods(string gameDirectory)
        {
            if (gameDirectory is null)
            {
                throw new ArgumentNullException(nameof(gameDirectory));
            }

            return EnumerateModsCore();

            IEnumerable<string> EnumerateModsCore()
            {
                foreach (var directory in Directory.EnumerateDirectories(gameDirectory))
                {
                    var liblistPath = Path.Combine(directory, LiblistFileName);

                    if (File.Exists(liblistPath))
                    {
                        yield return Path.GetFileName(directory);
                    }
                }
            }
        }

        /// <summary>
        /// Helper function to load a liblist file from a mod directory.
        /// If the file exists and could be successfully opened and parsed, returns the liblist data.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="gameDirectory"/> or <paramref name="modDirectory"/> are <see langword="null"/>.
        /// </exception>
        public static Liblist? TryLoadLiblist(string gameDirectory, string modDirectory)
        {
            if (gameDirectory is null)
            {
                throw new ArgumentNullException(nameof(gameDirectory));
            }

            if (modDirectory is null)
            {
                throw new ArgumentNullException(nameof(modDirectory));
            }

            var path = Path.Combine(gameDirectory, modDirectory, LiblistFileName);

            try
            {
                using var stream = File.OpenRead(path);

                return new Liblist(stream);
            }
            catch (Exception e) when (e is IOException || e is NotSupportedException || e is UnauthorizedAccessException)
            {
                return null;
            }
        }
    }
}
