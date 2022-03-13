using Microsoft.Win32;
using System;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace HalfLife.UnifiedSdk.Utilities.Tools
{
    /// <summary>Helpers for querying Steam registry values on Windows.</summary>
    public static class SteamUtilities
    {
        private const string SteamKey = @"Software\Valve\Steam";

        private static readonly Lazy<ImmutableList<string>> _steamLanguagesExceptEnglish = new(() => ImmutableList.Create(
            "arabic",
            "bulgarian",
            "schinese",
            "tchinese",
            "czech",
            "danish",
            "dutch",
            "finnish",
            "french",
            "german",
            "greek",
            "hungarian",
            "italian",
            "japanese",
            "koreana",
            "norwegian",
            "polish",
            "portuguese",
            "brazilian",
            "romanian",
            "russian",
            "spanish",
            "latam",
            "swedish",
            "thai",
            "turkish",
            "ukrainian",
            "vietnamese"
            ));

        private static readonly Lazy<ImmutableList<string>> _steamLanguages = new(() => _steamLanguagesExceptEnglish.Value.Add("english"));

        /// <summary>Gets an immutable list of all Steam languages except English.</summary>
        /// <remarks>
        /// The default language for game assets is <c>english</c>, so this list can be used to format localization paths.
        /// Not all languages have localization files.
        /// </remarks>
        public static ImmutableList<string> SteamLanguagesExceptEnglish => _steamLanguagesExceptEnglish.Value;

        /// <summary>Gets an immutable list of all Steam languages.</summary>
        public static ImmutableList<string> SteamLanguages => _steamLanguages.Value;

        private static object? TryGetSteamRegistryValue(string valueName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using var key = Registry.CurrentUser.OpenSubKey(SteamKey);

                if (key?.GetValue(valueName) is string path)
                {
                    return path;
                }
            }

            return null;
        }

        private static string? TryGetSteamRegistryValueString(string valueName)
        {
            if (TryGetSteamRegistryValue(valueName) is string value)
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// On Windows, tries to get the Half-Life mod install path from the registry key set by Steam.
        /// </summary>
        /// <returns>The path if it exists, <see langword="null"/> otherwise.</returns>
        public static string? TryGetModInstallPath() => TryGetSteamRegistryValueString("ModInstallPath");

        /// <summary>
        /// On Windows, tries to get the Source mod install path (<c>steamapps/sourcemods</c>) from the registry key set by Steam.
        /// </summary>
        /// <returns>The path if it exists, <see langword="null"/> otherwise.</returns>
        public static string? TryGetSourceModInstallPath() => TryGetSteamRegistryValueString("SourceModInstallPath");

        /// <summary>
        /// On Windows, tries to get the Steam executable path from the registry key set by Steam.
        /// </summary>
        /// <returns>The path if it exists, <see langword="null"/> otherwise.</returns>
        public static string? TryGetSteamExePath() => TryGetSteamRegistryValueString("SteamExe");

        /// <summary>
        /// On Windows, tries to get the Steam directory path from the registry key set by Steam.
        /// </summary>
        /// <returns>The path if it exists, <see langword="null"/> otherwise.</returns>
        public static string? TryGetSteamPath() => TryGetSteamRegistryValueString("SteamPath");
    }
}
