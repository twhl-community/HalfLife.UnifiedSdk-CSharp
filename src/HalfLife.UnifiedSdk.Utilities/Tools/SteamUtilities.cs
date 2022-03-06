using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace HalfLife.UnifiedSdk.Utilities.Tools
{
    /// <summary>Helpers for querying Steam registry values on Windows.</summary>
    public static class SteamUtilities
    {
        private const string SteamKey = @"Software\Valve\Steam";

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
