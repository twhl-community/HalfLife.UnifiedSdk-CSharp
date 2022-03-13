using Sledge.Formats.Valve;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Tools
{
    /// <summary>Constants and utility functions for dealing with mods.</summary>
    public static class ModUtilities
    {
        /// <summary>The name of the liblist.gam file used to describe mod information.</summary>
        public const string LiblistFileName = "liblist.gam";

        /// <summary>Types of content that can be enabled in Half-Life 1 mods.</summary>
        public static class ContentTypes
        {
            /// <summary>The suffix used for high definition content directories.</summary>
            public const string HighDefinition = "hd";

            /// <summary>The suffix used for low violence content directories.</summary>
            public const string LowViolence = "lv";

            /// <summary>The suffix used for addon content directories.</summary>
            public const string Addon = "addon";

            /// <summary>The suffix used for downloads content directories.</summary>
            public const string Downloads = "downloads";

            /// <summary>Content types that are "public", meaning they should be distributed with mods.</summary>
            /// <remarks>
            /// Addon is meant for user-installed content separate from a game or mod.
            /// Downloads is meant for files downloaded from servers.
            /// </remarks>
            public static IEnumerable<string> PublicTypes
            {
                get
                {
                    yield return HighDefinition;
                    yield return LowViolence;
                }
            }

            /// <summary>All content types.</summary>
            public static IEnumerable<string> AllTypes
            {
                get
                {
                    yield return HighDefinition;
                    yield return LowViolence;
                    yield return Addon;
                    yield return Downloads;
                }
            }
        }

        /// <summary>
        /// Returns an enumerable collection of all mod directory suffixes that are considered to be "public",
        /// meaning they should be distributed with mods.
        /// </summary>
        public static IEnumerable<string> AllPublicModDirectorySuffixes => ContentTypes.PublicTypes.Concat(SteamUtilities.SteamLanguagesExceptEnglish);

        /// <summary>Formats a mod directory that contains a suffix.</summary>
        /// <seealso cref="ContentTypes"/>
        /// <seealso cref="AllPublicModDirectorySuffixes"/>
        /// <seealso cref="SteamUtilities.SteamLanguagesExceptEnglish"/>
        public static string FormatModDirectory(string modDirectory, string suffix)
        {
            ArgumentNullException.ThrowIfNull(modDirectory);
            ArgumentNullException.ThrowIfNull(suffix);

            return $"{modDirectory}_{suffix}";
        }

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
            ArgumentNullException.ThrowIfNull(gameDirectory);
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
            ArgumentNullException.ThrowIfNull(gameDirectory);
            ArgumentNullException.ThrowIfNull(modDirectory);

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
